using System.Collections.Generic;
using InAudioSystem;
using UnityEngine;

namespace InAudioSystem
{

    public class InRuntimeAudioData : MonoBehaviour
    {
        public Dictionary<int, InAudioEventNode> Events;

        public InAudioEventNode GetEvent(int id)
        {
            InAudioEventNode audioEvent;
            Events.TryGetValue(id, out audioEvent);
            return audioEvent;
        }

        public void UpdateEvents(InAudioEventNode root)
        {
            Events = new Dictionary<int, InAudioEventNode>();
            BuildEventSet(root, Events);
        }

        private void BuildEventSet(InAudioEventNode audioevent, Dictionary<int, InAudioEventNode> events)
        {
            if (audioevent.IsRootOrFolder)
            {
                events[audioevent._guid] = audioevent;
            }
            for (int i = 0; i < audioevent._children.Count; ++i)
            {
                BuildEventSet(audioevent._children[i], events);
            }
        }
    }

}