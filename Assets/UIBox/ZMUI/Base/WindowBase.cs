using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;


namespace UIBox.ZMUI
{
    public class WindowBase:WindowBehaviour
    {

        private List<Button> m_BtnList = new();
        private List<Toggle> m_ToggleList = new();
        private List<InputField> m_InputFieldList = new();
        #region 生命周期
        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override void OnShow()
        {
            base.OnShow();
        }

        public override void OnHide()
        {
            base.OnHide();
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

        public override void SetVisible(bool isVisible)
        {
            //会造成UI重绘
            GameObject.SetActive(isVisible);
            Visible = isVisible;
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