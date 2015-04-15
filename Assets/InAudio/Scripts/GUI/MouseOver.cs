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
        
        

        public void OnPointerEnter(PointerEventData eventData)
        {
            InAudio.PostEvent(gameObject, ActivateOnMouseOver);
            InAudio.Play(gameObject, playNodeWhileMouseOver);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (stopNodeOnExit)
                InAudio.Stop(gameObject, playNodeWhileMouseOver);
        }
    }

}