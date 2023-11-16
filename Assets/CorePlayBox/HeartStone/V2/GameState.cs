using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityParticleSystem;

namespace CorePlayBox.HeartStone.V2
{
    public class GameState
    {
        public Player nowPlayer;
        /// <summary>
        /// 队列
        /// </summary>
        public Queue<Command> commands;
        /// <summary>
        /// 牌桌
        /// </summary>
        public CardTable cardTable;
    }
}