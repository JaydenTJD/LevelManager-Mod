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
        List<int> min = Enumerable.Repeat(100000000, 999).ToList();
        List<int> max = Enumerable.Repeat(0, 999).ToList();
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
        public override void OnDeinitializeMelon()
        {
            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "Mods/Data");
            string filePath = Path.Combine(dirPath, "min.txt");
            File.WriteAllText(filePath, string.Join(",", min));
            filePath = Path.Combine(dirPath, "max.txt");
            File.WriteAllText(filePath, string.Join(",", max));
            Melon<Core>.Logger.Msg("Succesfully wrote min/max");
        }
        public override void OnGUI()
        {
            window = GUI.Window(85721, window, (GUI.WindowFunction)DrawWindow, "Levelup Window");
        }
        private void DrawWindow(int id)
        {
            GUI.DragWindow(new Rect(0, 0, 150, 20));

            if (Button(new Rect(0, 20, 150, 30), "Start..."))
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
                            writer.Write(",");
                        }
                        else
                        {
                            isFirst = false;
                        }
                        int num = GetCurrentNumber();
                        writer.Write(num);
                        if (min[i] > num)
                        {
                            min[i] = num;
                        }
                        if (max[i] < num)
                        {
                            max[i] = num;
                        }
                        lm.ForceNextLevel();
                    }
                }
                Melon<Core>.Logger.Msg("Success!!!");
            }
        }
        private static bool Button(Rect rect, string label)
        {
            bool clicked = false;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    clicked = true;
                    Event.current.Use();
                }
            }
            GUI.Button(rect, label);
            return clicked;
        }
    }
}