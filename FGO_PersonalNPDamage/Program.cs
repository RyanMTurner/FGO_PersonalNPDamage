using System;

namespace FGO_PersonalNPDamage {
    class Program {
        static Version Version = new Version(1, 0, 0);

        static void Main(string[] args) {
            CommandManager commandManager = new CommandManager();

            Console.WriteLine($"FGO NP Damage Calculator v{Version} by Ryan Turner. I use YOUR servants specifically!");

            while (true) {
                string input = Console.ReadLine();
                string command = input.ToLower();
                if (string.IsNullOrWhiteSpace(command)) {
                    continue;
                }
                if (command == "exit") {
                    break;
                }
                string[] commandWords = command.Split(' ');
                switch (commandWords[0]) {
                    case "help":
                        commandManager.Help();
                        break;
                    case "loadinventory":
                        if (commandWords.Length == 1) {
                            continue;
                        }
                        var task = commandManager.LoadInventory(input.Substring("loadinventory ".Length, input.Length - "loadinventory ".Length));
                        break;
                    case "npboosts":
                        if (commandWords.Length == 1) {
                            continue;
                        }
                        if (!int.TryParse(commandWords[1], out int boostCollectionNo)) {
                            continue;
                        }
                        commandManager.NPBoosts(boostCollectionNo);
                        break;
                    case "npdamage":
                        if (commandWords.Length == 1) {
                            continue;
                        }
                        if (!int.TryParse(commandWords[1], out int damageCollectionNo)) {
                            continue;
                        }
                        commandManager.NPDamage(damageCollectionNo);
                        break;
                    case "npdamagechart":
                        if (commandWords.Length == 1) {
                            continue;
                        }
                        commandManager.NPDamageChart(input.Substring("npdamagechart ".Length, input.Length - "npdamagechart ".Length));
                        break;
                }
            }
        }
    }
}
