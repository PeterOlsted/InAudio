using System;
using InAudioSystem.Internal;
using UnityEditor;
using UnityEngine;

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

            int levelSize = 3;
            GameObject audioGO = new GameObject();
            GameObject eventGO = new GameObject();
            GameObject bankGO = new GameObject();
            GameObject musicGO = new GameObject();
            GameObject interactiveGO = new GameObject();

            Manager.BankLinkTree = AudioBankWorker.CreateTree(bankGO);
            Manager.AudioTree = AudioNodeWorker.CreateTree(audioGO, levelSize);
            Manager.MusicTree = MusicWorker.CreateTree(musicGO, levelSize);
            Manager.InteractiveMusicTree = InteractiveMusicWorker.CreateTree(interactiveGO, 0);
            Manager.EventTree = AudioEventWorker.CreateTree(eventGO, levelSize);



            SaveAndLoad.CreateDataPrefabs(Manager.AudioTree.gameObject, Manager.MusicTree.gameObject, Manager.EventTree.gameObject, Manager.BankLinkTree.gameObject, Manager.InteractiveMusicTree.gameObject);

            Manager.Load(true);

            if (Manager.BankLinkTree != null)
            {
                var bankLink = Manager.BankLinkTree._children[0];
                bankLink._name = "Default - Auto loaded";
                bankLink._autoLoad = true;

                NodeWorker.AssignToNodes(Manager.AudioTree, node =>
                {
                    var data = (node._nodeData as InFolderData);
                    if (data != null)
                        data.BankLink = Manager.BankLinkTree._getChildren[0];
                });

                NodeWorker.AssignToNodes(Manager.MusicTree, musicNode =>
                {
                    var folder = musicNode as InMusicFolder;
                    if (folder != null)
                        folder._bankLink = Manager.BankLinkTree._getChildren[0];
                });


                var firstAudioFolder = Manager.AudioTree._children[0];
                var random = AudioNodeWorker.CreateChild(firstAudioFolder, AudioNodeType.Random);
                random.Name = "Random Node Example";
                AudioNodeWorker.CreateChild(random, AudioNodeType.Audio, "Empty Example Audio Node");
                AudioNodeWorker.CreateChild(random, AudioNodeType.Audio, "Empty Example Audio Node");
                AudioNodeWorker.CreateChild(random, AudioNodeType.Audio, "Empty Example Audio Node");


                var multi = AudioNodeWorker.CreateChild(firstAudioFolder, AudioNodeType.Random, "Multi-Sound Example");
                AudioNodeWorker.CreateChild(multi, AudioNodeType.Audio, "Empty Example Audio Node");
                AudioNodeWorker.CreateChild(multi, AudioNodeType.Audio, "Empty Example Audio Node");
                AudioNodeWorker.CreateChild(firstAudioFolder, AudioNodeType.Audio, "Empty Example Audio Node");

                var firstEventFolder = Manager.EventTree._children[0];
                firstEventFolder.FoldedOut = true;
                var audioEvent = AudioEventWorker.CreateNode(firstEventFolder, EventNodeType.Event);
                audioEvent.Name = "Playing Random Audio Event";
                var action = AudioEventWorker.AddEventAction<InEventAudioAction>(audioEvent, EventActionTypes.Play);
                audioEvent.FoldedOut = true;
                action.Node = random;

                var firstMusicFolder = Manager.MusicTree._children[0];
                var musicGroup = MusicWorker.CreateMusicGroup(firstMusicFolder, "Empty Music Group");
                firstMusicFolder.FoldedOut = true; 
                MusicWorker.CreateMusicGroup(musicGroup, "Empty Music Group - Child 1");
                MusicWorker.CreateMusicGroup(musicGroup, "Empty Music Group - Child 2");
                musicGroup.FoldedOut = true;


                AssetDatabase.Refresh();
                DataCleanup.Cleanup(DataCleanup.CleanupVerbose.Silent);
                EditorApplication.SaveCurrentSceneIfUserWantsTo();
            }
            else
            {
                Debug.LogError("InAudio: There was a problem creating the data.\nPlease report this bug to inaudio@outlook.com or via the feedback window.");
            }
        }
    }

}