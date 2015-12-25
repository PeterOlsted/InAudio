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
            this.SetTitle("Event Window");
            if (audioEventCreatorGUI == null)
            {
                audioEventCreatorGUI = new AudioEventCreatorGUI(this);
            }
            audioEventCreatorGUI.OnEnable();
        }

        public static EventWindow Launch()
        {
            EventWindow window = GetWindow <EventWindow>();
            window.Show();
            
            return window;
        }

        public void ReceiveNode(InMusicGroup group)
        {
            audioEventCreatorGUI.ReceiveNode(group);
        }

        public void ReceiveNode(InAudioNode node)
        {
            audioEventCreatorGUI.ReceiveNode(node);
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