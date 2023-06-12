using System;
using System.Diagnostics;
using UnityEngine;


namespace Box1.TClassOrTFunc
{
    public class TestT : MonoBehaviour
    {
        private SomeClass<Entity> sc = new SomeClass<Entity>();
        private Stopwatch sw = new Stopwatch();
        private void Start()
        {
            sc.Init(1000000);
            sw.Start();
            Sum();
            sw.Stop();
            UnityEngine.Debug.Log(sw.ElapsedTicks);
            sw.Reset();
            sw.Start();
            GeneralSum();
            sw.Stop();
            UnityEngine.Debug.Log(sw.ElapsedTicks);
        }

        void Sum()
        {
            int sum = 0;
            Entity[] entities = sc.Entities;
            for (int i = 0; i < entities.Length; i++)
            {
               sum = sc.Add(sum,entities[i]);
            }
        }

        void GeneralSum()
        {
            int sum = 0;
            Entity[] entities = sc.Entities;
            for (int i = 0; i < entities.Length; i++)
            {
                sum = sc.Add<Entity>(sum,entities[i]);
            }
        }
    }
}