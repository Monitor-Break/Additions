using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonitorBreak.TimeManagment
{
    public class TimeManagment
    {
        private static object objectWithPriority = null;

        public static void SetPriority(object self) 
        {
            objectWithPriority = self;
        }

        public static void ReleasePriority(object self) 
        {
            if(objectWithPriority != self) 
            {
                return;
            }

            ReleasePriority();
        }

        public static void ReleasePriority() 
        {
            objectWithPriority = null;
        }

        public static void SetTimeScale(float newTimeScale, object self = null) 
        {
            if(objectWithPriority != null && objectWithPriority != self) 
            {
                //An object has priority and it is not the one passed
                return;
            }

            Time.timeScale = newTimeScale;
        }
    }
}

