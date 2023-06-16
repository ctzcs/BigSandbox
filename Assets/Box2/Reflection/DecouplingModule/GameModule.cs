using UnityEngine;

namespace Box2.Reflection.DecouplingModule
{
    public class GameModule:IModule
    {
        public void Init()
        {
            Debug.Log("GameModule Init");
        }

        public void Destroy()
        {
            Debug.Log("GameModule Destroy");
        }
    }
}