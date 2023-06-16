using UnityEngine;

namespace Box2.Reflection.DecouplingModule
{
    public class SoundModule:IModule
    {
        public void Init()
        {
            Debug.Log("SoundModule Init");
        }

        public void Destroy()
        {
            Debug.Log("SoundModule Destroy");
        }
    }
}