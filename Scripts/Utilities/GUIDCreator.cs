using UnityEngine;

namespace InAudioSystem
{
    public static class GUIDCreator
    {
        public static int Create()
        {
            int guid = System.DateTime.Now.Millisecond + Random.Range(-100000,100000);

            guid = ((guid >> 16) ^ guid) * 0x45d9f3b;
            guid = ((guid >> 16) ^ guid) * 0x45d9f3b;
            guid = ((guid >> 16) ^ guid);

            return guid;
        }

        public static int CreateWithAssurance<T>(T root) where T : Object, InITreeNode<T>
        {
            int guid = Create();
            for (int i = 0; i < 10; i++)
            {
                if (!TreeWalker.Any(root, node => node != null && node._ID != guid))
                {
                    return guid;
                }
                else
                {
                    guid = Create();
                }
            }
            Debug.LogWarning("Duplicate ID created");
            return guid;
        }
    }
}