using System.Collections.Generic;

namespace Box1.TClassOrTFunc
{
    public class Entity
    {
        public int id;

        public Entity(int id)
        {
            this.id = id;
        }
    }

    //大概是转换T为entity，然后才能加载字段，压入栈顶返回
    //这个问题只在有约束条件的时候出现，所以最终的答案是，box只在value类型出现的时候起作用，如果box的是引用类型，没啥效果，忽略就行了
    public class SomeClass<T> where T :Entity
    {
        private List<Entity> _entities = new List<Entity>();
        public void Init(int nums)
        {
            _entities.Capacity = nums;
            for (int i = 0; i < nums; i++)
            {
                _entities.Add(new Entity(i));
            }
        }
        public int Add(int sum,T entity)
        {
            sum += entity.id;
            return sum;
        }

        public int Add<TK>(int sum,TK entity) where TK : Entity
        {
            sum += entity.id;
            return sum;
        }

        public int Get<TK>(TK value) where TK:Entity
        {
            return value.id;
        }
        public Entity[] Entities => _entities.ToArray();
    }
}
