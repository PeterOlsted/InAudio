using InAudioLeanTween;
using UnityEngine;

namespace InAudioSystem
{

    public class InEventAudioAction : AudioEventAction
    {
        public InAudioNode Node;
        public float Fadetime;
        public LeanTweenType TweenType = LeanTweenType.linear;

        public override Object Target
        {
            get { return Node; }
            set
            {
                if (value is InAudioNode)
                    Node = value as InAudioNode;
            }
        }

        public override string ObjectName
        {
            get
            {
                if (Node != null)
                    return Node.GetName;
                else
                {
                    return "Missing Audio";
                }

            }
        }
    }

}