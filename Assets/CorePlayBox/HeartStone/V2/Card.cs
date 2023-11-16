using System.Collections.Generic;
using UnityEngine;

namespace CorePlayBox.HeartStone.V2
{
    /// <summary>
    /// 卡牌信息
    /// </summary>
    public class Card
    {
        public int baseAtk;
        public int baseDef;
        public int baseCost;

        public int GlobalAtk;
        public int GlobalDef;
        public int GlobalCost;

        public int BattleAtk;
        public int BattleDef;
        public int BattleCost;
        private List<KeyWord> _keyWords;
        
    }
}
