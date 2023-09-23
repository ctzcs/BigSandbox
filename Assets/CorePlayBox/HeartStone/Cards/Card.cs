using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Serialization;

namespace CorePlayBox.HeartStone
{
    public interface ICard
    {
        string Name { get; }
        CardSO CardSO { get; set; }
        GameObject GO { get; }
        public void Excute(ICardPlayer p,ICard c);
    }
    
    public class Card : MonoBehaviour,ICard
    {   [SerializeField]
        private CardSO _cardSO;
        public CardSO CardSO
        {
            get => _cardSO;
            set => _cardSO = value;
        }
        

        [SerializeField]
        private string _name;
        public string Name { 
            get => _name;
            set => _name = $"{_cardSO.Name}:{value}";
        }
        [SerializeField]
        private Core _core;
        public Core Core
        {
            get => _core;
            set => _core = value;
        }

        public CardPlayer player;

        public GameObject GO { get=>gameObject; }

        public virtual void Excute(ICardPlayer p, ICard c)
        {
        }
    }
}
