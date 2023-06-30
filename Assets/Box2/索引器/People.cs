using UnityEngine;

namespace Box2.索引器
{
    public class People
    {
        public int x;

        public int this[int index]
        {
            get
            {
                return x;
            }
        }

        public People(int a)
        {
            this.x = a;
        }
    }

    public class PeopleArray
    {
        public People[] people;
        public int length;
        public PeopleArray(int length)
        {
            people = new People[length];
            this.length = length;
        }


        public int this[int index]
        {
            get
            {
                if (index < length && index >=0)
                {
                    return people[index].x;
                }
                else
                {
                    return -1;
                }
            }
            set
            {
                if (index < length && index >=0)
                {
                    if (people[index] != null)
                    {
                        people[index].x = value;
                    }
                    else
                    {
                        people[index] = new People(value);
                    }

                }
            }
        }
    }
    
    
}
