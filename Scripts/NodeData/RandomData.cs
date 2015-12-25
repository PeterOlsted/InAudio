using System;
using System.Collections.Generic;

namespace InAudioSystem
{
    public class RandomData : InAudioNodeData
    {
        public List<int> weights = new List<int>();

        public int doNotRepeat = 1;

        [NonSerialized] public Queue<int> lastPlayed = new Queue<int>();
    }
}