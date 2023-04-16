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
        private Transform targetQuadTransform;

        [Header("Camera Bounds")]
        public float width = 5.0f;
        public float height = 5.0f;

        [Header("Ouput Settings")]
        public Vector2Int outputRes;
        public bool dynamicRes = true;
        public int pixelsPerSquareInch = 16;
        private RenderTexture outputTexture;

        [Header("World Layer")]
        public int originalLayer = 9;
        public int renderTargetLayer = 8;

        [Header("Sorting Layer")]
        public string sortingLayerName;
        public int sortingLayerValue;

        private void Start()
        {
            localGameObject = gameObject;
            targetQuadTransform = targetQuad.transform;

            //Setup camera
            targetCamera.enabled = false;

            SetRenderTextureRes(outputRes);

            targetQuad.sortingLayerName = sortingLayerName;
            targetQuad.sortingOrder = sortingLayerValue;

            SetActive(true);

            CameraHelper.RunCodeAfterCameraPositionUpdate(Render);
        }

        private void OnDestroy()
        {
            CameraHelper.StopRunningCodeAfterCameraPositionUpdate(Render);
        }

        private void Render()
        {
            if (running)
            {
                targetCamera.aspect = width / height;
                targetCamera.orthographicSize = height * 0.5f;

                Vector3 cameraScale = new Vector3(width, height);

                if (dynamicRes)
                {
                    //Update scale of render texture
                    Vector3 newTextureScale = cameraScale * pixelsPerSquareInch;

                    SetRenderTextureRes(new Vector2Int((int)newTextureScale.x, (int)newTextureScale.y));
                }

                //Set gameobject layer to be the one targeted by the renderer
                localGameObject.layer = renderTargetLayer;

                //Render
                targetCamera.Render();

                //Copy render output to target quad
                targetQuad.sharedMaterial.SetTexture("_MainTex", outputTexture);

                //Update target quad to match camera size
                targetQuadTransform.localScale = cameraScale;

                //Set gameobject layer back to original
                localGameObject.layer = originalLayer;
            }
        }

        private void SetRenderTextureRes(Vector2Int newRes)
        {
            if (outputTexture != null)
            {
                outputTexture.Release();
                //targetCamera.targetTexture.Release();
            }

            outputTexture = new RenderTexture(newRes.x, newRes.y, 24);
            outputTexture.filterMode = FilterMode.Point;
            if (outputTexture.Create())
            {
                targetCamera.targetTexture = outputTexture;
            }
            else
            {
                throw new System.Exception("Render Texture Creation failed!");
            }
        }

        public void SetActive(bool _bool)
        {
            running = _bool;

            targetQuad.gameObject.SetActive(running);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0.0f));
        }
    }
}


