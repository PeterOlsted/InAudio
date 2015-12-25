using System;
using InAudioSystem.Internal;
using InAudioSystem.TreeDrawer;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.InAudioEditor
{
    public class AudioBankCreatorGUI : BaseCreatorGUI<InAudioBankLink>
    {
        public AudioBankCreatorGUI(AuxWindow window) : base(window)
        {
            this.window = window;
        }

        private int leftWidth;
        private int height;

        public bool OnGUI(int leftWidth, int height)
        {
            //BaseOnGUI();

            this.leftWidth = leftWidth;
            this.height = height;

            EditorGUIHelper.DrawColums(DrawLeftSide, DrawRightSide);

            return isDirty;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            treeDrawer.CanPlaceHere = (parent, place) =>
            {
                if (place._type == AudioBankTypes.Bank && place._type == AudioBankTypes.Bank)
                    return false;
                return true;
            };
        }


        private void DrawLeftSide(Rect area)
        {
            Rect treeArea = EditorGUILayout.BeginVertical(GUILayout.Width(leftWidth), GUILayout.Height(height));
            DrawSearchBar();

            EditorGUILayout.BeginVertical();
            treeArea.y -= 25;

            isDirty |= treeDrawer.DrawTree(InAudioInstanceFinder.DataManager.BankLinkTree, treeArea);
            SelectedNode = treeDrawer.SelectedNode;

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private void DrawRightSide(Rect area)
        {
            EditorGUILayout.BeginVertical();

            if (SelectedNode != null)
            {
                AudioBankLinkDrawer.Draw(SelectedNode);
            }

            EditorGUILayout.EndVertical();
        }

        protected override bool CanDropObjects(InAudioBankLink node, Object[] objects)
        {
            if (node == null || objects == null)
                return false;

            if (objects.Length > 0 && objects[0] as InAudioBankLink != null && node._type != AudioBankTypes.Bank)
            {
                return !NodeWorker.IsChildOf(objects[0] as InAudioBankLink, node);
            }
            return false;
        }

        protected override bool OnNodeDraw(InAudioBankLink node, bool isSelected, out bool clicked)
        {
            return GenericTreeNodeDrawer.Draw(node, isSelected, out clicked);
        }

        protected override void OnDrop(InAudioBankLink node, Object[] objects)
        {
            InUndoHelper.DragNDropUndo(node, "Bank Drag N Drop");
            InAudioBankLink target = objects[0] as InAudioBankLink;
            NodeWorker.ReasignNodeParent(target, node);
        }

        protected override void OnContext(InAudioBankLink node)
        {
            if (node == null)
                return;
            var menu = new GenericMenu();

            if (node._type == AudioBankTypes.Folder)
            {
                menu.AddItem(new GUIContent(@"Create Child/Folder"), false,
                    data => CreateBank(node, AudioBankTypes.Folder), node);
                menu.AddItem(new GUIContent(@"Create Child/Bank"), false, data => CreateBank(node, AudioBankTypes.Bank),
                    node);
            }
            else if (node._type == AudioBankTypes.Bank)
            {
                menu.AddDisabledItem(new GUIContent(@"Create Child/Folder"));
                menu.AddDisabledItem(new GUIContent(@"Create Child/Bank"));
            }

            menu.AddSeparator("");

            if (node.IsRoot)
                menu.AddDisabledItem(new GUIContent(@"Cannot delete root"));
            else
            {
                menu.AddItem(new GUIContent(@"Delete If Empty"), false,
                    data => DeleteNode(InAudioInstanceFinder.DataManager.BankLinkTree, data as InAudioBankLink), node);
            }
            menu.ShowAsContext();
        }

        private void DeleteNode(InAudioBankLink root, InAudioBankLink toDelete)
        {
            if (toDelete._getChildren.Count > 0)
            {
                EditorUtility.DisplayDialog("Cannot delete bank", "Cannot delete folder with bank children", "ok");
                return;
            }


            Func<InAudioNode, bool> usedBankRoot = node =>
            {
                if (node._type == AudioNodeType.Folder)
                {
                    var data = node._nodeData as InFolderData;
                    if (node.IsRoot && data.BankLink == toDelete)
                    {
                        return true;
                    }
                    else if (node._type == AudioNodeType.Folder && data.BankLink == toDelete)
                    {
                        return true;
                    }
                }

                return false;
            };

            if (TreeWalker.Any(InAudioInstanceFinder.DataManager.AudioTree, usedBankRoot))
            {
                EditorUtility.DisplayDialog("Cannot delete bank", "Cannot delete bank that is in use", "ok");
                return;
            }

            int nonFolderCount = TreeWalker.Count(root, link => link._type == AudioBankTypes.Bank);
            if (nonFolderCount == 1 && toDelete._type == AudioBankTypes.Bank)
            {
                EditorUtility.DisplayDialog("Cannot delete the bank", "Cannot delete the last bank", "ok");
                return;
            }

            if (toDelete._type == AudioBankTypes.Bank)
                AudioBankWorker.DeleteBank(toDelete);
            else if (toDelete._type == AudioBankTypes.Folder)
                AudioBankWorker.DeleteFolder(toDelete);
        }

        private void CreateBank(InAudioBankLink parent, AudioBankTypes type)
        {
            //TODO make real undo
            InUndoHelper.RecordObjectFull(parent, "Bank " + (type == AudioBankTypes.Folder ? "Folder " : "") + "Creation");
            if (type == AudioBankTypes.Folder)
                AudioBankWorker.CreateFolder(parent.gameObject, parent, GUIDCreator.Create());
            else
                AudioBankWorker.CreateBankLink(parent.gameObject, parent, GUIDCreator.Create());
        }

        protected override InAudioBankLink Root()
        {
            if (InAudioInstanceFinder.DataManager == null)
            {
                return null;
            }
            return InAudioInstanceFinder.DataManager.BankLinkTree;
        }

        protected override GUIPrefs GUIData
        {
            get
            {
                if (InAudioInstanceFinder.InAudioGuiUserPrefs != null)
                    return InAudioInstanceFinder.InAudioGuiUserPrefs.BankGUIData;
                else
                    return null;
            }
        }


       
    }
}