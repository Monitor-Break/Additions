using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonitorBreak
{
    public class TimeManagement
    {
        private static object objectWithPriority = null;

        public static Notify onNoPriority;

        public static void SetPriority(object self)
        {
            objectWithPriority = self;
        }

        public static bool HasPriority(object self)
        {
            return objectWithPriority == self;
        }

        public static bool AnythingHasPriority()
        {
            return objectWithPriority != null;
        }

        public static void ReleasePriority(object self)
        {
            if (objectWithPriority != self)
            {
                return;
            }

            ReleasePriority();
        }

        public static void ReleasePriority()
        {
            objectWithPriority = null;
            onNoPriority?.Invoke();
        }

        public static void SetTimeScale(float newTimeScale, object self)
        {
            if (objectWithPriority != null && objectWithPriority != self)
            {
                //An object has priority and it is not the one passed
                return;
            }

            Time.timeScale = newTimeScale;
        }
    }
}

