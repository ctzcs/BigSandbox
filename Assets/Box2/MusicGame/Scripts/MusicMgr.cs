using Box2.MusicGame.Resources.Scripts;
using UnityEngine;

namespace Box2.MusicGame.Scripts
{
    public class MusicMgr : CustomMono
    {
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
    
        /// <summary>
        /// 现在播放的歌曲
        /// </summary>
        [SerializeField]
        private Song song;

        public void Init()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        /// <summary>
        /// 暂停音乐
        /// </summary>
        void StopMusic()
        {
            _audioSource.Stop();
        }

        /// <summary>
        /// 播放音乐
        /// </summary>
        void PlayMusic()
        {
            if (!_isStart && _audioSource.clip == null)
            {
                _audioSource.clip = song.Clip;
                _isStart = true;
            }

            if (!_isPlay)
            {
                _audioSource.Play();
                _isPlay = true;
            }
        }

        void Update()
        {
            //如果歌曲的播放时间大于片段的miss时间点，仍没有检查过，自动将检查点改为miss，并且设置检查点为已经检查。
            //1. 如果输入了按键，检查和歌曲中最近的未检查片段距离，如果在范围内，检查输入按键的精准度，如果不在范围内，则设置该按键为无效。
        }
    }
    
}
