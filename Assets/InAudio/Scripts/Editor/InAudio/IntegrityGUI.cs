using InAudioSystem;
using InAudioSystem.InAudioEditor;
using UnityEditor;
using UnityEngine;

public class IntegrityGUI
{
    public IntegrityGUI(InAudioBaseWindow window)
    {
    }

    public void OnEnable()
    {
           
    }

    public bool OnGUI()
    {
        EditorGUILayout.HelpBox("Do not Undo these operations! No guarantee about what could break.", MessageType.Warning);
        EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
        EditorGUILayout.HelpBox("While Banks should work every time, it can in theory happen that audio/music nodes gets deattached from their bank when working in the editor. \nThis will reassign all nodes to their correct bank.", MessageType.Info);
        if (GUILayout.Button("Fix Bank integrity"))
        {
            AudioBankWorker.RebuildBanks();
            Debug.Log("All Banks rebuild");
        }

        EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();

        EditorGUILayout.HelpBox("No nodes should be unused, but in the case there is this will remove all unused data.\nNo performance is lost if unused nodes remains, but it does waste a bit of memory. This will clean up any unused data", MessageType.Info);
        

        if (GUILayout.Button("Clean up unused data"))
        {
            DataCleanup.Cleanup();
        }
        return false;
    }

}
