using UnityEditor;

namespace Box2.MusicGame.Scripts
{
    public class Score
    {
        private int _totalScore;
        public int TotalScore => _totalScore;

        private int _perfect = 3;
        private int _good = 2;
        private int _normal = 1;
        private int _miss = 0;
        public void AddScore(EScore eScore)
        {
            switch (eScore)
            {
               case EScore.Perfect:
                   _totalScore += _perfect;
                   break;
               case EScore.Good:
                   _totalScore += _good;
                   break;
               case  EScore.Normal:
                   _totalScore += _normal;
                   break;
               case EScore.Miss:
                   _totalScore += _miss;
                   break;
               default:
                   return;
            }
        }
        public void ShowScore(EScore eScore)
        {
            //调用导演，展示分数
            //导演应该是在什么地方，显示什么图片
        }
    }

    public enum EScore
    {
        Perfect,
        Good,
        Normal,
        Miss
    }
}
