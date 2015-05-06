using UnityEngine;

namespace InAudioSystem.Runtime
{
    public class InDebug
    {
        public InDebug()
        {
            
        }

        public bool DoLog { get; set; }

        public void CleanupInstance()
        {
            Debug.LogWarning("InAudio: Could not run cleanup");
        }

        public void UnusedActionType(GameObject controllingObject, AudioEventAction eventData)
        {
            Debug.LogWarning("InAudio: Could not run action of type " + eventData._eventActionType + " on object " + controllingObject.name + " as the actionis not in use by InAudio yet");
        }

        public void MissingActionTarget(GameObject controllingObject, AudioEventAction eventData)
        {
            Debug.LogWarning("InAudio: Could not run action of type " + eventData._eventActionType + " on object " + controllingObject.name + "\nas the actions target is a null reference");
        }

        public void BankUnloadMissing()
        {
            Debug.LogWarning("InAudio: Could not unload bank as the reference was null");
        }

        public void BankLoadMissing()
        {
            Debug.LogWarning("InAudio: Could not load bank as the reference was null");
        }

        public void InstanceMissing(string function)
        {
            Debug.LogWarning("InAudio: No instance of InAudio was found. Is the manager in the scene?\nCalled via "+function);
        }

        public void MissingArgumentsForNode(string functionName, InAudioNode node)
        {
            if (!InAudio.DoesExist)
            {
                InAudioInstanceMissing();
            }
            else if (node == null)
            {
                Debug.LogWarning("InAudio: Missing arguments on " + functionName);
            }
            else
            {
                Debug.LogWarning("InAudio: Missing arguments on " + functionName);
            }
        }

        public void MissingArguments(string functionName, GameObject gameObject, InAudioNode node)
        {
            if (!InAudio.DoesExist)
            {
                InAudioInstanceMissing();
            }
            else if (gameObject == null && node == null)
            {
                Debug.LogWarning("InAudio: Missing arguments on " + functionName);
            } else if (gameObject == null)
            {
                Debug.LogWarning("InAudio: Missing arguments on " + functionName + " playing node " + node.GetName);
            }
            else
            {
                Debug.LogWarning("InAudio: Missing arguments on " + functionName + " on game object " + gameObject.name);
            }
        }

        public void InAudioInstanceMissing(GameObject go = null)
        {
            if (DoLog)
            {
                if (go != null)
                    Debug.LogWarning("InAudio: Could not post event(s) on " + go.name + " as the InAudio was not found");
                else
                    Debug.LogWarning("InAudio: Could not post event(s) as the InAudio was not found");
            }
        }

        public void MissingControllingObject()
        {
            Debug.LogWarning("InAudio: Could not post events as game object was a null reference");
        }

        public void MissingEventList(GameObject controllingObject)
        {
            Debug.LogWarning("InAudio: Could not post event list on " + controllingObject.gameObject + " as the event list was null or list of events was null");
        }

        public void MissingEvent(GameObject controllingObject)
        {
            Debug.LogWarning("InAudio: Could not post event on " + controllingObject.gameObject + " as the event list was null or list of events was null");
        }
    }


}
