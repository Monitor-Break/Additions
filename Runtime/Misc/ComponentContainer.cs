using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

namespace MonitorBreak 
{
    public class ComponentContainer : MonoBehaviour
    {
        [Serializable]
        public struct smallComponent
        {
            public bool enabled;
            public string name;
            public MethodInfo OnGenerate;
            public MethodInfo Update;

            public void Run(MethodInfo method, GameObject _instance)
            {
                if (!enabled || method == null)
                {
                    return;
                }

                try
                {
                    method.Invoke(null, new object[] { _instance });
                }
                catch (TargetParameterCountException)
                {
                    method.Invoke(null, null);
                }
            }
        }

        [HideInInspector]
        public List<smallComponent> smallComponents = new List<smallComponent>();

        public void GenerateSmallComponentsList()
        {
            //Create dictionary of components name and enabled status
            Dictionary<string, bool> scEnabledDict = new Dictionary<string, bool>();
            foreach (smallComponent sc in smallComponents)
            {
                scEnabledDict.Add(sc.name, sc.enabled);
            }
            smallComponents = new List<smallComponent>();
            Type t = this.GetType();
            Type[] nestedTypes = t.GetNestedTypes();
            //Generate list of smallComponent variable for later checks
            FieldInfo[] scFields = typeof(smallComponent).GetFields().Where(x => x.FieldType == typeof(MethodInfo)).ToArray();
            foreach (Type nested in nestedTypes)
            {
                smallComponent newSmallComponent = new smallComponent();

                bool createSmallComponent = false;
                //Get methods in struct
                MethodInfo[] methods = nested.GetMethods();
                foreach (MethodInfo method in methods)
                {
                    if (!method.IsStatic)
                    {
                        continue;
                    }

                    for (int i = 0; i < scFields.Length; i++)
                    {
                        FieldInfo currentSmallComponentMethod = scFields[i];
                        if (currentSmallComponentMethod.Name == method.Name)
                        {
                            object boxed = newSmallComponent;
                            currentSmallComponentMethod.SetValue(boxed, method);
                            newSmallComponent = (smallComponent)boxed;
                            createSmallComponent = true;
                        }
                    }
                }

                if (createSmallComponent)
                {
                    //Actuallly add smallComponent to smallComponentList
                    if (scEnabledDict.ContainsKey(nested.Name))
                    {
                        newSmallComponent.enabled = scEnabledDict[nested.Name];
                    }
                    else
                    {
                        newSmallComponent.enabled = true;
                    }
                    newSmallComponent.name = nested.Name;
                    smallComponents.Add(newSmallComponent);
                }
            }

            //*Run on generate functions
            foreach (smallComponent sc in smallComponents)
            {
                sc.Run(sc.OnGenerate, gameObject);
            }
        }

        private void Start()
        {
            GenerateSmallComponentsList();
        }

        private void Update()
        {
            //*Run on update functions
            foreach (smallComponent sc in smallComponents)
            {
                sc.Run(sc.Update, gameObject);
            }
        }

        public void SetActiveSmallComponent(int index, bool _bool)
        {
            smallComponent sc = smallComponents[index];
            sc.enabled = _bool;
            smallComponents[index] = sc;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void RegenerateOnRecompile()
        {
            ComponentContainer[] scriptInstances = FindObjectsOfType(typeof(ComponentContainer)) as ComponentContainer[];
            foreach (ComponentContainer cc in scriptInstances)
            {
                cc.GenerateSmallComponentsList();
            }
        }
    }

    [CustomEditor(typeof(ComponentContainer))]
    [CanEditMultipleObjects]
    public class ComponentContainerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ComponentContainer script = (ComponentContainer)target;

            int scLength = script.smallComponents.Count;
            for (int i = 0; i < scLength; i++)
            {
                GUILayout.Space(10);

                GUILine(10 + (i * 27));

                ComponentContainer.smallComponent sc = script.smallComponents[i];

                GUILayout.BeginHorizontal();

                bool currentEnabledStatus = GUILayout.Toggle(sc.enabled, $" |   {sc.name}");

                if (currentEnabledStatus != sc.enabled)
                {
                    script.SetActiveSmallComponent(i, currentEnabledStatus);
                }

                GUILayout.FlexibleSpace();

                GUILayout.Label("|");

                GUILayout.EndHorizontal();
            }

            int finalHeight = 10 + (scLength * 27);

            if (scLength == 0)
            {
                GUILayout.Space(3);
                GUILayout.Label("No Small Components");
                finalHeight += 20;
            }
            else
            {
                //Left black space        
                Rect rect = new Rect(0, 10, 10, finalHeight - 10);
                EditorGUI.DrawRect(rect, new Color(0.1f, 0.1f, 0.1f));
            }

            //Final line
            GUILine(finalHeight);

            GUILayout.Space(12);

            // if(GUILayout.Button("Recalculate Small Components"))
            // {
            //     script.GenerateStructsList();
            // }

            DrawDefaultInspector();
        }

        void GUILine(int yHeight)
        {
            Rect rect = new Rect(0, yHeight, 10000, 1);
            EditorGUI.DrawRect(rect, new Color(0.1f, 0.1f, 0.1f, 1));
        }
    }
}
