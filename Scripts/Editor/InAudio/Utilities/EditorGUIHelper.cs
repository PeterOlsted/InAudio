using System;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem
{
    public static class EditorGUIHelper
    {
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

        private static readonly Color splitterColor = EditorGUIUtility.isProSkin
            ? new Color(0.157f, 0.157f, 0.157f)
            : new Color(0.5f, 0.5f, 0.5f);

        // GUILayout Style
        public static void Splitter(Color rgb, float thickness = 1)
        {
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitter, GUILayout.Height(thickness));

            if (Event.current.type == EventType.Repaint)
            {
                Color restoreColor = GUI.color;
                GUI.color = rgb;
                splitter.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }

        public static void Splitter(float thickness, GUIStyle splitterStyle)
        {
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitterStyle, GUILayout.Height(thickness));

            if (Event.current.type == EventType.Repaint)
            {
                Color restoreColor = GUI.color;
                GUI.color = splitterColor;
                splitterStyle.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }

        public static void Splitter(float thickness = 1)
        {
            Splitter(thickness, splitter);
        }

        // GUI Style
        public static void Splitter(Rect position)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color restoreColor = GUI.color;
                GUI.color = splitterColor;
                splitter.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }
    }
}