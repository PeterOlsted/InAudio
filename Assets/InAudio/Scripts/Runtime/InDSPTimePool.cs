using UnityEngine;

namespace InAudioSystem.Runtime
{
    [AddComponentMenu(FolderSettings.ComponentPathPrefabsManager + "DSP Time Pool")]
    public class InDSPTimePool : InAudioObjectPool<DSPTime>
    {

    }

    public class DSPTime
    {
        public double CurrentEndTime;
        public AudioSource Player;

        public DSPTime(double currentEndTime)
        {
            CurrentEndTime = currentEndTime;
        }

        public DSPTime()
        {
        }

        public DSPTime(DSPTime time)
        {
            CurrentEndTime = time.CurrentEndTime;
        }
    }
}

 