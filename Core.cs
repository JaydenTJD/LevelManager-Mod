using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

[assembly: MelonInfo(typeof(Levelup.Core), "Levelup", "1.0.0", "Jayden", null)]
[assembly: MelonGame("Notional Games", "Beltmatic")]

namespace Levelup
{
    public class Core : MelonMod
    {
        Il2CppNcp.Game.ILevelManager lm;
        private Rect window = new(10, 10, 150, 50);
        private int PopulateLM()
        {
            Melon<Core>.Logger.Msg("Attempting to populate lm instance");
            GameObject dummy = new();
            UnityEngine.Object.DontDestroyOnLoad(dummy);
            Scene ddolScene = dummy.scene;
            UnityEngine.Object.Destroy(dummy);
            Melon<Core>.Logger.Msg("Obtained a reference to the ddol scene");
            GameObject gameManagerGO = null;
            var rootObjects = ddolScene.GetRootGameObjects();
            foreach (var GO in rootObjects)
            {
                if (GO != null && GO.name == "GameManager")
                {
                    gameManagerGO = GO;
                    break;
                }
            }
            Melon<Core>.Logger.Msg("Obtained a reference to the GameManager");
            var gameManagerScript = gameManagerGO.GetComponent<GameManager>();
            Melon<Core>.Logger.Msg("Obtained a reference to the GM script");
            var simulationInstance = gameManagerScript.RealSimulation;
            Melon<Core>.Logger.Msg("Obtained a reference to the Simulation");
            lm = simulationInstance.LevelManager;
            Melon<Core>.Logger.Msg("Populated Succesfully");
            return simulationInstance.Seed;
        }
        private int GetCurrentNumber()
        {
            if (lm == null)
            {
                return 1;
            } else
            {
                return lm.CurrentNumber;
            }
        }
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }
        public override void OnGUI()
        {
            window = GUI.Window(5, window, (GUI.WindowFunction)DrawWindow, "Levelup Window");
        }
        private void DrawWindow(int id)
        {
            GUI.DragWindow(new Rect(0, 0, 150, 20));

            if (GUI.Button(new Rect(0, 20, 150, 30), "Start..."))
            {
                int seed = PopulateLM();
                string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "Mods/Data");
                string filePath = Path.Combine(dirPath, $"{seed}.txt");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                using (StreamWriter writer = new(filePath, append: false))
                {
                    bool isFirst = true;
                    for (int i = 0; i < 999; i++)
                    {
                        if (!isFirst)
                        {
                            writer.Write(", ");
                        } else
                        {
                            isFirst = false;
                        }
                        writer.Write(GetCurrentNumber());
                        lm.ForceNextLevel();
                    }
                }
                Melon<Core>.Logger.Msg("Success!!!");
            }
        }
    }
}