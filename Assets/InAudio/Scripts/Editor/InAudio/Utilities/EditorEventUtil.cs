using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public static class EditorEventUtil
    {
        public static void UseEvent()
        {
            Event.current.Use();
        }

        public static void UseEvent(this Event currentEvent)
        {
            currentEvent.Use();
        }
    }


}
