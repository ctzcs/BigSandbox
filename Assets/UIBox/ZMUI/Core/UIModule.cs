using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UIBox.ZMUI
{
    public class UIModule
    {
        private static UIModule m_Instance;

        private static UIModule Instance { get { return m_Instance ??= new UIModule(); } }
        
        private Camera m_UICamera;
        private Transform m_UIRoot;

        /// <summary>
        /// 所有窗口的Dic
        /// </summary>
        private Dictionary<string, WindowBase> m_AllWindowDic = new();
        /// <summary>
        /// 所有窗口的列表
        /// </summary>
        private List<WindowBase> m_AllWindowList = new();
        /// <summary>
        /// 所有可见的窗口
        /// </summary>
        private List<WindowBase> m_VisibleWindowList = new();

        private Queue<WindowBase> m_WindowQueue = new();
        private bool m_StartPopQueueWindowStatus = false;
        
        public void Initialize()
        {
            m_UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
            m_UIRoot = GameObject.Find("UIRoot").transform;
        }


        #region 窗口管理

        

        

        /// <summary>
        /// 弹出窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T PopUpWindow<T>() where T:WindowBase,new()
        {
            Type type = typeof(T);
            string windowName = type.Name;
            var window = GetWindow(windowName);
            if (window is not null)
            {
                return ShowWindow(window) as T;
            }

            T t = new T();
            return InitializedWindow(windowName, t) as T;
        }

        public WindowBase PopUpWindow(WindowBase windowBase)
        {
            Type type = windowBase.GetType();
            string windowName = type.Name;
            var window = GetWindow(windowName);
            if (window is not null)
            {
                return ShowWindow(window);
            }
            return InitializedWindow(windowName, windowBase);
        }
        /// <summary>
        /// 初始化窗口
        /// </summary>
        /// <param name="wdnName"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        private WindowBase InitializedWindow(string wdnName,WindowBase window)
        {
            GameObject newWindow = Object.Instantiate(Resources.Load<GameObject>($"Window/{wdnName}"));
            if (newWindow is not null)
            {
                newWindow.transform.SetParent(m_UIRoot);
                newWindow.transform.localScale = Vector3.one;
                newWindow.transform.localPosition =Vector3.zero;
                newWindow.transform.rotation = Quaternion.identity;

                window.GameObject = newWindow;
                window.Transform = newWindow.transform;
                window.Name = wdnName;
                window.Canvas = newWindow.GetComponent<Canvas>();
                window.Canvas.worldCamera = m_UICamera;
                window.Transform.SetAsLastSibling();
                
                window.OnAwake();
                window.SetVisible(true);
                window.OnShow();

                RectTransform rectTransform = newWindow.GetComponent<RectTransform>();
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;
                
                m_AllWindowDic.Add(wdnName,window);
                m_VisibleWindowList.Add(window);
                return window;
            }
            Debug.LogError($"窗口{wdnName}初始化失败");
            return default;
        }

        /// <summary>
        /// 已经弹出过的窗口再弹出
        /// </summary>
        /// <param name="windowName"></param>
        private WindowBase ShowWindow(string windowName)
        {
            if (m_AllWindowDic.TryGetValue(windowName,out var window))
            {
                return ShowWindow(window);
            }
            else
            {
                Debug.Log($"{windowName} 窗口不存在，调用PopUpWindow进行首次弹出");
            }
            return default;
        }

        /// <summary>
        /// 弹出窗口
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        private WindowBase ShowWindow(WindowBase window)
        {
            if (window is null)
            {
                Debug.Log("弹出时传入window为空");
                return default;
            }
            if (window.GameObject is not null && window.Visible == false)
            {
                m_VisibleWindowList.Add(window);
                window.Transform.SetAsLastSibling();
                window.SetVisible(true);
                window.OnShow();
            }
            return window;
        }

        private WindowBase GetWindow(string winName)
        {
            return m_AllWindowDic.GetValueOrDefault(winName);
        }
        
        #endregion

        #region 堆栈系统

        public void EnqueueWindow<T>(Action<WindowBase> popCallback = null) where T : WindowBase, new()
        {
            T windowBase = new T();
            windowBase.PopWindowListener = popCallback;
            m_WindowQueue.Enqueue(windowBase);
        }

        public void DequeueWindow()
        {
            if (m_StartPopQueueWindowStatus)return;
            m_StartPopQueueWindowStatus = true;

            if (m_WindowQueue.Count > 0)
            {
                WindowBase windowBase = m_WindowQueue.Dequeue();
                WindowBase popWindow = PopUpWindow(windowBase);
                popWindow.PopWindowListener = windowBase.PopWindowListener;
                popWindow.PopQueue = true;
                popWindow.PopWindowListener?.Invoke(popWindow);
                popWindow.PopWindowListener = null;
                
            }
            else
            {
                m_StartPopQueueWindowStatus = false;
                return;
            }
        }

        

        #endregion
    }
    
    
}
