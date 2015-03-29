using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public class EventWindow : InAudioBaseWindow
    {
        private AudioEventCreatorGUI audioEventCreatorGUI;

        private void OnEnable()
        {
            BaseEnable();

            if (audioEventCreatorGUI == null)
                audioEventCreatorGUI = new AudioEventCreatorGUI(this);
            audioEventCreatorGUI.OnEnable();
        }

        public static void Launch()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof (EventWindow));
            window.Show();

            window.title = "Event Window";
        }

        private void Update()
        {
            BaseUpdate();
            if (audioEventCreatorGUI != null && Manager != null)
                audioEventCreatorGUI.OnUpdate();
        }

        public void Find(InAudioEventNode toFind)
        {
            audioEventCreatorGUI.Find(toFind);
        }

        private void OnGUI()
        {
            CheckForClose();
            if (!HandleMissingData())
            {
                return;
            }


            if (audioEventCreatorGUI == null)
                audioEventCreatorGUI = new AudioEventCreatorGUI(this);

            isDirty = false;
            DrawTop(0);

            isDirty |= audioEventCreatorGUI.OnGUI(LeftWidth, (int) position.height - topHeight);

            if (isDirty)
                Repaint();

            PostOnGUI();
        }

        private void DrawTop(int topHeight)
        {
            EditorGUILayout.BeginVertical(GUILayout.Height(topHeight));
            EditorGUILayout.EndVertical();
        }

    }

}