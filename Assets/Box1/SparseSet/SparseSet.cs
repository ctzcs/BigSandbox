using System.Collections;
using System.Collections.Generic;

namespace Box1.SparseSet
{
    public class Entity
    {
        public int id;

        public Entity(int i)
        {
            id = i;
        }
    }
    public class SparseSet<T>:ICollection<T> where T: Entity
    {
        private int[] _sparse;
        private T[] _density;
        private int _count;
        private int _capacity;
        private int _maxValue;

        public SparseSet(int capacity,int maxValue)
        {
            _capacity = capacity;
            _maxValue = maxValue;
            //创建容量和稀疏数组的最大值。
            _density = new T[capacity];
            _sparse = new int[maxValue];
            

        }
        
        public void Add(T item)
        {
            int id = item.id;
            if (id > _maxValue)
            {
                return;
            }
            if (_count > _capacity)
            {
                return;
            }

            _density[_count++] = item;
            _sparse[id] = _count;
        }

        public void Clear()
        {
            _count = 0;
        }

        public bool Contains(T item)
        {
            var id = item.id;
            return Contains(id);
        }

        public bool Contains(int id)
        {
            if (id >= _maxValue)
            {
                return false;
            }
            int densityId = _sparse[id];
            if (densityId < _count && _density[densityId].id == id)
            {
                return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(T item)
        {
            int id = item.id;
            return Remove(id);
        }
        public bool Remove(int id)
        {
            int densityId = _sparse[id];
            if (densityId >= _count)
            {
                return false;
            }
            _count--;
            _density[densityId] = _density[_count];
            _sparse[_density[densityId].id] = densityId;
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
            {
                yield return _density[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _count;
        public bool IsReadOnly { get; }
    }
}