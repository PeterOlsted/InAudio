using UnityEditor;
using UnityEngine;

namespace InAudioSystem.RuntimeHelperClass
{
    [CustomPropertyDrawer(typeof(Runtime.MecanimParameter))]
    public class MecanimParameterDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return base.GetPropertyHeight(prop, label)*2+ base.GetPropertyHeight(prop.FindPropertyRelative("Node"), label);
        }


        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(pos, label, prop);


            EditorGUI.PropertyField(pos, prop.FindPropertyRelative("Node"));
            //prop.FindPropertyRelative("Node").objectReferenceValue = EditorGUI.ObjectField(pos,
            //    prop.FindPropertyRelative("Node").objectReferenceValue, typeof(InAudioNode), false);

            pos.height = EditorGUIUtility.singleLineHeight;
            pos.y += EditorGUIUtility.singleLineHeight * 2;
            prop.FindPropertyRelative("Target").floatValue = EditorGUI.Slider(pos,
                prop.FindPropertyRelative("Target").floatValue, 0, 1);
            

            
            EditorGUI.EndProperty();


        }
    }


}