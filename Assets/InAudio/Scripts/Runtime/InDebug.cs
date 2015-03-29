using UnityEngine;

namespace InAudioSystem.Runtime
{
    public class InDebug
    {
        public bool DoLog = true;

        public System.Action<object> Log;
        public System.Action<object> LogWarning;
        public System.Action<object> LogError;

        public InDebug()
        {
            Log  = o =>
            {
                if(DoLog)
                    Debug.Log(o);
            };
            LogWarning = o =>
            {
                if(DoLog)
                    Debug.LogWarning(o);
            };
            LogError = o =>
            {
                if (DoLog)
                    Debug.LogError(o);
            };
        }

        public void CleanupInstance()
        {
            LogWarning("InAudio: Could not run cleanup");
        }

        public void UnusedActionType(GameObject controllingObject, AudioEventAction eventData)
        {
            LogWarning("InAudio: Could not run action of type " + eventData._eventActionType + " on object " + controllingObject.name + " as the actionis not in use by InAudio yet");
        }

        public void MissingActionTarget(GameObject controllingObject, AudioEventAction eventData)
        {
            LogWarning("InAudio: Could not run action of type " + eventData._eventActionType + " on object " + controllingObject.name + "\nas the actions target is a null reference");
        }

        public void BankUnloadMissing()
        {
            LogWarning("InAudio: Could not unload bank as the reference was null");
        }

        public void BankLoadMissing()
        {
            LogWarning("InAudio: Could not load bank as the reference was null");
        }

        public void InAudioInstanceMissing(GameObject go)
        {
            if (go != null)
                LogWarning("InAudio: Could not post event(s) on " + go.name + " as the InAudio was not found");
            else
                LogWarning("InAudio: Could not post event(s) as the InAudio was not found");
        }

        public void MissingControllingObject()
        {
            LogWarning("InAudio: Could not post events as game object was a null reference");
        }

        public void MissingEventList(GameObject controllingObject)
        {
            LogWarning("InAudio: Could not post event list on " + controllingObject.gameObject + " as the event list was null or list of events was null");
        }

        public void MissingEvent(GameObject controllingObject)
        {
            LogWarning("InAudio: Could not post event on " + controllingObject.gameObject + " as the event list was null or list of events was null");
        }
    }


}
