using UnityEngine;

namespace ScriptsBox.DI_VContainer
{
    /// <summary>
    /// 负责随时可以调用的功能
    /// </summary>
    public class HelloWorldService
    {
        public void Hello()
        {
            Debug.Log("HelloWorld");
        }

        public void Battle()
        {
            Debug.Log("战斗");
        }
    }
}