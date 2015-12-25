using System;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using InAudioSystem.TreeDrawer;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public class MusicCreatorGUI : BaseCreatorGUI<InMusicNode>
    {
        public MusicCreatorGUI(InMusicWindow window)
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

        private void DrawLeftSide(Rect area)
        {
            Rect treeArea = EditorGUILayout.BeginVertical(GUILayout.Width(leftWidth), GUILayout.Height(height));
            DrawSearchBar();

            EditorGUILayout.BeginVertical();

            isDirty |= treeDrawer.DrawTree(InAudioInstanceFinder.DataManager.MusicTree, treeArea);
            SelectedNode = treeDrawer.SelectedNode;

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private void DrawRightSide(Rect area)
        {
            EditorGUILayout.BeginVertical();

            if (SelectedNode != null)
            {
                DrawTypeControls(SelectedNode);

                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.EndVertical();
        }

        private void DrawTypeControls(InMusicNode node)
        {
            try
            {
                var type = node._type;
                
                switch (type)
                {
                    case MusicNodeType.Music:
                        MusicGroupDrawer.Draw(node as InMusicGroup);
                        break;
                    case MusicNodeType.Folder:
                        MusicFolderDrawer.Draw(node as InMusicFolder);
                        break;
                    case MusicNodeType.Root:
                        MusicFolderDrawer.Draw(node as InMusicFolder);
                        break;
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

        protected override bool CanDropObjects(InMusicNode node, UnityEngine.Object[] objects)
        {
            if (node == null || objects == null || objects.Length == 0)
                return false;


            var obj = objects[0];
            var dragged = obj as InMusicNode;
            if (dragged != null)
            {
                if (dragged.IsRoot || dragged == node || TreeWalker.IsParentOf(dragged, node))
                    return false;

                if (!node.IsRootOrFolder && dragged.IsRootOrFolder)
                    return false;
                return true;
            }
            else if(node._type == MusicNodeType.Music)
            {
                var clips = objects.Convert(o => o as AudioClip).TakeNonNulls();
                if (clips.Length > 0)
                {
                    return true;
                }
            }
            
            return false;

        }

        protected override void OnDrop(InMusicNode newParent, UnityEngine.Object[] objects)
        {
            if (newParent == null || objects == null )
                return; 

            var dragged = objects[0] as InMusicNode;

            if (dragged != null)
            {
                if (dragged.IsRoot || dragged == newParent)
                    return;

                InUndoHelper.DoInGroup(() =>
                {
                    if (dragged.gameObject != newParent.gameObject)
                    {
                        if (EditorUtility.DisplayDialog("Move?",
                            "Warning, this will break all external references to this and all child nodes!\n" +
                            "Move node from\"" + dragged.gameObject.name +
                            "\" to \"" + newParent.gameObject.name + "\"?", "Ok", "Cancel"))
                        {
                            treeDrawer.SelectedNode = TreeWalker.GetPreviousVisibleNode(treeDrawer.SelectedNode);
                            MusicWorker.Duplicate(newParent.gameObject, dragged, newParent);
                            DeleteNodeRec(dragged);
                            AudioBankWorker.RebuildBanks(); 
                        }
                    }
                    else
                    {
                        var oldParent = dragged._parent;
                        InUndoHelper.RecordObjects("Music drag-n-drop", dragged, oldParent, newParent);

                        dragged.MoveToNewParent(newParent);

                       
                        newParent.IsFoldedOut = true;
                    }

                 
                    Event.current.UseEvent();
                });
            }
            else if (newParent._type == MusicNodeType.Music)
            {
                var clips = objects.Convert(o => o as AudioClip).TakeNonNulls();
                var musicGroup = newParent as InMusicGroup;
                if (musicGroup != null)
                {
                    InUndoHelper.DoInGroup(() =>
                    {
                        InUndoHelper.RecordObject(newParent,"Music Clip Add");
                        foreach (var audioClip in clips)
                        {
                            musicGroup._clips.Add(audioClip);
                        }
                    });
                }

            }
        }

        protected override void OnContext(InMusicNode node)
        {
            var menu = new GenericMenu();

            menu.AddDisabledItem(new GUIContent(node._name));
            menu.AddSeparator("");
            if (node.IsRootOrFolder)
            {
                menu.AddItem(new GUIContent("Create Folder"), false, () => CreateFolder(node));
                menu.AddItem(new GUIContent("Create Music Group"), false, () => CreateMusicGroup(node));
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Create Folder"));
                menu.AddItem(new GUIContent("Create Music Group"), false, () => CreateMusicGroup(node));
            }
            menu.AddSeparator("");

            #region Send to event


            if (node.IsRootOrFolder)
            {
                menu.AddItem(new GUIContent("Create child folder in new prefab"), false, obj =>
                {
                    CreateFolderInNewPrefab(node);
                }, node);
            }

            #endregion

            #region Send to event


            if (!node.IsRootOrFolder)
            {
                menu.AddItem(new GUIContent("Send to Event Window"), false, () => EventWindow.Launch().ReceiveNode(node as InMusicGroup));
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Send to Event Window"));
            }

            #endregion

            menu.AddSeparator("");

            if (node.IsRoot)
            {
                menu.AddDisabledItem(new GUIContent("Delete"));
            }
            else
            {
                menu.AddItem(new GUIContent("Delete"), false, ()=> DeleteNode(node) );
            }

            menu.ShowAsContext();
        }

        private void DeleteNode(InMusicNode toDelete)
        {
            InUndoHelper.DoInGroup(() => DeleteNodeRec(toDelete));
        }

        private void CreateFolderInNewPrefab(InMusicNode parent)
        {
            InAudioWindowOpener.ShowNewDataWindow((gameObject =>
            {
                var node = MusicWorker.CreateFolder(gameObject, parent);
                node._name += " (External)";
                node._externalPlacement = true;
            }));
        }

        private void DeleteNodeRec(InMusicNode toDelete)
        {
            for (int i = 0; i < toDelete._children.Count; i++)
            {
                DeleteNodeRec(toDelete._children[i]);
            }
            InUndoHelper.Destroy(toDelete);
        }

        private void CreateFolder(InMusicNode parent)
        {
            InUndoHelper.DoInGroup(() =>
            {
                InUndoHelper.RecordObjectFull(parent, "Create Music Folder");
                parent.FoldedOut = true;
                MusicWorker.CreateFolder(parent.gameObject, parent);
            });
            
        }

        private void CreateMusicGroup(InMusicNode parent)
        {
            InUndoHelper.DoInGroup(() =>
            {
                InUndoHelper.RecordObjectFull(parent, "Create Music Folder");
                parent.FoldedOut = true;
                MusicWorker.CreateMusicGroup(parent);
            });
        }

        public override void OnEnable()
        {
            treeDrawer.Filter(n => false);
            treeDrawer.OnNodeDraw = MusicDrawer.Draw;
            treeDrawer.OnContext = OnContext;
            treeDrawer.CanDropObjects = CanDropObjects;
            treeDrawer.OnDrop = OnDrop;
        }

        protected override bool OnNodeDraw(InMusicNode node, bool isSelected, out bool clicked)
        {
            return GenericTreeNodeDrawer.Draw(node, isSelected, out clicked);
        }

        internal void FindAudio(Func<InMusicNode, bool> filter)
        {
            searchingFor = "Finding nodes in bank";
            lowercaseSearchingFor = "Finding nodes in bank";
            treeDrawer.Filter(filter);
        }

        protected override InMusicNode Root()
        {
            if (InAudioInstanceFinder.DataManager == null)
            {
                return null;
            }
            return InAudioInstanceFinder.DataManager.MusicTree;
        }

        protected override GUIPrefs GUIData
        {
            get
            {
                if (InAudioInstanceFinder.InAudioGuiUserPrefs != null)
                    return InAudioInstanceFinder.InAudioGuiUserPrefs.MusicGUIData;
                else
                    return null;
            }
        }
    }
}