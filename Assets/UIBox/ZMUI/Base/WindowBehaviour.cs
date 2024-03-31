
using System;
using UnityEngine;

namespace UIBox.ZMUI
{
    public abstract class WindowBehaviour
    {
        /// <summary>
        /// 窗口物体
        /// </summary>
        public GameObject GameObject { get; set; }
        /// <summary>
        /// 窗口坐标
        /// </summary>
        public Transform Transform { get; set; }
        /// <summary>
        /// 窗口的画布
        /// </summary>
        public Canvas Canvas { get; set; }
        /// <summary>
        /// 窗口名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 窗口是否可视
        /// </summary>
        public bool Visible { get; set; }
        
        public Action<WindowBase> PopWindowListener { get; set; }
        public bool DeQueue { get; set; }
        /// <summary>
        /// 创建时执行一次
        /// </summary>
        public virtual void OnAwake(){}
        /// <summary>
        /// 显示时执行
        /// </summary>
        public virtual void OnShow(){}
        /// <summary>
        /// 更新时执行
        /// </summary>
        public virtual void OnUpdate(){}
        /// <summary>
        /// 隐藏时执行
        /// </summary>
        public virtual void OnHide(){}
        /// <summary>
        /// 销毁时执行
        /// </summary>
        public virtual void OnDestroy(){}
        public virtual void SetVisible(bool isVisible){}
        
    }
}
