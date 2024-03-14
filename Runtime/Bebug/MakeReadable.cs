
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

namespace MonitorBreak.Bebug
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class MakeReadable : Attribute
    {
        public string key;

        public MakeReadable(string name)
        {
            key = name;
        }
    }

    public class MakeReadableExecution : MonoBehaviour
    {   
        [RuntimeInitializeOnLoadMethod]
        public static void Main()
        {
            if(!BebugManagement.DebugEnabled) return;

            Dictionary<string, FieldInfo> newReadableFields = new Dictionary<string, FieldInfo>();

            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] assemblyTypes = assembly.GetTypes();

                foreach(Type type in assemblyTypes)
                {
                    FieldInfo[] typeFields = type.GetFields().Where(x => x.IsStatic).ToArray();

                    foreach(FieldInfo field in typeFields)
                    {
                        MakeReadable mr = (MakeReadable)field.GetCustomAttribute(typeof(MakeReadable), false);

                        if (mr != null)
                        {
                            newReadableFields.Add(mr.key.ToLower(), field);
                        }
                    }
                }
            }

            Console.SetReadableFields(newReadableFields);
        }
    }
}
