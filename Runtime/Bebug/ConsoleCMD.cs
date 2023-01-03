
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

namespace MonitorBreak.Bebug
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ConsoleCMD : Attribute
    {
        public string commandString;

        public ConsoleCMD(string commandString)
        {
            this.commandString = commandString;
        }
    }

    public class ConsoleCMDExecution : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Main()
        {
            if(!BebugManagement.DebugEnabled) return;

            List<Console.CustomCommand> ccList = new List<Console.CustomCommand>();

            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] assemblyTypes = assembly.GetTypes();

                foreach(Type type in assemblyTypes)
                {
                    MethodInfo[] typeMethods = type.GetMethods().Where(x => x.IsStatic).ToArray();

                    foreach(MethodInfo method in typeMethods)
                    {
                        ConsoleCMD ccmd = (ConsoleCMD)method.GetCustomAttribute(typeof(ConsoleCMD), false);

                        if(ccmd != null)
                        {
                            ccList.Add(new Console.CustomCommand(){ identifier = ccmd.commandString.ToLower(), method = method});
                        }
                    }
                }
            }

            Console.SetCustomCommands(ccList);
        }
    }
}
