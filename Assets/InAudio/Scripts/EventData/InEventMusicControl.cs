using InAudioLeanTween;
using UnityEngine;
using System.Collections;

namespace InAudioSystem
{
    public class InEventMusicControl : AudioEventAction
    {
        public InMusicGroup MusicGroup;
        //public MusicState SwitchTo;

        public bool Fade;
        public float Duration = 0;
        public LeanTweenType TweenType = LeanTweenType.easeInOutQuad;

        public bool ChangeVolume;
        public float VolumeTarget = 1f;

        public override Object Target
        {
            get
            {
                return MusicGroup;
            }
            set
            {
                var newTarget = value as InMusicGroup;
                if (newTarget != null)
                {
                    MusicGroup = newTarget;
                }
            }
        }

        public override string ObjectName
        {
            get
            {
                if (MusicGroup == null)
                {
                    return "Missing Music";
                }
                else return MusicGroup.GetName;
            }
        }
    }


}

