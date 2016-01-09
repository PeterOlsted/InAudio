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
                firstMusicFolder.FoldedOut = true;
                MusicWorker.CreateMusicGroup(musicGroup, "Example Music Group - Extra Layer 1");
                MusicWorker.CreateMusicGroup(musicGroup, "Example Music Group - Extra Layer 2");
                musicGroup.FoldedOut = true;



                var firstEventFolder = Manager.EventTree._children[0];
                firstEventFolder.FoldedOut = true;

                var audioEvent = AudioEventWorker.CreateNode(firstEventFolder, EventNodeType.Event);
                audioEvent.Name = "Playing Music & Random Audio Event";
                var musicAction = AudioEventWorker.AddEventAction<InEventMusicControl>(audioEvent, EventActionTypes.PlayMusic);
                audioEvent.FoldedOut = true;
                musicAction.MusicGroup = musicGroup;
                var action = AudioEventWorker.AddEventAction<InEventAudioAction>(audioEvent, EventActionTypes.Play);
                audioEvent.FoldedOut = true;
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
                CreateAudioPrefab(Manager);
            if (Manager.EventTree == null || ignoreMissing)
                CreateEventPrefab(Manager);
            if (Manager.MusicTree == null || ignoreMissing)
                CreateMusicPrefab(Manager);
        }

        public static void CreateIfMissing(InCommonDataManager Manager)
        {
            Create(Manager, false);
        }

        public static void CreateAll(InCommonDataManager Manager)
        {
            Create(Manager, true);
        }

        private const int levelSize = 3;
        private static GameObject CreateEventPrefab(InCommonDataManager Manager)
        {
            GameObject go = new GameObject();
            Manager.EventTree = AudioEventWorker.CreateTree(go, levelSize);
            return SaveAndLoad.CreatePrefab(Manager.EventTree.gameObject, FolderSettings.EventSaveDataPath);
        }

        private static GameObject CreateMusicPrefab(InCommonDataManager Manager)
        {
            GameObject go = new GameObject();
            Manager.MusicTree = MusicWorker.CreateTree(go, levelSize);
            return SaveAndLoad.CreatePrefab(Manager.MusicTree.gameObject, FolderSettings.MusicSaveDataPath);
        }

        private static GameObject CreateAudioPrefab(InCommonDataManager Manager)
        {
            GameObject go = new GameObject();
            Manager.AudioTree = AudioNodeWorker.CreateTree(go, levelSize);
            return SaveAndLoad.CreatePrefab(Manager.AudioTree.gameObject, FolderSettings.AudioSaveDataPath);
        }
    }

}