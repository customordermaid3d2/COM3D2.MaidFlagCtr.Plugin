using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace COM3D2.MaidFlagCtr.Plugin
{
    public static class MyAttribute
    {
        public const string PLAGIN_NAME = "MaidFlagCtr";
        public const string PLAGIN_VERSION = "22.3.25";
        public const string PLAGIN_FULL_NAME = "COM3D2.MaidFlagCtr.Plugin";
    }

    [BepInPlugin(MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_NAME, MyAttribute.PLAGIN_VERSION)]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임
    [BepInProcess("COM3D2x64.exe")]
    public class MaidFlagCtr : BaseUnityPlugin
    {

        Harmony harmony;

        public static ManualLogSource MyLog;

        public void Awake()
        {
            MyLog = Logger;

            MyLog.LogMessage("Awake");

            MaidFlagCtrPatch.init(Config, MyAttribute.PLAGIN_FULL_NAME);
            MaidFlagCtrGUI.init(Config, Logger);
        }


        public void OnEnable()
        {
            MyLog.LogMessage("OnEnable");

            SceneManager.sceneLoaded += this.OnSceneLoaded;
            MaidFlagCtrPatch.JSONLoad();
            // 하모니 패치
            harmony = Harmony.CreateAndPatchAll(typeof(MaidFlagCtrPatch));            
        }

        public void OnDisable()
        {
            MaidFlagCtrPatch.JSONSave();
            harmony?.UnpatchSelf();
        }

        public void OnGUI()
        {
            MaidFlagCtrGUI.OnGUI();
        }

        public static string scene_name = string.Empty;

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            scene_name = scene.name;
            MaidFlagCtrGUI.SetingFlag();
        }

    }
}
