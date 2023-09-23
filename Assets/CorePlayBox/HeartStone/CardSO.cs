using UnityEditor;
using UnityEngine;

namespace CorePlayBox.HeartStone
{

    [CreateAssetMenu(fileName = "Card",menuName = "HeartStoneLike/Card")]
    public class CardSO : ScriptableObject
    {
        public int Attack;
        public int Defend;
        public string Effect;
        public string Name;
    }
}
