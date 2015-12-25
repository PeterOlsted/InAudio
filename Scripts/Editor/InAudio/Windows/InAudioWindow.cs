using System;
using InAudioSystem.Internal;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public class InAudioWindow : InAudioBaseWindow
    {
        private AudioCreatorGUI audioCreatorGUI;

        private void OnEnable()
        {
            BaseEnable();

            if (audioCreatorGUI == null)
            {
                audioCreatorGUI = new AudioCreatorGUI(this);

            }
            audioCreatorGUI.OnEnable();
        }

        public void Find(Func<InAudioNode, bool> filter)
        {
            
            audioCreatorGUI.FindAudio(filter);
        }

        public void Find(InAudioNode toFind)
        {
            if (InAudioInstanceFinder.Instance != null)
                audioCreatorGUI.Find(toFind);
            else
            {
                Debug.LogError("InAudio: Cannot open window without having the manager in the scene");
            }
        }

        public static InAudioWindow Launch()
        {
            InAudioWindow window = EditorWindow.GetWindow < InAudioWindow>();
            window.Show();

            //window.minSize = new Vector2(800, 200);
            window.SetTitle("Audio Window");
            return window;
        }

        private GameObject cleanupGO;

        private void Update()
        {
            BaseUpdate();

            if (cleanupGO == null)
            {
                cleanupGO = Resources.Load("PrefabGO") as GameObject;
                DontDestroyOnLoad(cleanupGO);
            }


            if (audioCreatorGUI != null && Manager != null)
                audioCreatorGUI.OnUpdate();
        }

        private void OnGUI()
        {
            CheckForClose();

            //int nextControlID = GUIUtility.GetControlID(FocusType.Passive) + 1;
            //Debug.Log(nextControlID);  
            if (!HandleMissingData())
            {
                return;
            }

            if (audioCreatorGUI == null)
                audioCreatorGUI = new AudioCreatorGUI(this);

            isDirty = false;


            try
            {
                DrawTop(topHeight);
                isDirty |= audioCreatorGUI.OnGUI(LeftWidth, (int) position.height - topHeight);
            }
            catch (ExitGUIException e)
            {
                throw e;
            }
                /*catch (ArgumentException e)
        {
            throw e;
        }*/
            catch (Exception e)
            {
                if (e.GetType() != typeof (ArgumentException))
                {
                    Debug.LogException(e);

                    //While this catch was made to catch persistent errors,  like a missing null check, it can also catch other errors
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.HelpBox(
                        "An exception is getting caught while trying to draw this window.\nPlease report this bug to InAudio and if possible how to reproduce it",
                        MessageType.Error);

                    EditorGUILayout.TextArea(e.ToString());
                    EditorGUILayout.EndVertical();
                }
            }


            if (isDirty)
                Repaint();

            PostOnGUI();
        }

        private void DrawTop(int topHeight)
        {
            EditorGUILayout.BeginVertical(GUILayout.Height(topHeight));
            EditorGUILayout.EndVertical();
        }

        private void OnDestroy()
        {
            if (InAudioInstanceFinder.Instance != null &&
                InAudioInstanceFinder.Instance.GetComponent<AudioSource>() != null)
                InAudioInstanceFinder.Instance.GetComponent<AudioSource>().clip = null;
        }

        private class FileModificationWarning : UnityEditor.AssetModificationProcessor
        {
            private static string[] OnWillSaveAssets(string[] paths)
            {
                if (InAudioInstanceFinder.Instance != null)
                    InAudioInstanceFinder.Instance.GetComponent<AudioSource>().clip = null;
                return paths;
            }
        }
    }

}