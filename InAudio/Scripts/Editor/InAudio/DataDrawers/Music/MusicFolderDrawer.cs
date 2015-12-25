using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public class MusicFolderDrawer
    {
        public static void Draw(InMusicFolder node)
        {
            node.ScrollPosition = EditorGUILayout.BeginScrollView(node.ScrollPosition);
            var prop = new SerializedObject(node);
            prop.Update();
            EditorGUILayout.BeginVertical();

            #region Bank

            InUndoHelper.GUIUndo(node, "Name Change", ref node._name, () =>
                EditorGUILayout.TextField("Name", node._name));


            if (!node.IsRoot)
            {
                bool overrideparent = EditorGUILayout.Toggle("Override Parent Bank", node._overrideParentBank);
                if (overrideparent != node._overrideParentBank)
                {
                    AudioBankWorker.ChangeBankOverride(node);
                }
            }
            else
            {
                EditorGUILayout.Separator();
            }
            
            if (node._overrideParentBank == false && !node.IsRoot)
            {
                GUI.enabled = false;
            }

            EditorGUILayout.BeginHorizontal();

            var parentLink = node.GetBank();
            if (node._overrideParentBank)
            {
                if (node._bankLink != null)
                {
                    EditorGUILayout.LabelField("Bank", node._bankLink.GetName);
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
            if (GUILayout.Button("Find", GUILayout.Width(50)))
            {
                EditorWindow.GetWindow<AuxWindow>().FindBank(parentLink);
            }

            Rect findArea = GUILayoutUtility.GetLastRect();
            findArea.y += 20;
            if (GUI.Button(findArea, "Find"))
            {
                EditorWindow.GetWindow<AuxWindow>().FindBank(node._bankLink);
            }

            GUI.enabled = wasEnabled;

            GUILayout.Button("Drag new bank here", GUILayout.Width(140));

            var newBank = OnDragging.BusDragging(GUILayoutUtility.GetLastRect());
            if (newBank != null)
            {
                AudioBankWorker.ChangeMusicNodeBank(node, newBank);
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            GUI.enabled = false;
            if (node._bankLink != null)
                EditorGUILayout.LabelField("Node Bank", node._bankLink.GetName);
            else
                EditorGUILayout.LabelField("Node Bank", "Missing Bank");
            GUI.enabled = true;
            if (Application.isPlaying)
            {
                EditorGUILayout.Toggle("Is Loaded", BankLoader.IsLoaded(parentLink));
            }
        
            #endregion

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            #region Mixer

            DataDrawerHelper.DrawMixer(node, prop.FindProperty("_mixerGroup"));

            #endregion
            EditorGUILayout.Separator();
            #region Volume & Pitch
            if (!Application.isPlaying)
                InUndoHelper.GUIUndo(node, "Volume", ref node._minVolume, () => EditorGUILayout.Slider("Initial Volume", node._minVolume, 0f, 1f));
            else
                InUndoHelper.GUIUndo(node, "Volume", ref node.runtimeVolume, () => EditorGUILayout.Slider("Current Volume", node.runtimeVolume, 0f, 1f));

            if (!Application.isPlaying)
                InUndoHelper.GUIUndo(node, "Pitch", ref node._minPitch, () => EditorGUILayout.Slider("Initial Pitch", node._minPitch, 0f, 3f));
            else
                InUndoHelper.GUIUndo(node, "Pitch", ref node.runtimePitch, () => EditorGUILayout.Slider("Current Pitch", node.runtimePitch, 0f, 3f));
            #endregion

            EditorGUILayout.EndVertical();
            prop.ApplyModifiedProperties();

            EditorGUILayout.EndScrollView();
        }
    }
}