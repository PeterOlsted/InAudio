using InAudioSystem;
using UnityEngine;
using System.Collections;

namespace InAudioSystem
{
    public class InEventSoloMuteMusic : AudioEventAction
    {
        public InMusicGroup MusicGroup;
        public bool SetSolo = false;
        public bool SetMute = false;

        public bool SoloTarget = false;
        public bool MuteTarget = false;

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