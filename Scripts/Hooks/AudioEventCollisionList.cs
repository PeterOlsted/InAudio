namespace InAudioSystem
{
    [System.Serializable]
    public class AudioEventCollisionList
    {
        //React to trigger and/or collison
        public bool CollisionReaction = true;

        public bool TriggerReaction = true;

        public int Layers = -1; //0 = Nothing. -1 is "everything"

        public InAudioEvent EventsEnter;

        public InAudioEvent EventsExit;
    }
}