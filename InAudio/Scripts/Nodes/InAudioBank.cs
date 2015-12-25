using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InAudioSystem
{
    [System.Serializable]
    public class BankData
    {
        [FormerlySerializedAs("Node")]
        public InAudioNode AudioNode;
        public InMusicGroup MusicNode;
    }
}