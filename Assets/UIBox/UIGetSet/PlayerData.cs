using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIGetSet
{
    public class PlayerData : MonoBehaviour
    {
        public int hp = 0;
        public event Action OnValueChange;
        public void SetHp(int blood)
        {
            hp = blood;
            OnValueChange?.Invoke();
        }
        
    }
}
