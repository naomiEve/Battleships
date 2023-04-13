namespace Battleships.Game.Data
{
    /// <summary>
    /// The current state of the game.
    /// </summary>
    internal enum GameState
    {
        ShipBuilding,
        Waiting,
        PlayerBombing,
        OtherPlayerBombing,
        GameOver
    }
}
