using System;
using InAudioSystem.Internal;
using UnityEditor;
using UnityEngine;

#if !UNITY_5_2
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

namespace InAudioSystem.InAudioEditor
{
    public class MissingDataHelper
    {
        public static void StartFromScratch(InCommonDataManager Manager)
        {
            try
            {
                DataCleanup.Cleanup(DataCleanup.CleanupVerbose.Silent);
            }
            catch (Exception)
            {
                Debug.LogError("InAudio: Problem cleaning data before creating project.\nPlease report bug to inaudio@outlook.com or via the feedback window.");
            }

            CreateAll(Manager);
            Manager.ForceLoad();

            try
            {
                var firstAudioFolder = Manager.AudioTree._children[0];

                AudioNodeWorker.CreateChild(firstAudioFolder, AudioNodeType.Audio, "Empty Example Audio Node");

                var random = AudioNodeWorker.CreateChild(firstAudioFolder, AudioNodeType.Random);
                random.Name = "Random Node Example";
                AudioNodeWorker.CreateChild(random, AudioNodeType.Audio, "Empty Example Audio Node");
                AudioNodeWorker.CreateChild(random, AudioNodeType.Audio, "Empty Example Audio Node");
                AudioNodeWorker.CreateChild(random, AudioNodeType.Audio, "Empty Example Audio Node");

                var multi = AudioNodeWorker.CreateChild(firstAudioFolder, AudioNodeType.Multi, "Multi-Sound Example");
                AudioNodeWorker.CreateChild(multi, AudioNodeType.Audio, "Played simultaneously");
                AudioNodeWorker.CreateChild(multi, AudioNodeType.Audio, "Played simultaneously");

                var sequence = AudioNodeWorker.CreateChild(firstAudioFolder, AudioNodeType.Sequence, "Sequence-Sound Example");
                AudioNodeWorker.CreateChild(sequence, AudioNodeType.Audio, "Played first");
                AudioNodeWorker.CreateChild(sequence, AudioNodeType.Audio, "Played secondly");

                var firstMusicFolder = Manager.MusicTree._children[0];
                var musicGroup = MusicWorker.CreateMusicGroup(firstMusicFolder, "Example Music Group");
                firstMusicFolder.EditorSettings.IsFoldedOut = true;
                MusicWorker.CreateMusicGroup(musicGroup, "Example Music Group - Layer 1");
                MusicWorker.CreateMusicGroup(musicGroup, "Example Music Group - Layer 2");
                musicGroup.EditorSettings.IsFoldedOut = true;



                var firstEventFolder = Manager.EventTree._children[0];
                firstEventFolder.EditorSettings.IsFoldedOut = true;

                var audioEvent = AudioEventWorker.CreateNode(firstEventFolder, EventNodeType.Event);
                audioEvent.Name = "Playing Music & Random Audio Event";
                var musicAction = AudioEventWorker.AddEventAction<InEventMusicControl>(audioEvent, EventActionTypes.PlayMusic);
                audioEvent.EditorSettings.IsFoldedOut = true;
                musicAction.MusicGroup = musicGroup;
                var action = AudioEventWorker.AddEventAction<InEventAudioAction>(audioEvent, EventActionTypes.Play);
                audioEvent.EditorSettings.IsFoldedOut = true;
                action.Node = random;
                
                AssetDatabase.Refresh();
                DataCleanup.Cleanup(DataCleanup.CleanupVerbose.Silent);

#if !UNITY_5_2
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
#else
                EditorApplication.MarkSceneDirty();
                EditorApplication.SaveCurrentSceneIfUserWantsTo();
#endif
            }
            catch (Exception)
            {
                Debug.LogError("InAudio: There was a problem creating the data.\nPlease report this bug to inaudio@outlook.com or via the feedback window.");
            }
        }

        private static void Create(InCommonDataManager Manager, bool ignoreMissing)
        {
            if (Manager.AudioTree == null || ignoreMissing)
                SaveAndLoad.CreatePrefab(ref Manager.AudioRoot, AudioNodeWorker.CreateTree, FolderSettings.AudioSaveDataPath);
            if (Manager.EventTree == null || ignoreMissing)
                SaveAndLoad.CreatePrefab(ref Manager.EventRoot, AudioEventWorker.CreateTree, FolderSettings.EventSaveDataPath);
            if (Manager.MusicTree == null || ignoreMissing)
                SaveAndLoad.CreatePrefab(ref Manager.MusicRoot, MusicWorker.CreateTree, FolderSettings.MusicSaveDataPath);
            if (Manager.SettingsTree == null || ignoreMissing)
                SaveAndLoad.CreatePrefab(ref Manager.SettingsRoot, NodeFactory.CreateSettingsTree, FolderSettings.SettingsSaveDataPath);
        }

        public static void CreateIfMissing(InCommonDataManager Manager)
        {
            Create(Manager, false);
        }

        public static void CreateAll(InCommonDataManager Manager)
        {
            Create(Manager, true);
        }

        
    }

}