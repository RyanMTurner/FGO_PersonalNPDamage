# FGO_PersonalNPDamage
General Usage will look like:
![image](https://user-images.githubusercontent.com/48457685/173130806-5956ff48-ea93-4ff0-b5b2-da79b4e7f85b.png)

The input file is expecting a saved login response from HTTPS sniffing. I think the [Chaldea app documentation](https://docs.chaldea.center/import_https/#principle) gives the best explanation of that. Neither they nor I guarantee your safety during that process.

You can also run the numbers on a given servant using their collection number:
![image](https://user-images.githubusercontent.com/48457685/173131178-053778f6-a9c1-45c5-ae25-5aaa829ccf8d.png)

Notes About My Chart (it differs from[ Ran's incredible NP Damage Table](https://docs.google.com/spreadsheets/d/1OTrMARN9I06zD_jIhGdmHFWpkePoSWv_xgEk3XPzZWY/edit#gid=1993499094), which you should also check out):

* Uses YOUR servants' current ATK, NP Level, and Skill levels.
* Class Base Damage Multiplier and Passives ARE included.
* Append Skills are NOT included*.
* Interludes and Strengthening Quests are presumed done if they exist on JP.
* Damage calculated against one target; AoE Servants benefit from their ST Debuffs.
* Min damage roll is 0.9f, Average is 1f, and Max is 1.099f.
* Class Advantage Multiplier is 1.5x for Berserker, AlterEgo, and Pretender. Others get 2x. I prefer this approach because it provides a more "realistic" result; i.e. the numbers are closer to what I would see using the servant in challenging content where I bring someone class advantageous.
* Attribute Multiplier NOT included*.
* Overcharge Stage 1*.
* Chance-based buffs are assumed to hit.
* Weighted effects (this-or-that, see [Corday's S3](https://apps.atlasacademy.io/db/JP/servant/259/skill-3)) choose the one which will do the most damage.
* Damage from DoT effects such as Burn, Curse, and Poison are not included.

*For a look at what else I'm thinknig about, check out my [TODO](TODO.txt).
