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
            bool missingaudio = manager.AudioTree == null;
            bool missingaudioEvent = manager.EventTree == null;
            bool missingMusic = manager.MusicTree == null;
            bool missingBankLink = manager.BankLinkTree == null;

            return missingaudio || missingMusic || missingaudioEvent || missingBankLink;
        }

        public static bool IsAllDataMissing(InCommonDataManager manager)
        {
            bool missingaudio = manager.AudioTree == null;
            bool missingaudioEvent = manager.EventTree == null;
            bool missingBankLink = manager.BankLinkTree == null;
            bool missingMusic = manager.MusicTree == null;

            return missingaudio && missingaudioEvent && missingBankLink && missingMusic;
        }

        public static bool MissingData(InCommonDataManager manager)
        {
            bool missingaudio = manager.AudioTree == null;
            bool missingaudioEvent = manager.EventTree == null;
            bool missingBankLink = manager.BankLinkTree == null;
            bool missingMusic = manager.MusicTree == null;

            bool areAnyMissing = missingaudio || missingaudioEvent || missingBankLink || missingMusic;

            if (areAnyMissing)
            {
                string missingAudioInfo = missingaudio ? "Missing Audio Data\n" : "";
                string missingEventInfo = missingaudioEvent ? "Missing Event Data\n" : "";
                string missingMusicInfo = missingMusic ? "Missing Music Data\n" : "";
                string missingBankLinkInfo = missingBankLink ? "Missing BankLink Data\n" : "";
                string missingInteractiveMusicInfo = missingBankLink ? "Missing Interactive Music Data\n" : "";
                EditorGUILayout.HelpBox(missingAudioInfo + missingMusicInfo + missingEventInfo + missingBankLinkInfo + missingInteractiveMusicInfo + "Some data is missing. Please go to the Aux Window and follow the guide.\nThis is likely due to InAudio needing to upgrade some data.",
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
            return areAnyMissing;
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
