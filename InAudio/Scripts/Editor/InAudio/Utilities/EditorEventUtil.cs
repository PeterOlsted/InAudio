using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public static class EditorEventUtil
    {
        /// <summary>
        /// This only exists to make it easy to track where events get used. Placing a breakpoint or a console.writeline here makes it easy to track.
        /// </summary>
        /// <param name="currentEvent"></param>
        public static void UseEvent(this Event currentEvent)
        {
            //Debug.Log(currentEvent.type);
            currentEvent.Use();
        }
    }


}
