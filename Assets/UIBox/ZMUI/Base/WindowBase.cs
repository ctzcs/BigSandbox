using System.Collections.Generic;
using DG.Tweening;
using UIBox.ZMUI.Setting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace UIBox.ZMUI
{
    public class WindowBase:WindowBehaviour
    {
        /// <summary>
        /// 按钮的列表
        /// </summary>
        private List<Button> m_BtnList = new();
        /// <summary>
        /// 多选框的列表
        /// </summary>
        private List<Toggle> m_ToggleList = new();
        /// <summary>
        /// 输入框列表
        /// </summary>
        private List<InputField> m_InputFieldList = new();

        private CanvasGroup m_UIMask;
        private CanvasGroup m_CanvasGroup;
        protected Transform m_UIContent;
        /// <summary>
        /// 是否禁用动画
        /// </summary>
        protected bool m_DisableAnim = false;


        /// <summary>
        /// 初始化基类组件
        /// </summary>
        private void InitializeBaseComponent()
        {
            m_UIMask = Transform.Find("UIMask").GetComponent<CanvasGroup>();
            m_CanvasGroup = Transform.GetComponent<CanvasGroup>();
            m_UIContent = Transform.Find("UIContent").transform;

        }
        #region 生命周期
        public override void OnAwake()
        {
            base.OnAwake();
            InitializeBaseComponent();
        }

        public override void OnShow()
        {
            base.OnShow();
            ShowAnim();
        }

        public override void OnHide()
        {
            base.OnHide();
            HideAnim();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RemoveAllButtonListener();
            RemoveAllToggleListener();
            RemoveAllInputFieldListener();
            m_BtnList.Clear();
            m_ToggleList.Clear();
            m_InputFieldList.Clear();
        }

        

        #endregion

        #region 动画管理

        public void HideWindow()
        {
            HideAnim();
        }
        
        protected virtual void ShowAnim()
        {
            //基础弹窗无动画
            if (Canvas.sortingOrder > 90 && m_DisableAnim == false)
            {
                //动画
                //Mask动画
                m_UIMask.alpha = 0;
                m_UIMask.DOFade(1, 0.2f);
                //缩放动画
                m_UIContent.localScale = Vector3.one * 0.8f;
                m_UIContent.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            }
        }

        protected virtual void HideAnim()
        {
            if (Canvas.sortingOrder > 90 && m_DisableAnim == false)
            {
                m_UIContent.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    UIModule.Instance.HideWindow(Name);
                });
            }
            else
            {
                UIModule.Instance.HideWindow(Name);
            }
        }

        
        public override void SetVisible(bool isVisible)
        {
            //会造成UI重绘
            /*GameObject.SetActive(isVisible);*/
            //通过设置透明度来决定是否可见
            m_CanvasGroup.alpha = isVisible ? 1 : 0;
            m_CanvasGroup.blocksRaycasts = isVisible;
            Visible = isVisible;
        }

        public void SetMaskVisible(bool isVisible)
        {
            //遮罩系统
            if (!UISetting.Instance.SINGMASK_SYSTEM)
            {
                return;
            }
            m_UIMask.alpha = isVisible ? 1 : 0;
        }

        #endregion
        #region 事件管理

        public void AddButtonClickListener(Button btn,UnityAction action)
        {
            if (btn is not null)
            {
                if (!m_BtnList.Contains(btn))
                {
                    m_BtnList.Add(btn);
                }
                //先清空btn上的事件
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(action);
            }
        }

        public void AddToggleClickListener(Toggle toggle,UnityAction<bool,Toggle> action)
        {
            if (toggle is not null)
            {
                if (!m_ToggleList.Contains(toggle))
                {
                    m_ToggleList.Add(toggle);
                }
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener((isChosen)=>{action?.Invoke(isChosen,toggle);});
            }
        }

        public void AddInputFieldListener(InputField inputField,UnityAction<string> changeAction,UnityAction<string> endAction)
        {
            if (inputField is not null)
            {
                if (!m_InputFieldList.Contains(inputField))
                {
                    m_InputFieldList.Add(inputField);
                }
                inputField.onValueChanged.RemoveAllListeners();
                inputField.onValueChanged.AddListener(changeAction);
                inputField.onEndEdit.AddListener(endAction);
            }
        }

        public void RemoveAllButtonListener()
        {
            foreach (var btn in m_BtnList)
            {
                btn.onClick.RemoveAllListeners();
            }
        }

        public void RemoveAllToggleListener()
        {
            foreach (var item in m_ToggleList)
            {
                item.onValueChanged.RemoveAllListeners();
            }
        }

        public void RemoveAllInputFieldListener()
        {
            foreach (var item in m_InputFieldList)
            {
                item.onValueChanged.RemoveAllListeners();
                item.onEndEdit.RemoveAllListeners();
            }
        }

        #endregion
    }
}