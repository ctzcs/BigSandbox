using UnityEngine;

namespace AboutAction
{
    public class Client : MonoBehaviour
    {
        readonly ReturnAction _returnAction = new ReturnAction();

        private void Update()
        {
            //这里本来是使用命令模式触发，传入，但是这里直接写死了,展示的是接收到传入的命令之后的状态。
            var action = _returnAction.TakeAction(KeyCode.A);
            action?.Invoke();
        }
    }
}