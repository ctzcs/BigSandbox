using System;
using System.Collections.Generic;
using Box2.MusicGame.Resources.Scripts;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

namespace Box2.MusicGame.Scripts
{
    public sealed class Song
    {
        
        private string _songName;
        
        private AudioClip _clip;
        public AudioClip Clip => _clip;
        /// <summary>
        /// 上一次检查成功的片段
        /// </summary>
        public int lastCheckedFragment = -1;
        /// <summary>
        /// 序列点，用来判断点击的时间是否正确的
        /// </summary>
        public Fragment[] fragments;
        
        // Start is called before the first frame update
        void Start()
        {
            //应该有一个一首歌中出生的时间点 
            //多久使用什么路径到达某个地方
            //=>这部分可以由动画录制 动画可以是统一的，只要改变动画的播放时长，就能得到不同的速度。
            //=>只要改变播放的开始时间，就是能对齐动画
            //这时候触发按键检查
            
        }

        public Song(string songName,AudioClip clip,Fragment[] fragments)
        {
            this._songName = songName;
            this._clip = clip;
            this.fragments = fragments;
        }

        public void SetAnimGameObject(GameObject gameObject)
        {
            
        }


    }

    
    [System.Serializable]
    public class Fragment
    {
        /// <summary>
        /// 播放这段动画的预制体
        /// </summary>
        public GameObject obj;
        /// <summary>
        /// 检查的时间点
        /// </summary>
        private float _fragmentPoint;

        /// <summary>
        /// 有没有被检查过,如果有一个按键判断成功了就代表这个按键被检查过了
        /// </summary>
        private bool _isChecked;
        /// <summary>
        /// 应该播放的动画
        /// </summary>
        public AnimationClip animationClip;

        public bool hasPlayed = false;
        /// <summary>
        /// 动画应该播放的速度
        /// </summary>
        public float speed;
        /// <summary>
        /// 应该播放动画的时间点
        /// </summary>
        public float fireEventTime;
        
        public float FragmentPoint => _fragmentPoint;
        
        
        

        public bool IsChecked
        {
            get => _isChecked;
            set => _isChecked = value;
        }

        public Fragment(float fireEventTime,float fragmentPoint,GameObject gameObject,AnimationClip clip = null,bool isChecked = false)
        {
            this.fireEventTime = fireEventTime;
            _fragmentPoint = fragmentPoint;
            obj = gameObject;
            animationClip = clip;
            _isChecked = isChecked;
            speed = 1/(fragmentPoint - fireEventTime);
            hasPlayed = false;
        }
        
        

    }
}
