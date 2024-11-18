using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonitorBreak.Bebug
{
    public static class Helper
    {
        public static void DrawWireCube(Vector3 center, Vector3 extents, Color color, float duration)
        {
            if (!Application.isEditor)
            {
                return;
            }

            Vector3 halfExtents = extents * 0.5f;

            Vector3 flatExtents = extents;
            flatExtents.y = 0;

            //Top
            DrawWirePlane(center + (Vector3.up * halfExtents.y), flatExtents, Vector3.up, color, duration, false);

            //Connectors
            //TopRight
            Debug.DrawLine(center + halfExtents, center + halfExtents - (Vector3.up * extents.y), color, duration);
            //BottomLeft
            Debug.DrawLine(center - halfExtents, center - halfExtents + (Vector3.up * extents.y), color, duration);
            //TopLeft
            Debug.DrawLine(center + new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z), center + new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z), color, duration);
            //Bottom Right
            Debug.DrawLine(center + new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z), center + new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z), color, duration);


            //Bottom
            DrawWirePlane(center - (Vector3.up * halfExtents.y), flatExtents, Vector3.down, color, duration, false);
        }

        public static void DrawWirePlane(Vector3 center, Vector3 extents, Vector3 normal, Color color, float duration, bool simple)
        {
            if (!Application.isEditor)
            {
                return;
            }

            Vector3 halfExtents = extents * 0.5f;

            Vector3 secondPoint = halfExtents;
            Vector3 thirdPoint = Vector3.Cross(secondPoint, normal);

            if (!simple)
            {
                Debug.DrawRay(center, thirdPoint, Color.yellow, duration);
                Debug.DrawRay(center, secondPoint, Color.green, duration);
            }

            Vector3 a = center + secondPoint;
            Vector3 b = center + thirdPoint;
            Vector3 c = center - secondPoint;
            Vector3 d = center - thirdPoint;

            Debug.DrawLine(a, b, color, duration);
            Debug.DrawLine(b, c, color, duration);
            Debug.DrawLine(c, d, color, duration);
            Debug.DrawLine(d, a, color, duration);
        }
    }
}
