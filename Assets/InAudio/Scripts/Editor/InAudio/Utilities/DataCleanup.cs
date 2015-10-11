using System;
using System.Collections.Generic;
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

            var audioRoot = InAudioInstanceFinder.DataManager.AudioTree;

            //Audio node cleanup
            Action<InAudioNode, HashSet<MonoBehaviour>> action = null;
            action = (node, set) =>
            {
                set.Add(node);
                if (node._nodeData != null)
                    set.Add(node._nodeData);
                for (int i = 0; i < node._children.Count; ++i)
                {
                    action(node._children[i], set);
                }
            };
            int nodesDeleted = Cleanup(audioRoot, action);
            if (nodesDeleted > 0 && verbose == CleanupVerbose.Normal)
                Debug.Log("Deleted " + nodesDeleted + " Unused Audio Nodes");
            deletedTotal += nodesDeleted;

            var eventRoot = InAudioInstanceFinder.DataManager.EventTree;

            //Audio node cleanup
            Action<InAudioEventNode, HashSet<MonoBehaviour>> eventAction = null;
            eventAction = (node, set) =>
            {
                set.Add(node);
                for (int i = 0; i < node._actionList.Count; ++i)
                {
                    set.Add(node._actionList[i]);
                }
                for (int i = 0; i < node._children.Count; ++i)
                {
                    eventAction(node._children[i], set);
                }
            };
            nodesDeleted = Cleanup(eventRoot, eventAction);
            if (nodesDeleted > 0 && verbose == CleanupVerbose.Normal)
                Debug.Log("Deleted " + nodesDeleted + " Unused Event Nodes");
            deletedTotal += nodesDeleted;

            nodesDeleted = DeleteUnusedBanks(InAudioInstanceFinder.DataManager.BankLinkTree);

            if (nodesDeleted > 0 && verbose == CleanupVerbose.Normal)
                Debug.Log("Deleted " + nodesDeleted + " Unused Audio Banks");
            deletedTotal += nodesDeleted;

            if (deletedTotal == 0 && verbose == CleanupVerbose.Normal)
            {
                Debug.Log("Nothing to clean up");
            }
        
        }

        private static int Cleanup<T>(T audioRoot, Action<T, HashSet<MonoBehaviour>> traverse) where T : MonoBehaviour
        {
            if (audioRoot == null)
                return 0;


            HashSet<MonoBehaviour> objects = new HashSet<MonoBehaviour>();
            var allNodes = audioRoot.GetComponents<MonoBehaviour>();
            for (int i = 0; i < allNodes.Length; ++i)
            {
                objects.Add(allNodes[i]);
            }

            HashSet<MonoBehaviour> inUse = new HashSet<MonoBehaviour>();

            traverse(audioRoot, inUse);

            int deleted = 0;
            //Delete all objects not in use
            foreach (MonoBehaviour node in objects)
            {
                if (!inUse.Contains(node))
                {
                    deleted += 1;
                    InUndoHelper.PureDestroy(node);
                }
            }
            return deleted;
        }

        private static int DeleteUnusedBanks(InAudioBankLink bankRoot)
        {
            #region Standard cleanup
            Action<InAudioBankLink, HashSet<MonoBehaviour>> bankAction = null;
            bankAction = (node, set) =>
            {
                set.Add(node);
                for (int i = 0; i < node._children.Count; ++i)
                {
                    bankAction(node._children[i], set);
                }
            };
            int deleteCount = 0;
            HashSet<MonoBehaviour> objects = new HashSet<MonoBehaviour>();
            if (bankRoot != null)
            {
                var allNodes = bankRoot.GetComponents<MonoBehaviour>();
                for (int i = 0; i < allNodes.Length; ++i)
                {
                    objects.Add(allNodes[i]);
                }


                HashSet<MonoBehaviour> inUse = new HashSet<MonoBehaviour>();
                
                bankAction(bankRoot, inUse);
                List<string> toDelete = new List<string>();
                //Delete all objects not in use
                foreach (InAudioBankLink node in objects)
                {
                    if (!inUse.Contains(node))
                    {
                        ++deleteCount;
                        toDelete.Add(node._ID.ToString());
                        InUndoHelper.PureDestroy(node);
                    }
                }
            }

            #endregion

            return deleteCount;
        }
    }
}
