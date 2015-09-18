using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    [CustomEditor(typeof(InAudioEventHook))]
    public class EventHookDrawer : Editor
    {
        SerializedObject hookObj
        {
            get { return serializedObject; }
        }

        public override void OnInspectorGUI()
        {
            hookObj.Update();
            EditorGUI.BeginChangeCheck();

            
            
            EditorGUILayout.PropertyField(hookObj.FindProperty("onEnable"));
            EditorGUILayout.PropertyField(hookObj.FindProperty("onStart"));
            EditorGUILayout.PropertyField(hookObj.FindProperty("onDisable"));
            EditorGUILayout.PropertyField(hookObj.FindProperty("onDestroy"));
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(hookObj.FindProperty("onEnableMusic"));
            EditorGUILayout.PropertyField(hookObj.FindProperty("onStartMusic"));
            EditorGUILayout.PropertyField(hookObj.FindProperty("onDisableMusic"));
            EditorGUILayout.PropertyField(hookObj.FindProperty("onDestroyMusic"));

            if (!HasCollision())
                EditorGUILayout.HelpBox("No colliders found on this object.", MessageType.Info, true);

            EditorGUILayout.PropertyField(hookObj.FindProperty("CollisionList"));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private bool HasCollision()
        {
            var hook = target as MonoBehaviour;

            if (hook.GetComponent<Collider>())
                return true;
            if (hook.GetComponent<Collider2D>())
                return true;
            return false;
        }

    }


}
