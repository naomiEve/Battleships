# Battleships

Final project for the 2023 C++ Object-Oriented Programming (Programowanie Obiektowe w C++) lecture at the Lublin University of Technology (Politechnika Lubelska). A 2-player networked game of Battleships (statki).

Author: Kacper Staroń

# Dependencies

- Raylib-cs (comes bundled with the Raylib binaries already)
- CommandLine
- The .NET 7 runtime

# Structure

The project is split into two modules, *Framework* and *Game*. 

## Framework

Within Framework resides a small game engine purpose built for this project. Internally it uses the fantastic [Raylib](https://github.com/raysan5/raylib) library for tasks like cross-platform window/audio device creation, asset loading, rendering. The rest of the project has been done completely by hand.

The main part of the engine is the **Game** class, which is to be derived by anything that wants to utilize the engine. It deals with creating the game window, initializing the Asset Database, creating the list of game objects and running the simulation loop.

Any game which wishes to utilize the 2-player lockstep networking system offered by the framework, should however, derive from the **NetworkedGame** subclass of  **Game**. It initializes the network peer, deals with establishing the connection between both parties, and automatically receives and parses the network messages.

Within the Framework reside a lot of other sub-modules: the asset subsystem, the networking subsystem, the gameobject subsystem and the tweening subsystem.

### The asset subsystem
The asset subsystem deals with loading assets and persisting them over the entire game. Every asset loaded is stored in the **Asset Database**, which is a member of the Game class, automatically initialized by the Game constructor.

This also allows us to get the asset from any place that has a reference to this game, and also automatically unloads all of the assets, once the game is done running.

In order to load an asset into the database, you need to specify the type, identifier and path to the asset. Like so:
```cs
AssetDatabase.Load<TAsset>("name", "./path/to/file.ext");
```

To retrieve an asset from the database, however, you do:

```cs
AssetDatabase.Get<TAsset>("name");
```

Which returns a `TAsset?`.

### The networking subsystem
The networking system within the Framework is a very rudimentary networking system that supports 2 players in a somewhat lockstep fashion.

At the heart of the networking system resides the **NetworkPeer** parent class, which is derived from by the **NetworkClient** and **NetworkServer**. The NetworkPeer contains shared functionality for polling the currently active socket for new data, or sending data to the other peer.

Data is sent between peers not directly, but by serializing structures known as messages. Every message to be sent over the wire has to derive from the **INetworkMessage** interface, and implement two methods.
```cs
void Serialize(ref NetworkWriter writer);
void Deserialize(ref NetworkReader reader);
```

In order to register messages for sending, the game has to register them within the **MessageRegistry**, which is a special class that can construct a message given its identifier.

Registering a message looks something like this:
```cs
MessageRegistry.RegisterMessage<TMessage>(message =>
{
	// handler code...
});
```
Where the argument passed to RegisterMessage is the handler that will be invoked when the game receives said message.

In order to facilitate easy serialization and deserialization on each peer, messages are sent in **NetworkPacket\<TMessage\>**, (where TMessage is a structure deriving from the INetworkMessage interface) wrappers, which automatically handle it.

The entire system is also written in a way to stress the garbage collector as little as we can, utilizing *ref structs* for things like the NetworkPacket, or NetworkWriters/NetworkReaders, which force them to be allocated on the stack, because they shouldn't live for longer than the de/serialization calls they've been created for. All the data is also written into pre-allocated buffers, access to which is passed around via Spans.

The final thing inside of the networking subsystem is the Service Discovery system. Whenever a host creates a game and is waiting for players, the game will automatically begin sending out service discovery UDP messages on the network's broadcast IP, on port **2023**. Clients can then listen to incoming messages on port 2023, and receive data on currently open games.

### The GameObject subsystem
The way the Framework is organized, is that each Game has a set of **GameObjects** associated with it. A GameObject is basically an actor whose lifetime is automatically managed by the game. Each GameObject is updated by the game loop, and calls the appropriate Start/Destroy methods on each point of its lifetime.

Everything that wants to do anything within the game must derive from the GameObject class.  To then add a GameObject into the game, there's a helper function:
```cs
Game.AddGameObject<TGameObject>();
```
Where TGameObject is a type deriving from GameObject.

You can also get a GameObject by its type, by issuing
```cs
Game.GetGameObjectOfType<TGameObject>();
```
or, alternatively, get an object within a different object via:
```cs
GameObject.GetGameObjectFromGame<TGameObject>();
```

GameObjects can do more than just be updated by the game, however. The Framework exposes a set of interfaces that can be derived from, to allow GameObjects to perform more tasks.

- **IDrawableObject** can be derived by any object that actually wants to draw to the stage
- **IUIObject** can be derived by any object that wants to draw in the separate 2D UI pass of rendering.
- **ISingletonObject** can be derived by any object that should have only one of its type exist in the GameObject list.
- **IIndestructibleObject** can be derived by any object that shouldn't ever be destroyed by the game during its lifetime. This is currently used by the TweenEngine.

Any GameObject also holds a reference to the game it's inside (ThisGame), alongside the NetworkPeer (Peer), if one exists.

