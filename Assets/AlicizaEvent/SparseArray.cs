using System.Collections.Generic;

namespace AlicizaFramework
{
    public class SparseArray<T>
    {
        private Dictionary<int, int> keyToIndexMap;
        private T[] valuesArray;
        private int currentIndex;

        public SparseArray(int maxSize)
        {
            keyToIndexMap = new Dictionary<int, int>(maxSize);
            valuesArray = new T[maxSize];
            currentIndex = 0;
        }

        public void Insert(int key, T value)
        {
            if (!keyToIndexMap.ContainsKey(key))
            {
                keyToIndexMap[key] = currentIndex;
                valuesArray[currentIndex] = value;
                currentIndex++;
            }
            else
            {
                valuesArray[keyToIndexMap[key]] = value;
            }
        }

        public T Get(int key)
        {
            if (keyToIndexMap.TryGetValue(key, out int index))
            {
                return valuesArray[index];
            }

            return default; // 或者处理不存在的键
        }
    }
}