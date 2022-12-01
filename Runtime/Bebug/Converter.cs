using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.ComponentModel;

namespace MonitorBreak.Bebug
{
    public class Converter
    {
        public static object Convert(string toConvert, Type type)
        {
            if (type == typeof(string))
            {
                return toConvert;
            }
            else
            {
                try
                {
                    //* Fundamental types only *//
                    TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
                    return typeConverter.ConvertFromString(toConvert);
                }
                catch
                {
                    string inbetweenString = toConvert.Substring(toConvert.IndexOf('(') + 1);
                    string[] objectArguments = inbetweenString.Substring(0, inbetweenString.Length - 1).Split(',');
                    //* Get the generic constructor and pass the parameters entered, pass them through this function again and again until we get to fundamental types *//
                    ParameterInfo[] pars = type.GetConstructors()[0].GetParameters();
                    List<object> parametersOfParameter = new List<object>();
                    for (int i = 0; i < pars.Length; i++)
                    {
                        parametersOfParameter.Add(Convert(objectArguments[i], pars[i].ParameterType));
                    }
                    return Activator.CreateInstance(type, parametersOfParameter.ToArray());
                }
            }
        }
    }
}
