
using System;

namespace InAudioSystem
{
    public class EnumIndex : Attribute
    {
#if UNITY_EDITOR
        public int Index { get; set; }
        public string Name { get; set; }
        public EnumIndex(int index, string name) { Index = index; Name = name;}
        
#else
        public EnumIndex(int index, string name) { }
#endif
    }
}
