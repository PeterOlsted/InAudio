using UnityEngine;
using System.Collections;

namespace InAudioSystem
{
    public class ButtonEvent : MonoBehaviour
    {
        public InAudioEvent Event;

        // Use this for initialization
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }

        public void PlayEvent()
        {
            InAudio.PostEvent(gameObject, Event);
        }

        
    }

}