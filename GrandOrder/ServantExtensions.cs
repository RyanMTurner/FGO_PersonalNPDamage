using System;
using System.Collections.Generic;
using System.Linq;

namespace GrandOrder {
    public static class ServantExtensions {
        public static List<int> UnplayableIDs = new List<int>() {
            9941730,
            9939130,
            9935530,
            9935500,
            9935400,
            1700100
        };

        public static void RemoveUnplayable(this List<ServantDefinition> servants) {
            for (int i = 0; i < servants.Count; i++) {
                if (UnplayableIDs.Contains(servants[i].id)) {
                    servants.RemoveAt(i);
                    i--;
                }
            }
        }

        public static float CardDamageValue(this ServantCardType servantCardType) {
            switch (servantCardType) {
                case ServantCardType.buster:
                    return 1.5f;
                case ServantCardType.quick:
                    return 0.8f;
                default:
                    return 1;
            }
        }

        public static float ClassAtkBonus(this ServantClass servantClass) {
            switch (servantClass) {
                case ServantClass.archer:
                    return 0.95f;
                case ServantClass.lancer:
                    return 1.05f;
                case ServantClass.caster:
                case ServantClass.assassin:
                    return 0.9f;
                case ServantClass.berserker:
                case ServantClass.ruler:
                case ServantClass.avenger:
                    return 1.1f;
                default:
                    return 1;
            }
        }

        //https://github.com/atlasacademy/fgo-docs/blob/master/deeper/battle/damage.md
        //https://blogs.nrvnqsr.com/entry.php/3309-How-is-damage-calculated
        public static float CardDamage(int ATK, float cardDamageValue = 1, float npDamageMultiplier = 1, int firstCardBonus = 0, float cardMod = 0, float classAtkBonus = 1,
                float triangleModifier = 2, float attributeModifier = 1, float randomModifier = 1, float atkMod = 0, float defMod = 0, bool crit = false,
                float extraCardModifier = 1, float specialDefMod = 0, float powerMod = 0, float selfDamageMod = 0, float critDamageMod = 0, float npDamageMod = 0,
                float damageSpecialMod = 0, float superEffectiveModifier = 1, int dmgPlusAdd = 0, int selfDmgCutAdd = 0, bool busterCardInBusterChain = false) {
            float damage = ATK
                * npDamageMultiplier
                * (firstCardBonus + (cardDamageValue * Math.Max(1 + cardMod, 0)))
                * classAtkBonus
                * triangleModifier
                * attributeModifier
                * randomModifier
                * 0.23f
                * Math.Max(1 + atkMod - defMod, 0)
                * (crit ? 2 : 1)
                * extraCardModifier
                * Math.Max(1 - specialDefMod, 0)
                * Math.Max(1 + powerMod + selfDamageMod + (critDamageMod * (crit ? 1 : 0)) + (npDamageMod * (npDamageMultiplier == 1 ? 0 : 1)), 0.001f)
                * Math.Max(1 + damageSpecialMod, 0.001f)
                * superEffectiveModifier
                + dmgPlusAdd
                + selfDmgCutAdd
                + (ATK * (busterCardInBusterChain ? 0.2f : 0));

            return (float)Math.Floor(Math.Max(damage, 0));
        }

        public static float ServantNPDamage(ServantInstance instance, ServantDefinition definition, float randomModifier = 1, bool specialEffect = false) {
            var npDamageBoosts = definition.NPDamageBoosts(new int[] { instance.skillLv1, instance.skillLv2, instance.skillLv3 },
                instance.treasureDeviceLv1,
                1); //TODO: Overcharge?

            return CardDamage(instance.atk,
                cardDamageValue: definition.noblePhantasms.Last().card.CardDamageValue(),
                npDamageMultiplier: definition.noblePhantasms.Last().NPDamage(instance.treasureDeviceLv1, 1).Value / 1000f,
                cardMod: npDamageBoosts.CardUp / 1000f,
                classAtkBonus: definition.className.ClassAtkBonus(),
                randomModifier: randomModifier,
                atkMod: npDamageBoosts.ATKUp / 1000f,
                defMod: -npDamageBoosts.DEFDown / 1000f,
                powerMod: specialEffect ? npDamageBoosts.SpecialTraitDamage / 1000f : 0,
                npDamageMod: npDamageBoosts.NPDamageUp / 1000f,
                superEffectiveModifier: specialEffect ? npDamageBoosts.SuperEffectiveMod / 1000f : 1,
                dmgPlusAdd: npDamageBoosts.Divinity);
        }
    }
}
