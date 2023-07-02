using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Box2.UIBlockQuestion
{
    /// <summary>
    /// Input.mousePosition == eventData.MousePosition
    /// _transform.anchoredPosition 没有z，transform.localPosition有z
    /// </summary>
    public class UIItem : MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler
    {
        private Image _image; 
        private Vector3 _startPoint;
        private RectTransform _transform;
        public Canvas canvas;

        private bool _isDrag = false;
        // Start is called before the first frame update
        void Start()
        {
            TryGetComponent(out _transform);
            TryGetComponent(out _image);
        }

        // Update is called once per frame
        void Update()
        {
            if (_isDrag)
            {
                //这里应该是转到UI坐标系
                //转到UI坐标系，最重要得是以Canvas的坐标为参照的相机，这个相机才是渲染UI的相机。所以如何要转到这个坐标系，就要使用这个相机
                //eventData.eventCamera就是这个东西
                RectTransformUtility.ScreenPointToLocalPointInRectangle( _transform.parent as RectTransform,
                    Input.mousePosition, canvas.worldCamera,out var localPosition);
                _transform.anchoredPosition = localPosition;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("drag");
            if (_image != null)
            {
                Debug.Log("正在拖拽");
                _startPoint = _transform.anchoredPosition;
                _isDrag = true;
                _image.raycastTarget = false;
            }
            Debug.Log(_image);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDrag = false;
            _image.raycastTarget = true;
        }
        /// <summary>
        /// 如果没有IDrag的话，OnBeginDrag也不会启用
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            /*_transform.anchoredPosition += eventData.delta;*/
            
            
            print($"封装的位置：{Input.mousePosition}    " + eventData.position);
            
            
        }
    }
}
