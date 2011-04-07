//Created by Devin Reimer (AlmostLogical Software) - http://www.almostlogical.com
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

[CustomEditor(typeof(Rigidbody))]
public class RigidbodySleepToggleInspector : Editor
{
    private bool doesRigidbodySleepOnAwakeExist = false;

    public void OnEnable()
    {
        doesRigidbodySleepOnAwakeExist = (target as Component).GetComponent<RigidbodySleepOnAwake>()!=null;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RigidbodySleepOnAwakeInspectorGUI();
    }

    public void RigidbodySleepOnAwakeInspectorGUI()
    {
        bool toggleCurrentState;
        bool hasComponentChanged = false;
        EditorGUILayout.BeginHorizontal();
        toggleCurrentState = EditorGUILayout.Toggle("Sleep On Awake", doesRigidbodySleepOnAwakeExist);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
   
        if (toggleCurrentState && !doesRigidbodySleepOnAwakeExist)
        {
            hasComponentChanged =  AddRigidbodySleepOnAwakeToGameObject((target as Component).gameObject,false);
            if (hasComponentChanged)
            {
                doesRigidbodySleepOnAwakeExist = true;
            }
        }
        else if (!toggleCurrentState && doesRigidbodySleepOnAwakeExist)
        {
            hasComponentChanged =  RemoveRigidbodySleepOnAwakeInOnInspector((target as Component).gameObject);
            if (hasComponentChanged)
            {
                doesRigidbodySleepOnAwakeExist=false;
            }
        }
    }

    //returns true if added
    public static bool AddRigidbodySleepOnAwakeToGameObject(GameObject gameObj,bool displayObjectNameInDialogTitle)
    {
        bool isComponentAdded = false;
        string dialogTitle = "";

        PrefabType prefabType = EditorUtility.GetPrefabType(gameObj);
        if (prefabType == PrefabType.None || prefabType == PrefabType.Prefab || prefabType == PrefabType.DisconnectedPrefabInstance)
        {
            gameObj.AddComponent<RigidbodySleepOnAwake>();
            isComponentAdded = true;
            Debug.Log("Add Component");
        }
        else
        {
            dialogTitle = "Losing prefab";
            if (displayObjectNameInDialogTitle)
            {
                dialogTitle += " for GameObject:" + gameObj.name;
            }
            int result = EditorUtility.DisplayDialogComplex(dialogTitle, "This action will lose the prefab connection. Are you sure you wish to continue or alter prefab directly?", "Continue", "Skip", "Apply To Prefab Itself");
            if (result == 0) //OK
            {
                gameObj.AddComponent<RigidbodySleepOnAwake>();
                isComponentAdded = true;
            }
            else if (result == 1) //CANCEL
            {
                //do nothing
            }
            else if (result == 2) //ALT
            {
                GameObject prefab = EditorUtility.GetPrefabParent(gameObj) as GameObject;
                prefab.AddComponent<RigidbodySleepOnAwake>();
                isComponentAdded = true;
            }
        }

        if (isComponentAdded)
        {
            EditorUtility.SetDirty(gameObj);
        }

        return isComponentAdded;
    }

    private bool RemoveRigidbodySleepOnAwakeInOnInspector(GameObject gameObj)
    {
        bool isComponentRemoved = false;
        string dialogTitle;
        PrefabType prefabType = EditorUtility.GetPrefabType(gameObj);

        if (prefabType == PrefabType.None || prefabType == PrefabType.Prefab || prefabType == PrefabType.DisconnectedPrefabInstance) //if not prefab instance nuke the component as it is no longer needed
        {
            DestroyRigidbodySleepOnAwakeComponentFromInOnInspector();
        }
        else //can't destroy the component as it will break the prefab was disable it instead
        {
            dialogTitle = "Losing prefab";
            int result = EditorUtility.DisplayDialogComplex(dialogTitle, "This action will lose the prefab connection. Are you sure you wish to continue or alter prefab directly?", "Continue", "Skip", "Apply To Prefab Itself");
            if (result == 0) //OK
            {
                DestroyRigidbodySleepOnAwakeComponentFromInOnInspector();
            }
            else if (result == 1) //CANCEL
            {
                //do nothing
            }
            else if (result == 2) //ALT
            {
                GameObject prefab = EditorUtility.GetPrefabParent(gameObj) as GameObject;
               // DestroyImmediate(prefab.GetComponent<RigidbodySleepOnAwake>(), true);
                DestroyRigidbodySleepOnAwakeComponent(prefab);
                isComponentRemoved = true;
            }
        }


        if (isComponentRemoved)
        {
            EditorUtility.SetDirty(gameObj);
        }

        return isComponentRemoved;
    }

    private void DestroyRigidbodySleepOnAwakeComponentFromInOnInspector()
    {
        EditorApplication.update += WaitThenDestroyRigidbodySleepOnAwakeComponent;
    }

    private static void DestroyRigidbodySleepOnAwakeComponent(GameObject gameObj)
    {
        DestroyImmediate(gameObj.GetComponent<RigidbodySleepOnAwake>(), true);
    }

    //returns true if removed

    public static bool RemoveRigidbodySleepOnAwakeToGameObject(GameObject gameObj, bool displayObjectNameInDialogTitle)
    {
        bool isComponentRemoved = false;
        string dialogTitle = "";
        PrefabType prefabType = EditorUtility.GetPrefabType(gameObj);
        
        if (prefabType == PrefabType.None || prefabType == PrefabType.Prefab || prefabType == PrefabType.DisconnectedPrefabInstance) //if not prefab instance nuke the component as it is no longer needed
        {
            DestroyRigidbodySleepOnAwakeComponent(gameObj);
            isComponentRemoved = true;
        }
        else //can't destroy the component as it will break the prefab was disable it instead
        {
            dialogTitle = "Losing prefab";
            if (displayObjectNameInDialogTitle)
            {
                dialogTitle += " for GameObject: " + gameObj.name;
            }
            int result = EditorUtility.DisplayDialogComplex(dialogTitle, "This action will lose the prefab connection. Are you sure you wish to continue or alter prefab directly?", "Continue", "Cancel", "Apply To Prefab Itself");
            if (result == 0) //OK
            {
                DestroyRigidbodySleepOnAwakeComponent(gameObj);
                isComponentRemoved = true;
            }
            else if (result == 1) //CANCEL
            {
                //do nothing
            }
            else if (result == 2) //ALT
            {
                GameObject prefab = EditorUtility.GetPrefabParent(gameObj) as GameObject;
                DestroyRigidbodySleepOnAwakeComponent(prefab);
                isComponentRemoved = true;
            }
        }

        
        if (isComponentRemoved)
        {
            EditorUtility.SetDirty(gameObj);
        }
         

        return isComponentRemoved;
    }

    private void WaitThenDestroyRigidbodySleepOnAwakeComponent()
    {
        Component comp = target as Component;
        GameObject gameObj;
        if (comp != null && comp.gameObject!=null)
        {
            gameObj = comp.gameObject;
            DestroyRigidbodySleepOnAwakeComponent(gameObj);

            EditorUtility.SetDirty(gameObj);
            doesRigidbodySleepOnAwakeExist = false;
        }

        EditorApplication.update -= WaitThenDestroyRigidbodySleepOnAwakeComponent;
    }

    
}