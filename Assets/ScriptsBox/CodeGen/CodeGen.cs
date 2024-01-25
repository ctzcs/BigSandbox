using System;
using UnityEngine;


    public class CodeGen : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
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
        public bool Val1;
        public string Val2;
        //目标是生成这个
        public void DoTransfer(ITransferAction action)
        {
            action.Transfer(Val0);
            action.Transfer(Val1);
            action.Transfer(Val2);
        }
    }

    
    
    public interface ITransferAction
    {
        public void Transfer<T>(T val);
    }

    public class JsonWriter : ITransferAction
    {
        public void Transfer<T>(T val)
        {
            
        }
    }
    
    public class JsonReader : ITransferAction
    {
        public void Transfer<T>(T val)
        {
            
        }
    }
