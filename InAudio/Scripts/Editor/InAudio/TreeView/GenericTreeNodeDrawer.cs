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
            {
                GUI.DrawTexture(area, EditorResources.Instance.GetBackground());
            }


            GUILayout.Space(EditorGUI.indentLevel * 16);

            bool folded = node.IsFoldedOut;

            Texture picture;
            if (EditorResources.Instance != null)
            {
                
                if (folded || node._getChildren.Count == 0)
                    picture = EditorResources.Instance.Minus;
                else
                    picture = EditorResources.Instance.Plus;
            }
            else
            {
                picture = null; 
            }

            if (GUILayout.Button(picture, GUIStyle.none, GUILayout.Height(EditorResources.Instance.Minus.height),
                GUILayout.Width(EditorResources.Instance.Minus.width)))
            {
                node.IsFoldedOut = !node.IsFoldedOut;
                Event.current.UseEvent();
            }
            Texture icon = TreeNodeDrawerHelper.LookUpIcon(node);


            TreeNodeDrawerHelper.DrawIcon(GUILayoutUtility.GetLastRect(), icon, GUIStyle.none);
            EditorGUILayout.LabelField("");


            EditorGUILayout.EndHorizontal();
            Rect labelArea = GUILayoutUtility.GetLastRect();

            var audioNode = node as InAudioNode;
            if(audioNode != null)
            {
                TreeNodeDrawerHelper.DrawVolume(fullArea, audioNode._nodeData as InFolderData);
            }
            
            labelArea.y += 6;
            labelArea.x += 60;
            EditorGUI.LabelField(labelArea, node.GetName);

            EditorGUILayout.EndHorizontal();

            if (Event.current.ClickedWithin(fullArea))
            {
                clicked = true;
            }

            return node.IsFoldedOut;
        }
    }

    public static class TreeNodeDrawerHelper
    {
        public static void DrawVolume(Rect fullArea, InFolderData @group)
        {
            if (group == null)
                return;
            GUI.enabled = false;
            Rect sliderRect = fullArea;
            sliderRect.x = sliderRect.width - 30;
            sliderRect.width = 20;
            sliderRect.height -= 5;

            GUI.VerticalSlider(sliderRect, @group.hiearchyVolume, 1f, 0f);

            GUI.enabled = true;
        }

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
                    return EditorResources.Instance.Audio;
                if (audioNode._type == AudioNodeType.Folder || audioNode.IsRoot)
                    return EditorResources.Instance.Folder;
                if (audioNode._type == AudioNodeType.Random)
                    return EditorResources.Instance.Dice;
                if (audioNode._type == AudioNodeType.Sequence)
                    return EditorResources.Instance.List;
                if (audioNode._type == AudioNodeType.Multi)
                    return EditorResources.Instance.Tree;
            }
            else if (node is InAudioBankLink)
            {
                InAudioBankLink link = node as InAudioBankLink;
                if(link._type == AudioBankTypes.Bank)
                    return EditorResources.Instance.Bank;
                else
                {
                    return EditorResources.Instance.Folder;
                }
            }

            return null;
        }
    }
    
}
