using InAudioSystem.InAudioSystem;
using UnityEngine;

namespace InAudioSystem
{
    public class InAudioNodeData : InAudioNodeBaseData
    {
        public bool RandomVolume = false;
        public float MinVolume = 1.0f;
        public float MaxVolume = 1.0f;

        public bool RandomizeDelay = false;
        public float InitialDelayMin = 0.0f;
        public float InitialDelayMax = 0.0f;

        public bool RandomPitch = false;
        public float MinPitch = 1.0f;
        public float MaxPitch = 1.0f;

        public bool RandomBlend = false;
        public float MinBlend = 1.0f;
        public float MaxBlend = 1.0f;

        public bool Loop = false;
        public bool LoopInfinite = false;
        public bool RandomizeLoops = false;
        public byte MinIterations = 1;
        public byte MaxIterations = 1;

        public bool OverrideAttenuation = false;
        public float MinDistance = 1;
        public float MaxDistance = 500;
        public AudioRolloffMode RolloffMode = AudioRolloffMode.Logarithmic;

        public bool LimitInstances = false;

        public int MaxInstances = 1;

        public InstanceStealingTypes InstanceStealingTypes = InstanceStealingTypes.NoStealing;

        public int Priority = 128;

        public bool RandomSecondsOffset;
        public float MinSecondsOffset;
        public float MaxSecondsOffset;

#if UNITY_EDITOR
        public int SelectedArea = 0;
#endif

        public AnimationCurve FalloffCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.1f, 1),
            new Keyframe(1, 0));

        /*public RandomType Random;

    public enum RandomType {
        Shuffle, Standard
    }*/
    }


    namespace InAudioSystem
    {
        public enum InstanceStealingTypes
        {
            NoStealing = 0,
            Oldest = 1,
            Newest = 2,
        }
    }
}