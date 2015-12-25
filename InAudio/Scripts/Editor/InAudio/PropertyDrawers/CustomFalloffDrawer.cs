//using UnityEditor;
//using UnityEngine;

//[CustomPropertyDrawer(typeof(CustomFalloffAttribute))]
//public class CustomFalloffDrawer : PropertyDrawer
//{
//    const int helpHeight = 30;
//    const int curveHeight = 40;

//    // Here you must define the height of your property drawer. Called by Unity.
//    public override float GetPropertyHeight(SerializedProperty prop,
//                                             GUIContent label)
//    {
//        if (!IsCustomRolloff(prop))
//            return 0;
//        else
//            return base.GetPropertyHeight(prop, label) + curveHeight - 16 + helpHeight;
//    }

//    void DrawHelpBox(Rect position)
//    {
//        EditorGUI.HelpBox(position, "For custom rolloff curves, recreate the rolloff curve here as there is no script access to the curve. Exactly the same behavior as normal rolloff curve.", MessageType.Info);
//    }

//    public override void OnGUI(Rect position,
//                                SerializedProperty prop,
//                                GUIContent label)
//    {
//        if (!IsCustomRolloff(prop))
//            return;

//        // Adjust height of the text field
//        position.height = curveHeight;
//        Rect textFieldPosition = position;
//        textFieldPosition.height = curveHeight;
//        DrawCurve(textFieldPosition, prop);

//        // Adjust the help box position to appear indented underneath the text field.
//        Rect helpPosition = EditorGUI.IndentedRect(position);
//        helpPosition.y += curveHeight;
//        helpPosition.height = helpHeight;
//        DrawHelpBox(helpPosition);
//    }

//    void DrawCurve(Rect position, SerializedProperty prop)
//    {
//        // Draw the text field control GUI.
//        EditorGUI.BeginChangeCheck();
//        var value = EditorGUI.CurveField(position, "Custom falloff", prop.animationCurveValue);
//        if (EditorGUI.EndChangeCheck())
//            prop.animationCurveValue = value;
//    }

//    // Test if the propertys string value matches the regex pattern.
//    /*bool IsCustomRolloff(SerializedProperty prop)
//    {
//        return (SerializedParent.GetParent(prop) as HDRAudioSource).audio.rolloffMode == AudioRolloffMode.Custom;
//    }*/
//}
