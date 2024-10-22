using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptsBox.DI_VContainer
{
    public class ClassInspector<T>:MonoBehaviour where T:class
    {
        [ShowInInspector]
        protected T instance;
    }
}