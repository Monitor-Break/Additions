using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonitorBreak.Bebug
{
    [IntializeAtRuntime]
    public class BebugManagement : MonoBehaviour
    {
        public const bool DebugEnabled = true;

        public static Texture2D baseTexture;

        //CONSOLE
        private static List<Console> consoles = new List<Console>();

        public static float NumberOfConsoles()
        {
            return consoles.Count;
        }

        public static void AddConsole(Console console)
        {
            console.SetConsoleIndex(consoles.Count);
            consoles.Add(console);
        }

        private static int indexOfConsoleToRemove = -1;
        public static void RemoveConsole(Console consoleToRemove)
        {
            indexOfConsoleToRemove = consoleToRemove.GetConsoleIndex();
        }

        public static Console GetConsole(int index)
        {
            if (NumberOfConsoles() == 0) //If all consoles have been closed but we want to get a console to do something with then make a new console
            {
                new Console(ConsoleName()); //This console is hidden by default so it isn't intrusive 
            }

            if (index < 0 || index >= consoles.Count)
            {
                return null;
            }

            return consoles[index];
        }

        private static int activeConsole = 0;

        public static bool IsActiveConsole(int index)
        {
            return activeConsole == index;
        }

        private static bool makeNewConsole = false;
        public static void MakeNewConsoleWhileInConsoleLoop()
        {
            makeNewConsole = true;
        }
        //CONSOLE

        //GRAPH
        private static List<Graph> graphs = new List<Graph>();

        public static void AddGraph(Graph graph)
        {
            graphs.Add(graph);
        }
        //GRAPH

        private void Awake()
        {
            baseTexture = GenerateTexture(Color.black);
            new Console(ConsoleName());
        }

        private void Update()
        {
            if ((Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.F2) || Input.GetKeyDown(KeyCode.F3)) && NumberOfConsoles() == 0)
            {
                new Console(ConsoleName());
            }

            if (Console.GetConsoleState() == Console.ConsoleState.FullScreen && Input.GetKeyDown(KeyCode.Tab))
            {
                activeConsole++;

                if (activeConsole >= NumberOfConsoles())
                {
                    activeConsole = 0;
                }
            }
        }

        private void OnGUI()
        {
            if (DebugEnabled)
            {
                GUI.skin.box.normal.background = baseTexture;

                //Console
                foreach (Console console in consoles)
                {
                    console.DrawConsole();
                }

                if (makeNewConsole)
                {
                    makeNewConsole = false;
                    new Console(ConsoleName());
                }

                if (indexOfConsoleToRemove != -1)
                {
                    consoles.RemoveAt(indexOfConsoleToRemove);

                    int newIndex = 0;
                    foreach (Console console in consoles)
                    {
                        console.SetConsoleIndex(newIndex);
                        newIndex++;
                        console.SetName($"Console {newIndex}");
                    }

                    if (activeConsole >= consoles.Count)
                    {
                        activeConsole--;
                    }

                    indexOfConsoleToRemove = -1;

                    if (NumberOfConsoles() == 0)
                    {
                        Console.HideAllConsoles();
                    }
                }

                //Graph
                foreach (Graph graph in graphs)
                {
                    graph.DrawGraph();
                }
            }
        }

        private static string ConsoleName()
        {
            return $"Console {NumberOfConsoles() + 1}";
        }

        public static Texture2D GenerateTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}

