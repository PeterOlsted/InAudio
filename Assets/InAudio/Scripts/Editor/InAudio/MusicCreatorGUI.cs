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

            isDirty |= treeDrawer.DrawTree(window.Manager.MusicTree, treeArea);

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
            if (dragged == null || dragged.IsRoot || dragged == node || TreeWalker.IsParentOf(dragged, node))
                return false;

            
            if (dragged.IsRoot)
                return false;

            if(!node.IsRootOrFolder && dragged.IsRootOrFolder)
                return false;
            return true;

        }

        protected override void OnDrop(InMusicNode newParent, UnityEngine.Object[] objects)
        {
            if (newParent == null || objects == null )
                return; 

            var dragged = objects[0] as InMusicNode;
            if (dragged == null || dragged.IsRoot || dragged == newParent)
                return;

            UndoHelper.DoInGroup(() =>
            {
                var oldParent = dragged._parent;
                UndoHelper.RecordObjects("Music drag-n-drop", dragged, oldParent, newParent);

                dragged.MoveToNewParent(newParent);

                AudioBankWorker.RebuildBanks();
                newParent.IsFoldedOut = true;
                EditorEventUtil.UseEvent();
            });
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
            UndoHelper.DoInGroup(() => DeleteNodeRec(toDelete));
        }

        private void DeleteNodeRec(InMusicNode toDelete)
        {
            for (int i = 0; i < toDelete._children.Count; i++)
            {
                DeleteNodeRec(toDelete._children[i]);
            }
            UndoHelper.Destroy(toDelete);
        }

        private void CreateFolder(InMusicNode parent)
        {
            UndoHelper.DoInGroup(() =>
            {
                UndoHelper.RecordObjectFull(parent, "Create Music Folder");
                parent.FoldedOut = true;
                MusicWorker.CreateFolder(parent.gameObject, parent);
            });
            
        }

        private void CreateMusicGroup(InMusicNode parent)
        {
            UndoHelper.DoInGroup(() =>
            {
                UndoHelper.RecordObjectFull(parent, "Create Music Folder");
                parent.FoldedOut = true;
                MusicWorker.CreateMusicGroup(parent.gameObject, parent);
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
            return InAudioInstanceFinder.DataManager.MusicTree;
        }

        protected override GUIPrefs GUIData 
        {
            get { return InAudioInstanceFinder.InAudioGuiUserPrefs.AudioGUIData; }
        }
    }
}