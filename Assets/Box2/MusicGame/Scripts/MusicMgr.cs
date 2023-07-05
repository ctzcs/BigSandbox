using System;
using Box2.MusicGame.Resources.Scripts;
using Unity.VisualScripting;
using UnityEngine;

namespace Box2.MusicGame.Scripts
{
    public class MusicMgr : CustomMono
    {
        private static MusicMgr _instance;
        public static MusicMgr Instance=>_instance;
        /// <summary>
        /// 歌曲播放了的时间
        /// </summary>
        private float _realElapsedTime;

        private bool _isPlay;
        private bool _isStart;
    
        /// <summary>
        /// 播放音乐的source
        /// </summary>
        private AudioSource _audioSource;

        private CheckBox _checkBox;

        /// <summary>
        /// 现在播放的歌曲
        /// </summary>
        [SerializeField]
        private Song _song;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            _instance = this;
        }

        public void Init()
        {
            _audioSource = GetComponent<AudioSource>();
            _song = new Song("testSong",UnityEngine.Resources.Load<AudioClip>("Music/Action 2"), 
                new Fragment[]
            {
                new Fragment(0,1,0,UnityEngine.Resources.Load<GameObject>("Prefabs/Triangle"),UnityEngine.Resources.Load<AnimationClip>("Anim/TriFly"),false),
                new Fragment(1,3,1,UnityEngine.Resources.Load<GameObject>("Prefabs/Triangle"),UnityEngine.Resources.Load<AnimationClip>("Anim/TriFly"),false)
            }
                );
            _checkBox = new CheckBox(0.05f,0.1f,0.2f,0.25f);


        }
        /// <summary>
        /// 暂停音乐
        /// </summary>
        void PauseMusic()
        {
            _audioSource.Pause();
            _isPlay = false;
        }

        /// <summary>
        /// 播放音乐
        /// </summary>
        void PlayMusic()
        {
            if (!_isStart && _audioSource.clip == null)
            {
                _audioSource.clip = _song.Clip;
                _isStart = true;
            }

            if (!_isPlay)
            {
                _audioSource.Play();
                _isPlay = true;
            }
        }

        /// <summary>
        /// 停止音乐播放
        /// </summary>
        void StopMusic()
        {
            _audioSource.Stop();
            _isPlay = false;
        }

        void Update()
        {
            //如果歌曲的播放时间大于片段的miss时间点，仍没有检查过，自动将检查点改为miss，并且设置检查点为已经检查。
            //1. 如果输入了按键，检查和歌曲中最近的未检查片段距离，如果在范围内，检查输入按键的精准度，如果不在范围内，则设置该按键为无效。
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
            }
        }

        private void CheckValidate()
        {
            
        }
    }
    
}
