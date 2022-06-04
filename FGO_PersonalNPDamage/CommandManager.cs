using GrandOrder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGO_PersonalNPDamage {
    public class CommandManager {
        AtlasClient client = new AtlasClient();
        List<ServantDefinition> servantDefinitions;
        FGOCacheReplace inventory;
        List<ServantInstance> servantInventory => inventory?.userSvt;

        public CommandManager() {
            Task.Run(async () => { servantDefinitions = await client.GetAllServants(); servantDefinitions.RemoveUnplayable(); });
        }

        public async Task LoadInventory(string filePath) {
            inventory = (await client.ServantInventoryFromFile(filePath)).cache.replaced;
            Console.WriteLine("Inventory loaded!");
        }

        public void Help() {
            Console.WriteLine("`loadinventory filePath` loads HTTPS Sniffed servant data from the specified file path.");
            Console.WriteLine("`npboosts collectionNo` gives YOUR servant's NP boosts (for use with The Wizard, Archimedes, etc).");
            Console.WriteLine("`npdamage collectionNo` calculates YOUR servant's NP Damage.");
            Console.WriteLine("`npdamagechart filePath` writes out the NP Damage of ALL your servants to the specified file path.");
        }

        public void NPBoosts(int collectionNo) {
            Tuple<ServantDefinition, ServantInstance> servantInfo = GetServantInfoFromCollectionNo(collectionNo);
            if (servantInfo == null) {
                return;
            }

            var npDamageBoosts = servantInfo.Item1.NPDamageBoosts(new int[] { servantInfo.Item2.skillLv1, servantInfo.Item2.skillLv2, servantInfo.Item2.skillLv3 },
                servantInfo.Item2.treasureDeviceLv1,
                1, //TODO: Overcharge?
                false);

            Console.WriteLine($"NP Damage Boosts for {servantInfo.Item1.name} {servantInfo.Item2}:");
            Console.WriteLine(npDamageBoosts);
        }

        public void NPDamage(int collectionNo) {
            Tuple<ServantDefinition, ServantInstance> servantInfo = GetServantInfoFromCollectionNo(collectionNo);
            if (servantInfo == null) {
                return;
            }

            Console.WriteLine($"NP Damage for {servantInfo.Item1.name} {servantInfo.Item2}:");
            Console.WriteLine($"[{ServantExtensions.ServantNPDamage(servantInfo.Item2, servantInfo.Item1, 0.9f):n0}, {ServantExtensions.ServantNPDamage(servantInfo.Item2, servantInfo.Item1, 1.099f):n0}]");
        }

        public void NPDamageChart(string filePath) {
            if (!RequireDefinitions() || !RequireInventory()) {
                return;
            }
            using (StreamWriter writer = new StreamWriter(filePath)) {
                writer.WriteLine("ID\tName\tClass\tRarity\tLevel\tATK\tAttribute\tNPType\tNPLevel\tNPHits\tMinNPDamage\tAvgNPDamage\tMaxNPDamage");
                foreach (var definition in servantDefinitions) {
                    ServantInstance instance = null;
                    foreach (var svt in servantInventory) {
                        if (svt.svtId == definition.id) {
                            instance = svt;
                            break;
                        }
                    }
                    if (instance == null) {
                        continue;
                    }
                    writer.WriteLine($"{definition.collectionNo}\t{definition.name}\t{definition.className}\t{definition.rarity}\t{instance.lv}\t{instance.atk}\t{definition.attribute}\t" +
                        $"{definition.noblePhantasms.Last().card}\t{instance.treasureDeviceLv1}\t{(definition.noblePhantasms.Last().Support ? 0 : definition.noblePhantasms.Last().npDistribution.Count)}\t" +
                        $"{(definition.noblePhantasms.Last().Support ? 0 : ServantExtensions.ServantNPDamage(instance, definition, 0.9f)):n0}\t" +
                        $"{(definition.noblePhantasms.Last().Support ? 0 : ServantExtensions.ServantNPDamage(instance, definition, 1)):n0}\t" +
                        $"{(definition.noblePhantasms.Last().Support ? 0 : ServantExtensions.ServantNPDamage(instance, definition, 1.099f)):n0}");
                }
            }
            Console.WriteLine($"Finished!");
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
