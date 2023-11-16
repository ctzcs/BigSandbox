using System.Collections.Generic;

namespace CorePlayBox.HeartStone.V2
{
    /// <summary>
    /// 牌桌，目前牌桌上每个人最多有四个怪物
    /// </summary>
    public class CardTable
    {
        /// <summary>
        /// 战场
        /// </summary>
        public Dictionary<string, List<Card>> battlePlace;
    }
}