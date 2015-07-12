using System;
using System.Text.RegularExpressions;
using InAudioSystem.InAudioFeedback;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public class FeedbackWindow : EditorWindow
    {
        private string emailTitle = "InAudio feedback";
        private string content = "Hi, ";
        private string @from = "anonymous@nobody.com";
        private Vector2 scroll;

        private enum SendingStatus
        {
            NotSend,
            Sending,
            Send,
            Error
        }

        private SendingStatus send = SendingStatus.NotSend;

        private void OnEnable()
        {
            this.SetTitle("Feedback");
        }


        private void OnGUI()
        {
            if (Event.current.keyCode == KeyCode.W && Event.current.type == EventType.keyDown && Event.current.modifiers == EventModifiers.Control)
            {
                base.Close();
                Event.current.Use();
            }

            GUI.skin.label.wordWrap = true;
            GUILayout.Label("Your feedback is appreciated for all aspect of InAudio.\nIf you are reporting a bug, please include reproduction steps and any error messages. \nDo not send confidential information.");
            EditorGUILayout.Separator();
            emailTitle = EditorGUILayout.TextField("Title", emailTitle);
            @from = EditorGUILayout.TextField("Contact Email", @from);
            
            EditorGUILayout.LabelField("Content");
            scroll = EditorGUILayout.BeginScrollView(scroll);
            content = EditorGUILayout.TextArea(content, GUILayout.MaxHeight(300));
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Separator();

            string sendText = "Send";
            switch (send)
            {
                case SendingStatus.NotSend:
                    sendText = "Send feedback";
                    break;
                case SendingStatus.Sending:
                    GUI.enabled = false;
                    sendText = "Sending feedback";
                    break;
                case SendingStatus.Send:
                    sendText = "Feedback send. Thank you!";
                    break;
                case SendingStatus.Error:
                    sendText = "There was a problem sending the feedback";
                    break;
            }

            
            if (GUILayout.Button(sendText))
            {
                if (!Regex.IsMatch(from, @".+\@.+\..+"))
                {
                    if (EditorUtility.DisplayDialog("Email", "The from email does not look like a valid email. It will be send from anonymous@nobody.com", "Ok", "Cancel"))
                    {
                        Send("anonymous@nobody.com");
                        
                    }
                }
                else
                {
                    Send(from);
                    
                }
            }
            GUI.enabled = true;
        }

        private void Send(string email)
        {
            try
            {
                FeedbackSender.Send(email, content, emailTitle, Application.unityVersion, InAudio.CurrentVersion);
                send = SendingStatus.Send;
            }
            catch (Exception)
            {
                send = SendingStatus.Error;
            }
        }
    }
}
