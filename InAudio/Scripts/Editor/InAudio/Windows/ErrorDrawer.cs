using InAudioSystem.ExtensionMethods;
using InAudioSystem.InAudioEditor;
using InAudioSystem.Internal;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public static class ErrorDrawer
    {
        public static void AddManagerToScene()
        {
            var go = AssetDatabase.LoadAssetAtPath(FolderSettings.AudioManagerPath, typeof(GameObject)) as GameObject;
            if (go != null)
                PrefabUtility.InstantiatePrefab(go);
            else
            {
                Debug.LogError("The audio manager could not be found in the project.\nEither try and find it manually or reimport InAudio from the Asset Store");
            }
        }

        public static void MissingAudioManager()
        {
            EditorGUILayout.HelpBox("The audio manager could not be found in the scene. Please click the \"Fix it automatically\" button to insert it.\n", MessageType.Error, true);
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Fix it automatically", GUILayout.ExpandWidth(true)))
            {
                AddManagerToScene();
            }
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Find Audio Manager Prefab", GUILayout.Width(180)))
            {
                EditorApplication.ExecuteMenuItem("Window/Project");
                var go = AssetDatabase.LoadAssetAtPath(FolderSettings.AudioManagerPath, typeof(GameObject)) as GameObject;
                if (go != null)
                {
                    EditorGUIUtility.PingObject(go);
                    Selection.objects = new Object[] { go };
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        public static bool IsDataMissing(InCommonDataManager manager)
        {
            return manager.Roots.TrueForAny(r => r == null);
        }

        public static bool IsAllDataMissing(InCommonDataManager manager)
        {
            return manager.Roots.TrueForAll(r => r == null);
        }

        public static bool MissingData(InCommonDataManager manager)
        {
            bool isDataMissing = IsDataMissing(manager);
            if (isDataMissing)
            {
                EditorGUILayout.HelpBox("Some data is missing. Please go to the Aux Window and follow the guide.\nThis is likely due to InAudio needing to upgrade some data.",
                    MessageType.Warning, true);
                if (GUILayout.Button("Open Aux Window"))
                {
                    EditorWindow.GetWindow<AuxWindow>().SelectDataCreation();
                }

                EditorGUILayout.Separator();
                if (GUILayout.Button("Try To Reload Data"))
                {
                    manager.Load(true);
                }
            }
            return isDataMissing;
        }

        public static bool AllDataMissing(InCommonDataManager manager)
        {
            EditorGUILayout.HelpBox("No InAudio project found. Create InAudio project?", MessageType.Info);
            if (GUILayout.Button("Create InAudio data"))
            {
                MissingDataHelper.StartFromScratch(manager);
            }

            return true;
        }

        public static void MissingGuiUserPrefs()
        {
            EditorGUILayout.HelpBox("Couldn't locate InAudio. Please try and reimport the project.\nIf the problem persists, please write to inaudio@outlook.com", MessageType.Error);
            
            if (GUILayout.Button("Auto try and fix it (low change it will work)"))
            {
                var go = new GameObject();
                go.AddComponent<InAudioGUIUserPrefs>();
                PrefabUtility.CreatePrefab(FolderSettings.GUIUserPrefs, go);
                Object.DestroyImmediate(go);
            }
        }
    }
}
