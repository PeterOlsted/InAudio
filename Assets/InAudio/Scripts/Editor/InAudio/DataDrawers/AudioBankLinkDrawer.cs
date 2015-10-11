using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
public static class AudioBankLinkDrawer
{
    public static void Draw(InAudioBankLink bankLink)
    { 
        EditorGUILayout.BeginVertical();

        InUndoHelper.GUIUndo(bankLink, "Name Change", ref bankLink._name, () => 
            EditorGUILayout.TextField("Name", bankLink._name));

        if (bankLink._type == AudioBankTypes.Bank)
        {
            EditorGUIHelper.DrawID(bankLink._guid);

            //UndoHelper.GUIUndo(bankLink, "Load async", ref bankLink.LoadASync, () =>
            //    EditorGUILayout.Toggle("Load ASync", bankLink.LoadASync));

            bool autoLoad = EditorGUILayout.Toggle("Auto load", bankLink._autoLoad);
            if (autoLoad != bankLink._autoLoad) //Value has changed
            {
                InUndoHelper.RecordObjectFull(bankLink, "Bank Auto Load");
                bankLink._autoLoad = autoLoad;
            }

            Rect lastArea = GUILayoutUtility.GetLastRect();
            lastArea.y += 28;
            lastArea.width = 200;
            if(GUI.Button(lastArea, "Find Folders using this bank"))
            {
                EditorWindow.GetWindow<InAudioWindow>().Find(audioNode => audioNode.GetBank() != bankLink);
            }
             
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            if (Application.isPlaying)
            {
                EditorGUILayout.Toggle("Is Loaded", bankLink.IsLoaded);
            }
        }

        EditorGUILayout.EndVertical();
        //UndoCheck.Instance.CheckDirty(node);
      
    }
}
}