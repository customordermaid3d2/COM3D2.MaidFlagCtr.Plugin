using BepInEx;
using BepInEx.Configuration;
using COM3D2.LillyUtill;
using COM3D2API;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace COM3D2.MaidFlagCtr.Plugin
{
    public class MyAttribute
    {
        public const string PLAGIN_NAME = "MaidFlagCtr";
        public const string PLAGIN_VERSION = "21.8.15.12";
        public const string PLAGIN_FULL_NAME = "COM3D2.MaidFlagCtr.Plugin";
    }

    [BepInPlugin(MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_VERSION)]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임
    [BepInProcess("COM3D2x64.exe")]
    public class MaidFlagCtr : BaseUnityPlugin
    {

        Harmony harmony;


        // 단축키 설정파일로 연동
        public static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;

        public static MyLog MyLog;

        public void Awake()
        {
            MyLog = new MyLog(Logger);// BepInEx.Logging.Logger.CreateLogSource("PresetLoadCtr");

            MyLog.LogMessage("Awake");


            // 기어 메뉴 추가. 이 플러그인 기능 자체를 멈추려면 enabled 를 꺽어야함. 그러면 OnEnable(), OnDisable() 이 작동함
            //SystemShortcutAPI.AddButton("PresetLoadCtr", new Action(delegate () { enabled = !enabled; }), "PresetLoadCtr", MyUtill.ExtractResource(Properties.Resources.icon));
            // 단축키 설정파일로 연동
            // 단축키 기본값 설정
            ShowCounter = Config.Bind("GUI", "OnOff", new BepInEx.Configuration.KeyboardShortcut(KeyCode.Alpha7, KeyCode.LeftControl));

            MyGUI.init(Config);
        }


        public void OnEnable()
        {
            MyLog.LogMessage("OnEnable");

            SceneManager.sceneLoaded += this.OnSceneLoaded;

            // 하모니 패치
            harmony = Harmony.CreateAndPatchAll(typeof(MaidFlagCtrPatch));

            MyGUI.myWindowRect.load();
        }

        /*
        public void FixedUpdate()
        {

        }

        public void LateUpdate()
        {

        }
        */
        public void OnDisable()
        {
            MyGUI.myWindowRect.save();
            harmony.UnpatchSelf();
        }


        public void OnGUI()
        {
            MyGUI.OnGUI();
        }

        public static string scene_name = string.Empty;

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            MyLog.LogMessage("OnSceneLoaded", scene.name, scene.buildIndex);
            //  scene.buildIndex 는 쓰지 말자 제발
            scene_name = scene.name;
            MyGUI.myWindowRect.save();

            MyGUI.SetingFlag();
        }
        /*
        public void Pause()
        {

        }

        public void Resume()
        {

        }

        public void Start()
        {

        }
        */
        public void Update()
        {
            //if (ShowCounter.Value.IsDown())
            //{
            //    MyLog.LogMessage("IsDown", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
            //if (ShowCounter.Value.IsPressed())
            //{
            //    MyLog.LogMessage("IsPressed", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
            if (ShowCounter.Value.IsUp())
            {
                MyGUI.isGUIOn = !MyGUI.isGUIOn;
                MyLog.LogMessage("IsUp", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            }
        }

    }
}
