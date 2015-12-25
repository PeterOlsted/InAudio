using System;
using UnityEngine;

namespace InAudioSystem.Runtime
{

    [System.Serializable]
    public class MecanimParameter
    {
        public InAudioNode Node;
        [Range(0, 1)] public float Target = 1.0f;

        [NonSerialized] public float StartVolume;
    }
}