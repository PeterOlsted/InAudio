using System;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{   public abstract class InAudioBaseWindow : EditorWindow
    {
        protected InCommonDataManager Manager;

        protected int topHeight = 0;
        protected int LeftWidth = 350;

        protected bool isDirty;

        protected void BaseEnable()
        {
            TryOpenIntroductionWindow();


            //autoRepaintOnSceneChange = true;
            EditorApplication.modifierKeysChanged += Repaint;

            Manager = InAudioInstanceFinder.DataManager;
            minSize = new Vector2(400, 100);
        }

        protected static void TryOpenIntroductionWindow()
        {
            try
            {
                if (!PlayerPrefs.HasKey("InAudioIntro"))
                {
                    PlayerPrefs.SetInt("InAudioIntro", 1);
                    InAudioWindowOpener.ShowIntroductionWindow();
                }
                else
                {
                    int allowIntro = PlayerPrefs.GetInt("InAudioAllowIntro");
                    int haveIntro = PlayerPrefs.GetInt("InAudioIntro");
                    if (allowIntro == 1 && haveIntro == 1)
                    {
                        PlayerPrefs.SetInt("InAudioAllowIntro", 0);
                        InAudioWindowOpener.ShowIntroductionWindow();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (OnScriptReloaded != null)
            {
                OnScriptReloaded();
            }
        }



        public void RepaintNextFrame()
        {
            
        }

        public delegate void OnScriptReloadedDelegate();

        public static event OnScriptReloadedDelegate OnScriptReloaded;

        protected void BaseUpdate()
        {
            if (Event.current != null)
            {
                if (Event.current.type == EventType.ValidateCommand)
                {
                    switch (Event.current.commandName)
                    {
                        case "UndoRedoPerformed":
                            Repaint();
                            break;
                    }
                }
            }
        }

        protected void CheckForClose()
        {
            if (Event.current.IsKeyDown(KeyCode.W) && Event.current.modifiers == EventModifiers.Control)
            {
                Close();
                Event.current.UseEvent();
            }
        }

        protected bool HandleMissingData()
        {
            if (InAudioInstanceFinder.InAudioGuiUserPrefs == null)
            {
                ErrorDrawer.MissingGuiUserPrefs();
                return false;
            }

            if (Manager == null)
            {
                Manager = InAudioInstanceFinder.DataManager;
                if (Manager == null)
                {
                    ErrorDrawer.MissingAudioManager();
                }
            }

            if (Manager != null)
            {
                bool areAnyMissing = ErrorDrawer.IsDataMissing(Manager);

                if (areAnyMissing)
                {
                    Manager.Load();
                }
                if (ErrorDrawer.IsAllDataMissing(Manager))
                {
                    ErrorDrawer.AllDataMissing(Manager);
                    return false;
                }
                if (ErrorDrawer.IsDataMissing(Manager))
                {
                    ErrorDrawer.MissingData(Manager);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
                return false;
        }

        protected void PostOnGUI()
        {
            
        }

        protected bool IsKeyDown(KeyCode code)
        {
            return Event.current.type == EventType.keyDown && Event.current.keyCode == code;
        }

    }

}