using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.Runtime
{

    public class InAudioComponentPool<T> : MonoBehaviour where T : Behaviour
    {
        private List<T> freeObjects = new List<T>();

        private Vector3 offscreen = new Vector3(-10000, -10000, -10000);

        [Range(0, 128)] public int InitialSize = 10;

        [Range(1, 128)] public int ChunkSize = 10;

        public GameObject Prefab;

        public string Prefix;

        private int maxNumber = 1;

        private List<T> toRelease = new List<T>();

        public Action<T> Initialization;

        public List<T> RawList
        {
            get { return freeObjects; }
        }

        private void Awake()
        {
            if (freeObjects.Count == 0)
                ReserveExtra(InitialSize);
        }

        public void DelayedRelease()
        {
            if (toRelease != null)
            {
                for (int i = 0; i < toRelease.Count; i++)
                {
                    var player = toRelease[i];
                    if (player != null)
                    {
                        player.transform.parent = transform;
                        player.transform.position = offscreen;
                        freeObjects.Add(player);
                    }
                }
                toRelease.Clear();
            }
        }

        public void ImmidiateRelease(T player)
        {
            if (player != null)
            {
                freeObjects.Add(player);
            }
        }

        public void QueueRelease(T player)
        {
            if (player != null)
            {
                toRelease.Add(player);
            }

        }

        public void ReserveExtra(int extra)
        {
            for (int i = 0; i < extra; ++i)
            {
                var go = Object.Instantiate(Prefab, offscreen, Quaternion.identity) as GameObject;
                go.name = Prefix + maxNumber;
                go.transform.parent = transform;
                var comp = go.GetComponent<T>();
                freeObjects.Add(comp);

                if (Initialization != null)
                {
                    Initialization(comp);
                }

                ++maxNumber;
            }
        }

        public T GetObject()
        {
            if (freeObjects.Count == 0)
            {
                ReserveExtra(ChunkSize);
            }

            T player = freeObjects[freeObjects.Count - 1];
            freeObjects.RemoveAt(freeObjects.Count - 1);

            return player;
        }
    }
}