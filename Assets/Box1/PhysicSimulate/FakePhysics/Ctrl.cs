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
            if (distance < a.radius + b.radius)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 如果质量为0，默认为1
        /// </summary>
        /// <param name="me"></param>
        /// <param name="other"></param>
        /// <param name="mass"></param>
        /// <param name="deltaTime"></param>
        public void PhysicsEffect(Transform me,Shape other,int mass,float deltaTime)
        {
            Vector3 dir = this.centre - other.centre;
            mass = mass == 0 ? 1 : mass;
            var dirs = dir * (deltaTime * 1) / mass;
            me.transform.position += dirs;
        }
    }
    

    public class Ctrl:MonoBehaviour
    {
        public Shape shape;
        public Transform trans;

        private void Start()
        {
            TryGetComponent(out trans);
            shape = new Shape(trans.position, 1);
            Simulator.I.BodyRegister(this);
        }
    }
}