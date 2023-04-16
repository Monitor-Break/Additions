using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MonitorBreak
{
    public class CameraHelper : MonoBehaviour
    {
        private static Transform cameraRootTransform;

        public static Transform GetCameraRootTransform()
        {
            return cameraRootTransform;
        }

        private static Camera mainCamera;

        private const float maxCameraMoveSpeed = 20.0f;
        private const float minCameraMoveSpeed = 0.1f;
        private Vector3 targetPos;

        public Bounds cameraBounds;

        public static Camera main
        {
            get
            {
                return mainCamera;
            }
        }

        private const int NORMALPOSPRIORITY = 0;
        private static Vector3 newCameraPos;
        private static int currentNewPosPriority = NORMALPOSPRIORITY - 100;
        public static Vector3 GetCameraPos()
        {
            return cameraRootTransform.position;
        }

        public static void SetCameraPos(Vector3 cameraPos, int priority = NORMALPOSPRIORITY)
        {
            if (priority >= currentNewPosPriority)
            {
                newCameraPos = cameraPos;
                currentNewPosPriority = priority;
            }
        }

        public static Vector2 GetCameraMainScale()
        {
            return GetCameraScale(main);
        }

        public static float GetCameraMainAspect()
        {
            return main.aspect;
        }

        public static Vector2 GetCameraScale(Camera camera)
        {
            float height = 2.0f * camera.orthographicSize;

            return new Vector2(height * camera.aspect, height);
        }

        private static UnityEvent afterCameraPositionUpdate = new UnityEvent();

        public static void RunCodeAfterCameraPositionUpdate(UnityAction function)
        {
            afterCameraPositionUpdate.AddListener(function);
        }

        public static void StopRunningCodeAfterCameraPositionUpdate(UnityAction function)
        {
            afterCameraPositionUpdate.RemoveListener(function);
        }


        private void Awake()
        {
            cameraRootTransform = transform;
            mainCamera = GetComponent<Camera>();
            newCameraPos = Vector3.back * 10.0f;
        }

        private void LateUpdate()
        {
            float distanceMagnitude = Vector3.Distance(cameraRootTransform.position, targetPos);
            cameraRootTransform.position = Vector3.MoveTowards(cameraRootTransform.position, targetPos, Time.deltaTime * 5.0f * Mathf.Clamp(distanceMagnitude, minCameraMoveSpeed, maxCameraMoveSpeed));

            afterCameraPositionUpdate.Invoke();

            //Update target position
            if (currentNewPosPriority <= NORMALPOSPRIORITY - 100)
            {
                return;
            }
            targetPos = LimitPositionToBounds(newCameraPos);
            currentNewPosPriority = NORMALPOSPRIORITY - 100;
        }

        private Vector3 LimitPositionToBounds(Vector3 unboundPosition)
        {
            Vector3 cameraDimensions = new Vector3(mainCamera.orthographicSize * Screen.width / Screen.height, mainCamera.orthographicSize);

            return new Vector3(
                Mathf.Clamp(unboundPosition.x, cameraBounds.min.x + cameraDimensions.x, cameraBounds.max.x - cameraDimensions.x),
                Mathf.Clamp(unboundPosition.y, cameraBounds.min.y + cameraDimensions.y, cameraBounds.max.y - cameraDimensions.y),
                unboundPosition.z);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(cameraBounds.center, cameraBounds.size);
        }

        public static void SetCameraToCopyMain(Camera inputCamera, bool copyPosAndRotation = false)
        {
            if (copyPosAndRotation)
            {
                inputCamera.transform.SetPositionAndRotation(cameraRootTransform.position, cameraRootTransform.rotation);
            }

            inputCamera.orthographicSize = main.orthographicSize;
            inputCamera.fieldOfView = main.fieldOfView;
        }
    }
}

