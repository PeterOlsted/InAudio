namespace InAudioSystem
{
    [System.Serializable]
    public class InSplineConnection
    {
        public InSplineNode NodeA;
        public InSplineNode NodeB;

        public InSplineConnection(InSplineNode nodeA, InSplineNode nodeB)
        {
            NodeA = nodeA;
            NodeB = nodeB;
        }

        public bool IsValid()
        {
            if (NodeA == null || NodeB == null)
                return false;
            return true;
        }
    }

}

