using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIGetSet
{
    public class Player : MonoBehaviour
    {
        private PlayerData _data;

        public PlayerData Data
        {
            get => _data;
        }

        // Start is called before the first frame update
        void Start()
        {
             _data = GetComponent<PlayerData>();
        }

        private void OnGUI()
        {
            GUI.BeginGroup(new Rect(100,100,200,300));
            if (GUILayout.Button("+",GUILayout.Width(200)))
            {
                
                _data.SetHp(_data.hp + 1);
            }
            
            if (GUILayout.Button("-",GUILayout.Width(200)))
            {
                _data.SetHp(_data.hp - 1);
            }
            
            GUI.EndGroup();
        }
    }
}

