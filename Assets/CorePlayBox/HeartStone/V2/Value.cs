using System.Collections.Generic;

namespace CorePlayBox.HeartStone.V2
{
    public class Value<T> where T:struct
    {
        public string Name;
        public T value;

        public T GetValue()
        {
            return value;
        }

        public void ChangeValue(T newValue)
        {
        }
    }
}