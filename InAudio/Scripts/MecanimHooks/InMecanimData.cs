using System.Collections.Generic;

namespace InAudioSystem.Runtime
{
    [System.Serializable]
    public class MecanimNodeEvent
    {
        public InAudioEvent ToPost;

        public List<InAudioNode> ToPlay;
        public List<InAudioNode> ToStop;
    }

    [System.Serializable]
    public class MecanimParameterList
    {
        public List<MecanimParameter> ParameterList = new List<MecanimParameter>();
    }

    
}