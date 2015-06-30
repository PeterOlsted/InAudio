using System;
using InAudioSystem.Internal;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{

    public class InteractiveMusicWindow : InAudioBaseWindow
    {
        private InteractiveMusicCreatorGUI interactiveMusicCreatorGUI;

        private void OnEnable()
        {
            BaseEnable();

            if(interactiveMusicCreatorGUI == null)
            {
                interactiveMusicCreatorGUI = new InteractiveMusicCreatorGUI(this);
            }
            interactiveMusicCreatorGUI.OnEnable();
        }

     

        public static InteractiveMusicWindow Launch()
        {
            InteractiveMusicWindow window = GetWindow<InteractiveMusicWindow>();
            window.Show();

            window.SetTitle("Interactive Music Window");
            return window;
        }

        private void Update()
        {
            BaseUpdate();
        }

        private void OnGUI()
        {
            CheckForClose();

            if (!HandleMissingData())
            {
                return;
            }
            

            isDirty = false;


            try
            {
                
            }
            catch (ExitGUIException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                if (e.GetType() != typeof(ArgumentException))
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