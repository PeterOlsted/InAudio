using InAudioSystem.ExtensionMethods.Repeat;
using InAudioSystem.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

#if !UNITY_5_2
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif 

namespace InAudioSystem.InAudioEditor
{

    public class AuxWindow : InAudioBaseWindow
    {
        private int selectedToolbar = 0;
        private readonly string[] toolbarOptions = { "Integrity", "Project Data" };

        private AudioMixerGroup selectedBus;

        private IntegrityGUI integrityGUI;

        public static AuxWindow Launch()
        {
            AuxWindow window = GetWindow<AuxWindow>(typeof(AuxWindow));

            window.Show();
            window.minSize = new Vector2(400, 400);
            window.SetTitle("Aux Window");
            return window;
        }

        public void OnEnable()
        {
            BaseEnable();
        
            if (integrityGUI == null)
                integrityGUI = new IntegrityGUI(this);
   
            integrityGUI.OnEnable();

        }

        private void Update()
        {
            BaseUpdate();

        }

        private void OnGUI()
        {
 
            CheckForClose();
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
                
                bool areAllMissing = ErrorDrawer.IsAllDataMissing(Manager);
                bool areAnyMissing = ErrorDrawer.IsDataMissing(Manager);

                if (areAllMissing)
                {
                    ErrorDrawer.AllDataMissing(Manager);
                    return;
                }
                else if (areAnyMissing)
                {
                    DrawMissingDataCreation();
                    return;
                }
                
            }
            else
            {
                return;
            }

            isDirty = false;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.EndVertical();
            selectedToolbar = GUILayout.Toolbar(selectedToolbar, toolbarOptions);


            if (selectedToolbar == 0)
                isDirty |= integrityGUI.OnGUI();

            if (selectedToolbar == 1)
            {
                bool areAnyMissing = ErrorDrawer.IsDataMissing(Manager);
                if (!areAnyMissing)
                {
                    EditorGUILayout.HelpBox("Everything seems good, all InAudio projects part are accounted for.", MessageType.Info);
                }


                DrawMissingDataCreation();

                DrawStartFromScratch();
            }

            if (isDirty)
                Repaint();

            PostOnGUI();
        }

        private void DrawMissingDataCreation()
        {
            if (ErrorDrawer.IsDataMissing(Manager))
            {
                EditorGUILayout.BeginVertical();
                
                if (!ErrorDrawer.IsAllDataMissing(Manager))
                {
                    EditorGUILayout.HelpBox("Some or all project data is missing. This may be because of new project data is required in this version of InAudio",
                    MessageType.Warning, true);
                    if (GUILayout.Button("Create missing content", GUILayout.Height(30)))
                    {
                        MissingDataHelper.CreateIfMissing(Manager);

                        Manager.ForceLoad();
#if !UNITY_5_2
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
#else
                        EditorApplication.MarkSceneDirty();
                        EditorApplication.SaveCurrentSceneIfUserWantsTo();
#endif

                    }
                }

                DrawStartFromScratch();
                
                EditorGUILayout.EndVertical();
            }

        }

        private void DrawStartFromScratch()
        {
            Repeater.Repeat(8, () =>
            {
                EditorGUILayout.Separator();
            });
            EditorGUILayout.HelpBox("Warning, this button will delete the entire InAudio project and create a new. This process cannot be undone.", MessageType.Error);
            if (GUILayout.Button("Start over from scratch", GUILayout.Height(30)))
            {
                if (ErrorDrawer.IsAllDataMissing(Manager) ||
                    EditorUtility.DisplayDialog("Create new project?", "This will delete ALL data!",
                        "Start over from scratch", "Do nothing"))
                {
                    MissingDataHelper.StartFromScratch(Manager);
                }
            }
        }

        public void SelectIntegrity()
        {
            selectedToolbar = 0;
        }

        public void SelectDataCreation()
        {
            selectedToolbar = 1;
        }


    }
}