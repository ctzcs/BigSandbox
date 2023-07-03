using Box2.MusicGame.Resources.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

namespace Box2.MusicGame.Scripts
{
    public class Song : CustomMono
    {
        [SerializeField]
        private AudioClip clip;
        public AudioClip Clip => clip;
        /// <summary>
        /// 上一次检查成功的片段
        /// </summary>
        public int lastCheckedFragment = -1;
        /// <summary>
        /// 序列点，用来判断点击的时间是否正确的
        /// </summary>
        public Fragment[] fragments;
        /// <summary>
        /// 事件发射，动画开始播放的时间点
        /// </summary>
        public float[] fireEventTime;
        // Start is called before the first frame update
        void Start()
        {
            //应该有一个一首歌中出生的时间点 
            //多久使用什么路径到达某个地方
            //=>这部分可以由动画录制 动画可以是统一的，只要改变动画的播放时长，就能得到不同的速度。
            //=>只要改变播放的开始时间，就是能对齐动画
            //这时候触发按键检查
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }

    public class Fragment
    {
        /// <summary>
        /// 检查的时间点
        /// </summary>
        private float _fragmentPoint;
        /// <summary>
        /// 检查的序列号
        /// </summary>
        private int _index;
        /// <summary>
        /// 有没有被检查过,如果有一个按键判断成功了就代表这个按键被检查过了
        /// </summary>
        private bool _isChecked;

        public float FragmentPoint => _fragmentPoint;
        public int Index => _index;

        public bool IsChecked
        {
            get => _isChecked;
            set => _isChecked = value;
        }

        public Fragment(float fragmentPoint,int index,bool isChecked = false)
        {
            _fragmentPoint = fragmentPoint;
            _index = index;
            _isChecked = isChecked;
        }

    }
}
