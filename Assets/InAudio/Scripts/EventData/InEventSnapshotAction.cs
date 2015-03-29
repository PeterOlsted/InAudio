using UnityEngine;
using UnityEngine.Audio;

namespace InAudioSystem
{

    public class InEventSnapshotAction : AudioEventAction
    {
        public AudioMixerSnapshot Snapshot;
        public float TransitionTime;

        public override Object Target
        {
            get { return Snapshot; }
            set
            {
                if (value is AudioMixerSnapshot)
                    Snapshot = value as AudioMixerSnapshot;
            }
        }

        public override string ObjectName
        {
            get
            {
                if (Snapshot != null)
                    return Snapshot.name + " (Audio Mixer Snapshot)";
                else
                {
                    return "Missing Snapshot";
                }

            }
        }
    }


}