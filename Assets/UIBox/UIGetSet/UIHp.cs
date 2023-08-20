using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIGetSet
{
    public class UIHp : MonoBehaviour
    {
        private Text _text;
        public PlayerData playerData;
        public Player player;
        private void Start()
        {
            _text = GetComponent<Text>();
            playerData = player.Data;
            //主要是UI上的text不是引用数据，所以得唤醒复制。
            _text.text = playerData.hp.ToString();
            playerData.OnValueChange += SetText;

        }

        void SetText()
        {
            _text.text = playerData.hp.ToString();
        }

    }
}
