using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Boids
{
    public class BoidsManager:BaseManager<BoidsManager>
    {
        private Dictionary<int, Boid> m_BoidDic;

        public BoidsManager()
        {
            m_BoidDic = new Dictionary<int, Boid>();
        }

        public void AddToDic(int id,Boid boid)
        {
            m_BoidDic.TryAdd(id,boid);
        }

        public void RemoveFromDic(int id)
        {
            m_BoidDic.Remove(id);
        }
        public Boid GetFromDic(int id)
        {
            Boid boid;
             if (m_BoidDic.TryGetValue(id, out boid))
            {
                return boid;
            }

            return null;
        }

        public void Clear()
        {
            m_BoidDic.Clear();
        }
    }

}
