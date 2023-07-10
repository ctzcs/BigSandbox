using System;
using Box2.MusicGame.Resources.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

        /// <summary>
        /// 播放音乐的source
        /// </summary>
        private AudioSource _audioSource;

        private CheckBox _checkBox;
        /// <summary>
        /// 应该检查的片段
        /// </summary>
        private int _lastCheckedFragment = 0;
        /// <summary>
        /// 现在播放的歌曲
        /// </summary>
        [SerializeField]
        private Song _song;

        /// <summary>
        /// 暂时都让三角形表演了，实际上应该实例化一个
        /// </summary>
        public GameObject tri;
        private static readonly int TriFly = Animator.StringToHash("TriFly");
        
        public GameObject sfx;

        public Text text;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            _instance = this;
        }

        void Start()
        {
            Init();
        }
        public void Init()
        {
            _audioSource = GetComponent<AudioSource>();
            _song = new Song("testSong",UnityEngine.Resources.Load<AudioClip>("Music/Action 2"), 
                new Fragment[]
            {
                new Fragment(0,2,UnityEngine.Resources.Load<GameObject>("Prefabs/Triangle"),UnityEngine.Resources.Load<AnimationClip>("Anim/TriFly"),false),
                new Fragment(2,3f,UnityEngine.Resources.Load<GameObject>("Prefabs/Triangle"),UnityEngine.Resources.Load<AnimationClip>("Anim/TriFly"),false),
                new Fragment(3f,4f,UnityEngine.Resources.Load<GameObject>("Prefabs/Triangle"),UnityEngine.Resources.Load<AnimationClip>("Anim/TriFly"),false),
                new Fragment(4f,5f,UnityEngine.Resources.Load<GameObject>("Prefabs/Triangle"),UnityEngine.Resources.Load<AnimationClip>("Anim/TriFly"),false),
                
                new Fragment(7,9,UnityEngine.Resources.Load<GameObject>("Prefabs/Triangle"),UnityEngine.Resources.Load<AnimationClip>("Anim/TriFly"),false),
                new Fragment(9,10f,UnityEngine.Resources.Load<GameObject>("Prefabs/Triangle"),UnityEngine.Resources.Load<AnimationClip>("Anim/TriFly"),false),
                new Fragment(10f,11f,UnityEngine.Resources.Load<GameObject>("Prefabs/Triangle"),UnityEngine.Resources.Load<AnimationClip>("Anim/TriFly"),false),
                new Fragment(11f,12f,UnityEngine.Resources.Load<GameObject>("Prefabs/Triangle"),UnityEngine.Resources.Load<AnimationClip>("Anim/TriFly"),false),
                
                new Fragment(13,13.8f,UnityEngine.Resources.Load<GameObject>("Prefabs/Triangle"),UnityEngine.Resources.Load<AnimationClip>("Anim/TriFly"),false),
                new Fragment(13.8f,14.6f,UnityEngine.Resources.Load<GameObject>("Prefabs/Triangle"),UnityEngine.Resources.Load<AnimationClip>("Anim/TriFly"),false),
                new Fragment(14.6f,15.4f,UnityEngine.Resources.Load<GameObject>("Prefabs/Triangle"),UnityEngine.Resources.Load<AnimationClip>("Anim/TriFly"),false),
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
        public void PlayMusic()
        {
            if (_audioSource.clip == null)
            {
                _audioSource.clip = _song.Clip;
                
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
            if (_lastCheckedFragment >= _song.fragments.Length)
            {
                //其实应该将帧更新注销
                return;
            }
            if (_isPlay)
            {
                //如果歌曲的播放时间大于片段的miss时间点，仍没有检查过，自动将检查点改为miss，并且设置检查点为已经检查。
                //1. 如果输入了按键，检查和歌曲中最近的未检查片段距离，如果在范围内，检查输入按键的精准度，如果不在范围内，则设置该按键为无效。
                Fragment nowFragment = _song.fragments[_lastCheckedFragment];
                //这里有个bug，就是如果一个动画还没结束，另一个动画时间到了，这时候会卡住另一个动画播放时间
                //TODO 这里要思考一个多动画播放的问题.可能应该创建物体去播放动画，而不是让这一个东西去播放动画
                if ( !nowFragment.hasPlayed && _realElapsedTime > nowFragment.fireEventTime )
                {
                    //让导演安排动画表演
                    Animator anim = tri.GetComponent<Animator>();
                    anim.speed = nowFragment.speed;
                    anim.SetTrigger(TriFly);
                    nowFragment.hasPlayed = true;
                    Debug.Log("Fire" + _realElapsedTime);
                    /*var state = anim.GetCurrentAnimatorStateInfo(0);
                    if (Mathf.Approximately(state.normalizedTime,1))
                    {
                        anim.Play("Idle");
                    }*/

                }
                
                
                if (_realElapsedTime > nowFragment.FragmentPoint + _checkBox.MissRange)
                {
                    _lastCheckedFragment++;
                    nowFragment.IsChecked = true;
                    //TODO 播放miss标签
                    Debug.Log("miss" + _realElapsedTime);
                    MusicMgr.Instance.ShowScore("Miss");
                    return;
                }
                
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    if (CheckValidate())
                    {
                        _lastCheckedFragment++;
                        return;
                    }
                }
                _realElapsedTime += Time.deltaTime;
            }
        }

        private bool CheckValidate()
        {
            Fragment[] fragments = _song.fragments;
            Fragment lastF = fragments[_lastCheckedFragment];
            int next = _lastCheckedFragment + 1;
            Fragment nextF = next < fragments.Length ? fragments[next] : null;
            if (nextF != null)
            {
               return _checkBox.Check(lastF,nextF,_realElapsedTime);
            }
            else
            {
                return _checkBox.SingleBeatCheck(lastF, _realElapsedTime);
            }
        }


        #region 音效

        public AudioClip[] clips;
        

        public void PlaySfx(AudioClip clip)
        {
            sfx.GetComponent<AudioSource>().PlayOneShot(clip);
        }

        #endregion

        #region UI

        public void ShowScore(string  score)
        {
            text.enabled = true;
            text.text = score;
            Invoke(nameof(Hide),0.5f);
            
        }
        void Hide()
        {
            text.enabled = false;
        }
        #endregion
    }
    
}
