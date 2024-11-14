using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AlicizaFramework
{
    /// <summary>
    /// 游戏事件数据类。
    /// </summary>
    internal struct EventDelegateData
    {
        private readonly int _eventType;
        private Delegate[] _handlers;
        private int _handlerCount;
        private Delegate[] _addList;
        private int _addCount;
        private Delegate[] _deleteList;
        private int _deleteCount;
        private bool _isExecute;
        private bool _dirty;

        // 使用对象池来管理委托数组
        private static readonly ArrayPool<Delegate> DelegateArrayPool = ArrayPool<Delegate>.Shared;

        public bool IsEmpty => _handlerCount == 0;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="eventType">事件类型。</param>
        internal EventDelegateData(int eventType)
        {
            _eventType = eventType;
            _handlers = DelegateArrayPool.Rent(16); // 预分配16个委托的空间
            _handlerCount = 0;
            _addList = DelegateArrayPool.Rent(4); // 预分配4个
            _addCount = 0;
            _deleteList = DelegateArrayPool.Rent(4); // 预分配4个
            _deleteCount = 0;
            _isExecute = false;
            _dirty = false;
        }

        /// <summary>
        /// 添加注册委托。
        /// </summary>
        /// <param name="handler">事件处理回调。</param>
        /// <returns>是否添加回调成功。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool AddHandler(Delegate handler)
        {
            if (Array.IndexOf(_handlers, handler, 0, _handlerCount) >= 0)
            {
                Debug.LogWarning("Repeated Add Handler");
                return false;
            }

            if (_isExecute)
            {
                EnsureCapacity(ref _addList, ref _addCount);
                _addList[_addCount++] = handler;
                _dirty = true;
            }
            else
            {
                EnsureCapacity(ref _handlers, ref _handlerCount);
                _handlers[_handlerCount++] = handler;
            }

            return true;
        }

        /// <summary>
        /// 移除反注册委托。
        /// </summary>
        /// <param name="handler">事件处理回调。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveHandler(Delegate handler)
        {
            if (_isExecute)
            {
                EnsureCapacity(ref _deleteList, ref _deleteCount);
                _deleteList[_deleteCount++] = handler;
                _dirty = true;
            }
            else
            {
                int index = Array.IndexOf(_handlers, handler, 0, _handlerCount);
                if (index >= 0)
                {
                    _handlers[index] = _handlers[--_handlerCount];
                    _handlers[_handlerCount] = null; // 避免内存泄漏
                }
                else
                {
                    Debug.LogWarning(string.Format("Delete handle failed, not exist, EventId: {0}", _eventType));
                }
            }
        }

        /// <summary>
        /// 确保数组容量足够，必要时扩容。
        /// </summary>
        /// <param name="array">数组引用。</param>
        /// <param name="count">当前元素数量。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(ref Delegate[] array, ref int count)
        {
            if (count >= array.Length)
            {
                var newArray = DelegateArrayPool.Rent(array.Length * 2);
                Array.Copy(array, newArray, array.Length);
                DelegateArrayPool.Return(array, clearArray: true);
                array = newArray;
            }
        }

        /// <summary>
        /// 检测脏数据修正。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckModify()
        {
            _isExecute = false;
            if (_dirty)
            {
                for (int i = 0; i < _addCount; i++)
                {
                    EnsureCapacity(ref _handlers, ref _handlerCount);
                    _handlers[_handlerCount++] = _addList[i];
                }
                _addCount = 0;

                for (int i = 0; i < _deleteCount; i++)
                {
                    int index = Array.IndexOf(_handlers, _deleteList[i], 0, _handlerCount);
                    if (index >= 0)
                    {
                        _handlers[index] = _handlers[--_handlerCount];
                        _handlers[_handlerCount] = null;
                    }
                }
                _deleteCount = 0;
                _dirty = false;
            }
        }

        /// <summary>
        /// 回调调用。
        /// </summary>
        public void Callback()
        {
            _isExecute = true;
            for (var i = 0; i < _handlerCount; i++)
            {
                if (_handlers[i] is Action action)
                {
                    action();
                }
            }

            CheckModify();
        }

        /// <summary>
        /// 带参数回调调用。
        /// </summary>
        public void Callback<TArg1>(TArg1 arg1)
        {
            _isExecute = true;
            for (var i = 0; i < _handlerCount; i++)
            {
                if (_handlers[i] is Action<TArg1> action)
                {
                    action(arg1);
                }
            }

            CheckModify();
        }

        /// <summary>
        /// 带参数回调调用。
        /// </summary>
        public void Callback<TArg1, TArg2>(TArg1 arg1, TArg2 arg2)
        {
            _isExecute = true;
            for (var i = 0; i < _handlerCount; i++)
            {
                if (_handlers[i] is Action<TArg1, TArg2> action)
                {
                    action(arg1, arg2);
                }
            }

            CheckModify();
        }

        /// <summary>
        /// 带参数回调调用。
        /// </summary>
        public void Callback<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            _isExecute = true;
            for (var i = 0; i < _handlerCount; i++)
            {
                if (_handlers[i] is Action<TArg1, TArg2, TArg3> action)
                {
                    action(arg1, arg2, arg3);
                }
            }

            CheckModify();
        }

        /// <summary>
        /// 带参数回调调用。
        /// </summary>
        public void Callback<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            _isExecute = true;
            for (var i = 0; i < _handlerCount; i++)
            {
                if (_handlers[i] is Action<TArg1, TArg2, TArg3, TArg4> action)
                {
                    action(arg1, arg2, arg3, arg4);
                }
            }

            CheckModify();
        }

        /// <summary>
        /// 带参数回调调用。
        /// </summary>
        public void Callback<TArg1, TArg2, TArg3, TArg4, TArg5>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            _isExecute = true;
            for (var i = 0; i < _handlerCount; i++)
            {
                if (_handlers[i] is Action<TArg1, TArg2, TArg3, TArg4, TArg5> action)
                {
                    action(arg1, arg2, arg3, arg4, arg5);
                }
            }

            CheckModify();
        }

        /// <summary>
        /// 带参数回调调用。
        /// </summary>
        public void Callback<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            _isExecute = true;
            for (var i = 0; i < _handlerCount; i++)
            {
                if (_handlers[i] is Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> action)
                {
                    action(arg1, arg2, arg3, arg4, arg5, arg6);
                }
            }

            CheckModify();
        }
    }
}
