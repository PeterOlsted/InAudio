using System.Collections.Generic;
using InAudioSystem;
using UnityEngine;

[System.Serializable]
public class InAudioEvent
{
    //Be carefull renamming this class. 
    //If Unitys' serializer does not understand it is the same field, every hook will lose its data
    [SerializeField]
    public List<EventHookListData> Events = new List<EventHookListData>();

    public void Add(InAudioEventNode audioEvent, EventHookListData.PostEventAt postAt)
    {
        var data = new EventHookListData(audioEvent);
        data.PostAt = postAt;
        Events.Add(data);
    }
}

namespace InAudioSystem
{
    [System.Serializable]
    public class EventHookListData
    {
        [SerializeField]
        public InAudioEventNode Event;
        [SerializeField]
        public PostEventAt PostAt;

        public EventHookListData(InAudioEventNode audioEvent)
        {
            Event = audioEvent;
        }

        public enum PostEventAt
        {
            AttachedTo = 0,
            AtPosition = 1,
        }
    }

}
