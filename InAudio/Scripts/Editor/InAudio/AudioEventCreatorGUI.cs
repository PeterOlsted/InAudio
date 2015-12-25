using System;
using InAudioSystem;
using InAudioSystem.InAudioEditor;
using InAudioSystem.Internal;
using InAudioSystem.TreeDrawer;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.InAudioEditor
{

    public class AudioEventCreatorGUI : BaseCreatorGUI<InAudioEventNode>
    {
        public AudioEventCreatorGUI(EventWindow window)
            : base(window)
        {
        }

        private int leftWidth;
        private int height;



        public bool OnGUI(int leftWidth, int height)
        {
            BaseOnGUI();

            this.leftWidth = leftWidth;
            this.height = height;


            EditorGUIHelper.DrawColums(DrawLeftSide, DrawRightSide);


            return isDirty;
        }

        private void DrawLeftSide(Rect area)
        {
            Rect treeArea = EditorGUILayout.BeginVertical(GUILayout.Width(leftWidth), GUILayout.Height(height));
            DrawSearchBar();
            EditorGUILayout.BeginVertical();

            isDirty |= treeDrawer.DrawTree(InAudioInstanceFinder.DataManager.EventTree, treeArea);
            SelectedNode = treeDrawer.SelectedNode;

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private void DrawRightSide(Rect area)
        {
            EditorGUILayout.BeginVertical();

            if (SelectedNode != null)
            {
                try
                {
                    isDirty |= AudioEventDrawer.Draw(SelectedNode);
                }
                catch (ExitGUIException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    EditorGUILayout.HelpBox(
                        "An exception is getting caught while trying to draw node. If persistent, please contact inaudio@outlook.com",
                        MessageType.Error,
                        true);
                    Debug.LogException(e);
                }

            }

            EditorGUILayout.EndVertical();
        }

        public override void OnEnable()
        {
            treeDrawer.Filter(n => false);
            treeDrawer.OnNodeDraw = EventDrawer.EventFoldout;
            treeDrawer.OnContext = OnContext;
            treeDrawer.CanDropObjects = CanDropObjects;
            treeDrawer.OnDrop = OnDrop;
            treeDrawer.CanPlaceHere = (parent, place) =>
            {
                if (parent._type == EventNodeType.Event)
                {
                    return false;
                }
                return true;
            };
        }

        public void ReceiveNode(InMusicGroup group)
        {
            if (SelectedNode != null)
            {
                InUndoHelper.DoInGroup(() =>
                {
                    TreeWalker.ForEachParent(SelectedNode, n => n.FoldedOut = true);
                    InUndoHelper.RecordObject(SelectedNode, "Send to Event");                
                    if (SelectedNode.IsRootOrFolder)
                    {
                        var myEvent = AudioEventWorker.CreateNode(SelectedNode, EventNodeType.Event);
                        var myAction = AudioEventWorker.AddEventAction<InEventMusicControl>(myEvent, EventActionTypes.PlayMusic);
                        myAction.Target = group;
                        SelectedNode = myEvent;
                        treeDrawer.SelectedNode = myEvent;
                    }
                    else
                    {
                        var myAction = AudioEventWorker.AddEventAction<InEventMusicControl>(SelectedNode, EventActionTypes.PlayMusic);
                        myAction.Target = group;
                    }
                });
            }
        }

        public void ReceiveNode(InAudioNode node)
        {
            if (SelectedNode != null)
            {
                InUndoHelper.DoInGroup(() =>
                {
                    TreeWalker.ForEachParent(SelectedNode, n => n.FoldedOut = true);
                    InUndoHelper.RecordObject(SelectedNode, "Send to Event");
                    if (SelectedNode.IsRootOrFolder)
                    {
                        var myEvent = AudioEventWorker.CreateNode(SelectedNode, EventNodeType.Event);
                        var myAction = AudioEventWorker.AddEventAction<InEventAudioAction>(myEvent, EventActionTypes.Play);
                        myAction.Target = node;
                        SelectedNode = myEvent;
                        treeDrawer.SelectedNode = myEvent;
                    }
                    else
                    {
                        var myAction = AudioEventWorker.AddEventAction<InEventAudioAction>(SelectedNode, EventActionTypes.Play);
                        myAction.Target = node;
                    }
                    
                });
            }
        }

        protected override void OnDrop(InAudioEventNode audioevent, Object[] objects)
        {
            AudioEventWorker.OnDrop(audioevent, objects);
            treeDrawer.SelectedNode = audioevent;
        }

        protected override bool CanDropObjects(InAudioEventNode audioEvent, Object[] objects)
        {
            return AudioEventWorker.CanDropObjects(audioEvent, objects);
        }

        protected override void OnContext(InAudioEventNode node)
        {
            var menu = new GenericMenu();

            #region Duplicate

            if (!node.IsRoot)
                menu.AddItem(new GUIContent("Duplicate"), false,
                    data => AudioEventWorker.Duplicate(data as InAudioEventNode),
                    node);
            else
                menu.AddDisabledItem(new GUIContent("Duplicate"));
            menu.AddSeparator("");

            #endregion
            #region Duplicate
           

            if (node.IsRootOrFolder)
            {
                menu.AddItem(new GUIContent("Create child folder in new prefab"), false, obj =>
                {
                    CreateFolderInNewPrefab(node);
                }, node);
                menu.AddSeparator("");
            }
            #endregion


            #region Create child

            if (node._type == EventNodeType.Root || node._type == EventNodeType.Folder)
            {
                menu.AddItem(new GUIContent(@"Create Event"), false,
                    data => { CreateChild(node, EventNodeType.Event); }, node);
                menu.AddItem(new GUIContent(@"Create Folder"), false,
                    data => { CreateChild(node, EventNodeType.Folder); }, node);

            }
            if (node._type == EventNodeType.Event)
            {
                menu.AddDisabledItem(new GUIContent(@"Create Event"));
                menu.AddDisabledItem(new GUIContent(@"Create Folder"));

            }

            #endregion

            menu.AddSeparator("");

            #region Delete

            menu.AddItem(new GUIContent(@"Delete"), false, data =>
            {
                treeDrawer.SelectedNode = TreeWalker.GetPreviousVisibleNode(treeDrawer.SelectedNode);
                AudioEventWorker.DeleteNode(node);
            }, node);

            #endregion

            menu.ShowAsContext();
        }

        private void CreateFolderInNewPrefab(InAudioEventNode parent)
        {
            InAudioWindowOpener.ShowNewDataWindow((gameObject =>
            {
                InUndoHelper.DoInGroup(() =>
                {
                    var node = AudioEventWorker.CreateNode(parent, gameObject, EventNodeType.Folder);
                    node.Name += " (External)";
                    node.PlacedExternaly = true;
                });
                
            }));
            isDirty = true;
        }

        private void CreateChild(InAudioEventNode node, EventNodeType type)
        {
            InUndoHelper.DoInGroup(() =>
            {
                InUndoHelper.RegisterUndo(node, "Event Creation");
                AudioEventWorker.CreateNode(node, type);
            });

            node.FoldedOut = true;
        }

        protected override InAudioEventNode Root()
        {
            if (InAudioInstanceFinder.DataManager == null)
            {
                return null;
            }
            return InAudioInstanceFinder.DataManager.EventTree;
        }

        protected override GUIPrefs GUIData
        {
            get
            {
                if (InAudioInstanceFinder.InAudioGuiUserPrefs != null)
                    return InAudioInstanceFinder.InAudioGuiUserPrefs.EventGUIData;
                else
                    return null;
            }
        }


    }

}