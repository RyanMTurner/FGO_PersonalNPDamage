using System.Collections.Generic;
using System.Linq;

namespace GrandOrder {
    public class ServantDefinition {
        public int id;
        public int collectionNo;
        public string name;
        public string ruby;
        public ServantClass className;
        public int rarity;
        public int cost;
        public int lvMax;
        //extraAssets
        public ServantGender gender;
        public ServantAttribute attribute;
        public List<Trait> traits;
        public int starAbsorb;
        public int starGen;
        public int instantDeathChance;
        public List<ServantCardType> cards;
        public Dictionary<ServantCardType, List<int>> hitsDistribution;
        //cardDetails
        public int atkBase;
        public int atkMax;
        public int hpBase;
        public int hpMax;
        //relateQuestIds
        //growthCurve
        //atkGrowth
        //hpGrowth
        //bondGrowth
        //expGrowth
        //expFeed
        //bondEquip
        //valentineEquip
        //ascensionAdd
        //traitAdd
        //ascensionMaterials
        //skillMaterials
        //appendSkillMaterials
        //costumeMaterials
        public List<Skill> skills;
        public List<Skill> classPassive;
        //extraPassive
        //appendPassive
        public List<NoblePhantasm> noblePhantasms;

        public string alignment {
            get {
                string retVal = "";
                foreach (Trait t in traits) {
                    if (t.name.StartsWith("alignment")) {
                        retVal += t.name.Remove(0, "alignment".Length);
                    }
                }
                return retVal;
            }
        }

        public string deck {
            get {
                int buster = 0, arts = 0, quick = 0;
                foreach (ServantCardType cardType in cards) {
                    if (cardType == ServantCardType.buster) { buster++; }
                    if (cardType == ServantCardType.arts) { arts++; }
                    if (cardType == ServantCardType.quick) { quick++; }
                }
                string retVal = "";
                for (int i = 0; i < buster; i++) {
                    retVal += "B";
                }
                for (int i = 0; i < arts; i++) {
                    retVal += "A";
                }
                for (int i = 0; i < quick; i++) {
                    retVal += "Q";
                }
                return retVal;
            }
        }

        public bool HasSkill(string buffType, string targetType = null) {
            foreach (Skill s in skills) {
                foreach (SkillFunction f in s.functions) {
                    foreach (Buff b in f.buffs) {
                        if (b.type == buffType) {
                            if (string.IsNullOrEmpty(targetType)) {
                                return true;
                            }
                            else {
                                if (f.funcTargetType == targetType) {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public NPDamageTrifecta NPDamageBoosts(int[] skillLevels = null, int npLevel = 1, int npOvercharge = 1, bool includePassives = true, int npIndex = -1) {
            if (skillLevels == null) {
                skillLevels = new int[] { 10, 10, 10 };
            }

            NPDamageTrifecta boosts = new NPDamageTrifecta();

            NoblePhantasm targetNP = noblePhantasms.Last();
            if (npIndex != -1) {
                targetNP = noblePhantasms[npIndex];
            }

            List<int> checkedSkills = new List<int>();
            for (int i = skills.Count - 1; i >= 0; i--) {
                if (!checkedSkills.Contains(skills[i].num - 1)) {
                    checkedSkills.Add(skills[i].num - 1);
                    foreach (SkillFunction sf in skills[i].functions) {
                        if (!sf.TargetsSelf) {
                            continue;
                        }
                        foreach (Buff b in sf.buffs) {
                            SkillValues sv = sf.svals[skillLevels[skills[i].num - 1] - 1];
                            boosts.AddBuff(b, sv, targetNP);
                        }
                    }
                }
            }

            foreach (NPFunction nf in targetNP.functions) {
                if (!nf.TargetsSelf) {
                    continue;
                }
                if (nf.funcType.Contains("damageNp")) {
                    if (nf.SkillValueForSituation(npLevel, npOvercharge).Correction != null) {
                        boosts.SuperEffectiveMod = (int)nf.SkillValueForSituation(npLevel, npOvercharge).Correction;
                    }
                    break;
                }
                foreach (Buff b in nf.buffs) {
                    SkillValues sv = nf.SkillValueForSituation(npLevel, npOvercharge);
                    boosts.AddBuff(b, sv, targetNP);
                }
            }

            if (includePassives) {
                foreach (Skill s in classPassive) {
                    foreach (SkillFunction sf in s.functions) {
                        if (!sf.TargetsSelf) {
                            continue;
                        }
                        foreach (Buff b in sf.buffs) {
                            SkillValues sv = sf.svals[0];
                            boosts.AddBuff(b, sv, targetNP);
                        }
                    }
                }
            }

            return boosts;
        }
    }

    public class Trait {
        public int id;
        public string name;
    }

    public class NoblePhantasm {
        public ServantCardType card;
        public List<int> npDistribution;
        public Dictionary<string, List<int>> npGain;
        public List<NPFunction> functions;

        public bool Support {
            get {
                foreach (var func in functions) {
                    if (func.funcType.StartsWith("damageNp")) {
                        return false;
                    }
                }
                return true;
            }
        }

        public SkillValues NPDamage(int npLevel, int npOvercharge) {
            foreach (var func in functions) {
                if (func.funcType.StartsWith("damageNp")) {
                    return func.SkillValueForSituation(npLevel, npOvercharge);
                }
            }
            return null;
        }
    }

    public class NPFunction : SkillFunction {
        //Different sval lists are for different overcharge levels
        public List<SkillValues> svals2; //The items in the sval list are for np levels
        public List<SkillValues> svals3;
        public List<SkillValues> svals4;
        public List<SkillValues> svals5;

        public SkillValues SkillValueForSituation(int npLevel, int npOvercharge) {
            switch (npOvercharge) {
                default:
                    return svals[npLevel - 1];
                case 2:
                    return svals2[npLevel - 1];
                case 3:
                    return svals3[npLevel - 1];
                case 4:
                    return svals4[npLevel - 1];
                case 5:
                    return svals5[npLevel - 1];
            }
        }
    }

    public class Skill {
        public int id;
        public int num; //what number skill it is; evaluate lists of skills backwards and only count each num once
        public SkillType type;
        public List<SkillFunction> functions;
    }

    public class SkillFunction {
        public string funcType;
        public string funcTargetType;
        public string funcTargetTeam;
        public List<Buff> buffs;
        public List<SkillValues> svals;

        public bool TargetsSelf {
            get {
                return funcTargetType != "ptOther"; //TODO: Get full targeting list
            }
        }
    }

    public class Buff {
        public string type;
        public List<RequiredIndividualityToApply> ckSelfIndv;
        public List<RequiredIndividualityToApply> ckOpIndv;
    }

    public class SkillValues {
        public int Value;
        public int? Correction;
        public int? ActSetWeight;
    }

    public class RequiredIndividualityToApply {
        public int id;
        public string name;
    }

    public class NPDamageTrifecta {
        public int ATKUp;
        public int DEFDown;
        public int CardUp;
        public int NPDamageUp;
        public int SpecialTraitDamage;
        public int SuperEffectiveMod = 1000;
        public int Divinity;

        public void AddBuff(Buff b, SkillValues sv, NoblePhantasm targetNP) {
            switch (b.type) {
                case "upAtk":
                    ATKUp += sv.Value;
                    break;
                case "downDefence":
                    DEFDown += sv.Value;
                    break;
                case "upCommandall":
                    foreach (var ck in b.ckSelfIndv) {
                        if (ck.name.ToLower().Contains(targetNP.card.ToString().ToLower())) {
                            CardUp += sv.Value;
                        }
                    }
                    break;
                case "downDefencecommandall":
                    foreach (var ck in b.ckOpIndv) {
                        if (ck.name.ToLower().Contains(targetNP.card.ToString().ToLower())) {
                            CardUp += sv.Value;
                        }
                    }
                    break;
                case "upNpdamage":
                    NPDamageUp += sv.Value;
                    break;
                case "upDamage":
                    SpecialTraitDamage += sv.Value;
                    break;
                case "addDamage":
                    Divinity += sv.Value;
                    break;
            }
        }

        public override string ToString() {
            string retVal = string.Empty;
            if (ATKUp != 0) {
                retVal = $"atkmod{ATKUp / 10f}";
            }
            if (DEFDown != 0) {
                retVal += $"{(string.IsNullOrEmpty(retVal) ? string.Empty : " ")}defmod-{DEFDown / 10f}";
            }
            if (CardUp != 0) {
                retVal += $"{(string.IsNullOrEmpty(retVal) ? string.Empty : " ")}cardmod{CardUp / 10f}";
            }
            if (NPDamageUp != 0) {
                retVal += $"{(string.IsNullOrEmpty(retVal) ? string.Empty : " ")}npmod{NPDamageUp / 10f}";
            }
            if (SpecialTraitDamage != 0) {
                retVal += $"{(string.IsNullOrEmpty(retVal) ? string.Empty : " ")}powermod{SpecialTraitDamage / 10f}";
            }
            if (SuperEffectiveMod != 1000) {
                retVal += $"{(string.IsNullOrEmpty(retVal) ? string.Empty : " ")}supereffectivemod{SuperEffectiveMod / 10f}";
            }
            if (Divinity != 0) {
                retVal += $"{(string.IsNullOrEmpty(retVal) ? string.Empty : " ")}fd{Divinity}";
            }
            return retVal;
        }
    }
}
