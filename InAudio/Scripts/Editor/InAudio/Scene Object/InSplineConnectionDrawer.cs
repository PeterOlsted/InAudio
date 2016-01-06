using InAudioSystem;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InSplineConnection))]
public class InSplineConnectionDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
            return base.GetPropertyHeight(property, label) * 3;
        else
        {
            return base.GetPropertyHeight(property, label);
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.indentLevel += 1;
        Rect folderRect = position;
        if(property.isExpanded)
            folderRect.height /= 3.0f;
        Rect areaA = position;
        areaA.height /= 3.0f;
        areaA.y += areaA.height;
        Rect areaB = position;
        areaB.height /= 3.0f;
        areaB.y = areaA.y + areaA.height;
        property.isExpanded = EditorGUI.Foldout(folderRect, property.isExpanded, "Connection");
        if (property.isExpanded)
        {
            EditorGUI.PropertyField(areaA, property.FindPropertyRelative("NodeA"));
            EditorGUI.PropertyField(areaB, property.FindPropertyRelative("NodeB"));
        }
        EditorGUI.indentLevel -= 1;
        EditorGUI.EndProperty();
    }
}
