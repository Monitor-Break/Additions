using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonitorBreak
{
    [IntializeAtRuntime]
    public class TimeManagement : MonoBehaviour
    {
        private struct TimeScale
        {
            public float actualTimeScale;
            public int priority;


            public TimeScale(float actualTimeScale, int priority)
            {
                this.actualTimeScale = actualTimeScale;
                this.priority = priority;
            }
        }

        private static Dictionary<object, TimeScale> timeScales = new Dictionary<object, TimeScale>();
        private static TimeScale defaultTimeScale;

        private static int currentHighestPriority;
        private static float normalFixedDeltaTime;

        private void Awake()
        {
            defaultTimeScale = new TimeScale(1.0f, 0);
            normalFixedDeltaTime = Time.fixedDeltaTime;
        }

        private void OnLevelWasLoaded(int level)
        {
            ResetTimeScales(this);
        }

        private static void ResetTimeScales(object selfOrigin)
        {
            timeScales = new Dictionary<object, TimeScale>
        {
            { selfOrigin, defaultTimeScale }
        };

            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f;
            currentHighestPriority = 0;
        }

        public static void ReplaceTimeScale(float timeScale, object origin)
        {
            if (!timeScales.ContainsKey(origin))
            {
                return;
            }

            timeScales[origin] = new TimeScale(timeScale, timeScales[origin].priority);

            if (timeScales[origin].priority == currentHighestPriority)
            {
                SetTimeScaleAndPriority(timeScale, currentHighestPriority);
            }
        }

        public static void AddTimeScale(float timeScale, int priority, object origin)
        {
            if (timeScales.ContainsKey(origin))
            {
                return;
            }

            TimeScale newTimeScale = new TimeScale(timeScale, priority);
            timeScales.Add(origin, newTimeScale);

            if (currentHighestPriority <= priority)
            {
                SetTimeScaleAndPriority(timeScale, priority);
            }
        }

        public static void RemoveTimeScale(object origin)
        {
            if (!timeScales.ContainsKey(origin))
            {
                return;
            }

            TimeScale timeScaleToBeRemoved = timeScales[origin];
            timeScales.Remove(origin);

            if (timeScaleToBeRemoved.priority == currentHighestPriority)
            {
                //Find highest priority in dictionary and set that as highest priority
                float newTimeScale = 1.0f;
                int newHighestPriority = 0;

                foreach (TimeScale scale in timeScales.Values)
                {
                    if (scale.priority > newHighestPriority)
                    {
                        newHighestPriority = scale.priority;
                        newTimeScale = scale.actualTimeScale;
                    }
                }

                SetTimeScaleAndPriority(newTimeScale, newHighestPriority);
            }
        }

        private static void SetTimeScaleAndPriority(float newTimeScale, int newPriority)
        {
            Time.timeScale = newTimeScale;
            if (Time.timeScale == 0.0f)
            {
                Time.fixedDeltaTime = 0.0f;
            }
            else
            {
                Time.fixedDeltaTime = normalFixedDeltaTime / (1.0f / Time.timeScale);
            }

            currentHighestPriority = newPriority;
        }
    }
}

