using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

namespace MonitorBreak
{
    public class SavingAndLoadingUtility : MonoBehaviour
    {
        //Seperates the type name, field name and value in the saved file
        private const char seperatorChar = ';';

        //Path for general data
        public static string Path()
        {
            return Application.persistentDataPath + "/generalData.data";
        }

        //Generate a line within the file
        private static string ConvertToString(Type type, FieldInfo field)
        {
            return $"{type.Name}{seperatorChar}{field.Name}{seperatorChar}{field.GetValue(null).ToString()}\n";
        }

        public static void Save()
        {
            //Get all types from the current assembly
            string textToSave = "";

            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) 
            {
                Type[] types = assembly.GetTypes();

                //Iterate through every type and get thier field
                foreach (Type type in types)
                {
                    FieldInfo[] fields = type.GetFields();
                    foreach (FieldInfo field in fields)
                    {
                        //Attempt to get attribute from field
                        SaveThis saveThis = (SaveThis)field.GetCustomAttribute(typeof(SaveThis), false);
                        if (saveThis != null)
                        {
                            //If the field has a SaveThis attribute and a line about the field to the final file
                            textToSave += ConvertToString(type, field);
                        }
                    }
                }
            }

            //Save file to disk
            File.WriteAllText(Path(), textToSave);
        }

        //Struct containg all data about a saved field in a class
        private struct FieldAndValue
        {
            public string fieldName;
            public string value;
        }

        [RuntimeInitializeOnLoadMethod]
        public static void Load()
        {
            //All the entries in the saved data
            string[] lines;
            try
            {
                lines = File.ReadAllLines(Path());
            }
            catch
            {
                Save(); //Create new file
                Load(); //try Load file again
                return; //Return so code below isn't run a second time after file is properly loaded
            }

            //Formatted data is used to link field information to types
            Dictionary<string, List<FieldAndValue>> formattedData = new Dictionary<string, List<FieldAndValue>>();

            foreach (string line in lines)
            {
                //Split the entry into its different parts
                string[] splitLine = line.Split(seperatorChar);

                //Create the new field and value
                FieldAndValue newFAV = new FieldAndValue() { fieldName = splitLine[1], value = splitLine[2] };

                //If a field linked to the type this field is linked to has already been computed just add it to the ones list
                if (formattedData.ContainsKey(splitLine[0]))
                {
                    formattedData[splitLine[0]].Add(newFAV);
                }
                else
                {
                    //Otherwise create a new entry in the dict with a new list
                    List<FieldAndValue> newList = new List<FieldAndValue>();
                    newList.Add(newFAV);

                    formattedData.Add(splitLine[0], newList);
                }
            }

            //Get all the actual Types (and not just the type names)
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) 
            {
                Type[] types = assembly.GetTypes().Where(x => formattedData.ContainsKey(x.Name)).ToArray();

                //Iterate through each type
                foreach (Type type in types)
                {
                    //Get all FieldAndValue objects linked to this class
                    List<FieldAndValue> typeData = formattedData[type.Name];
                    //Iterate through them and apply the values saved on disk
                    foreach (FieldAndValue data in typeData)
                    {
                        FieldInfo field = type.GetField(data.fieldName);
                        field.SetValue(null, TypeDescriptor.GetConverter(field.FieldType).ConvertFromString(data.value));
                    }
                }
            }
        }
    }
}
