using UnityEngine;

namespace Box2.MusicGame.Scripts
{
    /// <summary>
    /// 检测盒子，用来判断是否在规定的精确度范围内
    /// </summary>
    public class CheckBox
    {
        
        #region 检测的精度
        private float _perfectRange;
        private float _goodRange;
        private float _normalRange;
        private float _missRange;
        #endregion

        public CheckBox(float perfectRange,float goodRange,float normalRange,float missRange)
        {
            this._perfectRange = perfectRange;
            this._goodRange = goodRange;
            this._normalRange = normalRange;
            this._missRange = missRange;
        }

        void Check(Fragment lastFragment,Fragment nextFragment, float nowTime)
        {
            Fragment checkFragment =
                Mathf.Abs(nowTime - lastFragment.FragmentPoint) < Mathf.Abs(nowTime - nextFragment.FragmentPoint) ? 
                    lastFragment : nextFragment;
            if (checkFragment.IsChecked)
            {
                return;
            }

            float interval = Mathf.Abs(nowTime - checkFragment.FragmentPoint);
            if (interval <= _perfectRange)
            {
            }
            else if(interval <= _goodRange)
            {
            }
            else if (interval <= _normalRange)
            {
                
            }
            else if (interval <= _missRange)
            {
                
            }
            checkFragment.IsChecked = true;
        }
        
    }
}
