﻿using COM3D2.MaidFlagCtr.Plugin;
using HarmonyLib;
using Newtonsoft.Json;
using scoutmode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace COM3D2.MaidFlagCtr.Plugin
{
    //[MyHarmony(MyHarmonyType.Base)]
    class MaidFlagCtrPatch
    {
        public static string jsonPath;
        public static string PLUGIN_GUID;

        public static Dictionary<string, HashSet<string>> flags = new Dictionary<string, HashSet<string>>();
        public static Dictionary<string, HashSet<string>> flagsOld = new Dictionary<string, HashSet<string>>();
        private static bool isRun;

        public static void init(BepInEx.Configuration.ConfigFile config, string pLAGIN_FULL_NAME)
        {
            jsonPath = Path.GetDirectoryName(config.ConfigFilePath);
            PLUGIN_GUID = pLAGIN_FULL_NAME;
        }

        public static void DeserializeObject<T>(string s, ref T t) where T : new()
        {
            if (File.Exists(jsonPath + $@"\{PLUGIN_GUID}-{s}.json")) t = JsonConvert.DeserializeObject<T>(File.ReadAllText(jsonPath + $@"\{PLUGIN_GUID}-{s}.json"));
            else t = new T();
        }

        public static void JSONLoad()
        {
            DeserializeObject("flags", ref flags);
            DeserializeObject("flagsOld", ref flagsOld);
        }

        public static void JSONSave()
        {
            File.WriteAllText(jsonPath + $@"\{PLUGIN_GUID}-flags.json", JsonConvert.SerializeObject(flags, Formatting.Indented)); // 자동 들여쓰기
            File.WriteAllText(jsonPath + $@"\{PLUGIN_GUID}-flagsOld.json", JsonConvert.SerializeObject(flagsOld, Formatting.Indented)); // 자동 들여쓰기
        }

        // public void SetFlag(string flagName, int value)
        [HarmonyPatch(typeof(MaidStatus.Status), "SetFlag")]
        //[HarmonyPatch(typeof(MaidStatus.Status), "AddFlag")]
        [HarmonyPostfix]
        public static void SetFlag(MaidStatus.Status __instance, string flagName)
        {
            if (!isRun)
            {
                if (!flags.ContainsKey(__instance.personal.replaceText))
                {
                    flags[__instance.personal.replaceText] = new HashSet<string>();
                }
                flags[__instance.personal.replaceText].Add(flagName);
            }
        }

        // public void SetFlag(string flagName, int value)
        [HarmonyPatch(typeof(MaidStatus.Old.Status), "SetFlag")]
        //[HarmonyPatch(typeof(MaidStatus.Old.Status), "AddFlag")]
        [HarmonyPostfix]
        public static void SetFlagOld(MaidStatus.Old.Status __instance, string flagName, MaidStatus.Status ___mainStatus)
        {
            if (!isRun)
            {
                if (!flagsOld.ContainsKey(___mainStatus.personal.replaceText))
                {
                    flagsOld[___mainStatus.personal.replaceText] = new HashSet<string>();
                }
                flagsOld[___mainStatus.personal.replaceText].Add(flagName);
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(MaidManagementMain), "OnSelectChara")]
        public static void OnSelectChara(Maid ___select_maid_, Dictionary<string, UIButton> ___button_dic_, MaidManagementMain __instance)
        {
            MaidFlagCtrGUI.SetingFlag(___select_maid_);
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(GameMain), "Deserialize")]
        public static void GameStart(bool __result)
        {
            if (__result)
            {
                GetFlagsAll();
            }
        }

        internal static void SetFlagsAll()
        {
            isRun = true;
            MaidFlagCtr.MyLog.LogMessage("SetFlagsAll st");
            try
            {
                foreach (var maid in GameMain.Instance.CharacterMgr.GetStockMaidList())
                {
                    MaidFlagCtr.MyLog.LogDebug($"{maid.status.fullNameEnStyle}");

                    if (flags.ContainsKey(maid.status.personal.replaceText))
                    {
                        foreach (var flag in flags[maid.status.personal.replaceText])
                        {
                            if (maid.status.GetFlag(flag) == 0)
                            {
                                MaidFlagCtr.MyLog.LogDebug($"{maid.status.fullNameEnStyle} , now , {flag}");
                                maid.status.SetFlag(flag, 1);
                            }
                        }
                    }
                    if (maid.status.OldStatus != null && flagsOld.ContainsKey(maid.status.personal.replaceText))
                    {
                        foreach (var flag in flagsOld[maid.status.personal.replaceText])
                        {
                            if (maid.status.OldStatus.GetFlag(flag) == 0)
                            {
                                MaidFlagCtr.MyLog.LogDebug($"{maid.status.fullNameEnStyle} , old , {flag}");
                                maid.status.OldStatus.SetFlag(flag, 1);
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                MaidFlagCtr.MyLog.LogError("SetFlagsAll : " + e.ToString());
            }
            MaidFlagCtr.MyLog.LogMessage("SetFlagsAll ed");
            isRun = false;
        }

        internal static void GetFlagsAll()
        {
            MaidFlagCtr.MyLog.LogMessage("GetFlagsAll st");
            foreach (var maid in GameMain.Instance.CharacterMgr.GetStockMaidList())
            {
                MaidFlagCtr.MyLog.LogDebug($"{maid.status.fullNameEnStyle}");

                Dictionary<string, int> flags_;
                System.Reflection.FieldInfo s;
                
                s = AccessTools.Field(typeof(MaidStatus.Status), "flags_");
                flags_= (Dictionary<string, int>)s.GetValue(maid.status);

                if (!flags.ContainsKey(maid.status.personal.replaceText))
                {
                    flags[maid.status.personal.replaceText] = new HashSet<string>();
                }
                foreach (var item in flags_.Keys)
                {
                    flags[maid.status.personal.replaceText].Add(item);
                }

                if (maid.status.OldStatus == null) continue;

                s = AccessTools.Field(typeof(MaidStatus.Old.Status), "flags_");
                flags_= (Dictionary<string, int>)s.GetValue(maid.status);

                if (!flagsOld.ContainsKey(maid.status.personal.replaceText))
                {
                    flagsOld[maid.status.personal.replaceText] = new HashSet<string>();
                }
                foreach (var item in flags_.Keys)
                {
                    flagsOld[maid.status.personal.replaceText].Add(item);
                }
            }
            MaidFlagCtr.MyLog.LogMessage("GetFlagsAll ed");
        }
    }
}
