using UnityEngine;
using UnityEngine.Serialization;

namespace InAudioSystem
{
    public class InAudioData : InAudioNodeData
    {
        [FormerlySerializedAs("EditorClip")]
        [SerializeField]
        private AudioClip _clip;

        public AudioClip AudioClip
        {
            get { return _clip; }
            set { _clip = value; }
        }

        public bool IsLoaded
        {
            get
            {
                if (_clip != null)
                    return _clip.loadState == AudioDataLoadState.Loaded || _clip.loadState == AudioDataLoadState.Loading;
                return false;
            }
        }
    }

}