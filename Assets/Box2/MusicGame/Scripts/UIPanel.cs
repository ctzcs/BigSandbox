using System;
using UnityEngine;
using UnityEngine.UI;

namespace Box2.MusicGame.Scripts
{
    public class UIPanel : MonoBehaviour
    {
        public Button playBtn;

        private void Awake()
        {
            playBtn.onClick.AddListener(() =>
            {
                MusicMgr.Instance.PlayMusic();
            });
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
