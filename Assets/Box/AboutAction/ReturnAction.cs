using System;
using Unity.VisualScripting;
using UnityEngine;

namespace AboutAction
{
    /// <summary>
    /// 当我们进入逻辑的时候，能返回一个可以调用的行为么？
    /// 比如我们按不同的按键，会调用不同的行为。
    /// </summary>
    public class ReturnAction
    {
        private Action _onPressedA;
        private Action _onPressB;

        public ReturnAction()
        {
            _onPressedA += OnAPress;
            _onPressB += OnBPress;
        }

        public Action TakeAction(KeyCode code)
        {
            switch (code)
            {
                case KeyCode.A:
                    return _onPressedA;
                case KeyCode.B:
                    return _onPressB;
            }
            return null;
        }

        void OnAPress()
        {
            Debug.Log("APress");
        }

        void OnBPress()
        {
            Debug.Log("BPress");
        }
    }

    
}