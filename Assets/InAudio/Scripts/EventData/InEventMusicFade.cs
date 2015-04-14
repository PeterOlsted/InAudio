using InAudioLeanTween;
using UnityEngine;
using System.Collections;

namespace InAudioSystem
{
    public class InEventMusicFade : AudioEventAction
    {
        public InMusicGroup From;
        public InMusicGroup To;
        public float Duration = 0;
        public LeanTweenType TweenType = LeanTweenType.easeInOutQuad;
        public MusicState DoAtEndFrom = MusicState.Nothing;
        public MusicState DoAtEndTo = MusicState.Nothing;

        public float FromVolumeTarget = 1;
        public float ToVolumeTarget = 1;


        public override Object Target
        {
            get
            {
                return To;
            }
            set
            {
                var newTarget = value as InMusicGroup;
                if (newTarget != null)
                {
                    To = newTarget;
                }
            }
        }

        public override string ObjectName
        {
            get
            {
                if (To == null)
                {
                    return "Missing Music";
                }
                else return To.GetName;
            } 
        }
    }


}
