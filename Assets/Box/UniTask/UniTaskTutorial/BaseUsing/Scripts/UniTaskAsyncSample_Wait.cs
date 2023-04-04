using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UniTaskTutorial.BaseUsing.Scripts
{
    public class UniTaskAsyncSample_Wait
    {
        public async UniTask<int> WaitYield(PlayerLoopTiming loopTiming)
        {
            await UniTask.Yield(loopTiming);
            return 0;
        }
        
        public async UniTask<int> WaitNextFrame()
        {
            await UniTask.NextFrame();
            return Time.frameCount;
        }
        
        public async UniTask<int> WaitEndOfFrame(MonoBehaviour behaviour)
        {
            await UniTask.WaitForEndOfFrame(behaviour);
            return Time.frameCount;
        }
    }
}