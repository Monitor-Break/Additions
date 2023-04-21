using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonitorBreak.MB2D
{
    public class PixelatedRender : MonoBehaviour
    {
        private GameObject localGameObject;

        private bool running;
        public Camera targetCamera;
        public Renderer targetQuad;

        [Header("Camera Bounds")]
        public float width = 5.0f;
        public float height = 5.0f;
        public Vector3 center;

        [Header("Ouput Settings")]
        public int pixelsPerSquareInch = 16;

        [Header("World Layer")]
        public int originalLayer = 9;
        public int renderTargetLayer = 8;

        [Header("Sorting Layer")]
        public string sortingLayerName;
        public int sortingLayerValue;

        private void Start()
        {
            localGameObject = gameObject;
            Transform targetQuadTransform = targetQuad.transform;

            //Setup camera
            targetCamera.enabled = false;

            targetCamera.aspect = width / height;
            targetCamera.orthographicSize = height * 0.5f;
            targetCamera.transform.position = center;

            //Calculate scale of render texture
            Vector3 cameraScale = new Vector3(width, height);
            Vector3 newTextureScale = cameraScale * pixelsPerSquareInch;

            SetRenderTextureRes(new Vector2Int((int)newTextureScale.x, (int)newTextureScale.y));

            targetQuad.sortingLayerName = sortingLayerName;
            targetQuad.sortingOrder = sortingLayerValue;

            //Set target quad to match camera size and position
            targetQuadTransform.localScale = cameraScale;
            targetQuadTransform.position = new Vector2(center.x, center.y);

            SetActive(true);

            CameraHelper.RunCodeAfterCameraPositionUpdate(this);
        }

        private void OnDestroy()
        {
            CameraHelper.StopRunningCodeAfterCameraPositionUpdate(this);
        }

        public void Render()
        {
            if (running)
            {
                //Set gameobject layer to be the one targeted by the renderer
                localGameObject.layer = renderTargetLayer;

                //Render
                targetCamera.Render();

                //Copy render output to target quad
                targetQuad.sharedMaterial.SetTexture("_MainTex", targetCamera.targetTexture);

                //Set gameobject layer back to original
                localGameObject.layer = originalLayer;
            }
        }

        private void SetRenderTextureRes(Vector2Int newRes)
        {
            if (targetCamera.targetTexture != null)
            {
                targetCamera.targetTexture.Release();
            }

            targetCamera.targetTexture = new RenderTexture(newRes.x, newRes.y, 24);
            targetCamera.targetTexture.filterMode = FilterMode.Point;
        }

        public void SetActive(bool _bool)
        {
            running = _bool;

            targetQuad.gameObject.SetActive(running);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawWireCube(center, new Vector3(width, height, 0.0f));
        }
    }
}


