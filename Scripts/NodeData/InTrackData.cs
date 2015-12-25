using System.Collections.Generic;
using InAudioSystem;

namespace InAudioSystem
{

    public class InTrackData : InAudioNodeData
    {
        public float TrackLength = 10f;
        public List<InLayerData> Layers = new List<InLayerData>();
    }

}