using System;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace InAudioSystem
{
    public enum EventActionTypes
    {
        [EnumIndex(1, "Play Audio")] Play = 1,
        [EnumIndex(2, "Stop Audio")] Stop = 2,
        [EnumIndex(3, "Break Audio")] Break = 3,
        [EnumIndex(5, "Stop All Audio")] StopAll = 5,
        [EnumIndex(10, "Bank Loading")] BankLoading = 8,
        [EnumIndex(6, "Mixer Snapshot")] SetSnapshot = 12,
        [EnumIndex(7, "Mixer Value")] MixerValue = 13,
        [EnumIndex(11, "Play Music")] PlayMusic = 14,
        [EnumIndex(12, "Stop Music")] StopMusic = 15,
        [EnumIndex(13, "Pause Music")] PauseMusic = 16,
        [EnumIndex(14, "Music Volume Control")] FadeMusic = 17,
        [EnumIndex(15, "Crossfade Music")] CrossfadeMusic = 18,
        [EnumIndex(16, "Solo-Mute Music")] SoloMuteMusic = 19,
        [EnumIndex(17, "Stop All Music")] StopAllMusic = 20,
    };


    public abstract class AudioEventAction : MonoBehaviour
    {
        public float Delay;
        [FormerlySerializedAs("EventActionType")]
        public EventActionTypes _eventActionType;
        public abstract Object Target { get; set; }

        public abstract string ObjectName { get; }

        public static Type ActionEnumToType(EventActionTypes actionType)
        {
            switch (actionType)
            {
                case EventActionTypes.Play:
                    return typeof(InEventAudioAction);
                case EventActionTypes.Stop:
                    return typeof(InEventAudioAction);
                case EventActionTypes.StopAll:
                    return typeof(InEventAudioAction);
                case EventActionTypes.BankLoading:
                    return typeof(InEventBankLoadingAction);
                case EventActionTypes.Break:
                    return typeof(InEventAudioAction);
                case EventActionTypes.SetSnapshot:
                    return typeof(InEventSnapshotAction);
                case EventActionTypes.MixerValue:
                    return typeof(InEventMixerValueAction);
                case EventActionTypes.PlayMusic:
                    return typeof(InEventMusicControl);
                case EventActionTypes.StopMusic:
                    return typeof(InEventMusicControl);
                case EventActionTypes.PauseMusic:
                    return typeof(InEventMusicControl);
                case EventActionTypes.FadeMusic:
                    return typeof(InEventMusicFade);
                case EventActionTypes.CrossfadeMusic:
                    return typeof(InEventMusicFade);
                case EventActionTypes.SoloMuteMusic:
                    return typeof(InEventSoloMuteMusic);
                case EventActionTypes.StopAllMusic:
                    return typeof(InEventMusicFade);
            }
            return null;
        }

    }


}

