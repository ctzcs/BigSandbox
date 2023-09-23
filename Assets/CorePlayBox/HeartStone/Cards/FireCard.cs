

using UnityEngine;

namespace CorePlayBox.HeartStone
{
    public class FireCard : Card
    {
        
        // Start is called before the first frame update
        public override void Excute(ICardPlayer arg1,ICard arg2)
        {
            Debug.Log($"{arg1.Name} 打出了 {arg2.Name}: {arg2.CardSO.Effect}");
        }
        
    }
}
