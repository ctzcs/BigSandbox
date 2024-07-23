using System.Collections.Generic;

namespace GameTimeline
{
    public class TimelineManager
    {
        private readonly List<Timeline> _timelines = new List<Timeline>();
        
        public bool Update(float delta)
        {
            if (_timelines.Count <= 0) return true;
            int index = 0;
            while (index < _timelines.Count)
            {
                if (_timelines[index] == null)
                {
                    _timelines.RemoveAt(index);
                    continue;
                }
                if (_timelines[index].Update(delta))
                {
                    if (_timelines[index].Next != null) _timelines.Add(_timelines[index].Next);
                    _timelines[index].OnDone?.Invoke();
                    _timelines.RemoveAt(index);
                }
                else index++;
            }
            return _timelines.Count <= 0;
        }

        public void Add(Timeline timeline)
        {
            _timelines.Add(timeline);
        }

        public void AddByNode(TimelineNode node)
        {
            _timelines.Add(new Timeline(node));
        }
    }
}