using System.Collections.Generic;
using UnityEngine;

namespace ScriptsBox.修改list中的结构体可以吗_
{
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
}