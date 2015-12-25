using System;
using InAudioSystem.InAudioEditor;
using InAudioSystem.Internal;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{

    public class InMusicWindow : InAudioBaseWindow
    {
        private MusicCreatorGUI musicCreatorGUI;

        private void OnEnable()
        {
            BaseEnable();

            if (musicCreatorGUI == null)
            {
                musicCreatorGUI = new MusicCreatorGUI(this);
                autoRepaintOnSceneChange = true;
            }
            musicCreatorGUI.OnEnable();
        }

        public void Find(Func<InMusicNode, bool> filter)
        {
            musicCreatorGUI.FindAudio(filter);
        }

        public void Find(InMusicNode toFind)
        {
            if (InAudioInstanceFinder.Instance != null)
                musicCreatorGUI.Find(toFind);
            else
            {
                Debug.LogError("InAudio: Cannot open window without having the manager in the scene");
            }
        }

        public static InMusicWindow Launch()
        {
            InMusicWindow window = GetWindow<InMusicWindow>();
            window.Show();

            //window.minSize = new Vector2(800, 200);
            window.SetTitle("Music Window");
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


            if (musicCreatorGUI != null && Manager != null)
                musicCreatorGUI.OnUpdate();
        }

        private void OnGUI()
        {
            CheckForClose();

            if (!HandleMissingData())
            {
                return;
            }

            if (musicCreatorGUI == null)
                musicCreatorGUI = new MusicCreatorGUI(this);

            isDirty = false;


            try
            {
                DrawTop(topHeight);
                isDirty |= musicCreatorGUI.OnGUI(LeftWidth, (int) position.height - topHeight);
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
                        "An exception is getting caught while trying to draw this window.\nPlease report this bug to inaudio@outlook.com and if possible how to reproduce it",
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