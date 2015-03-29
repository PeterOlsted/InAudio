using System;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem { 
public static class EditorGUIHelper  {
    public static readonly GUIStyle splitter;

    public static Rect DrawColums(params Action<Rect>[] drawCalls)
    {
        Rect r = GUILayoutUtility.GetLastRect();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        if (drawCalls != null)
        {
            foreach (Action<Rect> action in drawCalls)
            {
                action(r);
            }
        }
            
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        return r;
    }

    public static Rect DrawRows(params Action[] drawCalls)
    {
        Rect r = EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        if (drawCalls != null)
        {
            foreach (Action a in drawCalls)
            {
                a();
            }
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        
        return r;
    }

    public static Rect DrawID(int id)
    {
        Rect area = EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("ID");
        EditorGUILayout.SelectableLabel(id.ToString(), GUILayout.Height(EditorGUIUtility.singleLineHeight));
        EditorGUILayout.EndHorizontal();
        return area;
    }

    static EditorGUIHelper()
    {
        //GUISkin skin = GUI.skin;
 
        splitter = new GUIStyle();
        splitter.normal.background = EditorGUIUtility.whiteTexture;
        splitter.stretchWidth = true;
        splitter.margin = new RectOffset(0, 0, 7, 7);
    }
 
    private static readonly Color splitterColor = EditorGUIUtility.isProSkin ? new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);
 
    // GUILayout Style
    public static void Splitter(Color rgb, float thickness = 1) {
        Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitter, GUILayout.Height(thickness));
 
        if (Event.current.type == EventType.Repaint) {
            Color restoreColor = GUI.color;
            GUI.color = rgb;
            splitter.Draw(position, false, false, false, false);
            GUI.color = restoreColor;
        }
    }
 
    public static void Splitter(float thickness, GUIStyle splitterStyle) {
        Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitterStyle, GUILayout.Height(thickness));
 
        if (Event.current.type == EventType.Repaint) {
            Color restoreColor = GUI.color;
            GUI.color = splitterColor;
            splitterStyle.Draw(position, false, false, false, false);
            GUI.color = restoreColor;
        }
    }
 
    public static void Splitter(float thickness = 1) {
        Splitter(thickness, splitter);
    }
 
    // GUI Style
    public static void Splitter(Rect position) {
        if (Event.current.type == EventType.Repaint) {
            Color restoreColor = GUI.color;
            GUI.color = splitterColor;
            splitter.Draw(position, false, false, false, false);
            GUI.color = restoreColor;
        }
    }
}

public class GUIHelper
{

    protected static bool clippingEnabled;

    protected static Rect clippingBounds;

    protected static Material lineMaterial;



    /* @ Credit: "http://cs-people.bu.edu/jalon/cs480/Oct11Lab/clip.c" */

    protected static bool clip_test(float p, float q, ref float u1, ref float u2)
    {

        float r;

        bool retval = true;

        if (p < 0.0)
        {

            r = q / p;

            if (r > u2)

                retval = false;

            else if (r > u1)

                u1 = r;

        }

        else if (p > 0.0)
        {

            r = q / p;

            if (r < u1)

                retval = false;

            else if (r < u2)

                u2 = r;

        }

        else

            if (q < 0.0)

                retval = false;



        return retval;

    }



    protected static bool segment_rect_intersection(Rect bounds, ref Vector2 p1, ref Vector2 p2)
    {

        float u1 = 0.0f, u2 = 1.0f, dx = p2.x - p1.x;

        if (clip_test(-dx, p1.x - bounds.xMin, ref u1, ref u2))

            if (clip_test(dx, bounds.xMax - p1.x, ref u1, ref u2))
            {

                float dy = p2.y - p1.y;

                if (clip_test(-dy, p1.y - bounds.yMin, ref u1, ref u2))

                    if (clip_test(dy, bounds.yMax - p1.y, ref u1, ref u2))
                    {

                        if (u2 < 1.0)
                        {

                            p2.x = p1.x + u2 * dx;

                            p2.y = p1.y + u2 * dy;

                        }

                        if (u1 > 0.0)
                        {

                            p1.x += u1 * dx;

                            p1.y += u1 * dy;

                        }

                        return true;

                    }

            }

        return false;

    }



    public static void BeginGroup(Rect position)
    {

        clippingEnabled = true;

        clippingBounds = new Rect(0, 0, position.width, position.height);

        GUI.BeginGroup(position);

    }



    public static void EndGroup()
    {

        GUI.EndGroup();

        clippingBounds = new Rect(0, 0, Screen.width, Screen.height);

        clippingEnabled = false;

    }



    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color)
    {

        if (clippingEnabled)

            if (!segment_rect_intersection(clippingBounds, ref pointA, ref pointB))

                return;



        if (!lineMaterial)
        {

            /* Credit:  */

            lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +

           "SubShader { Pass {" +

           "   BindChannels { Bind \"Color\",color }" +

           "   Blend SrcAlpha OneMinusSrcAlpha" +

           "   ZWrite Off Cull Off Fog { Mode Off }" +

           "} } }");

            lineMaterial.hideFlags = HideFlags.HideAndDontSave;

            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

        }



        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);

        GL.Color(color);

        GL.Vertex3(pointA.x, pointA.y, 0);

        GL.Vertex3(pointB.x, pointB.y, 0);

        GL.End();

    }

};
}
