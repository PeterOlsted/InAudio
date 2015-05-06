using UnityEngine;
using UnityEngine.EventSystems;

namespace InAudioSystem
{
    [AddComponentMenu("InAudio/GUI/Mouse Over")]
    public class MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Event Play")]
        public InAudioEvent ActivateOnMouseOver;

        [Space(5)]
        [Header("Node Play")]
        public bool stopNodeOnExit = true;
        public InAudioNode playNodeWhileMouseOver;

        //Method to enable the Enable toggle in the inspector
        private void OnEnable()
        { }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enabled)
            {
                if (ActivateOnMouseOver != null)
                    InAudio.PostEvent(gameObject, ActivateOnMouseOver);
                if (playNodeWhileMouseOver != null)
                    InAudio.Play(gameObject, playNodeWhileMouseOver);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (enabled)
            {
                if (stopNodeOnExit)
                    InAudio.Stop(gameObject, playNodeWhileMouseOver);
            }
        }
    }

}