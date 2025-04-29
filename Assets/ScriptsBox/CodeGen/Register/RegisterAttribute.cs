using System;
using UnityEngine;


public class RegisterAttribute:Attribute
{
    public string registerTypeName;

    public RegisterAttribute(string registerTypeName)
    {
        this.registerTypeName = registerTypeName;
    }
    
}

namespace CodeGen_Register
{
    public static class Magic
    {
        public const string BaseSample1 = nameof(BaseSample1);
        
    }

    public class BaseSample1
    {
        
    }

    [Register(Magic.BaseSample1)]
    public class SampleA : BaseSample1
    {
    }


    [Register(Magic.BaseSample1)]
    public class SampleB : BaseSample1
    {
    }
    


    public class BaseSample2 { }
    
    
    [Register("Test")]
    public class SampleA2 : BaseSample2
    {
    }


    [Tooltip("a"),Register("Test")]
    public class SampleB2 : BaseSample2
    {
    }
    
    
    public class Main
    {
        void main()
        {
             var list1 = BaseSample1Helper.Type;
             var list2 = TestHelper.Type;
        }
    }
}
