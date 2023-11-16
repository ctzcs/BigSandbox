using System;
using System.Collections.Generic;
using UnityEngine;

namespace CorePlayBox.HeartStone
{
    public class Timeline
    {
        public string Caster { get; }

        public string Target { get; }

        public Core Core { get; }

        public event Action<ICardPlayer, ICard> Emit;
        

        public Timeline(string caster,string target,Action<ICardPlayer,ICard> action,Core core)
        {
            Caster = caster;
            Target = target;
            Emit += action;
            Core = core;
            //创建的时候就将时间节点注册到核心事件上
            //这里因为是卡牌游戏所以这样做
            Core.CardEmit += OnEmit;
        }
        public void OnEmit(ICardPlayer arg1, ICard arg2)
        {
            Emit?.Invoke(arg1, arg2);
        }

        public void Destroy()
        {
            //销毁的时候移除
            Core.CardEmit -= OnEmit;
        }
    }


    public class TimelineMgr
    {
        private static TimelineMgr i = new TimelineMgr();
        public static TimelineMgr I => i;

        public Dictionary<GameObject, Timeline> Dic = new();
    }
}
