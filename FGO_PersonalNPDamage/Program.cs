using System;

namespace FGO_PersonalNPDamage {
    class Program {
        static void Main(string[] args) {
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
                if (commandWords[0] == "loadinventory") {
                    if (commandWords.Length == 1) {
                        continue;
                    }


                }
            }
        }
    }
}
