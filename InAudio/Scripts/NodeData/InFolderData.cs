using System.Collections.Generic;

namespace InAudioSystem
{
    public class InFolderData : InAudioNodeBaseData
    {
        public bool ExternalPlacement;

        public float VolumeMin = 1.0f;
        
        public float runtimeVolume;

        [System.NonSerialized]
        public float hiearchyVolume;

        [System.NonSerialized]
        public List<InPlayer> runtimePlayers = new List<InPlayer>();
    }

}