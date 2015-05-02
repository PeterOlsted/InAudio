using InAudioSystem.ExtensionMethods;
using InAudioSystem.InAudioEditor;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.TreeDrawer
{
    public class GenericTreeNodeDrawer
    {
        public static bool Draw<T>(T node, bool isSelected, out bool clicked) where T : Object, InITreeNode<T>
        {
            clicked = false;

            Rect fullArea = EditorGUILayout.BeginHorizontal();
            Rect area = EditorGUILayout.BeginHorizontal();
            if (isSelected)
                GUI.DrawTexture(area, EditorResources.Background);

            GUILayout.Space(EditorGUI.indentLevel * 16);

            bool folded = node.IsFoldedOut;

            Texture picture;
            if (folded || node._getChildren.Count == 0)
                picture = EditorResources.Minus;
            else
                picture = EditorResources.Plus;

            if (GUILayout.Button(picture, GUIStyle.none, GUILayout.Height(EditorResources.Minus.height),
                GUILayout.Width(EditorResources.Minus.width)))
            {
                node.IsFoldedOut = !node.IsFoldedOut;
                EditorEventUtil.UseEvent();
            }
            Texture icon = TreeNodeDrawerHelper.LookUpIcon(node);


            TreeNodeDrawerHelper.DrawIcon(GUILayoutUtility.GetLastRect(), icon, GUIStyle.none);
            EditorGUILayout.LabelField("");


            EditorGUILayout.EndHorizontal();
            Rect labelArea = GUILayoutUtility.GetLastRect();
            
            labelArea.y += 6;
            labelArea.x += 60;
            EditorGUI.LabelField(labelArea, node.GetName);

            EditorGUILayout.EndHorizontal();

            if (Event.current.ClickedWithin(fullArea, 0))
            {
                clicked = true;
            }

            return node.IsFoldedOut;
        }
    }

    public static class TreeNodeDrawerHelper
    {
        public static void DrawIcon(Rect lastArea, Texture icon, GUIStyle style)
        {
            Rect iconRect = GUILayoutUtility.GetLastRect();
            iconRect.height = 16;
            iconRect.width = 16;
            iconRect.x += 33;
            iconRect.y += 8;
            GUI.Label(iconRect, icon, style);
        }

        public static Texture LookUpIcon<T>(T node) where T : Object, InITreeNode<T>
        {
            if (node is InAudioNode) 
            {
                InAudioNode audioNode = node as InAudioNode;
                if (audioNode._type == AudioNodeType.Audio)
                    return EditorResources.Audio;
                if (audioNode._type == AudioNodeType.Folder || audioNode.IsRoot)
                    return EditorResources.Folder;
                if (audioNode._type == AudioNodeType.Random)
                    return EditorResources.Dice;
                if (audioNode._type == AudioNodeType.Sequence)
                    return EditorResources.List;
                if (audioNode._type == AudioNodeType.Multi)
                    return EditorResources.Tree;
            }
            else if (node is InAudioBankLink)
            {
                InAudioBankLink link = node as InAudioBankLink;
                if(link._type == AudioBankTypes.Bank)
                    return EditorResources.Bank;
                else
                {
                    return EditorResources.Folder;
                }
            }

            return null;
        }
    }
    
}
