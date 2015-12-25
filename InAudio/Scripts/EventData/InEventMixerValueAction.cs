using InAudioLeanTween;
using UnityEngine;
using UnityEngine.Audio;

namespace InAudioSystem
{


    public class InEventMixerValueAction : AudioEventAction
    {
        public AudioMixer Mixer;
        public string Parameter;
        public float Value;
        public float TransitionTime;
        public LeanTweenType TransitionType = LeanTweenType.linear;

        public override Object Target
        {
            get { return Mixer; }
            set { Mixer = value as AudioMixer; }
        }

        public override string ObjectName
        {
            get
            {
                if (Mixer != null)
                    return Mixer.ToString();
                else
                {
                    return "Missing Mixer";
                }

            }
        }
    }
}