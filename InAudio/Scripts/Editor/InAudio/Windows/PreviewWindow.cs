using System;
using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace InAudioSystem.InAudioEditor
{

    public class PreviewWindow : EditorWindow
    {
        // The position of the window
        public Rect windowRect = new Rect(100, 100, 250, 200);

        public Vector2 scrollPos;

        private void OnGUI()
        {
            scrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), scrollPos,
                new Rect(0, 0, 1000, 1000));
            BeginWindows();


            // All GUI.Window or GUILayout.Window must come inside here

            var area = windowRect;
            if (!toogle)
            {
                area.height = 0;
            }
            else
            {
                area.height = 200;
            }
            windowRect = GUILayout.Window(1, area, DoWindow, "");

            EndWindows();
            GUI.EndScrollView();
        }

        private bool toogle;
        // The window function. This works just like ingame GUI.Window
        private void DoWindow(int unusedWindowID)
        {
            GUILayout.Label("", GUILayout.Height(0));
            var area = GUILayoutUtility.GetLastRect();

            area.y -= 20;
            area.x -= 2;
            area.width = 20;
            area.height = 20;
            if (GUI.Button(area, ">", GUIStyle.none))
            {
                toogle = !toogle;
            }
            area.width = 180;

            GUI.DragWindow();
        }

        //[MenuItem("Window/PreviewWindow")]
        //public static void OpenPreviewWindow()
        //{
        //    EditorWindow.GetWindow(typeof(PreviewWindow));
        //}


    }

}