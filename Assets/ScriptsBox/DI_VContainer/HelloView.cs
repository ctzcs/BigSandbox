using UnityEngine;
using UnityEngine.UI;

namespace ScriptsBox.DI_VContainer
{
    public class HelloView:MonoBehaviour
    {
        public Button helloBtn;
     
        //View中注册需要标注[Inject]，同时，需要通过 builder.RegisterComponent 注册(还有另外两种方法)
        
    }
}