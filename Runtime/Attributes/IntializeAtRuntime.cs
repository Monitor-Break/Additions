using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

namespace MonitorBreak 
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IntializeAtRuntime : Attribute
    {
        public string resourcesPath;

        public IntializeAtRuntime(string _resourcesPath = "")
        {
            resourcesPath = _resourcesPath;
        }
    }

    public class IntializeAtRuntimeExecution : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        private static void Main()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes().Where(x => typeof(MonoBehaviour).IsAssignableFrom(x) && x != typeof(MonoBehaviour)))
                {
                    IntializeAtRuntime iar = (IntializeAtRuntime)type.GetCustomAttribute(typeof(IntializeAtRuntime), false);
                    if (iar != null)
                    {
                        GameObject newGameObject;
                        if (string.IsNullOrEmpty(iar.resourcesPath))
                        {
                            //Create new gameobject and add class to it
                            newGameObject = new GameObject(type.Name);
                            newGameObject.AddComponent(type);
                        }
                        else
                        {
                            //Instantiate from resources
                            newGameObject = (GameObject)Instantiate(Resources.Load(iar.resourcesPath), Vector3.zero, Quaternion.identity);
                        }

                        DontDestroyOnLoad(newGameObject);
                    }
                }
            }  
        }
    }
}


