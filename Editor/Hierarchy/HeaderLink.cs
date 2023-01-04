using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HeaderLink : MonoBehaviour
{
    public HeaderLink sibilingObject;
    private Transform lastParent = null;

    public void SetSibilingObject(HeaderLink newSibiling)
    {
        sibilingObject = newSibiling;

        newSibiling.sibilingObject = this;
    }

    private void Update() 
    {
        if(lastParent != transform.parent)
        {
            lastParent = transform.parent;

            sibilingObject.transform.parent = lastParent;
            sibilingObject.lastParent = lastParent;
        }
    }

    [HideInInspector]
    public bool destroySignalRecevied = false;

    private void OnDestroy() {
        sibilingObject.destroySignalRecevied = true;
        if(!destroySignalRecevied)
        {
            //This mean we won't try to destroy an object twice
            DestroyImmediate(sibilingObject.gameObject);
        }
    }
}
