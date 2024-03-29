﻿using GrandOrder;
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
                writer.WriteLine("ID\tName\tClass\tBondLevel\tRarity\tLevel\tATK\tAttribute\tNPType\tNPCard\tNPLevel\tNPHits\tMinNPDamage\tAvgNPDamage\tMaxNPDamage");
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
                    List<ServantCardType> cardTypes = new List<ServantCardType>();
                    for (int i = definition.noblePhantasms.Count - 1; i >= 0; i--) {
                        if (!cardTypes.Contains(definition.noblePhantasms[i].card)) {
                            cardTypes.Add(definition.noblePhantasms[i].card);
                            writer.WriteLine($"{definition.collectionNo}\t{definition.name}\t{definition.className}\t{inventory.GetServantCollectionItemByID(instance.svtId).friendshipRank}\t{definition.rarity}\t{instance.lv}\t{instance.atk}\t{definition.attribute}\t" +
                                $"{definition.noblePhantasms[i].NPType}\t{definition.noblePhantasms[i].card}\t{instance.treasureDeviceLv1}\t{(definition.noblePhantasms[i].Support ? 0 : definition.noblePhantasms[i].npDistribution.Count)}\t" +
                                $"{(definition.noblePhantasms[i].Support ? 0 : ServantExtensions.ServantNPDamage(instance, definition, 0.9f, npIndex: i)):n0}\t" +
                                $"{(definition.noblePhantasms[i].Support ? 0 : ServantExtensions.ServantNPDamage(instance, definition, 1, npIndex: i)):n0}\t" +
                                $"{(definition.noblePhantasms[i].Support ? 0 : ServantExtensions.ServantNPDamage(instance, definition, 1.099f, npIndex: i)):n0}");
                        }
                    }
                }
            }
            Console.WriteLine($"Finished!");
        }

        public void EventAttendance(string filePath) {
            if (!RequireDefinitions()) {
                return;
            }
            using (StreamWriter writer = new StreamWriter(filePath)) {
                writer.WriteLine("ID\tName\tClass\tRarity\tEventsAttended");
                foreach (var definition in servantDefinitions) {
                    writer.WriteLine($"{definition.collectionNo}\t{definition.name}\t{definition.className}\t{definition.rarity}\t" +
                        $"{definition.extraPassive.Where(x => x.extraPassive.Count > 0 && x.extraPassive[0].eventId != 80038).Count()}");
                }
            }
            Console.WriteLine($"Finished!");
        }

        public void RelatedQuests(string filePath) {
            if (!RequireDefinitions()) {
                return;
            }
            using (StreamWriter writer = new StreamWriter(filePath)) {
                writer.WriteLine("ID\tName\tClass\tRarity\tRelatedQuests");
                foreach (var definition in servantDefinitions) {
                    writer.WriteLine($"{definition.collectionNo}\t{definition.name}\t{definition.className}\t{definition.rarity}\t" +
                        $"{definition.relateQuestIds?.Count ?? 0}");
                }
            }
            Console.WriteLine($"Finished!");
        }

        public async Task RelatedQuestRequirements(string filePath) {
            if (!RequireDefinitions()) {
                return;
            }
            using (StreamWriter writer = new StreamWriter(filePath)) {
                writer.WriteLine("ID\tName\tClass\tRarity\tRelatedQuests\tRequired Ascension\tRequired Bond");
                foreach (var definition in servantDefinitions) {
                    List<int> ascensionRequirements = new List<int>();
                    List<int> bondRequirements = new List<int>();
                    string ascReq = "None";
                    string bondReq = "None";
                    if (definition.relateQuestIds?.Count > 0) {
                        foreach (int id in definition.relateQuestIds) {
                            var quest = await client.GetQuest(id);
                            if (quest?.releaseConditions?.Count > 0) {
                                foreach (var condition in quest.releaseConditions) {
                                    if (condition.type == "svtLimit" && !ascensionRequirements.Contains(condition.value)) {
                                        ascensionRequirements.Add(condition.value);
                                        if (ascReq == "None") {
                                            ascReq = condition.value.ToString();
                                        }
                                        else {
                                            ascReq += $", {condition.value}";
                                        }
                                    }
                                    if (condition.type == "svtFriendship" && !bondRequirements.Contains(condition.value)) {
                                        bondRequirements.Add(condition.value);
                                        if (bondReq == "None") {
                                            bondReq = condition.value.ToString();
                                        }
                                        else {
                                            bondReq += $", {condition.value}";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    writer.WriteLine($"{definition.collectionNo}\t{definition.name}\t{definition.className}\t{definition.rarity}\t" +
                        $"{definition.relateQuestIds?.Count ?? 0}\t{ascReq}\t{bondReq}");
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
