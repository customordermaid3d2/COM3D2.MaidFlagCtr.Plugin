using BepInEx.Configuration;

using COM3D2API;
using LillyUtill.MyWindowRect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace COM3D2.MaidFlagCtr.Plugin
{


    static class MaidFlagCtrGUI
    {
        internal static ConfigFile Config;

        private const float wScrol = 290;
        private const float wFild = 225;
        private const float wValue = 35;
        private const float wBtn = 265;

        private static readonly string[] typesAll = new string[] { "new", "old" };
        private static readonly string[] typesone = new string[] { "new" };
        private static string[] types = new string[] { "new", "old" };

        private static string[] maidPleyars = new string[] { "maid", "Pleyar" };
        private static int selectedmaidPleyars;

        private static string flagName = string.Empty;
        private static string flagValueS = string.Empty;

        private static int flagValue = 1;
        private static int type = 0;

        private static int selectedFlag;

        private static Vector2 scrollPosition;

        private static event Action action = SetBodyFlag;

        private static Maid maid;

        /// <summary>
        /// Key : maid.flags.key + maid.flags.value
        /// value : maid.flags.key
        /// </summary>
        public static Dictionary<string, string> flags;

        public static string[] flagsStats = new string[] { };

        public static string[] flagsKey = new string[] { };

        public static WindowRectUtill myWindowRect;


        public static void init(ConfigFile Config, BepInEx.Logging.ManualLogSource logger)
        {
            MaidFlagCtrGUI.Config = Config;
            
            myWindowRect = new WindowRectUtill(Config, logger, MyAttribute.PLAGIN_NAME, "Flag");

            SystemShortcutAPI.AddButton(
                MyAttribute.PLAGIN_FULL_NAME, 
                new Action(delegate () { myWindowRect.IsGUIOnOffChg(); }), 
                MyAttribute.PLAGIN_NAME , 
                (Properties.Resources.icon));


        }

        public static void OnGUI()
        {
            if (!myWindowRect.IsGUIOn)
            {
                return;
            }
            
            // 윈도우 리사이즈시 밖으로 나가버리는거 방지
            myWindowRect.WindowRect = GUILayout.Window(myWindowRect.winNum, myWindowRect.WindowRect, MaidFlagCtrGUI.WindowFunction, "", GUI.skin.box);
        }

        internal static void WindowFunction(int id)
        {
            // base.SetBody();
            GUILayout.BeginHorizontal();
            GUILayout.Label(myWindowRect.windowName, GUILayout.Height(20));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { myWindowRect.IsOpen = !myWindowRect.IsOpen; }
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20))) { myWindowRect.IsGUIOn = false; }
            GUILayout.EndHorizontal();

            if (myWindowRect.IsOpen)
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Width(wScrol));

                if (GUILayout.Button("All Maid Flag Setting"))//, guio[GUILayoutOptionUtill.Type.Width, 20]
                {
                    MaidFlagCtrPatch.SetFlagsAll();
                }

                selectedmaidPleyars = GUILayout.SelectionGrid(selectedmaidPleyars, maidPleyars, 2);

                if (selectedmaidPleyars == 1)
                {
                    if (GUI.changed)
                    {
                        SetingFlag();
                    }

                    SetBodyFlagPleyar();
                }
                else
                {
                    #region maid
                    GUI.enabled = MaidFlagCtr.scene_name == "SceneMaidManagement";
                    if (!GUI.enabled)
                    {
                        GUILayout.Label("Scene Maid Management Need");
                        //return;
                    }
                    else
                    {
                        if (GUI.changed)
                        {
                            SetingFlag(maid);
                        }

                        GUILayout.Label("* info");
                        GUILayout.Label(maid.status.fullNameEnStyle);
                        GUILayout.Label("heroineType : " + maid.status.heroineType);
                        GUILayout.Label("voiceGroup : " + maid.status.voiceGroup);
                        GUILayout.Label("feeling : " + maid.status.feeling);
                        GUILayout.Label("relation : " + maid.status.relation);
                        GUILayout.Label("additionalRelation : " + maid.status.additionalRelation);
                        GUILayout.Label("boMabataki : " + maid.boMabataki);
                        GUILayout.Label("boMAN : " + maid.boMAN);
                        GUILayout.Label("boNPC : " + maid.boNPC);

                        type = GUILayout.SelectionGrid(type, types, 2);

                        if (GUI.changed)
                        {
                            SetingFlag(maid);
                        }

                        action();
                    }

                    #endregion
                }

                GUILayout.EndScrollView();

            }
            GUI.enabled = true;
            GUI.DragWindow();
        }

        private static void SetBodyFlagPleyar()
        {



            GUILayout.Label("* flag name , flag value(int)");

            GUILayout.BeginHorizontal();
            flagName = GUILayout.TextField(flagName, GUILayout.Width(wFild));
            flagValueS = GUILayout.TextField(flagValue.ToString("D"), GUILayout.Width(wValue));
            if (GUI.changed)
            {
                int.TryParse(flagValueS, out flagValue);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("add"))//, guio[GUILayoutOptionUtill.Type.Width, 20]
            {
                if (!string.IsNullOrEmpty(flagName))
                {
                    GameMain.Instance.CharacterMgr.status.AddFlag(flagName, flagValue);
                    SetingFlag();
                }
            }
            if (GUILayout.Button("set"))//, guio[GUILayoutOptionUtill.Type.Width, 20]
            {
                if (!string.IsNullOrEmpty(flagName))
                {
                    GameMain.Instance.CharacterMgr.status.SetFlag(flagName, flagValue);
                    SetingFlag();
                }
            }
            if (GUILayout.Button("del"))//, guio[GUILayoutOptionUtill.Type.Width, 20]
            {
                if (!string.IsNullOrEmpty(flagName))
                {
                    GameMain.Instance.CharacterMgr.status.RemoveFlag(flagName);
                    SetingFlag();
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("have flag count : " + flags.Count);
            if (GUILayout.Button("to log"))//, guio[GUILayoutOptionUtill.Type.Width, 20]
            {
                FlagsToLog();
            }

            GUILayout.EndHorizontal();

            selectedFlag = GUILayout.SelectionGrid(selectedFlag, flagsStats, 1, GUILayout.Width(wBtn));

            if (GUI.changed)
            {
                if (flagsKey.Length>0)
                {
                flagName = flagsKey[selectedFlag];
                flagValue = GameMain.Instance.CharacterMgr.status.GetFlag(flagName);
                }
                else
                {
                    flagName = string.Empty;
                    flagValue = 1;
                }
            }

            GUILayout.Label("warning! all flag del");
            GUILayout.BeginHorizontal();
            GUILayout.Label("warning! all flag del => ");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("del", GUILayout.Width(40)))
            {
                foreach (var item in flags)
                {
                    //MaidFlagCtr.MyLog.LogMessage("del flag", item.Key);
                    GameMain.Instance.CharacterMgr.status.RemoveFlag(item.Value);
                }
                SetingFlag();
            }
            GUILayout.EndHorizontal();
        }

        private static void FlagsToLog()
        {
            MaidFlagCtr.MyLog.LogMessage("=== FlagsToLog st ===");
            foreach (var item in flagsStats)
            {
                MaidFlagCtr.MyLog.LogMessage( item);
            }
            MaidFlagCtr.MyLog.LogMessage("=== FlagsToLog ed ===");            
        }

        private static void SetBodyFlag()
        {
            GUILayout.Label("flag name , flag value(int)");

            GUILayout.BeginHorizontal();
            flagName = GUILayout.TextField(flagName, GUILayout.Width(wFild));
            flagValueS = GUILayout.TextField(flagValue.ToString("D"), GUILayout.Width(wValue));
            if (GUI.changed)
            {
                int.TryParse(flagValueS, out flagValue);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("add"))//, guio[GUILayoutOptionUtill.Type.Width, 20]
            {
                if (!string.IsNullOrEmpty(flagName))
                {
                    maid.status.AddFlag(flagName, flagValue);
                    SetingFlag(maid);
                }
            }
            if (GUILayout.Button("set"))//, guio[GUILayoutOptionUtill.Type.Width, 20]
            {
                if (!string.IsNullOrEmpty(flagName))
                {
                    maid.status.SetFlag(flagName, flagValue);
                    SetingFlag(maid);
                }
            }
            if (GUILayout.Button("del"))//, guio[GUILayoutOptionUtill.Type.Width, 20]
            {
                if (!string.IsNullOrEmpty(flagName))
                {
                    maid.status.RemoveFlag(flagName);
                    SetingFlag(maid);
                }
            }

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("have flag count : " + flags.Count);
            if (GUILayout.Button("to log"))//, guio[GUILayoutOptionUtill.Type.Width, 20]
            {
                FlagsToLog();
            }
            GUILayout.EndHorizontal();


            selectedFlag = GUILayout.SelectionGrid(selectedFlag, flagsStats, 1, GUILayout.Width(wBtn));

            if (GUI.changed)
            {
                if (flagsKey.Length > 0)
                {
                    flagName = flagsKey[selectedFlag];
                    flagValue = maid.status.GetFlag(flagName);
                }
                else
                {
                    flagName = string.Empty;
                    flagValue = 1;
                }
            }

            GUILayout.Label("warning! all flag del");
            GUILayout.BeginHorizontal();
            GUILayout.Label("warning! all flag del => ");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("del", GUILayout.Width(40)))
            {
                maid.status.RemoveFlagAll();
                SetingFlag(maid);
            }
            GUILayout.EndHorizontal();
        }


        private static void SetBodyFlagOld()
        {
            GUILayout.Label("flag Set : name , value(int)");
            GUILayout.BeginHorizontal();
            flagName = GUILayout.TextField(flagName, GUILayout.Width(wFild));
            flagValueS = GUILayout.TextField(flagValue.ToString("D"), GUILayout.Width(wValue));
            if (GUI.changed)
            {
                int.TryParse(flagValueS, out flagValue);
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("add"))//, guio[GUILayoutOptionUtill.Type.Width, 20]
            {
                if (string.IsNullOrEmpty(flagName))
                {
                    maid.status.OldStatus.AddFlag(flagName, flagValue);
                    SetingFlag(maid);
                }
            }
            if (GUILayout.Button("set 0"))//, guio[GUILayoutOptionUtill.Type.Width, 20]
            {
                if (string.IsNullOrEmpty(flagName))
                {
                    maid.status.OldStatus.SetFlag(flagName, flagValue);
                    SetingFlag(maid);
                }
            }
            if (GUILayout.Button("del"))//, guio[GUILayoutOptionUtill.Type.Width, 20]
            {
                if (string.IsNullOrEmpty(flagName))
                {
                    maid.status.OldStatus.RemoveFlag(flagName);
                    SetingFlag(maid);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("have flag count : " + flags.Count);

            selectedFlag = GUILayout.SelectionGrid(selectedFlag, flagsStats, 1, GUILayout.Width(wBtn));
            if (GUI.changed)
            {
                if (flagsKey.Length>0)
                {                
                flagName = flagsKey[selectedFlag];
                flagValue = maid.status.OldStatus.GetFlag(flagName);
            }
            else
            {
                flagName = string.Empty;
                flagValue = 1;
            }
        }

            GUILayout.Label("warning! all flag del");
            GUILayout.BeginHorizontal();
            GUILayout.Label("warning! all flag del => ");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("del", GUILayout.Width(40)))
            {
                foreach (var item in flags)
                {
                    maid.status.OldStatus.RemoveFlag(item.Value);
                }
                SetingFlag(maid);
            }
            GUILayout.EndHorizontal();

        }


        public static void SetingFlag(Maid maid)
        {
            MaidFlagCtrGUI.maid = maid;
            if (maid.status.OldStatus == null)
            {
                types = typesone;
                type = 0;
            }
            else
            {
                types = typesAll;
            }

            switch (type)
            {
                case 0:
                    action = SetBodyFlag;
                    flags = maid.status.flags.ToDictionary(x => x.Key + " , " + x.Value, x => x.Key);
                    break;
                case 1:
                    action = SetBodyFlagOld;
                    flags = maid.status.OldStatus.flags.ToDictionary(x => x.Key + " , " + x.Value, x => x.Key);
                    break;
                default:
                    break;
            }
            flagsKey = flags.Values.ToArray();
            flagsStats = flags.Keys.ToArray();

        }

        public static void SetingFlag()
        {
            if (selectedmaidPleyars == 1)
            {
                flags = GameMain.Instance.CharacterMgr.status.flags.ToDictionary(x => x.Key + " , " + x.Value, x => x.Key);
            flagsKey = flags.Values.ToArray();
            flagsStats = flags.Keys.ToArray();
            }
        }
    }


}
