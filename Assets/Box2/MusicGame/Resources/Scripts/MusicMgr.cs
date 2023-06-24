using System.Collections;
using System.Collections.Generic;
using Box2.MusicGame.Resources.Scripts;
using UnityEngine;

public class MusicMgr : CustomMono
{
    /// <summary>
    /// 歌曲播放了的时间
    /// </summary>
    private float _realElapsedTime;

    private bool _isPlay;
    private bool _isStart;

    [SerializeField]
    private Song song;

    private AudioSource _audioSource;

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
}
