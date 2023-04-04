using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UniTaskTutorial.BaseUsing.Scripts
{
    public class UniTaskAsyncSample_Base
    {
        public async UniTask<Object> LoadAsync<T>(string path) where T: Object
        {
            var asyncOperation = Resources.LoadAsync<T>(path);
            return (await asyncOperation);
        }

    }
}