using System;
using System.Linq;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using InAudioSystem.TreeDrawer;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public class AudioCreatorGUI : BaseCreatorGUI<InAudioNode>
    {
        public AudioCreatorGUI(InAudioWindow window)
            : base(window)
        {
            this.window = window;
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

        public override void OnEnable()
        {
            base.OnEnable();    
            treeDrawer.CanPlaceHere = CanPlaceHere;
            treeDrawer.AssignNewParent = AssignNewParent;
            treeDrawer.DeattachFromParent = DeattachFromParent;
        }

        private void AssignNewParent(InAudioNode newParent, InAudioNode node, int index)
        {
            if (newParent._type == AudioNodeType.Random)
            {
                var randomData = newParent._nodeData as RandomData;
                InUndoHelper.RecordObject(randomData, "Random weights");
                randomData.weights.Insert(index, 50);                
            }
            newParent._children.Insert(index, node);
            node._parent = newParent;
        }

        private void DeattachFromParent(InAudioNode node)
        {
            var parent = node._parent;
            if (parent._type == AudioNodeType.Random)
            {
                Undo.RecordObject(parent._nodeData, "Random weights");
                int currentIndexInParent = parent._children.IndexOf(node);
                (parent._nodeData as RandomData).weights.RemoveAt(currentIndexInParent);
            }
            parent._getChildren.Remove(node);
        }

        private bool CanPlaceHere(InAudioNode newParent, InAudioNode toPlace)
        {
            if (newParent._type == AudioNodeType.Audio)
                return false;
            return true;
        }

        private void DrawLeftSide(Rect area)
        {
            Rect treeArea = EditorGUILayout.BeginVertical(GUILayout.Width(leftWidth), GUILayout.Height(height));
            DrawSearchBar();

            EditorGUILayout.BeginVertical();

            isDirty |= treeDrawer.DrawTree(InAudioInstanceFinder.DataManager.AudioTree, treeArea);
            SelectedNode = treeDrawer.SelectedNode;
            if (GUIData != null)
            {
                GUIData.SelectedNode = treeDrawer.SelectedNode._ID;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private void DrawRightSide(Rect area) 
        {
            EditorGUILayout.BeginVertical();

            if (SelectedNode != null)
            {
                DrawTypeControls(SelectedNode);
                GUI.enabled = true;              

            }

            EditorGUILayout.EndVertical();
        }

        private void DrawTypeControls(InAudioNode node)
        {
            try
            {
                var type = node._type;
                if (node._nodeData != null)
                {
                    switch (type)
                    {
                        case AudioNodeType.Audio:
                            AudioDataDrawer.Draw(node);
                            break;
                        case AudioNodeType.Sequence:
                            SequenceDataDrawer.Draw(node);
                            break;
                        case AudioNodeType.Random:
                            RandomDataDrawer.Draw(node);
                            break;
                        case AudioNodeType.Multi:
                            MultiDataDrawer.Draw(node);
                            break;
                        case AudioNodeType.Folder:
                            FolderDrawer.Draw(node);
                            break;
                        case AudioNodeType.Track:
                            TrackDataDrawer.Draw(node);
                            break;
                        case AudioNodeType.Root:
                            FolderDrawer.Draw(node);
                            break;
                    }
                }
                else if (SelectedNode._type == AudioNodeType.Folder || SelectedNode._type == AudioNodeType.Root)
                {
                    FolderDrawer.Draw(node);
                }
                else
                {
                    EditorGUILayout.HelpBox("Corrupt data. The data for this node does either not exist or is corrupt.",
                        MessageType.Error, true);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Create new element", GUILayout.Width(150)))
                    {
                        AudioNodeWorker.AddDataClass(node);
                    }


                    EditorGUILayout.EndHorizontal();
                }
            }
            catch (ExitGUIException e)
            {
                throw e;
            }
            catch (ArgumentException e)
            {
                throw e;
            }
            /*catch (Exception e)
            {
                //While this catch was made to catch persistent errors,  like a missing null check, it can also catch other errors
                EditorGUILayout.BeginVertical();
                EditorGUILayout.HelpBox("An exception is getting caught while trying to draw node. ", MessageType.Error,
                    true);
                if (GUILayout.Button("Create new element", GUILayout.Width(150)))
                {
                    AudioNodeWorker.AddDataClass(node);
                }
                EditorGUILayout.TextArea(e.ToString());
                EditorGUILayout.EndVertical();
            }*/
        }

        protected override bool CanDropObjects(InAudioNode node, UnityEngine.Object[] objects)
        {
            if (node == null || objects == null)
                return false;

            int clipCount = DragAndDrop.objectReferences.Count(p => p is AudioClip);
            int nodeCount = DragAndDrop.objectReferences.Count(p => p is InAudioNode);

            if (DragAndDrop.objectReferences.Length == 0)
                return false;

            if (clipCount == objects.Length) //Handle clip count
            {
                //clipCount == 1 means it overrides the current clip
                if (node._type == AudioNodeType.Audio && clipCount != 1)
                    return false;
                return true;
            }
            else if (nodeCount == objects.Length) //Handle audio node drag n drop
            {
                if (node._type == AudioNodeType.Audio) //Can't drop on an audionode as it can't have children
                    return false;

                var draggingNode = objects[0] as InAudioNode;

                if (!node.IsRootOrFolder && draggingNode.IsRootOrFolder)
                    return false;

                if (node == draggingNode)
                    return false;

                return !NodeWorker.IsChildOf(objects[0] as InAudioNode, node);
            }
            return false;
        }

        protected override void OnDrop(InAudioNode node, UnityEngine.Object[] objects)
        {
            if (objects[0] as InAudioNode != null) //Drag N Drop internally in the tree, change the parent
            {
                
                InUndoHelper.DoInGroup(() =>
                {
                    node.IsFoldedOut = true;
                    var nodeToMove = objects[0] as InAudioNode;
                    
                    if (node.gameObject != nodeToMove.gameObject)
                    {
                        if (EditorUtility.DisplayDialog("Move?",
                            "Warning, this will break all external references to this and all child nodes!\n" +
                            "Move node from\"" + nodeToMove.gameObject.name +
                            "\" to \"" + node.gameObject.name + "\"?", "Ok", "Cancel"))
                        {
                            treeDrawer.SelectedNode = treeDrawer.SelectedNode._getParent;
                            isDirty = false;
                            AudioNodeWorker.CopyTo(nodeToMove, node);
                            AudioNodeWorker.DeleteNodeNoGroup(nodeToMove);
                        }

                    }
                    else
                    {
                        MoveNode(node, nodeToMove);
                    }
                });
                
            }
            else if (node._type != AudioNodeType.Audio) //Create new audio nodes when we drop clips
            {
                InUndoHelper.DoInGroup(() =>
                {
                    InUndoHelper.RecordObject(InUndoHelper.NodeUndo(node), "Adding Nodes to " + node.Name);

                    AudioClip[] clips = objects.Convert(o => o as AudioClip);

                    Array.Sort(clips, (clip, audioClip) => StringLogicalComparer.Compare(clip.name, audioClip.name));

                    for (int i = 0; i < clips.Length; ++i)
                    {
                        var clip = clips[i];
                        var child = AudioNodeWorker.CreateChild(node, AudioNodeType.Audio);
                        var path = AssetDatabase.GetAssetPath(clip);
                        try
                        {
                            //Try and get the name of the clip. Gets the name and removes the end. Assets/IntroSound.mp3 -> IntroSound
                            int lastIndex = path.LastIndexOf('/') + 1;
                            child.Name = path.Substring(lastIndex, path.LastIndexOf('.') - lastIndex);
                        }
                        catch (Exception)
                        //If it happens to be a mutant path. Not even sure if this is possible, but better safe than sorry
                        {
                            child.Name = node.Name + " Child";
                        }

                        var audioData = (child._nodeData as InAudioData);
                        audioData._clip = clip;

                        AudioBankWorker.AddNodeToBank(child);
                        Event.current.UseEvent();
                    }
                });
                
            }
            else //Then it must be an audio clip dropped on an audio node, so assign the clip to that node
            {
                InUndoHelper.DoInGroup(() =>
                {
                    var nodeData = (node._nodeData as InAudioData);
                    if (nodeData != null)
                    {
                        InUndoHelper.RecordObject(InUndoHelper.NodeUndo(node), "Change Audio Clip In " + node.Name);
                        nodeData._clip = objects[0] as AudioClip;
                    }
                });
                Event.current.UseEvent();

            }
        }

        private static void MoveNode(InAudioNode node, InAudioNode nodeToMove)
        {
            InUndoHelper.RecordObject(
                new UnityEngine.Object[]
                {node, node._nodeData, node._parent, node._parent != null ? node._parent._nodeData : null, nodeToMove._parent._nodeData, nodeToMove, nodeToMove._parent, nodeToMove._parent._nodeData}.AddObj(
                    AudioBankWorker.GetAllBanks().ToArray()),
                "Audio Node Move");

            NodeWorker.ReasignNodeParent(nodeToMove, node);
            AudioBankWorker.RebuildBanks();
            Event.current.UseEvent();
        }

        protected override void OnContext(InAudioNode node)
        {
            var menu = new GenericMenu();
            menu.AddDisabledItem(new GUIContent(node.Name));
            menu.AddSeparator("");
            #region Duplicate

            if (!node.IsRoot)
                menu.AddItem(new GUIContent("Duplicate"), false, data => AudioNodeWorker.Duplicate(node), node);
            else
                menu.AddDisabledItem(new GUIContent("Duplicate"));
            menu.AddSeparator("");

            #endregion

            if (node.IsRootOrFolder)
            {
                menu.AddItem(new GUIContent("Create child folder in new prefab"), false, obj =>
                {
                    CreateFolderInNewPrefab(node);
                }, node);
                menu.AddSeparator("");
            }

            #region Create child

                if (node._type == AudioNodeType.Audio || node._type == AudioNodeType.Voice)
            //If it is a an audio source, it cannot have any children
            {
                menu.AddDisabledItem(new GUIContent(@"Create Child/Folder"));
                menu.AddDisabledItem(new GUIContent(@"Create Child/Audio"));
                menu.AddDisabledItem(new GUIContent(@"Create Child/Random"));
                menu.AddDisabledItem(new GUIContent(@"Create Child/Sequence"));
                menu.AddDisabledItem(new GUIContent(@"Create Child/Multi"));
#if UNITY_TRACK
                menu.AddDisabledItem(new GUIContent(@"Create Child/Track"));
#endif
                //menu.AddDisabledItem(new GUIContent(@"Create Child/Voice"));
            }
            else
            {
                if (node.IsRootOrFolder)
                    menu.AddItem(new GUIContent(@"Create Child/Folder"), false,
                        (obj) => CreateChild(node, AudioNodeType.Folder), node);
                else
                    menu.AddDisabledItem(new GUIContent(@"Create Child/Folder"));
                menu.AddItem(new GUIContent(@"Create Child/Audio"), false,
                    (obj) => CreateChild(node, AudioNodeType.Audio), node);
                menu.AddItem(new GUIContent(@"Create Child/Random"), false,
                    (obj) => CreateChild(node, AudioNodeType.Random), node);
                menu.AddItem(new GUIContent(@"Create Child/Sequence"), false,
                    (obj) => CreateChild(node, AudioNodeType.Sequence), node);
                menu.AddItem(new GUIContent(@"Create Child/Multi"), false,
                    (obj) => CreateChild(node, AudioNodeType.Multi), node);
#if UNITY_TRACK
                if (node.IsRootOrFolder)
                {
                    menu.AddItem(new GUIContent(@"Create Child/Track"), false,
                        (obj) => CreateChild(node, AudioNodeType.Track), node);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(@"Create Child/Track"));
                }
#endif
                //menu.AddItem(new GUIContent(@"Create Child/Track"), false,      (obj) => CreateChild(node, AudioNodeType.Track), node);
                //menu.AddItem(new GUIContent(@"Create Child/Voice"), false,      (obj) => CreateChild(node, AudioNodeType.Voice), node);
            }

            #endregion

            menu.AddSeparator("");

            #region Add new parent

            if (node._parent != null &&
                (node._parent._type == AudioNodeType.Folder || node._parent._type == AudioNodeType.Root))
                menu.AddItem(new GUIContent(@"Add Parent/Folder"), false,
                    (obj) => AudioNodeWorker.AddNewParent(node, AudioNodeType.Folder), node);
            else
                menu.AddDisabledItem(new GUIContent(@"Add Parent/Folder"));
            menu.AddDisabledItem(new GUIContent(@"Add Parent/Audio"));
            if (node._parent != null && node._type != AudioNodeType.Folder)
            {
                menu.AddItem(new GUIContent(@"Add Parent/Random"), false,
                    (obj) => AudioNodeWorker.AddNewParent(node, AudioNodeType.Random), node);
                menu.AddItem(new GUIContent(@"Add Parent/Sequence"), false,
                    (obj) => AudioNodeWorker.AddNewParent(node, AudioNodeType.Sequence), node);
                menu.AddItem(new GUIContent(@"Add Parent/Multi"), false,
                    (obj) => AudioNodeWorker.AddNewParent(node, AudioNodeType.Multi), node);
                //menu.AddItem(new GUIContent(@"Add Parent/Track"), false, (obj) =>       AudioNodeWorker.AddNewParent(node, AudioNodeType.Track), node);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(@"Add Parent/Random"));
                menu.AddDisabledItem(new GUIContent(@"Add Parent/Sequence"));
                menu.AddDisabledItem(new GUIContent(@"Add Parent/Multi"));
                //menu.AddDisabledItem(new GUIContent(@"Add Parent/Track"));
            }
            //menu.AddDisabledItem(new GUIContent(@"Add Parent/Voice"));

            #endregion

            menu.AddSeparator("");

            #region Convert to

            if (node._children.Count == 0 && !node.IsRootOrFolder)
                menu.AddItem(new GUIContent(@"Convert To/Audio"), false,
                    (obj) => AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Audio), node);
            else
                menu.AddDisabledItem(new GUIContent(@"Convert To/Audio"));
            if (!node.IsRootOrFolder)
            {
                menu.AddItem(new GUIContent(@"Convert To/Random"), false,
                    (obj) => AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Random), node);
                menu.AddItem(new GUIContent(@"Convert To/Sequence"), false,
                    (obj) => AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Sequence), node);
                menu.AddItem(new GUIContent(@"Convert To/Multi"), false,
                    (obj) => AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Multi), node);
                //menu.AddItem(new GUIContent(@"Convert To/Track"), false, (obj) =>       AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Track), node);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(@"Convert To/Random"));
                menu.AddDisabledItem(new GUIContent(@"Convert To/Sequence"));
                menu.AddDisabledItem(new GUIContent(@"Convert To/Multi"));
                //menu.AddDisabledItem(new GUIContent(@"Add Parent/Track"));
            }

            #endregion

            menu.AddSeparator("");

            #region Send to event

            if (!node.IsRootOrFolder)
            {
                menu.AddItem(new GUIContent("Send to Event Window"), false,
                    () => EditorWindow.GetWindow<EventWindow>().ReceiveNode(node));
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Send to Event Window"));
            }

            #endregion

            menu.AddSeparator("");

            #region Delete

            if (node._type != AudioNodeType.Root)
                menu.AddItem(new GUIContent("Delete"), false, obj =>
                {
                    treeDrawer.SelectedNode = TreeWalker.GetPreviousVisibleNode(treeDrawer.SelectedNode);
                    
                    AudioNodeWorker.DeleteNode(node);
                }, node);
            else
                menu.AddDisabledItem(new GUIContent("Delete"));

            #endregion

            menu.ShowAsContext();
        }

        private void CreateFolderInNewPrefab(InAudioNode parent)
        {
            InAudioWindowOpener.ShowNewDataWindow((gameObject =>
            {
                var node = AudioNodeWorker.CreateChild(gameObject, parent, AudioNodeType.Folder);
                node.Name += " (External)";
                (node._nodeData as InFolderData).ExternalPlacement = true;
            }));
        }

        private void CreateChild(InAudioNode parent, AudioNodeType type)
        {
            InUndoHelper.RecordObjectFull(new UnityEngine.Object[] { parent, parent.GetBank() },
                "Create Audio Node");
            var newNode = AudioNodeWorker.CreateChild(parent, type);

            if (type == AudioNodeType.Audio)
            {
                AudioBankWorker.AddNodeToBank(newNode);
            }
        }

        protected override bool OnNodeDraw(InAudioNode node, bool isSelected, out bool clicked)
        {
            return GenericTreeNodeDrawer.Draw(node, isSelected, out clicked);
        }

        internal void FindAudio(Func<InAudioNode, bool> filter)
        {
            searchingFor = "Finding nodes in bank";
            lowercaseSearchingFor = "Finding nodes in bank";
            treeDrawer.Filter(filter);
        }

        protected override InAudioNode Root()
        {
            if (InAudioInstanceFinder.DataManager == null)
            {
                return null;
            }
            return InAudioInstanceFinder.DataManager.AudioTree;
        }

        protected override GUIPrefs GUIData
        {
            get 
            { 
                if(InAudioInstanceFinder.InAudioGuiUserPrefs != null)
                    return InAudioInstanceFinder.InAudioGuiUserPrefs.AudioGUIData;
                else 
                    return null;
            }
        }

       
    }
}