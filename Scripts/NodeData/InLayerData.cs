using UnityEngine;

namespace InAudioSystem
{
    [System.Serializable]
    public class InLayerData
    {
        public InAudioNode Node;
        public double Position;


#if UNITY_EDITOR
        public Vector2 ScrollPos;
        public float Zoom = 1;
#endif
    }

}

