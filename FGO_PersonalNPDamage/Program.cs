using System;

namespace FGO_PersonalNPDamage {
    class Program {
        static void Main(string[] args) {
            AtlasClient client = new AtlasClient();

            while (true) {
                string command = Console.ReadLine().ToLower();
                if (string.IsNullOrWhiteSpace(command)) {
                    continue;
                }
                if (command == "exit") {
                    break;
                }
                string[] commandWords = command.Split(' ');
            }
        }
    }
}
