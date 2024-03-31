using System;
using System.Collections.Generic;
using UIBox.ZMUI.Setting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UIBox.ZMUI
{
    public class UIModule
    {
        private static UIModule m_Instance;
        public static UIModule Instance { get { return m_Instance ??= new UIModule(); } }
        
        /// <summary>
        /// 目前的Camera
        /// </summary>
        private Camera m_UICamera;
        /// <summary>
        /// UI的根
        /// </summary>
        private Transform m_UIRoot;

        private WindowConfig m_WindowConfig;
        /// <summary>
        /// 所有窗口的Dic
        /// </summary>
        private readonly Dictionary<string, WindowBase> m_WindowDic = new();
        /// <summary>
        /// 所有窗口的列表
        /// </summary>
        private List<WindowBase> m_WindowList = new();
        /// <summary>
        /// 所有可见的窗口
        /// </summary>
        private readonly List<WindowBase> m_VisibleWindowList = new();

        private readonly Queue<WindowBase> m_WindowQueue = new();
        private bool m_StartPopQueueWindowStatus = false;
        
        /// <summary>
        /// 初始化模块
        /// </summary>
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
        public T PopWindow<T>() where T:WindowBase,new()
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

        public WindowBase PopWindow(WindowBase windowBase)
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


        public void HideWindow(string windowName)
        {
            WindowBase windowBase = GetWindow(windowName);
            
        }

        public void HideWindow<T>() where T : WindowBase
        {
            HideWindow(typeof(T).Name);
        }

        private void HideWindow(WindowBase window)
        {
            if (window != null && window.Visible)
            {
                m_VisibleWindowList.Remove(window);
                window.SetVisible(false);//隐藏弹窗物体
                SetWidnowMaskVisible();
                window.OnHide();
            }
            //在出栈的情况下，上一个界面隐藏时，自动打开栈种的下一个界面
            PopNextStackWindow(window);
        }
        
        private void SetWidnowMaskVisible()
        {
            if (!UISetting.Instance.SINGMASK_SYSTEM)
            {
                return;
            }
            WindowBase maxOrderWndBase = null;//最大渲染层级的窗口
            int maxOrder = 0;//最大渲染层级
            int maxIndex = 0;//最大排序下标 在相同父节点下的位置下标
            //1.关闭所有窗口的Mask 设置为不可见
            //2.从所有可见窗口中找到一个层级最大的窗口，把Mask设置为可见
            for (int i = 0; i < m_VisibleWindowList.Count; i++)
            {
                WindowBase window = m_VisibleWindowList[i];
                if (window != null && window.GameObject != null)
                {
                    window.SetMaskVisible(false);
                    if (maxOrderWndBase == null)
                    {
                        maxOrderWndBase = window;
                        maxOrder = window.Canvas.sortingOrder;
                        maxIndex = window.Transform.GetSiblingIndex();
                    }
                    else
                    {
                        //找到最大渲染层级的窗口，拿到它
                        if (maxOrder < window.Canvas.sortingOrder)
                        {
                            maxOrderWndBase = window;
                            maxOrder = window.Canvas.sortingOrder;
                        }
                        //如果两个窗口的渲染层级相同，就找到同节点下最靠下一个物体，优先渲染Mask
                        else if (maxOrder == window.Canvas.sortingOrder && maxIndex < window.Transform.GetSiblingIndex())
                        {
                            maxOrderWndBase = window;
                            maxIndex = window.Transform.GetSiblingIndex();
                        }
                    }
                }
            }
            if (maxOrderWndBase != null)
            {
                maxOrderWndBase.SetMaskVisible(true);
            }
        }
        /// <summary>
        /// 初始化窗口
        /// </summary>
        /// <param name="wdnName"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        private WindowBase InitializedWindow(string wdnName,WindowBase window)
        {
            //实例化预制体
            GameObject newWindow = Object.Instantiate(Resources.Load<GameObject>($"Window/{wdnName}"));
            if (newWindow is not null)
            {
                //设置根节点和相对位置
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
                
                //添加到管理列表中
                m_WindowDic.Add(wdnName,window);
                m_VisibleWindowList.Add(window);
                return window;
            }
            Debug.LogError($"窗口{wdnName}初始化失败");
            return default;
        }

        /// <summary>
        /// 已经弹出过的窗口再弹出
        /// </summary>
        /// <param name="windowName">窗口的名字</param>
        private WindowBase ShowWindow(string windowName)
        {
            if (m_WindowDic.TryGetValue(windowName,out var window)) return ShowWindow(window);
            Debug.Log($"{windowName} 窗口不存在，调用PopWindow进行首次弹出");
            return null;
        }

        /// <summary>
        /// 已经弹出过的窗口再弹出
        /// </summary>
        /// <param name="window">窗口</param>
        /// <returns></returns>
        private WindowBase ShowWindow(WindowBase window)
        {
            if (window == null)
            {
                Debug.Log("弹出时传入window为空");
                return default;
            }
            if (window.GameObject != null && window.Visible == false)
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
            return m_WindowDic.GetValueOrDefault(winName);
        }
        
        public GameObject TempLoadWindow(string wndName)
        {
            GameObject window = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(m_WindowConfig.GetWindowPath(wndName)), m_UIRoot);
            //window.transform.SetParent(mUIRoot);
            window.transform.localScale = Vector3.one;
            window.transform.localPosition = Vector3.zero;
            window.transform.rotation = Quaternion.identity;
            window.name = wndName;
            return window;
        }
        
        
        #endregion

        #region 堆栈系统
        /// <summary>
        /// 进栈一个界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="popCallBack"></param>
        public void PushWindowToStack<T>(Action<WindowBase> popCallBack=null) where T : WindowBase, new()
        {
            T wndBase = new T();
            wndBase.PopWindowListener = popCallBack;
            m_WindowQueue.Enqueue(wndBase);
        }
        /// <summary>
        /// 弹出堆栈中第一个弹窗
        /// </summary>
        public void StartPopFirstStackWindow()
        {
            if (m_StartPopQueueWindowStatus) return;
            m_StartPopQueueWindowStatus = true;//已经开始进行堆栈弹出的流程，
            PopStackWindow();
        }
        /// <summary>
        /// 压入并且弹出堆栈弹窗
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="popCallBack"></param>
        public void PushAndPopStackWindow<T>(Action<WindowBase> popCallBack = null) where T : WindowBase, new()
        {
            PushWindowToStack<T>(popCallBack);
            StartPopFirstStackWindow();
        }
        /// <summary>
        /// 弹出堆栈中的下一个窗口
        /// </summary>
        /// <param name="windowBase"></param>
        private void PopNextStackWindow(WindowBase windowBase)
        {
            if (windowBase != null&&m_StartPopQueueWindowStatus&&windowBase.DeQueue)
            {
                windowBase.DeQueue = false;
                PopStackWindow();
            }
        }
        /// <summary>
        /// 弹出堆栈弹窗
        /// </summary>
        /// <returns></returns>
        public bool PopStackWindow()
        {
            if (m_WindowQueue.Count>0)
            {
                WindowBase window = m_WindowQueue.Dequeue();
                WindowBase popWindow= PopWindow(window);
                popWindow.PopWindowListener = window.PopWindowListener;
                popWindow.DeQueue = true;
                popWindow.PopWindowListener?.Invoke(popWindow);
                popWindow.PopWindowListener = null;
                return true;
            }
            else
            {
                m_StartPopQueueWindowStatus = false;
                return false;
            }
        }
        public void ClearStackWindows()
        {
            m_WindowQueue.Clear();
        }
        
        
        
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
                WindowBase popWindow = PopWindow(windowBase);
                popWindow.PopWindowListener = windowBase.PopWindowListener;
                popWindow.DeQueue = true;
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
