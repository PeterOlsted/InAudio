using System;
using InAudioSystem.ExtensionMethods;
using UnityEngine;
using UnityEditor;

namespace InAudioSystem.InAudioEditor
{
    [InitializeOnLoad]
    public class Startup
    {
        static Startup()
        {
            if (!PlayerPrefs.HasKey("InAudioIntro"))
            {
                PlayerPrefs.SetInt("InAudioIntro", 1);
                PlayerPrefs.SetInt("InAudioAllowIntro", 1);
            }
            else
            {
                int t = PlayerPrefs.GetInt("InAudioIntro");
                if (t == 1)
                {
                    PlayerPrefs.SetInt("InAudioAllowIntro", 1);
                }
            }
        }
    }

    public class GuideWindow : EditorWindow
	{
		private Vector2 scrollPos;
	    private GUIStyle wordwrapStyle;

        

        public static GuideWindow Launch()
		{
			GuideWindow window = GetWindow<GuideWindow>();
            window.maxSize = new Vector2(600, 100000);
            window.minSize = new Vector2(600, 0);   
            window.Show();
            
			window.SetTitle("InAudio Introduction");
			return window;
		}

		private void OnGUI()
		{
		    if (wordwrapStyle == null)
		    {
                wordwrapStyle = new GUIStyle(GUI.skin.label);
                wordwrapStyle.wordWrap = true;

            }

            if (Event.current.IsKeyDown(KeyCode.W) && Event.current.modifiers == EventModifiers.Control)
			{
				Close();
				Event.current.UseEvent();
			}
            float pictureSize = 128;
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            GUI.skin.label.fontSize = 20;
		    try
		    {

		    }
		    catch (ArgumentException)
		    {

		        EditorGUILayout.BeginHorizontal();

		        EditorGUILayout.LabelField("InAudio Introduction", GUI.skin.label, GUILayout.Height(30), GUILayout.Width(250));
		        GUILayout.FlexibleSpace();
		        EditorGUILayout.EndHorizontal();
		    }
		    GUI.skin.label.fontSize = 11;

            //*********************************************************************//
            EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.BeginVertical(GUILayout.Width(pictureSize + 3));
		    if (GUILayout.Button(EditorResources.Book, GUILayout.Width(pictureSize), GUILayout.Height(pictureSize)))
		    {
		        Application.OpenURL("http://innersystems.net/wiki/");
		    }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Documentation", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("If you are looking to getting started or need more details, the documentation is for you.", wordwrapStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            //*********************************************************************//
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(pictureSize+3)); 
		    if (GUILayout.Button(EditorResources.Question, GUILayout.Width(pictureSize), GUILayout.Height(pictureSize)))
		    {
		        Application.OpenURL("http://forum.unity3d.com/threads/inaudio-2-new-open-source.232490/");
		    }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Forum", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Do you have a general question? Check out the Unity forum and ask your question or see if it is already answered.", wordwrapStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            //*********************************************************************//
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.LabelField("Do you have a question, bug report or feedback you want to say directly to the developer? Please do! Your feedback is always welcome. Please send to:\n", wordwrapStyle);

		    EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("inaudio@outlook.com");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            
            EditorGUILayout.EndHorizontal();
            //*********************************************************************//

            //GUILayout.Label(EditorResources.Support, GUIStyle.none);
            //GUILayout.Label(EditorResources.Question, GUIStyle.none);

            EditorGUILayout.EndScrollView();
			GUILayout.FlexibleSpace();
		    EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

		    try
		    {
		        if (!PlayerPrefs.HasKey("InAudioIntro"))
		        {
		            PlayerPrefs.SetInt("InAudioIntro", 1);
                    
                }
                int toggle = PlayerPrefs.GetInt("InAudioIntro");
                bool t = GUILayout.Toggle(Convert.ToBoolean(toggle), "Show on startup");
                PlayerPrefs.SetInt("InAudioIntro", Convert.ToInt32(t));
            }
            catch(Exception)
            { }
		    EditorGUILayout.EndHorizontal();
		}
	}
}