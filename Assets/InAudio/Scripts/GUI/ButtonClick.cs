using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace InAudioSystem
{
    [AddComponentMenu("InAudio/GUI/Button Click")]
    public class ButtonClick : MonoBehaviour, IPointerClickHandler
    {
        public InAudioEvent Event;

        public void OnPointerClick(PointerEventData eventData)
        {
            InAudio.PostEvent(gameObject, Event);
        }
    }
}