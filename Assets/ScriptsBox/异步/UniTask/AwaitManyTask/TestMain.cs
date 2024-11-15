using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ScriptsBox.异步.AwaitManyTask
{
    public class TestMain : MonoBehaviour
    {
        
        // Start is called before the first frame update
        void Start()
        {
            TaskList taskList = new TaskList();
            taskList.Add(new WaitForSecond());
            taskList.Add(new NewFrame());
            taskList.ExecuteTask().Forget();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }

    public class WaitForSecond : ITaskOperation
    {
        public async Cysharp.Threading.Tasks.UniTask Execute()
        {
            Debug.Log(Time.time);
            await UniTask.Delay(1000);
            Debug.Log(Time.time);
        }
    }
    
    public class NewFrame:ITaskOperation
    {
        public async UniTask Execute()
        {
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            Debug.Log(Time.time);
        }
    }



    public interface ITaskOperation
    {
        UniTask Execute();
    }

    public class TaskList
    {
        private List<ITaskOperation> _operations;

        public TaskList()
        {
            _operations = new();
        
        }

        public void Add<T>(T task) where T:ITaskOperation
        {
            _operations.Add(task);
        }

        public void Remove<T>(T task) where T : ITaskOperation
        {
            _operations.Remove(task);
        }

        public async Cysharp.Threading.Tasks.UniTask ExecuteTask()
        {
            foreach (var task in _operations)
            {
                await task.Execute();
            }
        }
    }
}