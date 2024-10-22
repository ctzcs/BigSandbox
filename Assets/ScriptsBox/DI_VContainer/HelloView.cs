using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace ScriptsBox.DI_VContainer
{
    public class HelloView:MonoBehaviour
    {
        public Button helloBtn;
     
        //View中注册需要标注[Inject]，同时，需要通过 builder.RegisterComponent 注册(还有另外两种方法)
        
        
        private void Awake() 
        {
            
        }
        
        [Inject]
        public void Constructor(IObjectResolver container)
        {
            Debug.Log("HelloView注册");
        }
    }
}