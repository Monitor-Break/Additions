using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

[InitializeOnLoad]
public static class HierarchyAdditions
{
    private const string headerNameStart = "MBHeader";
    private static readonly Color unityEditorDarkThemeBackgroundColor = new Color(0.22f,0.22f,0.22f); 
    private static readonly Color unityEditorDarkThemeBackgroundAccentColor = new Color(0.175f,0.175f,0.175f); 
    private static readonly Color boxColor = unityEditorDarkThemeBackgroundAccentColor;

    private static Rect topRect;

    static HierarchyAdditions()
    {
        EditorApplication.hierarchyWindowItemOnGUI += Draw;
    }

    [MenuItem("GameObject/Heirarchy/Create New Headers Box", false, 0)]
    public static void CreateNewHeadersBox()
    {
        GameObject firstPart = null;
        for(int i = 0; i < 2; i++)
        {
            GameObject newHeaderPart = new GameObject("MBHeader Box");
            //newHeaderPart.hideFlags = HideFlags.HideInInspector;
            newHeaderPart.tag = "EditorOnly";
            if(firstPart == null)
            {
                firstPart = newHeaderPart;
            }
            else
            {
                firstPart.AddComponent<HeaderLink>().SetSibilingObject(newHeaderPart.AddComponent<HeaderLink>());
            }
        }
    }

    private static bool firstPartOfHeaderFound = false;
    private static string firstPartOfHeaderName = "";
    private static int firstID = int.MaxValue;
    private static void Draw(int instanceID, Rect selectionRect)
    {
        if(firstID == int.MaxValue)
        {
            firstID = instanceID;
        }

        if(instanceID == firstID)
        {
            firstPartOfHeaderFound = false;
        }

        GameObject heirarchyGameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if(heirarchyGameObject != null)
        {
            if(NameCheck(heirarchyGameObject.name, headerNameStart))
            {
                if(!firstPartOfHeaderFound)
                {
                    //Top part of header
                    selectionRect.x -= 20.0f;
                    selectionRect.width += 20.0f;

                    HideArea(selectionRect);
                    EditorGUI.DrawRect(selectionRect, unityEditorDarkThemeBackgroundAccentColor);
                    EditorGUI.LabelField(selectionRect, heirarchyGameObject.name.Replace(headerNameStart,""),new GUIStyle("miniLabel"));
                    selectionRect.y += selectionRect.height;

                    //Save selection rect for bottom
                    topRect = selectionRect;
                    firstPartOfHeaderName = heirarchyGameObject.name;
                    firstPartOfHeaderFound = true;
                }
                else
                {
                    heirarchyGameObject.name = firstPartOfHeaderName;

                    //Bottom part of header
                    HideArea(selectionRect);
                    selectionRect.x = topRect.x;
                    selectionRect.y += selectionRect.height * 0.5f;
                    selectionRect.height *= 0.1f;
                    selectionRect.width = topRect.width;
                    EditorGUI.DrawRect(selectionRect, boxColor);

                    Rect leftRect = new Rect(topRect.x, selectionRect.y, topRect.height * 0.1f, topRect.y - selectionRect.y);
                    EditorGUI.DrawRect(leftRect, boxColor);

                    Rect rightRect = new Rect(topRect.x + (topRect.width - (topRect.height * 0.1f)), selectionRect.y, topRect.height * 0.1f, topRect.y - selectionRect.y);
                    EditorGUI.DrawRect(rightRect, boxColor);
                    firstPartOfHeaderFound = false;
                }
            }
            else if(heirarchyGameObject.TryGetComponent<HierarchyColourTag>(out HierarchyColourTag hct))
            {
                float cachedX = selectionRect.x;
                selectionRect.width = 10;
                selectionRect.x = Screen.width - 5;
                EditorGUI.DrawRect(selectionRect, hct.tagColour);
                selectionRect.x = 0;
                EditorGUI.DrawRect(selectionRect, hct.tagColour);
            }
        }
    }

    private static void HideArea(Rect area)
    {
        area.x = 0;
        EditorGUI.DrawRect(area, unityEditorDarkThemeBackgroundAccentColor);

        area.width = Screen.width;
        area.x = 32;    //Width of grey segment on left of objects in hierarchy
        EditorGUI.DrawRect(area, unityEditorDarkThemeBackgroundColor);
    }

    private static bool NameCheck(string enteredName, string checkFor)
    {
        return enteredName.StartsWith(checkFor, System.StringComparison.OrdinalIgnoreCase);
    }
}
