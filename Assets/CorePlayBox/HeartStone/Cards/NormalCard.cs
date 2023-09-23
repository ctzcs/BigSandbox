using UnityEngine;

namespace CorePlayBox.HeartStone
{
    public class NormalCard : Card
    {

        // Start is called before the first frame update
        public override void Excute(ICardPlayer arg1,ICard arg2)
        {
            Debug.Log($"{arg1.Name} 打出了 {arg2.Name} : {arg2.CardSO.Effect}");
            if (!TimelineMgr.I.Dic.ContainsKey(this.gameObject))
            {
                Timeline timeline = new Timeline(player.Name, " ", Listen, Core);
                TimelineMgr.I.Dic.TryAdd(this.gameObject, timeline);
            }
        }
        void Listen(ICardPlayer arg1, ICard arg2)
        {
            if (arg2.CardSO.Name == "火")
            {
                arg2.Excute(arg1,arg2);
                TimelineMgr.I.Dic.TryGetValue(this.gameObject, out var l);
                l?.Destroy();
                TimelineMgr.I.Dic.Remove(this.gameObject);
            }
        }
    }
}
