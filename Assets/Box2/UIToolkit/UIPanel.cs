using UnityEngine;
using UnityEngine.UIElements;

namespace Box2.UIToolkit
{
    public class UIPanel:MonoBehaviour
    {
        private UIDocument _document;

        private Button _button;

        private Label _label;
        private Label _label2;
        private int _count = 0;
        // Start is called before the first frame update
        void Start()
        {
            TryGetComponent(out _document);
            var root = _document.rootVisualElement;//获取根的元素
            _button = root.Q<Button>("button");//绑定
            _label = root.Q<Label>("label");//绑定的是name字段
            _button.RegisterCallback<ClickEvent>(ev =>BtnClick());
            
            _label2 = root.Q<GroupBox>("groupbox3").Q<Label>("label2");//树状查找，似乎得一层层找下去，多了可以遍历
        }

        void BtnClick()
        {
            _count++;
            _label.text = $"{_count}";
            _label2.text = _label.text;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
