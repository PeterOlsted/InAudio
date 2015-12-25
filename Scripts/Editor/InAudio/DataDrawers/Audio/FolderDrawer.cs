using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{

public static class FolderDrawer
{
    public static void Draw(InAudioNode node)
    {
            var nodeData = node._nodeData;
        EditorGUILayout.BeginVertical();
            var data = node._nodeData as InFolderData;
            #region Bank 

            InUndoHelper.GUIUndo(node, "Name Change", ref node.Name, () =>
            EditorGUILayout.TextField("Name", node.Name));
       
        if (node._type == AudioNodeType.Folder)
        {
            bool overrideparent = EditorGUILayout.Toggle("Override Parent Bank", data.OverrideParentBank);
            if (overrideparent != data.OverrideParentBank)
            {
                AudioBankWorker.ChangeBankOverride(node);
            }
        }
        else
            EditorGUILayout.LabelField(""); //To fill out the area from the toggle

        
        if (data.OverrideParentBank == false && node._type != AudioNodeType.Root)
        {
            GUI.enabled = false;
        }

        EditorGUILayout.BeginHorizontal();

        var parentLink = FindParentBank(node);
        if (data.OverrideParentBank)
        {
            if (data.BankLink != null)
            {
                EditorGUILayout.LabelField("Bank", data.BankLink.GetName);
            }
            else
            {
                if (parentLink != null)
                    EditorGUILayout.LabelField("Bank", "Missing Bank, using parent bank" + parentLink.GetName);
                else
                {
                    EditorGUILayout.LabelField("Bank", "Missing Banks, no bank found");
                }
            }
        }
        else
        {
            if (parentLink != null)
                EditorGUILayout.LabelField("Using Bank", parentLink.GetName);
            else
            {
                EditorGUILayout.LabelField("Using Bank", "Missing");
            }
        }

        bool wasEnabled = GUI.enabled;
        GUI.enabled = true;
        if(GUILayout.Button("Find", GUILayout.Width(50)))
        {
            EditorWindow.GetWindow<AuxWindow>().FindBank(parentLink);
        }
        
        Rect findArea = GUILayoutUtility.GetLastRect();
        findArea.y += 20;
        if (GUI.Button(findArea, "Find"))
        {
            EditorWindow.GetWindow<AuxWindow>().FindBank(data.BankLink);
        }

        GUI.enabled = wasEnabled;

        GUILayout.Button("Drag new bank here", GUILayout.Width(140));

        var newBank = OnDragging.BusDragging(GUILayoutUtility.GetLastRect());
        if (newBank != null)
        {
            AudioBankWorker.ChangeAudioNodeBank(node, newBank);
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        GUI.enabled = false;
        if (data.BankLink != null)
            EditorGUILayout.LabelField("Node Bank", data.BankLink.GetName);
        else
            EditorGUILayout.LabelField("Node Bank", "Missing Bank");

        GUI.enabled = true;
        if (Application.isPlaying)
        {
            EditorGUILayout.Toggle("Is Loaded", BankLoader.IsLoaded(parentLink));
        }
            #endregion

        if (data.ExternalPlacement)
        {
            EditorGUILayout.Separator();

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Placed on", node.gameObject, typeof (GameObject), false);
            GUI.enabled = true;

            EditorGUILayout.Separator();
        }

        #region Mixer
        DataDrawerHelper.DrawMixer(node);
        #endregion
        EditorGUILayout.Separator();
        #region Volume
        if (Application.isPlaying)
        {
            InUndoHelper.GUIUndo(nodeData, "Folder volume", ref data.runtimeVolume, () => EditorGUILayout.Slider("Runtime Volume", data.runtimeVolume, 0, 1));
        }
        else
        {
            InUndoHelper.GUIUndo(nodeData, "Folder volume", ref data.VolumeMin, () => EditorGUILayout.Slider("Initial Volume", data.VolumeMin, 0, 1));
                
        }
        #endregion 

        EditorGUILayout.EndVertical();
    } 

    private static InAudioBankLink FindParentBank(InAudioNode node)
    {
        if (node == null)
            return null;

        var data = node._nodeData as InFolderData;
        if (data.OverrideParentBank)
        {
            return data.BankLink;
        }
        else if (node.IsRoot)
        {
            return data.BankLink;
        }
        else
            return FindParentBank(node._parent);
    }
}


}