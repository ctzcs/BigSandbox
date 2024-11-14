using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace AlicizaFramework
{
    public class EventPublisher
    {
        private EventDelegateData[] _eventContainer;
        private int _maxEventCode;

        /// <summary>
        /// 游戏总事件大概预估一下 避免频繁扩容GC
        /// </summary>
        /// <param name="initialSize"></param>
        public EventPublisher(int initialSize = 128)
        {
            _eventContainer = ArrayPool<EventDelegateData>.Shared.Rent(initialSize);
            _maxEventCode = initialSize;
        }

        internal void Sub(int eventCode, Delegate handler)
        {
            if (eventCode >= _maxEventCode)
            {
                ExpandArray(eventCode);
            }

            if (_eventContainer[eventCode].IsEmpty)
            {
                _eventContainer[eventCode] = new EventDelegateData(eventCode);
            }

            _eventContainer[eventCode].AddHandler(handler);
        }

        internal void UnSub(int eventCode, Delegate handler)
        {
            if (eventCode < _maxEventCode && !_eventContainer[eventCode].IsEmpty)
            {
                _eventContainer[eventCode].RemoveHandler(handler);
            }
        }

        #region 事件分发接口

        internal void Send(int eventCode)
        {
            if (eventCode < _maxEventCode && !_eventContainer[eventCode].IsEmpty)
            {
                _eventContainer[eventCode].Callback();
            }
        }

        internal void Send<TArg1>(int eventCode, TArg1 arg1)
        {
            if (eventCode < _maxEventCode && !_eventContainer[eventCode].IsEmpty)
            {
                _eventContainer[eventCode].Callback(arg1);
            }
        }

        internal void Send<TArg1, TArg2>(int eventCode, TArg1 arg1, TArg2 arg2)
        {
            if (eventCode < _maxEventCode && !_eventContainer[eventCode].IsEmpty)
            {
                _eventContainer[eventCode].Callback(arg1, arg2);
            }
        }

        internal void Send<TArg1, TArg2, TArg3>(int eventCode, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            if (eventCode < _maxEventCode && !_eventContainer[eventCode].IsEmpty)
            {
                _eventContainer[eventCode].Callback(arg1, arg2, arg3);
            }
        }

        internal void Send<TArg1, TArg2, TArg3, TArg4>(int eventCode, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            if (eventCode < _maxEventCode && !_eventContainer[eventCode].IsEmpty)
            {
                _eventContainer[eventCode].Callback(arg1, arg2, arg3, arg4);
            }
        }

        internal void Send<TArg1, TArg2, TArg3, TArg4, TArg5>(int eventCode, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            if (eventCode < _maxEventCode && !_eventContainer[eventCode].IsEmpty)
            {
                _eventContainer[eventCode].Callback(arg1, arg2, arg3, arg4, arg5);
            }
        }

        internal void Send<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(int eventCode, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            if (eventCode < _maxEventCode && !_eventContainer[eventCode].IsEmpty)
            {
                _eventContainer[eventCode].Callback(arg1, arg2, arg3, arg4, arg5, arg6);
            }
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExpandArray(int requiredSize)
        {
            int newSize = Math.Max(_maxEventCode * 2, requiredSize + 1);
            var newArray = ArrayPool<EventDelegateData>.Shared.Rent(newSize);
            Array.Copy(_eventContainer, newArray, _maxEventCode);
            ArrayPool<EventDelegateData>.Shared.Return(_eventContainer);
            _eventContainer = newArray;
            _maxEventCode = newSize;
        }
    }
}