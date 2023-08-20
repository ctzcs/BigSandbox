using System;
using System.Collections.Generic;
using UnityEngine;

namespace Box1.PhysicSimulate.FakePhysics
{
    public class Shape
    {
        //一般就是0
        public Vector3 centre;
        public float radius;

        public Shape(Vector3 c,float r)
        {
            centre = c;
            radius = r;
        }
        public static bool CheckCol(Shape a,Transform at,Shape b,Transform bt)
        {
            var aWorldCentre = at.TransformPoint(a.centre);
            var bWorldCentre = bt.TransformPoint(b.centre);
            float distance = Vector3.Distance(aWorldCentre, bWorldCentre);
            if (distance <= a.radius + b.radius)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 如果质量为0，默认为1
        /// </summary>
        public static void PhysicsEffect(Shape a,Transform at,Shape b,Transform bt,int mass,float deltaTime)
        {
            var aWorldCentre = at.TransformPoint(a.centre);
            var bWorldCentre = bt.TransformPoint(b.centre);
            Vector3 dir = aWorldCentre - bWorldCentre;
            mass = mass == 0 ? 1 : mass;
            var dirs = dir * (deltaTime * 1)/ mass;
            at.position += dirs;
        }
    }
    

    public class Ctrl:MonoBehaviour
    {
        public Shape shape;
        public Transform trans;

        private void Start()
        {
            TryGetComponent(out trans);
            shape = new Shape(Vector3.zero, 0.5f);
            Simulator.I.BodyRegister(this);
        }
    }
}