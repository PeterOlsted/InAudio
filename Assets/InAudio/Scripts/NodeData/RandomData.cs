using System;
using System.Collections.Generic;

public class RandomData : InAudioNodeData
{
    public List<int> weights = new List<int>();

    public int doNotRepeat;

    [NonSerialized]
    public Queue<int> lastPlayed = new Queue<int>();
}
