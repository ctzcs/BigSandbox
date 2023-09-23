using System.Collections.Generic;
using UnityEngine;

namespace CorePlayBox.HeartStone
{
    public interface ICardPlayer
    {
        string Name { get; set; }
        bool IsTurn { get; set; }
        
        Core Core { set; }
    }
    public class CardPlayer : MonoBehaviour,ICardPlayer
    {
        [SerializeField]
        private string _name;
        [SerializeField]
        private bool _isTurn;

        [SerializeField] private int _cardNum = 2;

        public bool IsTurn
        {
            get => _isTurn;
            set => _isTurn = value;
        }
        public string Name
        {
            get => _name;
            set => _name = value;
        }
        [SerializeField]
        private Core _core;
        public Core Core
        {
            set => _core = value;
        }

        public List<Card> myCard;
        // Start is called before the first frame update
        void Start()
        {
            CreateCard(_cardNum);
        }

        void CreateCard(int num)
        {
            myCard = new List<Card>(num);
            var obj = Resources.Load<GameObject>("Card");
            var fire = Resources.Load<ScriptableObject>("Fire");
            var normal = Resources.Load<ScriptableObject>("Normal");
            

            for (int i = 0; i < num;i++)
            {
                var ooo = Object.Instantiate(obj);
                Card card = i % 2 == 0 ? ooo.AddComponent<NormalCard>() : ooo.AddComponent<FireCard>();
                card.CardSO = i%2 == 0?normal as CardSO:fire as CardSO;
                card.Name = string.Format($"{i}");
                card.Core = _core;
                card.player = this;
                myCard.Add(card);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(!_isTurn)return;
            if (Input.GetKeyUp(KeyCode.K))
            {
                Debug.Log("出牌！");
                int num = Random.Range(0, _cardNum);
                _core.OnCardEmit(this,myCard[num]);
                //TODO 出牌逻辑
            }
        }
        
        
    }
}
