using System;
using UnityEngine;


    public class CodeGen : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            MyStruct @struct = new MyStruct(1,true,"泥蒿");
            @struct.DoTransfer(new JsonWriter());
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }

    //将代码自动转换
    //1. 反射找到特性，有一定的运行时性能消耗
    public class AutoTransferAttribute : Attribute
    {
    
    }
    
    public class AutoTransferFieldAttribute : Attribute
    {
    
    }
    
    //2. Roslyn提供了语法树的生成点，CodeGenerator类似一个cpp里的宏展开

    [AutoTransfer]
    public partial struct MyStruct
    {
        [AutoTransferField]
        public int Val0;
        [AutoTransferField]
        public bool Val1;
        public string Val2;

        public MyStruct(int Val0,bool Val1,string Val2)
        {
            this.Val0 = Val0;
            this.Val1 = Val1;
            this.Val2 = Val2;
        }
        /*//目标是生成这个
        public void DoTransfer(ITransferAction action)
        {
            action.Transfer(Val0);
            action.Transfer(Val1);
            action.Transfer(Val2);
        }*/
    }

    
    
    public interface ITransferAction
    {
        public void Transfer<T>(T val);
    }

    public class JsonWriter : ITransferAction
    {
        public void Transfer<T>(T val)
        {
            if (val is int @int)
            {
                Debug.Log($"写入{@int}");
            }

            if (val is bool @bool)
            {
                Debug.Log($"写入{@bool}");
            }

            if (val is string @string)
            {
                Debug.Log($"写入{@string}");
            }
        }
    }
    
    public class JsonReader : ITransferAction
    {
        public void Transfer<T>(T val)
        {
            Debug.Log($"读出{val}");
        }
    }
