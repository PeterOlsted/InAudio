using System;
using System.Collections.Generic;
using System.Linq;
using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.InAudioEditor
{
    public static class AudioEventWorker
    {
        private static InAudioEventNode CreateRoot(GameObject go, int guid)
        {
            var node = go.AddComponentUndo<InAudioEventNode>();
            node._type = EventNodeType.Root;
            node._guid = guid;
            node.FoldedOut = true;
            node.Name = "Event Root";
            return node;
        }

        private static InAudioEventNode CreateFolder(GameObject go, int guid, InAudioEventNode parent)
        {
            var node = go.AddComponentUndo<InAudioEventNode>();
            node._type = EventNodeType.Folder;
            node._guid = guid;
            node.Name = parent.Name + " Child";
            node.AssignParent(parent);
            return node;
        }

        public static void DeleteNodeNoGroup(InAudioEventNode node)
        {
            
            InUndoHelper.RegisterUndo(node._parent, "Event Deletion");
            node._parent._children.Remove(node);
            DeleteNodeRec(node);
            
        }

        public static void DeleteNode(InAudioEventNode node)
        {
            InUndoHelper.DoInGroup(() =>
            {
                InUndoHelper.RegisterUndo(node._parent, "Event Deletion");
                node._parent._children.Remove(node);
                DeleteNodeRec(node);
            });
        }

        private static void DeleteNodeRec(InAudioEventNode node)
        {
            for (int i = 0; i < node._actionList.Count; i++)
            {
                InUndoHelper.Destroy(node._actionList[i]);
            }
            for (int i = 0; i < node._children.Count; ++i)
            {
                DeleteNodeRec(node._children[i]);
            }

            InUndoHelper.Destroy(node);
        }


        private static InAudioEventNode CreateEvent(GameObject go, InAudioEventNode parent, int guid, EventNodeType type)
        {
            var node = go.AddComponentUndo<InAudioEventNode>();
            node._type = type;
            node._guid = guid;
            node.Name = parent.Name + " Child";
            InUndoHelper.RecordObject(parent, "Parrent asign");
            node.AssignParent(parent);
            return node;
        }

        public static InAudioEventNode CreateTree(GameObject go, int levelSize)
        {
            var tree = CreateRoot(go, GUIDCreator.Create());

            for (int i = 0; i < levelSize; ++i)
            {
                var eventNode = CreateFolder(go, GUIDCreator.Create(), tree);
                eventNode.Name = "Event Folder " + i;
            }

            return tree;
        }

        public static InAudioEventNode CreateNode(InAudioEventNode parent, EventNodeType type, string name)
        {
            var node = CreateNode(parent, type);
            node.Name = name;
            return node;
        }

        public static InAudioEventNode CreateNode(InAudioEventNode parent, EventNodeType type)
        {
            var child = CreateEvent(parent.gameObject, parent, GUIDCreator.Create(), type);
            child.FoldedOut = true;

            return child;
        }

        public static InAudioEventNode CreateNode(InAudioEventNode parent, GameObject go, EventNodeType type)
        {
            var child = CreateEvent(go, parent, GUIDCreator.Create(), type);
            child.FoldedOut = true;

            return child;
        }

        public static void ReplaceActionDestructiveAt(InAudioEventNode audioEvent, EventActionTypes enumType, int toRemoveAndInsertAt)
        {
            //A reel mess this function.
            //It adds a new component of the specied type, replaces the current at the toRemoveAndInsertAt index, and then deletes the old one
            float delay = audioEvent._actionList[toRemoveAndInsertAt].Delay;
            Object target = audioEvent._actionList[toRemoveAndInsertAt].Target;
            var newActionType = AudioEventAction.ActionEnumToType(enumType);

            InUndoHelper.Destroy(audioEvent._actionList[toRemoveAndInsertAt]);
            //UndoHelper.RecordObject(audioEvent, "Event Action Creation");

            audioEvent._actionList.RemoveAt(toRemoveAndInsertAt);
            var added = AddEventAction(audioEvent, newActionType, enumType);

            added.Delay = delay;
            added.Target = target; //Attempt to set the new value, will only work if it is the same type

            audioEvent._actionList.Insert(toRemoveAndInsertAt, added);
            audioEvent._actionList.RemoveLast();
        }

        public static T AddEventAction<T>(InAudioEventNode audioevent, EventActionTypes enumType) where T : AudioEventAction
        {
            var eventAction = audioevent.gameObject.AddComponentUndo<T>();
            audioevent._actionList.Add(eventAction);
            eventAction._eventActionType = enumType;
            return eventAction;
        }

        public static AudioEventAction AddEventAction(InAudioEventNode audioevent, Type eventActionType, EventActionTypes enumType)
        {
            InUndoHelper.RecordObject(audioevent, "Event Action Creation");
            var eventAction = audioevent.gameObject.AddComponentUndo(eventActionType) as AudioEventAction;
            audioevent._actionList.Add(eventAction);
            eventAction._eventActionType = enumType;

            return eventAction;
        }

        public static InAudioEventNode DeleteActionAtIndex(InAudioEventNode audioevent, int index)
        {

            InUndoHelper.RecordObject(audioevent, "Event Action Creation");
            InUndoHelper.Destroy(audioevent._actionList[index]);


            audioevent._actionList.RemoveAt(index);

            return audioevent;
        }

        public static InAudioEventNode CopyTo(InAudioEventNode audioEvent, InAudioEventNode newParent)
        {
            return NodeWorker.DuplicateHierarchy(audioEvent, newParent, newParent.gameObject, (@oldNode, newNode) =>
            {
                newNode._actionList.Clear();
                for (int i = 0; i < oldNode._actionList.Count; i++)
                {
                    newNode._actionList.Add(NodeWorker.CopyComponent(oldNode._actionList[i]));
                }
            });
        }

        public static InAudioEventNode Duplicate(InAudioEventNode audioEvent)
        {
            return NodeWorker.DuplicateHierarchy(audioEvent, (@oldNode, newNode) =>
            {
                newNode._actionList.Clear();
                for (int i = 0; i < oldNode._actionList.Count; i++)
                {
                    newNode._actionList.Add(NodeWorker.CopyComponent(oldNode._actionList[i]));
                }
            });
        }

        public static bool CanDropObjects(InAudioEventNode audioEvent, Object[] objects)
        {
            if (objects.Length == 0 || audioEvent == null)
                return false;

            if (audioEvent._type == EventNodeType.Event)
            {
                var nonEventDrop = CanDropNonEvent(objects);

                return nonEventDrop;
            }
            else if (audioEvent._type == EventNodeType.Folder || audioEvent._type == EventNodeType.Root)
            {
                var draggingEvent = objects[0] as InAudioEventNode;
                if (draggingEvent != null)
                {

                    if (draggingEvent._type == EventNodeType.Event)
                        return true;
                    if ((draggingEvent._type == EventNodeType.Folder && !NodeWorker.IsChildOf(draggingEvent, audioEvent)) ||
                        draggingEvent._type == EventNodeType.EventGroup)
                        return true;
                }
                else
                {
                    var nonEventDrop = CanDropNonEvent(objects) && !audioEvent.IsRootOrFolder;

                    return nonEventDrop;
                }
            }
            else if (audioEvent._type == EventNodeType.EventGroup)
            {
                var draggingEvent = objects[0] as InAudioEventNode;
                if (draggingEvent == null)
                    return false;
                if (draggingEvent._type == EventNodeType.Event)
                    return true;
            }


            return false;
        }

        private static bool CanDropNonEvent(Object[] objects)
        {
            var audioNodes = GetConvertedList<InAudioNode>(objects.ToList());
            bool audioNodeDrop = audioNodes.TrueForAll(node => node != null && node.IsPlayable);

            var audioBankLinks = GetConvertedList<InAudioBankLink>(objects.ToList());
            bool bankLinkDrop = audioBankLinks.TrueForAll(node => node != null && node._type == AudioBankTypes.Bank);

            var musicLinks = GetConvertedList<InMusicGroup>(objects.ToList());
            bool musicDrop = musicLinks.TrueForAll(node => node != null && node._type == MusicNodeType.Music);

            return audioNodeDrop | bankLinkDrop | musicDrop;
        }

        private static List<T> GetConvertedList<T>(List<Object> toConvert) where T : class
        {
            return toConvert.ConvertAll(obj => obj as T);
        }

        public static bool OnDrop(InAudioEventNode audioevent, Object[] objects)
        {
            InUndoHelper.DoInGroup(() =>
            {
            //if (audioevent.Type == EventNodeType.Folder)
            //{
            //    UndoHelper.RecordObjectInOld(audioevent, "Created event");
            //    audioevent = CreateNode(audioevent, EventNodeType.Event);
            //}

                if (objects[0] as InAudioEventNode)
                {
                    var movingEvent = objects[0] as InAudioEventNode;
                    

                    if (movingEvent.gameObject != audioevent.gameObject)
                    {
                        if (EditorUtility.DisplayDialog("Move?",
                            "Warning, this will break all external references to this and all child nodes!\n" +
                            "Move node from\"" + movingEvent.gameObject.name +
                            "\" to \"" + audioevent.gameObject.name + "\"?", "Ok", "Cancel"))
                        {
                            InUndoHelper.DoInGroup(() =>
                            {
                                CopyTo(movingEvent, audioevent);
                                DeleteNodeNoGroup(movingEvent);
                            });
                        }
                    }
                    else
                    {
                        InUndoHelper.RecordObjectFull(new Object[] { audioevent, movingEvent, movingEvent._parent }, "Event Move");
                        NodeWorker.ReasignNodeParent(movingEvent, audioevent);
                        audioevent.IsFoldedOut = true;
                    }
                }

                var audioNode = objects[0] as InAudioNode;
                if (audioNode != null && audioNode.IsPlayable)
                {

                    InUndoHelper.RecordObjectFull(audioevent, "Adding of Audio Action");
                    var action = AddEventAction<InEventAudioAction>(audioevent,
                        EventActionTypes.Play);
                    action.Node = audioNode;

                }

                var musicGroup = objects[0] as InMusicGroup;
                if (musicGroup != null)
                {

                    InUndoHelper.RecordObjectFull(audioevent, "Adding of Music Action");
                    var action = AddEventAction<InEventMusicControl>(audioevent,
                        EventActionTypes.PlayMusic);
                    action.MusicGroup = musicGroup;

                }

                var audioBank = objects[0] as InAudioBankLink;
                if (audioBank != null)
                {
                    InUndoHelper.RecordObjectFull(audioevent, "Adding of Bank Load Action");
                    var action = AddEventAction<InEventBankLoadingAction>(audioevent,
                        EventActionTypes.BankLoading);
                    action.BankLink = audioBank;
                }
                Event.current.UseEvent();
            });
            return true;
        }

    }
}
