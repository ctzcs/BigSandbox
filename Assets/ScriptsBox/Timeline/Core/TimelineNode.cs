using System;
using System.Collections.Generic;

namespace GameTimeline
{
    ///每个节点都有一些发生的事情
    public class TimelineNode
    {
        private readonly Func<float, bool> _method;
        private float _timeElapsed;
        ///该节点导致的事情，放到这里
        public List<TimelineNode> next;

        public TimelineNode(Func<float, bool> method)
        {
            _timeElapsed = 0;
            _method = method;
            next = new List<TimelineNode>();
        }

        public bool Update(float delta)
        {
            bool done = _method == null || _method(_timeElapsed) ;
            _timeElapsed += delta;
            return done;
        }

        public void AddNext(TimelineNode nextNode)
        {
            if (nextNode != null) next.Add(nextNode);
        }

        public void AddNext(List<TimelineNode> nextNode)
        {
            if (nextNode == null || nextNode.Count <= 0) return;
            next.AddRange(nextNode);
        }

        /// <summary>
        /// 和AddNext不同，this.next.add(next[0]), next[0].next.add(next[1])的模式
        /// 所以他们是按照播放队列加入了，并不是说列表里的都是这之后同时播放
        /// </summary>
        /// <param name="nextNodes"></param>
        /// <param name="finalNode">最后加入的那个节点</param>
        public void AddNextInQueue(List<TimelineNode> nextNodes, out TimelineNode finalNode)
        {
            int index = 0;
            finalNode = this;
            while (index < nextNodes.Count)
            {
                finalNode.AddNext(nextNodes[index]);
                finalNode = nextNodes[index];
                index++;
            }
        }

        public static TimelineNode Nothing => new TimelineNode(_ => true);

        public static TimelineNode WaitSec(float sec) => new TimelineNode(e => e >= sec);

        /// <summary>
        /// 仅仅执行一下一些事，为了穿插在时间线上，不需要等待
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TimelineNode Action(Action action) => new TimelineNode(_ =>
        {
            action?.Invoke();
            return true;
        });

        public static TimelineNode ArrangeInOrder(params TimelineNode[] nodes)
        {
            if (nodes.Length <= 0) return Nothing;
            TimelineNode res = null;
            TimelineNode c = null;
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] == null) continue;
                if (res == null)
                {
                    res = nodes[i];
                    c = res;
                }
                else
                {
                    c.AddNext(nodes[i]);
                    c = nodes[i];
                }
                
            }
            return res;
        }
    }
}