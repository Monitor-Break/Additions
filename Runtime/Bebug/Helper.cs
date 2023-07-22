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

            //Top
            DrawWirePlane(center + (Vector3.up * halfExtents.y), extents, color, duration);

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
            DrawWirePlane(center - (Vector3.up * halfExtents.y), extents, color, duration);
        }

        public static void DrawWirePlane(Vector3 center, Vector3 extents, Color color, float duration)
        {
            if (!Application.isEditor)
            {
                return;
            }

            extents.y = 0.0f;
            Vector3 halfExtents = extents * 0.5f;

            Vector3 topRight = center + halfExtents;
            Vector3 topLeft = center + new Vector3(-halfExtents.x, 0, halfExtents.z);
            Vector3 bottomRight = center + new Vector3(halfExtents.x, 0, -halfExtents.z);
            Vector3 bottomLeft = center - halfExtents;

            Debug.DrawLine(topRight, bottomRight, color, duration);
            Debug.DrawLine(bottomRight, bottomLeft, color, duration);
            Debug.DrawLine(bottomLeft, topLeft, color, duration);
            Debug.DrawLine(topLeft, topRight, color, duration);
        }
    }
}
