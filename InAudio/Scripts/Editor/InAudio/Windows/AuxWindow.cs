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
                bool missingaudio = Manager.AudioTree == null;
                bool missingaudioEvent = Manager.EventTree == null;
                bool missingMusic = Manager.MusicTree == null;

                bool areAllMissing = missingaudio && missingaudioEvent && missingMusic;
                bool areAnyMissing = missingaudio || missingaudioEvent || missingMusic;

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
                DrawMissingDataCreation();

                DrawStartFromScratch();
            }

            if (isDirty)
                Repaint();

            PostOnGUI();
        }

        private void DrawMissingDataCreation()
        {
            bool missingaudio = Manager.AudioTree == null;
            bool missingaudioEvent = Manager.EventTree == null;
            bool missingMusic = Manager.MusicTree == null;


            bool areAnyMissing = missingaudio | missingaudioEvent  | missingMusic;
            if (areAnyMissing)
            {
                string missingAudioInfo = missingaudio ? "Audio Data\n" : "";
                string missingEventInfo = missingaudioEvent ? "Event Data\n" : "";
                string missingMusicInfo = missingMusic ? "Music Data\n" : "";

                EditorGUILayout.BeginVertical();
                EditorGUILayout.HelpBox(missingAudioInfo + missingEventInfo + missingMusicInfo + "is missing.\nThis may be because of new project data is required in this version of InAudio",
                    MessageType.Error, true);

                bool areAllMissing = missingaudio && missingaudioEvent && missingMusic;
                if (!areAllMissing)
                {

                    if (GUILayout.Button("Create missing content", GUILayout.Height(30)))
                    {
                        int levelSize = 3;
                        //How many subfolders by default will be created. Number is only a hint for new people
                        if (missingaudio)
                            CreateAudioPrefab(levelSize);
                        if (missingaudioEvent)
                            CreateEventPrefab(levelSize);
     
                        if (missingMusic)
                            CreateMusicPrefab(levelSize);
     
                        Manager.Load(true);
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

        private bool AreAllMissing()
        {
            bool missingaudio = Manager.AudioTree == null;
            bool missingaudioEvent = Manager.EventTree == null;
            bool missingMusic = Manager.MusicTree == null;

            return missingaudio && missingaudioEvent && missingMusic ;
        }

        private void DrawStartFromScratch()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Start over from scratch", GUILayout.Height(30)))
            {
                if (AreAllMissing() ||
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

        private void CreateEventPrefab(int levelSize)
        {
            GameObject go = new GameObject();
            Manager.EventTree = AudioEventWorker.CreateTree(go, levelSize);
            SaveAndLoad.CreateAudioEventRootPrefab(go);
        }

        private void CreateMusicPrefab(int levelSize)
        {
            GameObject go = new GameObject();
            Manager.MusicTree = MusicWorker.CreateTree(go, levelSize);
            SaveAndLoad.CreateMusicRootPrefab(go);
        }


        private void CreateAudioPrefab(int levelSize)
        {
            GameObject go = new GameObject();
            Manager.AudioTree = AudioNodeWorker.CreateTree(go, levelSize);
            SaveAndLoad.CreateAudioNodeRootPrefab(go);
        }
    }
}