using BepInEx.Configuration;
using COM3D2.Lilly.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace COM3D2.MaidFlagCtr.Plugin
{


    class MyGUI
    {
        internal static ConfigFile Config;

        private static readonly string[] typesAll = new string[] { "new", "old" };
        private static readonly string[] typesone = new string[] { "new" };
        private static string[] types = new string[] { "new", "old" };

        private static string flagName = string.Empty;
        private static string flagValueS = string.Empty;

        private static int flagValue = 1;
        private static int type = 0;

        private static int selectedFlag;

        private static Vector2 scrollPosition;

        private static event Action action = SetBodyFlag;

        private static Maid maid;
        //private static int selectedFlagold;

        /// <summary>
        /// Key : maid.flags.key + maid.flags.value
        /// value : maid.flags.key
        /// </summary>
        public static Dictionary<string, string> flags;
        //public static Dictionary<string, string> flagsOld = new Dictionary<string, string>();

        public static string[] flagsStats = new string[] { };
        //public static string[] flagsOldStats;

        public static string[] flagsKey = new string[] { };
        //public static string[] flagsOldKey;

        public static void init(ConfigFile Config)
        {
            MyGUI.Config = Config;
        }

        internal static void WindowFunction(int id)
        {
            // base.SetBody();
            GUI.enabled = MaidFlagCtr.scene_name == "SceneMaidManagement";

            if (!GUI.enabled)
            {
                GUILayout.Label("Scene Maid Management Need");
                //return;
            }
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

                GUILayout.Label(MyUtill.GetMaidFullName(maid));

                type = GUILayout.SelectionGrid(type, types, 2);

                if (GUI.changed)
                {
                    SetingFlag(maid);
                }

                action();

                GUILayout.EndScrollView();
            }
            GUI.DragWindow();
            GUI.enabled = true;
        }

        private static void SetBodyFlag()
        {
            GUILayout.Label("flag name , flag value(int)");

            GUILayout.BeginHorizontal();
            flagName = GUILayout.TextField(flagName);
            flagValueS = GUILayout.TextField(flagValue.ToString("D"));
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



            GUILayout.Label("have flag count : " + flags.Count);

            selectedFlag = GUILayout.SelectionGrid(selectedFlag, flagsStats, 1);

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
            flagName = GUILayout.TextField(flagName);
            flagValueS = GUILayout.TextField(flagValue.ToString());
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

            selectedFlag = GUILayout.SelectionGrid(selectedFlag, flagsStats, 1);
            if (GUI.changed)
            {
                flagName = flagsKey[selectedFlag];
                flagValue = maid.status.OldStatus.GetFlag(flagName);
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
            MyGUI.maid = maid;
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
    }


}
