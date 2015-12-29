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
        private readonly string[] toolbarOptions = { "Banks", "Integrity", "Project Data" };

        private AudioMixerGroup selectedBus;

        private AudioBankCreatorGUI bankGUI;
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
            if (bankGUI == null)
                bankGUI = new AudioBankCreatorGUI(this);
            if (integrityGUI == null)
                integrityGUI = new IntegrityGUI(this);
            bankGUI.OnEnable();
            integrityGUI.OnEnable();

        }

        private void Update()
        {
            BaseUpdate();
            bankGUI.OnUpdate();
        }

        private void OnGUI()
        {
            bankGUI.BaseOnGUI();
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
                bool missingBank = Manager.BankLinkTree == null;
                bool missingMusic = Manager.MusicTree == null;

                bool areAllMissing = missingaudio && missingaudioEvent && missingBank && missingMusic;
                bool areAnyMissing = missingaudio || missingaudioEvent || missingBank || missingMusic;

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
            {
                isDirty |= bankGUI.OnGUI(LeftWidth, (int)position.height - (int)EditorGUIUtility.singleLineHeight);
            }

            if (selectedToolbar == 1)
                isDirty |= integrityGUI.OnGUI();

            if (selectedToolbar == 2)
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
            bool missingbankLink = Manager.BankLinkTree == null;
            bool missingMusic = Manager.MusicTree == null;


            bool areAnyMissing = missingaudio | missingaudioEvent | missingbankLink | missingMusic;
            if (areAnyMissing)
            {
                string missingAudioInfo = missingaudio ? "Audio Data\n" : "";
                string missingEventInfo = missingaudioEvent ? "Event Data\n" : "";
                string missingBankInfo = missingbankLink ? "BankLink Data\n" : "";
                string missingMusicInfo = missingMusic ? "Music Data\n" : "";

                EditorGUILayout.BeginVertical();
                EditorGUILayout.HelpBox(missingAudioInfo + missingEventInfo + missingMusicInfo + missingBankInfo + "is missing.\nThis may be because of new project data is required in this version of InAudio",
                    MessageType.Error, true);

                bool areAllMissing = missingaudio && missingaudioEvent && missingbankLink && missingMusic;
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
                        if (missingbankLink)
                            CreateBankLinkPrefab();
                        if (missingMusic)
                            CreateMusicPrefab(levelSize);
     
                        Manager.Load(true);

                        if (Manager.AudioTree != null && Manager.BankLinkTree != null)
                            NodeWorker.AssignToNodes(Manager.AudioTree, node =>
                            {
                                var data = (node._nodeData as InFolderData);
                                if (data != null)
                                    data.BankLink = Manager.BankLinkTree._getChildren[0];
                            });
                        if (Manager.MusicTree != null && Manager.BankLinkTree != null)
                            NodeWorker.AssignToNodes(Manager.MusicTree, node =>
                            {
                                var folder = (node as InMusicFolder);
                                if (folder != null)
                                    folder._bankLink = Manager.BankLinkTree._getChildren[0];
                            });

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
            bool missingbankLink = Manager.BankLinkTree == null;
            bool missingMusic = Manager.MusicTree == null;

            return missingaudio && missingaudioEvent && missingbankLink && missingMusic ;
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



        public void SelectBankCreation()
        {
            selectedToolbar = 0;
        }

        public void SelectIntegrity()
        {
            selectedToolbar = 1;
        }

        public void SelectDataCreation()
        {
            selectedToolbar = 2;
        }

        public void FindBank(InAudioBankLink bankLink)
        {
            selectedToolbar = 0;
            bankGUI.Find(bankLink);

        }

        private void CreateEventPrefab(int levelSize)
        {
            GameObject go = new GameObject();
            Manager.EventTree = AudioEventWorker.CreateTree(go, levelSize);
            SaveAndLoad.CreateAudioEventRootPrefab(go);
        }

        private void CreateBankLinkPrefab()
        {
            GameObject go = new GameObject();
            Manager.BankLinkTree = AudioBankWorker.CreateTree(go);
            SaveAndLoad.CreateAudioBankLinkPrefab(go);
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