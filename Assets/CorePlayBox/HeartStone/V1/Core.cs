using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

namespace CorePlayBox.HeartStone
{
    public class Core : MonoBehaviour
    {
        public event UnityAction GameStart;
        public event UnityAction FirstHand;
        
        public event UnityAction<ICardPlayer> OneTurnStart;
        public event UnityAction<ICardPlayer, ICard> CardEmit;
        public event UnityAction<ICardPlayer> OneTurnEnd;
        
        public ICardPlayer[] players;
        [SerializeField]
        [Header("NowTurn")]
        private int _turn = 0;
        [Header("PlayerCount")]
        private readonly int _playerCount = 2;
        [Header("一回合时间")]
        private float _oneTurnTime = 60f;

        private float _elapsedTime = 0f;

        #region private function

        void CreatePlayer(int num)
        {
            players = new ICardPlayer[num];
            var obj = Resources.Load<GameObject>("CardPlayer");
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = Object.Instantiate(obj).GetComponent<ICardPlayer>();
                players[i].Name = string.Format("Player" + i);
                players[i].Core = this;
            }
        }

        #endregion
        void Start()
        {
            
            CreatePlayer(_playerCount);
            OnGameStart();
            OnFirstHand();
        }

        // Update is called once per frame
        void Update()
        {
            if (_elapsedTime == 0)
            {
                OnOneTurnStart(players[_turn]);
            }

            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > _oneTurnTime)
            {
                OnOneTurnEnd(players[_turn]);
                _elapsedTime = 0;
            }
            
        }

        protected void OnGameStart()
        {
            GameStart?.Invoke();
            Debug.Log("游戏开始");
        }

        protected void OnFirstHand()
        {
            FirstHand?.Invoke();
            _turn = Random.Range(0, _playerCount);
            Debug.Log($"掷出筛子，获得{_turn},由{players[_turn].Name}");
        }

        protected void OnOneTurnStart(ICardPlayer arg0)
        {
            if (_turn < _playerCount - 1 )
            {
                _turn++;
            }
            else
            {
                _turn = 0;
            }
            Debug.Log("开始计时！");
            OneTurnStart?.Invoke(arg0);
            arg0.IsTurn = true;
            
        }


        protected void OnOneTurnEnd(ICardPlayer arg0)
        {
            
            Debug.Log("计时结束，退出该玩家回合");
            OneTurnEnd?.Invoke(arg0);
            arg0.IsTurn = false;
        }

        public void OnCardEmit(ICardPlayer arg0, ICard arg1)
        {
            CardEmit?.Invoke(arg0, arg1);
            arg1.Excute(arg0,arg1);
        }
    }
}
