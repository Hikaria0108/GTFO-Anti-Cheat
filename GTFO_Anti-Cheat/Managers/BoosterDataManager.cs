using BoosterImplants;
using GameData;
using Hikaria.GTFO_Anti_Cheat.Utils;
using System.Collections.Generic;
using Newtonsoft.Json;
using Player;
using SNetwork;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Linq;
namespace Hikaria.GTFO_Anti_Cheat.Managers
{
    internal static class BoosterDataManager
    {
        public static float GetBoosterDamageMultiForPlayer(InventorySlot slot, SNet_Player player)
        {
            AgentModifier targetModifier = AgentModifier.None;
            switch (slot)
            {
                case InventorySlot.GearMelee:
                    targetModifier = AgentModifier.MeleeDamage;
                    break;
                case InventorySlot.GearStandard:
                    targetModifier = AgentModifier.StandardWeaponDamage;
                    break;
                case InventorySlot.GearSpecial:
                    targetModifier = AgentModifier.SpecialWeaponDamage;
                    break;
                default: //不是伤害类强化剂
                    return 1f;
            }

            int playerSlotIndex = player.PlayerSlotIndex();

            PlayerAgent playerAgent;
            if (!PlayerManager.TryGetPlayerAgent(ref playerSlotIndex, out playerAgent))
            {
                return 1f;
            }

            return 1f + AgentModifierManager.GetModifierValue(playerAgent, targetModifier);
        }

        public static List<Booster_Template> CreateBooster_Template(pBoosterImplantsWithOwner boosterImplantsWithOwner)
        {
            List<Booster_Template> boosters = new List<Booster_Template>();

            List<pBoosterImplantData> BoosterImplantDatas = new List<pBoosterImplantData>();

            //排除没有强化剂的槽位
            if (boosterImplantsWithOwner.BasicImplant.BoosterEffectCount != 0) 
            {
                BoosterImplantDatas.Add(boosterImplantsWithOwner.BasicImplant);
            }
            if (boosterImplantsWithOwner.AdvancedImplant.BoosterEffectCount != 0)
            {
                BoosterImplantDatas.Add(boosterImplantsWithOwner.AdvancedImplant);
            }
            if (boosterImplantsWithOwner.SpecializedImplant.BoosterEffectCount != 0)
            {
                BoosterImplantDatas.Add(boosterImplantsWithOwner.SpecializedImplant);
            }

            foreach (pBoosterImplantData boosterImplantData in BoosterImplantDatas)
            {
                //用于判断是否存在有效属性
                bool hasEffect = false;

                Booster_Template booster = new Booster_Template()
                {
                    Conditions = new List<uint>(),
                    RandomConditions = new List<uint>(),
                    Effects = new List<Booster_Effect>(),
                    RandomEffectsGroups = new List<List<Booster_Effect>>()
                };

                if (EntryPoint.EnableDebugInfo)
                {
                    Logs.LogMessage("============================");
                    Logs.LogMessage("Create Booster");
                }

                foreach (uint conditionID in boosterImplantData.Conditions)
                {
                    if (conditionID != 0U)
                    {
                        if (EntryPoint.EnableDebugInfo)
                        {
                            Logs.LogMessage(string.Format("Add Condition: id:{0}", conditionID));
                        }
                        booster.Conditions.Add(conditionID);
                    }
                }

                foreach (pBoosterEffectData data in boosterImplantData.BoosterEffectDatas)
                {
                    if (data.BoosterEffectID != 0U)
                    {
                        hasEffect = true;
                        Booster_Effect effect = new Booster_Effect
                        {
                            id = data.BoosterEffectID,
                            value = data.EffectValue,
                            maxValue = 0f,
                            minValue = 0f
                        };
                        if (EntryPoint.EnableDebugInfo)
                        {
                            Logs.LogMessage(string.Format("Add Effect: id:{0}, value:{1}", effect.id, effect.value)); 
                        }
                        booster.Effects.Add(effect);
                    }
                }

                if (hasEffect)
                {
                    if (EntryPoint.EnableDebugInfo)
                    {
                        Logs.LogMessage("Add Booster");
                        Logs.LogMessage("============================");
                    }
                    boosters.Add(booster);
                }
            }

            return boosters;
        }


        //为players存储强化剂数据
        /*
        public static void StorePlayerBoosters(SNet_Player player, pBoosterImplantsWithOwner boosterImplantsWithOwner)
        {
            int playerSlotIndex = player.PlayerSlotIndex();

            if (playerSlotIndex == -1)
                return;
            
            if(!playerBoosters.ContainsKey(playerSlotIndex))
            {
                playerBoosters.Add(playerSlotIndex, CreateBooster_Template(boosterImplantsWithOwner));

                return;
            }

            playerBoosters[playerSlotIndex] = CreateBooster_Template(boosterImplantsWithOwner);
        }
        */

        public static bool CheckBoosters(pBoosterImplantsWithOwner boosterImplantsWithOwner)
        {
            if (boosterImplantsWithOwner == null)
            {
                return true;
            }

            //排除没有强化剂的槽位
            if (boosterImplantsWithOwner.BasicImplant.BoosterEffectCount != 0)
            {
                if (!IsValidBooster(BoosterImplantCategory.Muted, boosterImplantsWithOwner.BasicImplant.Conditions, boosterImplantsWithOwner.BasicImplant.BoosterEffectDatas))
                {
                    return false;
                }
            }

            if (boosterImplantsWithOwner.AdvancedImplant.BoosterEffectCount != 0)
            {
                if (!IsValidBooster(BoosterImplantCategory.Bold, boosterImplantsWithOwner.AdvancedImplant.Conditions, boosterImplantsWithOwner.AdvancedImplant.BoosterEffectDatas))
                {
                    return false;
                }
            }

            if (boosterImplantsWithOwner.SpecializedImplant.BoosterEffectCount != 0)
            {
                if (!IsValidBooster(BoosterImplantCategory.Aggressive, boosterImplantsWithOwner.SpecializedImplant.Conditions, boosterImplantsWithOwner.SpecializedImplant.BoosterEffectDatas))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CheckBoostersForPlayer(SNet_Player player)
        {
            if (player.PlayerSlotIndex() == -1)
            {
                return true;
            }

            pBoosterImplantData boosterImplantData;

            for (int i = 0; i < 3; i++)
            {
                BoosterImplantCategory category = (BoosterImplantCategory)i;
                if (BoosterImplantManager.TryGetBoosterImplant(player, category, out boosterImplantData))
                {
                    if (!IsValidBooster(category, boosterImplantData.Conditions, boosterImplantData.BoosterEffectDatas))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool IsValidBooster(BoosterImplantCategory category, uint[] conditions, pBoosterEffectData[] effectDatas)
        {
            //conditions长度固定为5，effectDatas长度固定为10

            if (EntryPoint.EnableDebugInfo)
            {
                Logs.LogMessage("===================");
                Logs.LogMessage("Booster Info:");
                Logs.LogMessage("Category: " + category.ToString());

                foreach (uint conditionId in conditions)
                {
                    if (conditionId != 0U) 
                    {
                        Logs.LogMessage("ConditionID:" + conditionId.ToString());
                    }
                }
                foreach (pBoosterEffectData data in effectDatas)
                {
                    if (data.BoosterEffectID != 0U) 
                    {
                        Logs.LogMessage("EffectID:" + data.BoosterEffectID.ToString() + ", EffectValue:" + data.EffectValue.ToString());
                    }
                }
                Logs.LogMessage("End of Booster Info");
            }

            if (EntryPoint.EnableDebugInfo)
            {
                Logs.LogMessage("Enter Booster Match");
            }
            //有效属性数目计数
            int conditionCount = 0;
            int effectCount = 0;

            //conditionid和effectid均不可为0，为0游戏无法启动，故以此作为判断是否为有效属性条目的标志来获取有效属性条目数目
            foreach (uint condition in conditions)
            {
                if (condition == 0U) //conditionid为0表示没有条件
                {
                    break;
                }
                conditionCount++;
            }

            foreach (pBoosterEffectData data in effectDatas)
            {
                if (data.BoosterEffectID == 0U) //effectid为0表示没有效果
                {
                    break;
                }
                effectCount++;
            }

            //属性匹配标记
            //bool[] conditionsMatch = new bool[conditionCount];
            //bool[] effectsMatch = new bool[effectCount];

            foreach (Booster_Template booster in booster_Templates[category])
            {
                //每一次循环开始先重置计数和标志
                //conditionsMatch = Enumerable.Repeat(false, conditionCount).ToArray();
                //effectsMatch = Enumerable.Repeat(false, effectCount).ToArray();
                int conditionIndex = 0;
                int effectIndex = 0;

                //首先判断有效属性数目是否匹配当前Booster_Template，不匹配直接下一个Template
                if (conditionCount != (booster.Conditions.Count + booster.RandomConditions.Count != 0 ? 1 : 0) || effectCount != (booster.Effects.Count + booster.RandomEffectsGroups.Count))
                {
                    if (EntryPoint.EnableDebugInfo)
                    {
                        Logs.LogMessage(string.Format("ConditionCount:{0},EffectCount:{1}", conditionCount, effectCount));
                    }
                    continue;
                }

                //非Random组必须顺序逐项匹配，Random组每一组必须有一项匹配
                if (conditionCount > 0)
                {
                    //接先匹配Booster_Template中的RandomConditions
                    //因为RandomCondition若有也只会有一组，所以只要存在匹配的id就算匹配，没有就直接下一个Template
                    if (booster.RandomConditions.Contains(conditions[conditionIndex]))
                    {
                        if (EntryPoint.EnableDebugInfo)
                        {
                            Logs.LogMessage(string.Format("RandomConditions:{0}", conditions[conditionIndex]));
                        }
                        //conditionsMatch[conditionIndex] = true;
                        conditionIndex++;
                    }
                    else
                    {
                        goto _nextTemplate;
                    }

                    //再匹配Booster_Template中的Conditions
                    foreach (uint templateCondition in booster.Conditions)
                    {
                        if (conditions[conditionIndex] == templateCondition)
                        {
                            if (EntryPoint.EnableDebugInfo)
                            {
                                Logs.LogMessage(string.Format("Conditions[{0}]:{1}", conditionIndex, conditions[conditionIndex]));
                            }
                            //conditionsMatch[conditionIndex] = true;
                            conditionIndex++;
                            continue;
                        }

                        //Conditions没有顺序逐个匹配时直接下一个Booster_Template
                        goto _nextTemplate;
                    }
                }

                if (effectCount > 0)
                {
                    //先遍历每个Booster_Template的每个RandomEffectGroups, 每一组Group都必须有一项匹配
                    for (int i = 0; i < booster.RandomEffectsGroups.Count; i++)
                    {
                        for (int j = 0; j < booster.RandomEffectsGroups[i].Count; j++)
                        {
                            if (effectDatas[effectIndex].BoosterEffectID == booster.RandomEffectsGroups[i][j].id && effectDatas[effectIndex].EffectValue >= booster.RandomEffectsGroups[i][j].minValue && effectDatas[effectIndex].EffectValue <= booster.RandomEffectsGroups[i][j].maxValue)
                            {
                                if (EntryPoint.EnableDebugInfo)
                                {
                                    Logs.LogMessage(string.Format("RandomEffectId:{0},EffectValue:{1}", effectDatas[effectIndex].BoosterEffectID, effectDatas[effectIndex].EffectValue));
                                }
                                //effectsMatch[effectIndex] = true;
                                effectIndex++;
                                goto _nextGroup;
                            }
                        }
                    _nextGroup:;
                    }

                    //再顺序匹配每个Booster_Template的每个Effect
                    for (int i = 0; i < booster.Effects.Count; i++)
                    {
                        for (int j = 0; j < booster.Effects.Count; j++)
                        {
                            if (effectDatas[effectIndex].BoosterEffectID == booster.Effects[i].id && effectDatas[effectIndex].EffectValue >= booster.Effects[i].minValue && effectDatas[effectIndex].EffectValue <= booster.Effects[i].maxValue)
                            {
                                if (EntryPoint.EnableDebugInfo)
                                {
                                    Logs.LogMessage(string.Format("EffectId:{0},EffectValue:{1}", effectDatas[effectIndex].BoosterEffectID, effectDatas[effectIndex].EffectValue));
                                }
                                //effectsMatch[effectIndex] = true;
                                effectIndex++;
                                continue;
                            }
                        }
                    }
                }

                //匹配index达到count时说明完全匹配
                if (conditionIndex == conditionCount && effectIndex == effectCount)
                {
                    if (EntryPoint.EnableDebugInfo)
                    {
                        Logs.LogMessage("Valid Booster!");
                        Logs.LogMessage("===================");
                    }
                    return true;
                }

            _nextTemplate:;
            }

            //到达这里时说明没有匹配的Booster_Template, 是修改过的Booster
            if (EntryPoint.EnableDebugInfo)
            {
                Logs.LogMessage("Invalid Booster!");
                Logs.LogMessage("===================");
            }

            return false;
        }

        public static void LoadData()
        {
            booster_Templates.Add(BoosterImplantCategory.Muted, new List<Booster_Template>());
            booster_Templates.Add(BoosterImplantCategory.Bold, new List<Booster_Template>());
            booster_Templates.Add(BoosterImplantCategory.Aggressive, new List<Booster_Template>());

            Il2CppArrayBase<BoosterImplantTemplateDataBlock> blocklist = GameDataBlockBase<BoosterImplantTemplateDataBlock>.GetAllBlocksForEditor();
            for (int i = 0; i < blocklist.Count; i++)
            {
                if (EntryPoint.EnableDebugInfo)
                {
                    Logs.LogMessage("===========================");
                    Logs.LogMessage(string.Format("Category:{0}, EffectType:{1}", blocklist[i].ImplantCategory, blocklist[i].MainEffectType));
                }

                Booster_Template booster = new Booster_Template()
                {
                    Conditions = new List<uint>(),
                    RandomConditions = new List<uint>(),
                    Effects = new List<Booster_Effect>(),
                    RandomEffectsGroups = new List<List<Booster_Effect>>()
                };

                Il2CppSystem.Collections.Generic.List<uint> conditions = blocklist[i].Conditions;
                for (int n = 0; n < conditions.Count; n++)
                {
                    if (EntryPoint.EnableDebugInfo)
                    {
                        Logs.LogMessage(string.Format("Condition id:{0}", conditions[n]));
                    }
                    booster.Conditions.Add(conditions[n]);
                }

                Il2CppSystem.Collections.Generic.List<uint> randomConditions = blocklist[i].RandomConditions;
                for (int m = 0; m < randomConditions.Count; m++)
                {
                    if (EntryPoint.EnableDebugInfo)
                    {
                        Logs.LogMessage(string.Format("RandomCondition id:{0}", randomConditions[m]));
                    }
                    booster.RandomConditions.Add(randomConditions[m]);
                }

                Il2CppSystem.Collections.Generic.List<BoosterImplantEffectInstance> effectsInBlock = blocklist[i].Effects;
                for (int j = 0; j < effectsInBlock.Count; j++)
                {
                    Booster_Effect effect = new Booster_Effect
                    {
                        id = effectsInBlock[j].BoosterImplantEffect,
                        maxValue = effectsInBlock[j].MaxValue,
                        minValue = effectsInBlock[j].MinValue,
                        value = 0f
                    };
                    if (EntryPoint.EnableDebugInfo)
                    {
                        Logs.LogMessage(string.Format("Effect: id:{0}, max:{1}, min:{2}", effectsInBlock[j].BoosterImplantEffect, effectsInBlock[j].MaxValue, effectsInBlock[j].MinValue));
                    }
                    booster.Effects.Add(effect);
                }

                Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<BoosterImplantEffectInstance>> randomEffectsGroups = blocklist[i].RandomEffects;
                for (int k = 0; k < randomEffectsGroups.Count; k++)
                {
                    List<Booster_Effect> effectsGroup = new List<Booster_Effect>();
                    for (int l = 0; l < randomEffectsGroups[k].Count; l++)
                    {
                        Booster_Effect effect = new Booster_Effect
                        {
                            id = randomEffectsGroups[k][l].BoosterImplantEffect,
                            maxValue = randomEffectsGroups[k][l].MaxValue,
                            minValue = randomEffectsGroups[k][l].MinValue,
                            value = 0f
                        };
                        if (EntryPoint.EnableDebugInfo)
                        {
                            Logs.LogMessage(string.Format("RandomEffect: id:{0}, max:{1}, min:{2}", randomEffectsGroups[k][l].BoosterImplantEffect, randomEffectsGroups[k][l].MaxValue, randomEffectsGroups[k][l].MinValue));
                        }
                        effectsGroup.Add(effect);
                    }
                    if (effectsGroup.Count > 0)
                    {
                        if (EntryPoint.EnableDebugInfo)
                        {
                            Logs.LogMessage(string.Format("RandomEffectGroup index:{0}", k));
                        }
                        booster.RandomEffectsGroups.Add(effectsGroup);
                    }
                    if (EntryPoint.EnableDebugInfo)
                    {
                        Logs.LogMessage(string.Format("Booster_Template index:[{0}][{1}]", i, k));
                        Logs.LogMessage("===========================");
                    }
                    booster_Templates[blocklist[i].ImplantCategory].Add(booster);
                }
            }
        }

        public static void AddOldValidBoosterTemplateDataBlock()
        {

            foreach (string block in OldValidBoosterTemplatesDataBlocks)
            {
            retry:
                BoosterImplantTemplateDataBlock blk = JsonConvert.DeserializeObject<BoosterImplantTemplateDataBlock>(block);

                //增加偏移值避免键值冲突
                blk.persistentID += offset;
                blk.name += "_OLD_" + offset.ToString();

                try
                {
                    GameDataBlockBase<BoosterImplantTemplateDataBlock>.AddBlock(blk);
                }
                catch
                {
                    offset += 1;
                    goto retry;
                }
            }
        }

        public static Dictionary<BoosterImplantCategory, List<Booster_Template>> booster_Templates = new Dictionary<BoosterImplantCategory, List<Booster_Template>>();

        public struct Booster_Template
        {
            public List<uint> Conditions;

            public List<uint> RandomConditions;

            public List<Booster_Effect> Effects;

            public List<List<Booster_Effect>> RandomEffectsGroups;
        }

        public struct Booster_Effect
        {
            public uint id;

            public float maxValue;

            public float minValue;

            public float value;
        }

        private static string[] OldValidBoosterTemplatesDataBlocks =
        {
            //OldBoosterTemplateDataBlocks
            //Rundown-005
            "{\"Deprecated\":false,\"PublicName\":\"PROVISION\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":1.0},\"DropWeight\":2.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[{\"BoosterImplantEffect\":8,\"MinValue\":1.1,\"MaxValue\":1.13}],\"RandomEffects\":[[{\"BoosterImplantEffect\":7,\"MinValue\":1.1,\"MaxValue\":1.15},{\"BoosterImplantEffect\":50,\"MinValue\":1.1,\"MaxValue\":1.15}]],\"ImplantCategory\":0,\"MainEffectType\":2,\"name\":\"Muted_HealthSupport_Revive\",\"internalEnabled\":true,\"persistentID\":1}",
            "{\"Deprecated\":false,\"PublicName\":\"DETOX\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":1.0},\"DropWeight\":1.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[{\"BoosterImplantEffect\":12,\"MinValue\":1.15,\"MaxValue\":1.25}],\"RandomEffects\":[],\"ImplantCategory\":0,\"MainEffectType\":2,\"name\":\"Muted_HealthSupport_InfectionRes\",\"internalEnabled\":true,\"persistentID\":22}",
            "{\"Deprecated\":false,\"PublicName\":\"RECOVERY\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":1.0},\"DropWeight\":3.0,\"Conditions\":[],\"RandomConditions\":[5,26],\"Effects\":[{\"BoosterImplantEffect\":6,\"MinValue\":1.15,\"MaxValue\":1.25}],\"RandomEffects\":[],\"ImplantCategory\":0,\"MainEffectType\":2,\"name\":\"Muted_Health_RegenSpeed\",\"internalEnabled\":true,\"persistentID\":18}",
            "{\"Deprecated\":false,\"PublicName\":\"PROTEC\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":1.0},\"DropWeight\":4.0,\"Conditions\":[],\"RandomConditions\":[5,29],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":10,\"MinValue\":1.05,\"MaxValue\":1.1},{\"BoosterImplantEffect\":11,\"MinValue\":1.05,\"MaxValue\":1.1}]],\"ImplantCategory\":0,\"MainEffectType\":2,\"name\":\"Muted_Health_Single_Resistance\",\"internalEnabled\":true,\"persistentID\":23}",
            "{\"Deprecated\":false,\"PublicName\":\"PROTEC+\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":1.0},\"DropWeight\":4.0,\"Conditions\":[],\"RandomConditions\":[1,7],\"Effects\":[{\"BoosterImplantEffect\":10,\"MinValue\":1.05,\"MaxValue\":1.1},{\"BoosterImplantEffect\":11,\"MinValue\":1.05,\"MaxValue\":1.1}],\"RandomEffects\":[],\"ImplantCategory\":0,\"MainEffectType\":2,\"name\":\"Muted_Health_Double_Resistance\",\"internalEnabled\":true,\"persistentID\":24}",
            "{\"Deprecated\":false,\"PublicName\":\"STEROIDS\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":1.0},\"DropWeight\":2.0,\"Conditions\":[],\"RandomConditions\":[1,29],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":54,\"MinValue\":1.05,\"MaxValue\":1.1}]],\"ImplantCategory\":0,\"MainEffectType\":1,\"name\":\"Muted_MeleeDamage\",\"internalEnabled\":true,\"persistentID\":4}",
            "{\"Deprecated\":false,\"PublicName\":\"STIM SHOT\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":1.0},\"DropWeight\":7.0,\"Conditions\":[],\"RandomConditions\":[1,7,5],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":52,\"MinValue\":1.05,\"MaxValue\":1.1},{\"BoosterImplantEffect\":53,\"MinValue\":1.05,\"MaxValue\":1.1}]],\"ImplantCategory\":0,\"MainEffectType\":1,\"name\":\"Muted_WeaponDamage\",\"internalEnabled\":true,\"persistentID\":25}",
            "{\"Deprecated\":false,\"PublicName\":\"DEX\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":1.0},\"DropWeight\":4.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":27,\"MinValue\":1.15,\"MaxValue\":1.25},{\"BoosterImplantEffect\":28,\"MinValue\":1.1,\"MaxValue\":1.15},{\"BoosterImplantEffect\":29,\"MinValue\":1.05,\"MaxValue\":1.1},{\"BoosterImplantEffect\":31,\"MinValue\":1.1,\"MaxValue\":1.15},{\"BoosterImplantEffect\":32,\"MinValue\":1.2,\"MaxValue\":1.3}]],\"ImplantCategory\":0,\"MainEffectType\":4,\"name\":\"Muted_ToolStrength\",\"internalEnabled\":true,\"persistentID\":7}",
            "{\"Deprecated\":false,\"PublicName\":\"DEX LITE\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":1.0},\"DropWeight\":2.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":40,\"MinValue\":1.2,\"MaxValue\":1.3},{\"BoosterImplantEffect\":39,\"MinValue\":1.2,\"MaxValue\":1.3},{\"BoosterImplantEffect\":33,\"MinValue\":1.05,\"MaxValue\":1.1}]],\"ImplantCategory\":0,\"MainEffectType\":4,\"name\":\"Muted_ToolWeakEffects\",\"internalEnabled\":true,\"persistentID\":20}",
            "{\"Deprecated\":false,\"PublicName\":\"AXON\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":1.0},\"DropWeight\":1.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[{\"BoosterImplantEffect\":34,\"MinValue\":1.2,\"MaxValue\":1.4},{\"BoosterImplantEffect\":35,\"MinValue\":1.05,\"MaxValue\":1.1}],\"RandomEffects\":[],\"ImplantCategory\":0,\"MainEffectType\":3,\"name\":\"Muted_ProcessingSpeed\",\"internalEnabled\":true,\"persistentID\":10}",
            "{\"Deprecated\":false,\"PublicName\":\"PURE\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":1.0},\"DropWeight\":2.0,\"Conditions\":[],\"RandomConditions\":[7,27],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":41,\"MinValue\":1.13,\"MaxValue\":1.18}]],\"ImplantCategory\":0,\"MainEffectType\":3,\"name\":\"Muted_BioscanSpeed\",\"internalEnabled\":true,\"persistentID\":21}",
            "{\"Deprecated\":false,\"PublicName\":\"SOLID\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":1.0},\"DropWeight\":4.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":36,\"MinValue\":1.1,\"MaxValue\":1.2},{\"BoosterImplantEffect\":37,\"MinValue\":1.1,\"MaxValue\":1.2},{\"BoosterImplantEffect\":38,\"MinValue\":1.1,\"MaxValue\":1.2},{\"BoosterImplantEffect\":5,\"MinValue\":1.15,\"MaxValue\":1.25}]],\"ImplantCategory\":0,\"MainEffectType\":0,\"name\":\"Muted_InitialState\",\"internalEnabled\":true,\"persistentID\":13}",
            "{\"Deprecated\":false,\"PublicName\":\"PROVISION\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":2.0},\"DropWeight\":1.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[{\"BoosterImplantEffect\":7,\"MinValue\":1.2,\"MaxValue\":1.3},{\"BoosterImplantEffect\":8,\"MinValue\":1.2,\"MaxValue\":1.3},{\"BoosterImplantEffect\":50,\"MinValue\":1.2,\"MaxValue\":1.3}],\"RandomEffects\":[],\"ImplantCategory\":1,\"MainEffectType\":2,\"name\":\"Bold_HealthSupport_Revive\",\"internalEnabled\":true,\"persistentID\":26}",
            "{\"Deprecated\":false,\"PublicName\":\"DETOX\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":2.0},\"DropWeight\":1.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[{\"BoosterImplantEffect\":12,\"MinValue\":1.4,\"MaxValue\":1.6}],\"RandomEffects\":[],\"ImplantCategory\":1,\"MainEffectType\":2,\"name\":\"Bold_HealthSupport_InfectionRes\",\"internalEnabled\":true,\"persistentID\":27}",
            "{\"Deprecated\":false,\"PublicName\":\"RECOVERY\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":2.0},\"DropWeight\":2.0,\"Conditions\":[],\"RandomConditions\":[5,26],\"Effects\":[{\"BoosterImplantEffect\":6,\"MinValue\":1.6,\"MaxValue\":2.0}],\"RandomEffects\":[],\"ImplantCategory\":1,\"MainEffectType\":2,\"name\":\"Bold_Health_RegenSpeed\",\"internalEnabled\":true,\"persistentID\":28}",
            "{\"Deprecated\":false,\"PublicName\":\"PROTEC\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":2.0},\"DropWeight\":4.0,\"Conditions\":[],\"RandomConditions\":[5,29],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":10,\"MinValue\":1.15,\"MaxValue\":1.2},{\"BoosterImplantEffect\":11,\"MinValue\":1.15,\"MaxValue\":1.2}]],\"ImplantCategory\":1,\"MainEffectType\":2,\"name\":\"Bold_Health_Single_Resistance\",\"internalEnabled\":true,\"persistentID\":29}",
            "{\"Deprecated\":false,\"PublicName\":\"PROTEC+\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":2.0},\"DropWeight\":2.0,\"Conditions\":[],\"RandomConditions\":[1,7],\"Effects\":[{\"BoosterImplantEffect\":10,\"MinValue\":1.15,\"MaxValue\":1.2},{\"BoosterImplantEffect\":11,\"MinValue\":1.15,\"MaxValue\":1.2}],\"RandomEffects\":[],\"ImplantCategory\":1,\"MainEffectType\":2,\"name\":\"Bold_Health_Double_Resistance\",\"internalEnabled\":true,\"persistentID\":30}",
            "{\"Deprecated\":false,\"PublicName\":\"STEROIDS\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":2.0},\"DropWeight\":2.0,\"Conditions\":[],\"RandomConditions\":[1,29],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":54,\"MinValue\":1.2,\"MaxValue\":1.3}]],\"ImplantCategory\":1,\"MainEffectType\":1,\"name\":\"Bold_MeleeDamage\",\"internalEnabled\":true,\"persistentID\":31}",
            "{\"Deprecated\":false,\"PublicName\":\"STEROIDS\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":2.0},\"DropWeight\":2.0,\"Conditions\":[],\"RandomConditions\":[1,29],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":54,\"MinValue\":1.2,\"MaxValue\":1.3}]],\"ImplantCategory\":1,\"MainEffectType\":1,\"name\":\"Bold_MeleeDamage\",\"internalEnabled\":true,\"persistentID\":31}",
            "{\"Deprecated\":false,\"PublicName\":\"STIM SHOT\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":2.0},\"DropWeight\":7.0,\"Conditions\":[],\"RandomConditions\":[1,7,5],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":52,\"MinValue\":1.15,\"MaxValue\":1.2},{\"BoosterImplantEffect\":53,\"MinValue\":1.15,\"MaxValue\":1.2}]],\"ImplantCategory\":1,\"MainEffectType\":1,\"name\":\"Bold_WeaponDamage\",\"internalEnabled\":true,\"persistentID\":32}",
            "{\"Deprecated\":false,\"PublicName\":\"DEX\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":2.0},\"DropWeight\":5.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":27,\"MinValue\":1.4,\"MaxValue\":1.6},{\"BoosterImplantEffect\":28,\"MinValue\":1.2,\"MaxValue\":1.25},{\"BoosterImplantEffect\":29,\"MinValue\":1.15,\"MaxValue\":1.2},{\"BoosterImplantEffect\":31,\"MinValue\":1.2,\"MaxValue\":1.3},{\"BoosterImplantEffect\":32,\"MinValue\":1.45,\"MaxValue\":1.55}],[{\"BoosterImplantEffect\":39,\"MinValue\":1.2,\"MaxValue\":1.3},{\"BoosterImplantEffect\":33,\"MinValue\":1.12,\"MaxValue\":1.18},{\"BoosterImplantEffect\":40,\"MinValue\":1.3,\"MaxValue\":1.4}]],\"ImplantCategory\":1,\"MainEffectType\":4,\"name\":\"Bold_ToolStrength\",\"internalEnabled\":true,\"persistentID\":33}",
            "{\"Deprecated\":false,\"PublicName\":\"AXON\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":2.0},\"DropWeight\":1.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[{\"BoosterImplantEffect\":34,\"MinValue\":1.6,\"MaxValue\":1.8},{\"BoosterImplantEffect\":35,\"MinValue\":1.12,\"MaxValue\":1.18}],\"RandomEffects\":[[{\"BoosterImplantEffect\":49,\"MinValue\":0.91,\"MaxValue\":0.95}]],\"ImplantCategory\":1,\"MainEffectType\":3,\"name\":\"Bold_ProcessingSpeed\",\"internalEnabled\":true,\"persistentID\":35}",
            "{\"Deprecated\":false,\"PublicName\":\"PURE\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":2.0},\"DropWeight\":2.0,\"Conditions\":[],\"RandomConditions\":[7,27],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":41,\"MinValue\":1.13,\"MaxValue\":1.18}]],\"ImplantCategory\":1,\"MainEffectType\":3,\"name\":\"Bold_BioscanSpeed\",\"internalEnabled\":true,\"persistentID\":36}",
            "{\"Deprecated\":false,\"PublicName\":\"SOLID\",\"Description\":\"\",\"DurationRange\":{\"x\":1.0,\"y\":2.0},\"DropWeight\":7.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":36,\"MinValue\":1.25,\"MaxValue\":1.35},{\"BoosterImplantEffect\":37,\"MinValue\":1.25,\"MaxValue\":1.35},{\"BoosterImplantEffect\":38,\"MinValue\":1.25,\"MaxValue\":1.35},{\"BoosterImplantEffect\":5,\"MinValue\":1.4,\"MaxValue\":1.6}],[{\"BoosterImplantEffect\":6,\"MinValue\":0.8,\"MaxValue\":0.87},{\"BoosterImplantEffect\":34,\"MinValue\":0.71,\"MaxValue\":0.83}]],\"ImplantCategory\":1,\"MainEffectType\":0,\"name\":\"Bold_InitialState\",\"internalEnabled\":true,\"persistentID\":37}",
            "{\"Deprecated\":false,\"PublicName\":\"PROVISION\",\"Description\":\"\",\"DurationRange\":{\"x\":2.0,\"y\":3.0},\"DropWeight\":4.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[{\"BoosterImplantEffect\":7,\"MinValue\":1.4,\"MaxValue\":1.5},{\"BoosterImplantEffect\":8,\"MinValue\":1.8,\"MaxValue\":2.0},{\"BoosterImplantEffect\":50,\"MinValue\":1.4,\"MaxValue\":1.5}],\"RandomEffects\":[[{\"BoosterImplantEffect\":54,\"MinValue\":0.91,\"MaxValue\":0.95},{\"BoosterImplantEffect\":12,\"MinValue\":0.8,\"MaxValue\":0.87},{\"BoosterImplantEffect\":11,\"MinValue\":0.8,\"MaxValue\":0.87}]],\"ImplantCategory\":2,\"MainEffectType\":2,\"name\":\"Aggressive_HealthSupport_Revive\",\"internalEnabled\":true,\"persistentID\":38}",
            "{\"Deprecated\":false,\"PublicName\":\"DETOX\",\"Description\":\"\",\"DurationRange\":{\"x\":2.0,\"y\":3.0},\"DropWeight\":1.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[{\"BoosterImplantEffect\":12,\"MinValue\":1.8,\"MaxValue\":2.0}],\"RandomEffects\":[[{\"BoosterImplantEffect\":54,\"MinValue\":0.91,\"MaxValue\":0.95}]],\"ImplantCategory\":2,\"MainEffectType\":2,\"name\":\"Aggressive_HealthSupport_InfectionRes\",\"internalEnabled\":true,\"persistentID\":39}",
            "{\"Deprecated\":false,\"PublicName\":\"RECOVERY\",\"Description\":\"\",\"DurationRange\":{\"x\":2.0,\"y\":3.0},\"DropWeight\":2.0,\"Conditions\":[],\"RandomConditions\":[5,26],\"Effects\":[{\"BoosterImplantEffect\":6,\"MinValue\":2.2,\"MaxValue\":2.5}],\"RandomEffects\":[],\"ImplantCategory\":2,\"MainEffectType\":2,\"name\":\"Aggressive_Health_RegenSpeed\",\"internalEnabled\":true,\"persistentID\":40}",
            "{\"Deprecated\":false,\"PublicName\":\"PROTEC\",\"Description\":\"\",\"DurationRange\":{\"x\":2.0,\"y\":3.0},\"DropWeight\":4.0,\"Conditions\":[],\"RandomConditions\":[5,29],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":10,\"MinValue\":1.3,\"MaxValue\":1.4},{\"BoosterImplantEffect\":11,\"MinValue\":1.3,\"MaxValue\":1.4}]],\"ImplantCategory\":2,\"MainEffectType\":2,\"name\":\"Aggressive_Health_Single_Resistance\",\"internalEnabled\":true,\"persistentID\":41}",
            "{\"Deprecated\":false,\"PublicName\":\"PROTEC+\",\"Description\":\"\",\"DurationRange\":{\"x\":2.0,\"y\":3.0},\"DropWeight\":4.0,\"Conditions\":[],\"RandomConditions\":[1,7],\"Effects\":[{\"BoosterImplantEffect\":10,\"MinValue\":1.3,\"MaxValue\":1.4},{\"BoosterImplantEffect\":11,\"MinValue\":1.3,\"MaxValue\":1.4}],\"RandomEffects\":[],\"ImplantCategory\":2,\"MainEffectType\":2,\"name\":\"Aggressive_Health_Double_Resistance\",\"internalEnabled\":true,\"persistentID\":42}",
            "{\"Deprecated\":false,\"PublicName\":\"STEROIDS\",\"Description\":\"\",\"DurationRange\":{\"x\":2.0,\"y\":3.0},\"DropWeight\":1.0,\"Conditions\":[],\"RandomConditions\":[1,29],\"Effects\":[{\"BoosterImplantEffect\":49,\"MinValue\":1.5,\"MaxValue\":1.6}],\"RandomEffects\":[[{\"BoosterImplantEffect\":11,\"MinValue\":0.83,\"MaxValue\":0.9}]],\"ImplantCategory\":2,\"MainEffectType\":1,\"name\":\"Aggressive_MeleeDamage\",\"internalEnabled\":true,\"persistentID\":43}",
            "{\"Deprecated\":false,\"PublicName\":\"ADROIT\",\"Description\":\"\",\"DurationRange\":{\"x\":2.0,\"y\":3.0},\"DropWeight\":7.0,\"Conditions\":[],\"RandomConditions\":[1,7,5],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":52,\"MinValue\":1.25,\"MaxValue\":1.3},{\"BoosterImplantEffect\":53,\"MinValue\":1.25,\"MaxValue\":1.3}]],\"ImplantCategory\":2,\"MainEffectType\":1,\"name\":\"Aggressive_WeaponDamage\",\"internalEnabled\":true,\"persistentID\":44}",
            "{\"Deprecated\":false,\"PublicName\":\"DEX\",\"Description\":\"\",\"DurationRange\":{\"x\":2.0,\"y\":3.0},\"DropWeight\":6.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":27,\"MinValue\":1.8,\"MaxValue\":2.0},{\"BoosterImplantEffect\":28,\"MinValue\":1.3,\"MaxValue\":1.4},{\"BoosterImplantEffect\":29,\"MinValue\":1.25,\"MaxValue\":1.3},{\"BoosterImplantEffect\":31,\"MinValue\":1.35,\"MaxValue\":1.4},{\"BoosterImplantEffect\":32,\"MinValue\":1.7,\"MaxValue\":2.0}],[{\"BoosterImplantEffect\":40,\"MinValue\":1.35,\"MaxValue\":1.4},{\"BoosterImplantEffect\":39,\"MinValue\":1.35,\"MaxValue\":1.4},{\"BoosterImplantEffect\":33,\"MinValue\":1.2,\"MaxValue\":1.3}],[{\"BoosterImplantEffect\":49,\"MinValue\":0.77,\"MaxValue\":0.83},{\"BoosterImplantEffect\":11,\"MinValue\":0.83,\"MaxValue\":0.87},{\"BoosterImplantEffect\":12,\"MinValue\":0.62,\"MaxValue\":0.71}]],\"ImplantCategory\":2,\"MainEffectType\":4,\"name\":\"Aggressive_ToolStrength\",\"internalEnabled\":true,\"persistentID\":45}",
            "{\"Deprecated\":false,\"PublicName\":\"AXON\",\"Description\":\"\",\"DurationRange\":{\"x\":2.0,\"y\":3.0},\"DropWeight\":6.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":34,\"MinValue\":2.0,\"MaxValue\":2.2},{\"BoosterImplantEffect\":35,\"MinValue\":1.2,\"MaxValue\":1.3}],[{\"BoosterImplantEffect\":28,\"MinValue\":1.3,\"MaxValue\":1.4},{\"BoosterImplantEffect\":33,\"MinValue\":1.2,\"MaxValue\":1.3},{\"BoosterImplantEffect\":50,\"MinValue\":1.4,\"MaxValue\":1.5}],[{\"BoosterImplantEffect\":49,\"MinValue\":0.77,\"MaxValue\":0.83},{\"BoosterImplantEffect\":11,\"MinValue\":0.83,\"MaxValue\":0.87}]],\"ImplantCategory\":2,\"MainEffectType\":3,\"name\":\"Aggressive_ProcessingSpeed\",\"internalEnabled\":true,\"persistentID\":47}",
            "{\"Deprecated\":false,\"PublicName\":\"PURE\",\"Description\":\"\",\"DurationRange\":{\"x\":2.0,\"y\":3.0},\"DropWeight\":2.0,\"Conditions\":[],\"RandomConditions\":[7,27],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":41,\"MinValue\":1.2,\"MaxValue\":1.25}]],\"ImplantCategory\":2,\"MainEffectType\":3,\"name\":\"Aggressive_BioscanSpeed\",\"internalEnabled\":true,\"persistentID\":48}",
            "{\"Deprecated\":false,\"PublicName\":\"SOLID\",\"Description\":\"\",\"DurationRange\":{\"x\":2.0,\"y\":3.0},\"DropWeight\":7.0,\"Conditions\":[],\"RandomConditions\":[],\"Effects\":[],\"RandomEffects\":[[{\"BoosterImplantEffect\":36,\"MinValue\":1.4,\"MaxValue\":1.53},{\"BoosterImplantEffect\":37,\"MinValue\":1.4,\"MaxValue\":1.53},{\"BoosterImplantEffect\":38,\"MinValue\":1.4,\"MaxValue\":1.53},{\"BoosterImplantEffect\":5,\"MinValue\":1.8,\"MaxValue\":2.0}],[{\"BoosterImplantEffect\":11,\"MinValue\":0.83,\"MaxValue\":0.87},{\"BoosterImplantEffect\":12,\"MinValue\":0.62,\"MaxValue\":0.71},{\"BoosterImplantEffect\":34,\"MinValue\":0.56,\"MaxValue\":0.62}]],\"ImplantCategory\":2,\"MainEffectType\":0,\"name\":\"Aggressive_InitialState\",\"internalEnabled\":true,\"persistentID\":49}",
        };
        private static uint offset = 41985;

        //private static Dictionary<int, List<Booster_Template>> playerBoosters = new Dictionary<int, List<Booster_Template>>();
    }
}
