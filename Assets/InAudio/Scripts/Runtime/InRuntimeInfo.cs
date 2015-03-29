namespace InAudioSystem.Runtime
{
    /// <summary>
    /// This class is a small container used for storing which nodes is being played on a game object
    /// </summary>
    public class RuntimeInfo
    {
        public InAudioNode Node;
        public InPlayer Player;
        public ObjectAudioList PlacedIn;
        public int ListIndex;

        public void Set(InAudioNode node, InPlayer player)
        {
            Node = node;
            Player = player;
        }
    }
}