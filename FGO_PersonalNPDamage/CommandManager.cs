using GrandOrder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FGO_PersonalNPDamage {
    public class CommandManager {
        AtlasClient client = new AtlasClient();
        List<ServantDefinition> servantDefinitions;
        List<ServantInstance> servantInventory;

        public CommandManager() {
            Task.Run(async () => { servantDefinitions = await client.GetAllServants(); servantDefinitions.RemoveUnplayable(); });
        }

        public async Task LoadInventory(string filePath) {
            servantInventory = (await client.ServantInventoryFromFile(filePath)).cache.replaced.userSvt;
            Console.WriteLine("Inventory loaded!");
        }

        public void Help() {
            Console.WriteLine("`loadinventory` loads HTTPS Sniffed servant data from the specified file path.");
            Console.WriteLine("`npdamage collectionNo` calculates YOUR servant's NP Damage.");
        }

        public void NPDamage(int collectionNo) {
            Tuple<ServantDefinition, ServantInstance> servantInfo = GetServantInfoFromCollectionNo(collectionNo);
            if (servantInfo == null) {
                return;
            }

            Console.WriteLine($"NP Damage for {servantInfo.Item1.name} {servantInfo.Item2}:");
            Console.WriteLine($"[{ServantExtensions.ServantNPDamage(servantInfo.Item2, servantInfo.Item1, 0.9f):n0}, {ServantExtensions.ServantNPDamage(servantInfo.Item2, servantInfo.Item1, 1.099f):n0}]");
        }

        public bool RequireDefinitions() {
            if (servantDefinitions != null) {
                return true;
            }
            Console.WriteLine("No response from Atlas. Check internet? Try again?");
            return false;
        }

        public bool RequireInventory() {
            if (servantInventory != null) {
                return true;
            }
            Console.WriteLine("No servant data loaded. Run `loadinventory`!");
            return false;
        }

        public Tuple<ServantDefinition, ServantInstance> GetServantInfoFromCollectionNo(int collectionNo) {
            if (!RequireDefinitions() || ! RequireInventory()) {
                return null;
            }
            ServantDefinition definition = null;
            foreach (var def in servantDefinitions) {
                if (def.collectionNo == collectionNo) {
                    definition = def;
                    break;
                }
            }
            if (definition == null) {
                Console.WriteLine($"Couldn't find servant definition for collection number {collectionNo}.");
                return null;
            }
            ServantInstance instance = null;
            foreach (var svt in servantInventory) {
                if (svt.svtId == definition.id) {
                    instance = svt;
                    break;
                }
            }
            if (instance == null) {
                Console.WriteLine($"Couldn't find collection number {collectionNo} in your inventory.");
                return null;
            }
            return new Tuple<ServantDefinition, ServantInstance>(definition, instance);
        }
    }
}
