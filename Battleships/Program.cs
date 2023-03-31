using Battleships;
using Battleships.Data;
using Battleships.Framework;
using CommandLine;

Parser.Default.ParseArguments<LaunchOptions>(args)
    .WithParsed(opts =>
    {
        var game = new Game(new Vector2Int(640, 480), $"Battleships ({opts.Mode})", new BattleshipsLogic(opts));
        game.SetFramerateLimit(60);
        game.Run();
    });
