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

        public float MissRange => _missRange;
        #endregion

        public CheckBox(float perfectRange,float goodRange,float normalRange,float missRange)
        {
            this._perfectRange = perfectRange;
            this._goodRange = goodRange;
            this._normalRange = normalRange;
            this._missRange = missRange;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastFragment"></param>
        /// <param name="nextFragment"></param>
        /// <param name="nowTime"></param>
        /// <returns>检查成功返回true，无效返回false</returns>
        public bool Check(Fragment lastFragment,Fragment nextFragment, float nowTime)
        {
            Fragment checkFragment =
                nextFragment == null || Mathf.Abs(nowTime - lastFragment.FragmentPoint) < Mathf.Abs(nowTime - nextFragment.FragmentPoint)? 
                    lastFragment : nextFragment;
            
            if (checkFragment.IsChecked)
            {
                return false;
            }

            float interval = Mathf.Abs(nowTime - checkFragment.FragmentPoint);
            if (interval <= _perfectRange)
            {
                Debug.Log("Perfect" + nowTime);
                MusicMgr.Instance.ShowScore("Perfect");
                //狗屎代码，演示用的
                MusicMgr.Instance.PlaySfx(MusicMgr.Instance.clips[0]);
            }
            else if(interval <= _goodRange)
            {
                Debug.Log("Good" + nowTime);
                MusicMgr.Instance.ShowScore("Good");
                MusicMgr.Instance.PlaySfx(MusicMgr.Instance.clips[1]);
            }
            else if (interval <= _normalRange)
            {
                Debug.Log("Normal" + nowTime);
                MusicMgr.Instance.ShowScore("Normal");
                MusicMgr.Instance.PlaySfx(MusicMgr.Instance.clips[2]);
            }
            else if (interval <= _missRange)
            {
                Debug.Log("Miss" + nowTime);
                MusicMgr.Instance.ShowScore("Miss");
                MusicMgr.Instance.PlaySfx(MusicMgr.Instance.clips[3]);
            }
            else
            {
                return false;
            }
            checkFragment.IsChecked = true;
            return true;
        }

        public bool SingleBeatCheck(Fragment fragment,float nowTime)
        {
            if (fragment.IsChecked)
            {
                return false;
            }

            float interval = Mathf.Abs(nowTime - fragment.FragmentPoint);
            if (interval <= _perfectRange)
            {
                Debug.Log("Perfect" + nowTime);
                //狗屎代码，演示用的
                MusicMgr.Instance.PlaySfx(MusicMgr.Instance.clips[0]);
            }
            else if(interval <= _goodRange)
            {
                Debug.Log("Good" + nowTime);
                MusicMgr.Instance.PlaySfx(MusicMgr.Instance.clips[1]);
            }
            else if (interval <= _normalRange)
            {
                Debug.Log("Normal" + nowTime);
                MusicMgr.Instance.PlaySfx(MusicMgr.Instance.clips[2]);
            }
            else if (interval <= _missRange)
            {
                Debug.Log("Miss" + nowTime);
                MusicMgr.Instance.PlaySfx(MusicMgr.Instance.clips[3]);
            }
            else
            {
                return false;
            }
            fragment.IsChecked = true;
            return true;
        }
    }
}
