using System;
using UnityEngine;

namespace Boids
{
    public class GameLogic:MonoBehaviour
    {
        public BoidsManager Boids;

        private void Awake()
        {
            Boids = BoidsManager.Instance;
            
        }
    }
}