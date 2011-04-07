//Created by Devin Reimer (AlmostLogical Software) - http://www.almostlogical.com
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

class RigidbodySleepToggleMenu : MonoBehaviour
{
    [MenuItem("Edit/Enable Sleep On Awake For All Rigidbodies In Scene")]
    static void EnableSleepOnAwakeForAllRigidbodiesInScene()
    {
        Rigidbody[] rigidbodies = FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
        Rigidbody rigidbody;
        for (int x=0; x<rigidbodies.Length; x++)
        {
            EditorUtility.DisplayProgressBar("Updating All Rigidbodies", "Please Wait...",(float)x/(float)rigidbodies.Length);
            rigidbody = rigidbodies[x];
            if (rigidbody.gameObject.GetComponent<RigidbodySleepOnAwake>() == null)
            {
                RigidbodySleepToggleInspector.AddRigidbodySleepOnAwakeToGameObject(rigidbody.gameObject,true);
            }
        }

        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Edit/Disable Sleep On Awake For All Rigidbodies In Scene")]
    static void DisableSleepOnAwakeForAllRigidbodiesInScene()
    {
        Rigidbody[] rigidbodies = FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
        Rigidbody rigidbody;
        RigidbodySleepOnAwake rigidbodySleepOnAwakeComponent;

        for (int x = 0; x < rigidbodies.Length; x++)
        {
            EditorUtility.DisplayProgressBar("Updating All Rigidbodies", "Please Wait...", (float)x / (float)rigidbodies.Length);
            rigidbody = rigidbodies[x];
            rigidbodySleepOnAwakeComponent = rigidbody.gameObject.GetComponent<RigidbodySleepOnAwake>();
            if (rigidbodySleepOnAwakeComponent != null)
            {
                RigidbodySleepToggleInspector.RemoveRigidbodySleepOnAwakeToGameObject(rigidbody.gameObject, true);
            }
        }

        EditorUtility.ClearProgressBar();
    }
}