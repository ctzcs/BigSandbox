using System.Collections.Generic;
using UnityEngine;


namespace ScriptsBox.修改list中的结构体可以吗_
{
    /// <summary>
    /// 结论是不行，因为List中是通过索引器访问那个对象，然而索引器在访问结构体的过程中会发生复制
    /// 所以得到的是复制的结构体
    /// 而数组中，通过索引器直接访问的
    /// </summary>
    public class ChangStructInList : MonoBehaviour
    {
        private List<MyVector2Int> _vList = new List<MyVector2Int>(1);
        // Start is called before the first frame update
        void Start()
        {
            _vList.Add(new MyVector2Int(0,0));
            Debug.Log(_vList[0].ToString());
            //结构体从来都是整个修改，不会修改其中的成员
            _vList[0] = new MyVector2Int(1,1);
            Debug.Log(_vList[0]);
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }

    public struct MyVector2Int
    {
        public int x;
        public int y;
        public MyVector2Int(int x,int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return string.Format($"{x},{y}");
        }
    }
    
    //CoreLib源码中，_item[]虽然是引用类型，但是_item[index]是值类型，这导致了返回的就是值类型，所以是复制，所以没法去改变结构体里面的某个值
    /*
    public T this[int index]
    {
        get
        {
            // Following trick can reduce the range check by one
            if ((uint)index >= (uint)_size)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException();
            }
            return _items[index];
        }
     
        set
        {
            if ((uint)index >= (uint)_size)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException();
            }
            _items[index] = value;
            _version++;
        }
    }
    */

}