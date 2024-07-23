using System;
using System.Collections.Generic;

namespace GameTimeline
{
    /// <summary>
    /// 一组TimelineNode，为了能有个结束
    /// </summary>
    public class Timeline
    {
        /// <summary>
        /// node本身是树，在Timeline里按顺序使用这个树
        /// </summary>
        public List<TimelineNode> Nodes;
        /// <summary>
        /// Timeline应该是个链表了，所以只有一个next，他不是一个树状
        /// </summary>
        public Timeline Next;
        public Action OnDone;

        private List<TimelineNode> _runningNodes = new List<TimelineNode>();
        private int _runningIndex = 0;

        public Timeline(List<TimelineNode> n)
        {
            Nodes = n ?? new List<TimelineNode>();
            _runningIndex = 0;
            _runningNodes.Clear();
            GatherRunningNode();
            Next = null;
        }

        public Timeline(TimelineNode n)
        {
            Nodes = new List<TimelineNode>();
            if (n != null) Nodes.Add(n);
            _runningIndex = 0;
            _runningNodes.Clear();
            GatherRunningNode();
            Next = null;
        }
        
        public Timeline(Func<float, bool> method)
        {
            TimelineNode n = new TimelineNode(method);
            Nodes = new List<TimelineNode> {n};
            _runningIndex = 0;
            _runningNodes.Clear();
            GatherRunningNode();
            Next =null;
        }

        /// <summary>
        /// 这个Timeline的Next的最后一环
        /// </summary>
        /// <returns></returns>
        public Timeline FinalOne()
        {
            Timeline res = this;
            while (res.Next != null)
                res = res.Next;
            return res;
        }

        /// <summary>
        /// 根据_runningIndex获得_runningNode，并且推进_runningIndex
        /// </summary>
        private void GatherRunningNode()
        {
            if (_runningIndex >= 0 && _runningIndex < Nodes.Count)
            {
                _runningNodes.Add(Nodes[_runningIndex]);
                _runningIndex++;
            }
        }

        public bool Update(float delta)
        {
            int index = 0;
            while (index < _runningNodes.Count)
            {
                if (_runningNodes[index] == null)
                {
                    _runningNodes.RemoveAt(index);
                    continue;
                }
                if (_runningNodes[index].Update(delta))
                {
                    foreach (TimelineNode timelineNode in _runningNodes[index].next)
                        _runningNodes.Add(timelineNode);
                    _runningNodes.RemoveAt(index);
                }
                else index++;
            }

            if (_runningNodes.Count <= 0)
            {
                GatherRunningNode();
                return (_runningNodes.Count <= 0);
            }

            return false;
        }

        public static Timeline InQueue(params Timeline[] timelines)
        {
            if (timelines.Length <= 0) return new Timeline(_ => true);
            int index = 0;
            while (index < timelines.Length - 1)
            {
                timelines[index].Next = timelines[index + 1];
                index++;
            }

            return timelines[0];
        }
    }

    

    
}

