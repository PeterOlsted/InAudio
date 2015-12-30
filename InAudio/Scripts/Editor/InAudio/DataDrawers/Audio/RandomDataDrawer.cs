using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public static class RandomDataDrawer
    {
        public static void Draw(InAudioNode node)
        {
            node.ScrollPosition = GUILayout.BeginScrollView(node.ScrollPosition);

            //UndoHandler.CheckUndo(new UnityEngine.Object[] { node, node.AudioData }, "Random Data Node Change");
            InUndoHelper.GUIUndo(node, "Name Change", ref node.Name, () =>
                EditorGUILayout.TextField("Name", node.Name));
            NodeTypeDataDrawer.Draw(node);

            EditorGUILayout.Separator();
            InAudioNodeData baseData = (InAudioNodeData)node._nodeData;
            
            
            if (baseData.SelectedArea == 0)
            {
                EditorGUILayout.BeginVertical();
                
                var randomData = (node._nodeData as RandomData);
                var weights = randomData.weights;

                InUndoHelper.GUIUndo(node._nodeData, "Do Not Repeat Last #", ref randomData.doNotRepeat, () => Mathf.Max(0,EditorGUILayout.IntField("Do Not Repeat Last #", randomData.doNotRepeat)));
                if (randomData.doNotRepeat >= weights.Count && weights.Count > 0)
                {
                    EditorGUILayout.HelpBox("The number of random elements that should be repeated exceeds the number of nodes.\nThe number will be clambed to "+(randomData.weights.Count-1)+".", MessageType.Info);
                }

                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Weights");
                if (node._children.Count == weights.Count)
                {
                    for (int i = 0; i < node._children.Count; ++i)
                    {
                        var child = node._children[i];

                        int index = i;
                        InUndoHelper.GUIUndo(node._nodeData, "Weights",
                            () => EditorGUILayout.IntSlider(child.Name, weights[index], 0, 100), i1 =>
                                weights[index] = i1);

                    }
                    if (node._children.Count == 0)
                    {
                        EditorGUILayout.HelpBox("Node has no children to weight.", MessageType.None);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("The number of weights does not match the children count.\n"+node._children.Count+" children was found and " + randomData.weights.Count + " weights.", MessageType.Error);
                    if (GUILayout.Button("Fix weights"))
                    {
                        weights.Clear();
                        for (int i = 0; i < node._children.Count; i++)
                        {
                            weights.Add(50);
                        }
                    }
                }

                EditorGUILayout.EndVertical();
            }
            //UndoHandler.CheckGUIChange();

            EditorGUILayout.EndScrollView();
        }
    }
}