using UnityEngine;
using UnityEngine.EventSystems;

namespace InAudioSystem
{
    [AddComponentMenu("InAudio/GUI/Pointer Click")]
    public class PointerClick : MonoBehaviour, IPointerClickHandler
    {
        public InAudioEvent onClick;

        //[Header("Sound Effect")]
        public InAudioNode onClickSound;

        //Method to enable the Enable toggle in the inspector
        private void OnEnable()
        { }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (enabled)
            {
                InAudio.PostEvent(gameObject, onClick);
                InAudio.Play(gameObject, onClickSound);
            }
        }
    }
}