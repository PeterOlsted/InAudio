using System.Collections.Generic;

namespace InAudioSystem
{


    public class ArrayPool<T> where T : class
    {
        // Use this for initialization
        private List<List<T[]>> freeObjects;

        private int initialArrayCount;

        public int InitialArrayCount
        {
            get { return initialArrayCount; }
            private set { initialArrayCount = value; }
        }

        private int chunkSize;

        public int ChunkSize
        {
            get { return chunkSize; }
            set
            {
                if (value > 0)
                {
                    chunkSize = value;
                }
            }
        }

        private int _initialCountAtEveryIndex;

        public int InitialCountAtEveryIndex
        {
            get { return _initialCountAtEveryIndex; }
            set
            {
                if (value > 0)
                {
                    _initialCountAtEveryIndex = value;
                }
            }
        }

        public ArrayPool(int numOfArrays, int initialCountAtEveryIndex, int chunkSize)
        {
            freeObjects = new List<List<T[]>>();
            this._initialCountAtEveryIndex = initialCountAtEveryIndex;
            this.initialArrayCount = numOfArrays;
            this.chunkSize = chunkSize;

            for (int i = 0; i < numOfArrays; i++)
            {
                freeObjects.Add(new List<T[]>());
                for (int j = 0; j < initialCountAtEveryIndex; j++)
                {
                    freeObjects[i].Add(new T[i]);
                }
            }

            //ReserveExtra(initialCountAtEveryIndex);
        }

        public void Release(T[] obj)
        {
            freeObjects[obj.Length].Add(obj);
        }

        private void ReserveExtra(int arraySize)
        {
            for (int i = freeObjects.Count; i < arraySize; ++i)
            {
                var newList = new List<T[]>();
                freeObjects.Add(newList); //Create a new list of arrays for this ListIndex
                ReserveExtraAt(i);
            }
        }

        private void ReserveExtraAt(int index)
        {
            freeObjects.Add(new List<T[]>());
            for (int i = 0; i < _initialCountAtEveryIndex; ++i)
            {
                var newArray = new T[index];
                freeObjects[index].Add(newArray);
            }
        }

        public T[] GetArray(int arraySize)
        {
            //arraySize -= 1;
            if (freeObjects.Count < arraySize)
            {
                ReserveExtra(arraySize);
            }

            var arrayList = freeObjects[arraySize];

            if (arrayList.Count == 0)
            {
                ReserveExtraAt(arraySize);
            }

            T[] obj = arrayList[arrayList.Count - 1];
            arrayList.RemoveAt(arrayList.Count - 1);

            return obj;
        }

    }
}