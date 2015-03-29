using InAudioSystem.InAudioEditor;
using InAudioSystem.Internal;
using UnityEditor;
using UnityEngine;

namespace  InAudioSystem.InAudioEditor
{
    public static class ErrorDrawer {
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
            EditorGUILayout.HelpBox("The audio manager could not be found in the scene\nClick the \"Fix it for me\" button or drag the prefab found at \"InAudio/Prefabs/InAudio Manager\" from the Project window into the scene", MessageType.Error, true);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Fix it"))
            {
                AddManagerToScene();
            }
            EditorGUILayout.Separator();
            if (GUILayout.Button("Find Audio Manager Prefab")) 
            {
                EditorApplication.ExecuteMenuItem("Window/Project");
                var go = AssetDatabase.LoadAssetAtPath(FolderSettings.AudioManagerPath, typeof(GameObject)) as GameObject;
                if (go != null)
                {
                    EditorGUIUtility.PingObject(go);
                    Selection.objects = new Object[] { go};
                }
            }
            EditorGUILayout.EndHorizontal();
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
                string missingEventInfo =  missingaudioEvent ? "Missing Event Data\n" : "";
                string missingMusicInfo = missingMusic ? "Missing Music Data\n" : "";
                string missingBankLinkInfo = missingBankLink ? "Missing BankLink Data\n" : "";
                EditorGUILayout.HelpBox(missingAudioInfo + missingMusicInfo + missingEventInfo + missingBankLinkInfo + "Please go to the Aux Window and create the missing data",
                    MessageType.Error, true);
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
    }   
}
