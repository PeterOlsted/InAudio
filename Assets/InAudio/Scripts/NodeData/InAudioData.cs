using UnityEngine;
using UnityEngine.Serialization;

namespace InAudioSystem
{
    public class InAudioData : InAudioNodeData
    {
        [FormerlySerializedAs("EditorClip")]
        public AudioClip _clip;

        public bool LoadedFromAssetBundle;

        public AudioClip AudioClip
        {
            get { return _clip; }
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