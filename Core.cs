using Il2Cpp;
using Il2CppNcp.Game;
using MelonLoader;
using UnityEngine;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;
using UniverseLib.Utility;

[assembly: MelonInfo(typeof(Level_Manager.Core), "Level Manager", "2.0.0", "Jayden", null)]
[assembly: MelonGame("Notional Games", "Beltmatic")]

namespace Level_Manager
{
    public class Core : MelonMod
    {
        public static UIBase UI { get; private set; }
        public static InputPanel LevelField { get; private set; }
        public override void OnInitializeMelon()
        {
            Universe.Init(1f, OnUniverseInit, LogHandler, new UniverseLib.Config.UniverseLibConfig());
        }

        private void OnUniverseInit()
        {
            UI = UniversalUI.RegisterUI("com.jayden.levelmanager", UpdateUIBehavior);
            LevelField = new(UI);
            LevelField.SetActive(true);
        }

        private void UpdateUIBehavior() { }

        public override void OnUpdate()
        {
            if (UnityEngine.InputSystem.Keyboard.current.f5Key.wasPressedThisFrame)
            {
                LevelField.SetActive(!LevelField.Enabled);
            }
        }

        private void LogHandler(string message, LogType logtype)
        {
            LoggerInstance.Msg($"[{logtype}] {message}");
        }
    }

    public class InputPanel(UIBase owner) : PanelBase(owner)
    {
        private InputFieldRef textInput;

        public override string Name => "Level Manager";
        public override int MinWidth => 150;
        public override int MinHeight => 80;
        public override Vector2 DefaultAnchorMin => new(0, 1);
        public override Vector2 DefaultAnchorMax => new(0, 1);
        public override bool CanDragAndResize => true;

        protected override void ConstructPanelContent()
        {
            textInput = UIFactory.CreateInputField(ContentRoot, "LevelInputField", GameManager.Instance.AnySimulation.LevelManager.Level.ToString());
            UIFactory.SetLayoutElement(textInput.UIRoot, minWidth: 130, minHeight: 30);

            textInput.Component.GetOnEndEdit().AddListener(val =>
            {
                if (int.TryParse(val, out int level) && level > 0 && level <= 1000)
                {
                    GameManager.Instance.AnySimulation.LevelManager.Cast<LevelManager>().SetLevel(level);
                }
            });
        }
    }
}