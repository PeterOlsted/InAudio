using InAudioSystem.InAudioEditor;
using InAudioSystem.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace InAudioSystem.Internal
{

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class InAudioInstanceFinder : MonoBehaviour
    {
        private static InAudioInstanceFinder instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        private void OnEnable()
        {
            if (instance == null)
                instance = this;
        }

        public static bool IsValid
        { 
            get { return instance != null; }
        }

        public static InAudioInstanceFinder Instance
        {
            get { return instance; }
        }

        private static InCommonDataManager _dataManager;

        public static InCommonDataManager DataManager
        {
            get
            {
                if (_dataManager == null)
                {
                    _dataManager = FindObjectOfType(typeof (InCommonDataManager)) as InCommonDataManager;
                    if (_dataManager != null)
                        _dataManager.Load();
                }
                return _dataManager;
            }
        }


        private static InRuntimeAudioData _runtimeAudioData;

        public static InRuntimeAudioData RuntimeAudioData
        {
            get
            {
                if (_runtimeAudioData == null)
                {
                    _runtimeAudioData = instance.GetComponent<InRuntimeAudioData>();
                }
                return _runtimeAudioData;
            }
        }

        private static InAudioEventWorker _inAudioEventWorker;

        public static InAudioEventWorker InAudioEventWorker
        {
            get
            {
                if (_inAudioEventWorker == null)
                {
                    _inAudioEventWorker = instance.GetComponent<InAudioEventWorker>();
                }
                return _inAudioEventWorker;
            }
        }

        private static InRuntimeInfoPool _runtimeInfoPool;

        public static InRuntimeInfoPool RuntimeInfoPool
        {
            get
            {
                if (_runtimeInfoPool == null && instance != null)
                {
                    _runtimeInfoPool = instance.GetComponent<InRuntimeInfoPool>();
                }
                return _runtimeInfoPool;
            }
        }

        private static InMusicPlayerPool _musicPlayerPool;

        public static InMusicPlayerPool InMusicPlayerPool
        {
            get
            {
                if (_musicPlayerPool == null && instance != null)
                {
                    _musicPlayerPool = instance.GetComponent<InMusicPlayerPool>();
                }
                return _musicPlayerPool;
            }
        }

        private static MusicPlayer _musicPlayer;

        public static MusicPlayer MusicPlayer
        {
            get
            {
                if (_musicPlayer == null && instance != null)
                {
                    _musicPlayer = instance.GetComponent<MusicPlayer>();
                }
                return _musicPlayer;
            }
        }

        private static InDSPTimePool _dspTimePool;

        public static InDSPTimePool DSPTimePool
        {
            get
            {
                if (_dspTimePool == null)
                {
                    _dspTimePool = instance.GetComponent<InDSPTimePool>();
                }
                return _dspTimePool;
            }
        }

        private static ArrayPool<DSPTime> _dspArrayPool;

        public static ArrayPool<DSPTime> DSPArrayPool
        {
            get
            {
                if (_dspArrayPool == null)
                    _dspArrayPool = new ArrayPool<DSPTime>(4, 1, 1);
                return _dspArrayPool;
            }
        }

        private static ArrayPool<Coroutine> _coroutinePool;

        public static ArrayPool<Coroutine> CoroutinePool
        {
            get
            {
                if (_coroutinePool == null)
                    _coroutinePool = new ArrayPool<Coroutine>(4, 1, 1);
                return _coroutinePool;
            }
        }

        private static InRuntimePlayerPool _inRuntimePlayerPool;

        public static InRuntimePlayerPool InRuntimePlayerPool
        {
            get
            {
                if (_inRuntimePlayerPool == null && instance != null)
                    _inRuntimePlayerPool = instance.GetComponent<InRuntimePlayerPool>();
                return _inRuntimePlayerPool;
            }
        }

        private static InRuntimePlayerControllerPool _runtimePlayerControllerPool;

        public static InRuntimePlayerControllerPool RuntimePlayerControllerPool
        {
            get
            {
                if (_runtimePlayerControllerPool == null && instance != null)
                    _runtimePlayerControllerPool = instance.GetComponent<InRuntimePlayerControllerPool>();
                return _runtimePlayerControllerPool;
            }
        }

#if UNITY_EDITOR
        private static InAudioGUIUserPrefs _inAudioGuiUserPref;

        public static InAudioGUIUserPrefs InAudioGuiUserPrefs
        {
            get
            {
                if (_inAudioGuiUserPref == null)
                {
                    string path = FolderSettings.GUIUserPrefs;
                    var prefGO = AssetDatabase.LoadAssetAtPath(path, typeof (GameObject)) as GameObject;
                    if (prefGO != null)
                    {
                        _inAudioGuiUserPref = prefGO.GetComponent<InAudioGUIUserPrefs>();
                        if (_inAudioGuiUserPref == null)
                        {
                            _inAudioGuiUserPref = prefGO.AddComponent<InAudioGUIUserPrefs>();
                        }
                    }

                }
                return _inAudioGuiUserPref;
            }
        }
#endif
    }

}