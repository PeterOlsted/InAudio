using System;
using System.Collections.Generic;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public static class DataCleanup 
    {
        public enum CleanupVerbose
        {
            Normal,
            Silent
        }

        public static void Cleanup(CleanupVerbose verbose = CleanupVerbose.Normal)
        {
            int deletedTotal = 0;

            int nodesDeleted = Cleanup(InAudioInstanceFinder.DataManager.AudioTree);
            if (nodesDeleted > 0 && verbose == CleanupVerbose.Normal)
                Debug.Log("Deleted " + nodesDeleted + " Unused Audio Nodes");
            deletedTotal += nodesDeleted;

            nodesDeleted = Cleanup(InAudioInstanceFinder.DataManager.EventTree);
            if (nodesDeleted > 0 && verbose == CleanupVerbose.Normal)
                Debug.Log("Deleted " + nodesDeleted + " Unused Event Nodes");
            deletedTotal += nodesDeleted;

            nodesDeleted = Cleanup(InAudioInstanceFinder.DataManager.MusicTree);
            if (nodesDeleted > 0 && verbose == CleanupVerbose.Normal)
                Debug.Log("Deleted " + nodesDeleted + " Unused Music Nodes");
            deletedTotal += nodesDeleted;

            if (deletedTotal == 0 && verbose == CleanupVerbose.Normal)
            {
                Debug.Log("Nothing to clean up");
            }
        }

        private static int Cleanup<T>(T audioRoot) where T : MonoBehaviour, InITreeNode<T>
        {
            if (audioRoot == null)
                return 0;

            HashSet<MonoBehaviour> objects = new HashSet<MonoBehaviour>();
            var allNodes = audioRoot.GetComponents<MonoBehaviour>();
            for (int i = 0; i < allNodes.Length; ++i)
            {
                objects.Add(allNodes[i]);
            }

            TreeWalker.ForEach(audioRoot, obj =>
            {
                objects.Add(obj);
                obj.GetAuxData().ForEach(o => objects.Add(o));
            });

          int deleted = 0;
            //Delete all objects not in use
            foreach (MonoBehaviour node in objects)
            {
                if (!objects.Contains(node))
                {
                    deleted += 1;
                    InUndoHelper.PureDestroy(node);
                }
            }
            return deleted;
        }
     }
}
