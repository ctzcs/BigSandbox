using System;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Box1.PhysicSimulate.FakePhysics
{
    public class Simulator : MonoBehaviour
    {
        private List<Ctrl> _bodys = new List<Ctrl>(20);

        private readonly float _fixedDeltaTime = 0.05f;

        private float _elaTime = 0;
        private static Simulator _i;
        private bool _openSimulate = false;

        public static Simulator I
        {
            get
            {
                if (_i == null)
                {
                    _i = FindObjectOfType<Simulator>();
                    if (_i == null)
                    {
                        var obj = new GameObject();
                        obj.name = "Simulator";
                        _i = obj.AddComponent<Simulator>();
                        DontDestroyOnLoad(obj);
                    }
                    
                }
                return _i;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _openSimulate = !_openSimulate;
            }
            if (_openSimulate)
            {
                MyUpdate();
            }
            
        }

        void MyUpdate()
        {
            _elaTime += Time.deltaTime;
            while (_elaTime > _fixedDeltaTime)
            {
                DoStep();
                _elaTime -= _fixedDeltaTime;
            }
        }

        void DoStep()
        {
            for (int i = 0; i < _bodys.Count; i++)
            {
                for (int j = i + 1; j < _bodys.Count; j++)
                {
                    var a = _bodys[i].shape;
                    var at = _bodys[i].trans;
                    var b = _bodys[j].shape;
                    var bt = _bodys[j].trans;
                    while (Shape.CheckCol(a,at,b,bt))
                    {
                        Shape.PhysicsEffect(a,at,b,bt,1,_fixedDeltaTime);
                        Shape.PhysicsEffect(b,bt,a,at,1,_fixedDeltaTime);
                    }
                }
            }
        }

        public void BodyRegister(Ctrl ctrl)
        {
            _bodys.Add(ctrl);
        }

        public void BodyDeRegister(Ctrl ctrl)
        {
            _bodys.Remove(ctrl);
        }
        
    }

}
