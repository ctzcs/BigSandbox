using UnityEngine;

namespace Box2.MusicGame.Resources.Scripts
{
    public class Song : CustomMono
    {
        [SerializeField]
        private AudioClip clip;
        public AudioClip Clip => clip;

        public float[] fragmentPoint;
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
}
