﻿using Battleships.Game;
using Battleships.Framework.Data;
using Battleships.Framework.Networking.ServiceDiscovery;
using CommandLine;

Parser.Default.ParseArguments<LaunchOptions>(args)
    .WithParsed(async opts =>
    {
        try
        {
            if (opts.ListenForGames)
            {
                var menu = new ServiceDiscoveryMenu();
                var pair = await menu.ShowMenu();

                opts.Ip = pair.Item1;
                opts.Port = pair.Item2;

                Console.WriteLine(opts);
            }

            var game = new BattleshipsGame(opts);
            game.SetFramerateLimit(60);
            game.Run();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    });
