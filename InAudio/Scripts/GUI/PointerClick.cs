using UnityEngine;
using UnityEngine.EventSystems;

namespace InAudioSystem
{
    [AddComponentMenu("InAudio/GUI/Pointer Click")]
    public class PointerClick : MonoBehaviour, IPointerClickHandler
    {
        public InAudioEvent onClick;

        public InAudioNode onClickSound;

        //Method to enable the Enable toggle in the inspector
        private void OnEnable()
        { }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (enabled)
            {
                if(onClick != null)
                    InAudio.PostEvent(gameObject, onClick);
                if(onClickSound != null)
                    InAudio.PlayPersistent(gameObject.transform.position, onClickSound);
            }
            
        }
    }
}