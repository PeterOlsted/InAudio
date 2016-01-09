using System;
using System.Collections;
using System.Globalization;
using InAudioSystem.ExtensionMethods;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.Internal
{
    public class InCommonDataManager : MonoBehaviour
    {
        private InAudioNode AudioRoot;
        private InAudioEventNode EventRoot;
        private InMusicNode MusicRoot;

        public InAudioNode AudioTree
        {
            get { return AudioRoot; }
            set { AudioRoot = value; }
        }

        public InAudioEventNode EventTree
        {
            get { return EventRoot; }
            set { EventRoot = value; }
        }

        public InMusicNode MusicTree
        { 
            get { return MusicRoot; }
            set { MusicRoot = value; }
        }


        private Component[] roots = null;
        public Component[] Roots
        {
            get
            {
                if (roots == null || roots.Length == 0)
                {
                    roots = new Component[] {AudioRoot, EventRoot, MusicRoot};
                }
                return roots;
            } 
        }

        public void ForceLoad()
        {
            Load(true);
        }

        public void Load() 
        {
            Load(false);
        }

        private void Load(bool forceReload)
        {
            if (!Loaded || forceReload)
            {
                AudioRoot = LoadData<InAudioNode>(FolderSettings.AudioLoadData);
                EventRoot = LoadData<InAudioEventNode>(FolderSettings.EventLoadData);
                MusicRoot = LoadData<InMusicNode>(FolderSettings.MusicLoadData);
                roots = new Component[] { AudioRoot, EventTree, MusicRoot };
            }
        }

        private T LoadData<T>(string location) where T : Object, InITreeNode<T>
        {
            return CheckData<T>(SaveAndLoad.LoadManagerData(location));
        }

        //Does the components actually exist and does it have a root?
        private T CheckData<T>(Component[] data) where T : Object, InITreeNode<T>
        {
            if (data != null && data.Length > 0 && data[0] is T)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    var iData = data[i] as InITreeNode<T>;
                    if (iData != null)
                    {
                        if (iData.IsRoot)
                        {
                            return data[i] as T;
                        }
                    }
                }
            }
            return null;
        }

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            //Instance = this;
            Load();
        }

        public bool Loaded
        {
            get { return !Roots.AnyNull(); }
        }


#if UNITY_EDITOR
        private bool checkVersion;

        private IEnumerator VersionCheck()
        {
            
            WWW website = new WWW("http://innersystems.net/version.html");
            yield return website;
            if (website.error == null)
            {
                PlayerPrefs.SetString("InAudioUpdateInfo", website.text);
                PlayerPrefs.SetString("InAudioUpdateCheckTime", DateTime.Now.Date.DayOfYear.ToString(CultureInfo.InvariantCulture));
                
            }
            website.Dispose();
        }

        void Update()
        {
            if (!checkVersion && PlayerPrefs.GetString("InAudioUpdateCheckTime") != DateTime.Now.Date.DayOfYear.ToString(CultureInfo.InvariantCulture))
            {
                checkVersion = true;
                StartCoroutine(VersionCheck());
            }
        }
#endif
    }

}