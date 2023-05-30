using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

namespace Box1.SparseSet
{
    /// <summary>
    /// 直接在这里写明测试结果
    /// 100w数据
    /// |             |稀疏集    |List    |Dic   |
    /// |遍历         |18.4ms  |18.3ms  |31.4ms|
    /// |移除9w连续数据|1.8ms   |2574.3ms|5.3ms |
    /// |清空数据     |0.0367ms|0.556ms |2.57ms|
    /// 可以看出：
    /// 1. 遍历上list和稀疏集由于是数组形式遍历，所以速度差不多，dic也不慢毕竟数据有100w。注意，这里基本上在数据超过10w的时候稀疏集遍历速度才会逐渐超过list
    /// 2. 连续移除数据，list由于每次都会产生位移的原因，所以肯定是很慢的，这里list的移除其实可以优化的，主要比较一下原生性能吧。但是也能看出稀疏集的移除性能是很优秀的。
    /// 3. 清空数据 由于稀疏集只是进行了计数置为0的操作，所以速度肯定是最快的，其他两个还进行了遍历处理，所以肯定慢。
    /// 4. 在查询的时候，也是字典的五倍
    /// 改进思考：由于稀疏集sparse占据的空间会随着数值增大而无限增大，这样浪费了很多空间，而在ECS中，我们进行随机查询的次数并不会达到这样的数量级，所以其实可以将sparse换成类似dic的映射关系的数据结构。或者我觉得直接用dic也可以。
    /// </summary>
    public class SparseSetMain : MonoBehaviour
    {
        private const int testNums = 20000;
        private List<Entity> _entities = new List<Entity>(testNums);
        SparseSet<Entity> ss = new SparseSet<Entity>(testNums, testNums);
        private Dictionary<int, Entity> dic = new Dictionary<int, Entity>(testNums);

        private Stopwatch _stopwatch;
        // Start is called before the first frame update
        void Start()
        {
            _stopwatch = new Stopwatch();
            long l = 0;
            long s = 0;
            long d = 0;
            for (int i = 0; i < testNums; i++)
            {
                int key = i;
                Entity entity = new Entity(key);
                _stopwatch.Reset();
                _stopwatch.Start();
                _entities.Add(entity);
                _stopwatch.Stop();
                l += _stopwatch.ElapsedTicks;
                _stopwatch.Reset();
                _stopwatch.Start();
                ss.Add(entity);
                _stopwatch.Stop();
                s += _stopwatch.ElapsedTicks;
                
                _stopwatch.Reset();
                _stopwatch.Start();
                dic.Add(key,entity);
                _stopwatch.Stop();
                d += _stopwatch.ElapsedTicks;

            }
            Debug.Log("list:" +l);
            Debug.Log("s:" + s);
            Debug.Log("dic:" +d);
            Debug.Log("Traverse S:"+TraverseSparseSet(ss)+" List:" +TraverseSList(_entities) + " Dic:" + TraverseDic(dic));
            Debug.Log("Contains S:"+ContainsSparse(ss,100,1000) + " Dic:"+ ContainsDic(dic,100,1000));
            Debug.Log("Remove S:"+RemoveSparseSet(ss,1000,10000)+" List:" +RemoveList(_entities,1000,10000) +" Dic:" + RemoveDic(dic,1000,10000));
            Debug.Log(ss.Contains(500));
            Debug.Log("Clear S:"+ClearSparseSet(ss)+" List:" +ClearList(_entities) + " Dic:"+ClearDic(dic));
   
        }

        long TraverseDic(Dictionary<int,Entity> dic)
        {
            _stopwatch.Reset();
            _stopwatch.Start();
            int sum = 0;
            foreach (var e in dic)
            {
                sum += e.Value.id;
            }
            _stopwatch.Stop();
            return _stopwatch.ElapsedTicks;
        }
        long RemoveDic(Dictionary<int,Entity> d,int start,int end)
        {
            _stopwatch.Reset();
            _stopwatch.Start();
            for (int i = start; i < end; i++)
            {
                dic.Remove(i);
            }
            _stopwatch.Stop();
            return _stopwatch.ElapsedTicks;
        }
        long ContainsDic(Dictionary<int,Entity> d,int start,int end)
        {
            _stopwatch.Reset();
            _stopwatch.Start();
            for (int i = start; i < end; i++)
            {
                d.ContainsKey(i);
            }
            _stopwatch.Stop();
            return _stopwatch.ElapsedTicks;
        }
        long ClearDic(Dictionary<int,Entity> d)
        {
            _stopwatch.Reset();
            _stopwatch.Start();
            d.Clear();
            _stopwatch.Stop();
            return _stopwatch.ElapsedTicks;
        }

        long TraverseSparseSet(SparseSet<Entity> s)
        {
            _stopwatch.Reset();
            _stopwatch.Start();
            int sum = 0;
            /*foreach (var v in s)
            {
                sum += v.id;
            }*/
            //枚举器到底有啥问题,速度起码慢三倍
            var d = s.Density;
            for (int i = 0; i < s.Count; i++)
            {
                sum += d[i].id;
            }
            _stopwatch.Stop();
            return _stopwatch.ElapsedTicks;
        }

        long ContainsSparse(SparseSet<Entity> s,int start,int end)
        {
            _stopwatch.Reset();
            _stopwatch.Start();
            for (int i = start; i < end; i++)
            {
                s.Contains(i);
            }
            _stopwatch.Stop();
            return _stopwatch.ElapsedTicks;
        }

        long RemoveSparseSet(SparseSet<Entity> s,int start,int end)
        {
            _stopwatch.Reset();
            _stopwatch.Start();
            for (int i = start; i < end; i++)
            {
                s.Remove(i);
            }
            _stopwatch.Stop();
            return _stopwatch.ElapsedTicks;
        }

        long ClearSparseSet(SparseSet<Entity> s)
        {
            _stopwatch.Reset();
            _stopwatch.Start();
            s.Clear();
            _stopwatch.Stop();
            return _stopwatch.ElapsedTicks;
        }
        
        long TraverseSList(List<Entity> e)
        {
            _stopwatch.Reset();
            _stopwatch.Start();
            int sum = 0;
            /*foreach (var v in e)
            {
                sum += v.id;
            }*/

            int count = e.Count;
            for (int i = 0; i < count; i++)
            {
                sum += e[i].id;
            }
            _stopwatch.Stop();
            return _stopwatch.ElapsedTicks;
        }
        long RemoveList(List<Entity> e,int start,int end)
        {
            _stopwatch.Reset();
            _stopwatch.Start();
            for (int i = start; i < end; i++)
            {
                e.RemoveAt(i);
            }
            _stopwatch.Stop();
            return _stopwatch.ElapsedTicks;
        }
        
        long ClearList(List<Entity> e)
        {
            _stopwatch.Reset();
            _stopwatch.Start();
            e.Clear();
            _stopwatch.Stop();
            return _stopwatch.ElapsedTicks;
        }
        
    }
}
