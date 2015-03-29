using System;
using UnityEngine;

//[Obsolete("No longer needed, InAudio now uses the variable name")]
[AttributeUsage(AttributeTargets.Field)]
public class InEventName : PropertyAttribute
{
    public string EventType;
    public bool FoldedOut = false;

    public InEventName(string eventType)
    {
        EventType = eventType;
    }

}