using System;
using UnityEngine;

namespace Box2.Reflection.CollectAttribute
{
    public class SomeAttribute : Attribute
    {
        public string Name;

        public SomeAttribute(string name)
        {
            this.Name = name;
        }
    }
}
