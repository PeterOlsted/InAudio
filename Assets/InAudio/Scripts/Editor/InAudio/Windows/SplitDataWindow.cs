using System;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public class SplitDataWindow : EditorWindow
    {
        private GameObject newObject;
        private Action<GameObject> callback;


        public void AssignCallback(Action<GameObject> callback)
        {
            this.callback = callback;
        }

        private void OnGUI()
        {
            if (callback != null)
            {
                EditorGUILayout.LabelField("Drag in a prefab where to store the new game object");
                newObject = EditorGUILayout.ObjectField(newObject, typeof (GameObject), false) as GameObject;
                if (newObject != null)
                {
                    if (GUILayout.Button("Place data here"))
                    {
                        callback(newObject);
                        Close();
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No callback excists for this window, please close the window and try the operation again.", MessageType.Error);
            }
        }
    }
}