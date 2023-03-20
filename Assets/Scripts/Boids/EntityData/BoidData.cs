using System.Collections.Generic;

namespace Boids
{
    public class BoidData
    {
        private  float _detectionRadius;
        private  float _detectionAngle;
        private int _detectionLayer;

        public BoidData(float detectionRadius, float detectionAngle)
        {
            _detectionRadius = detectionRadius;
            _detectionAngle = detectionAngle;
        }

        public float DetectionRadius
        {
            get
            {
                return _detectionRadius;
            }
        }

        public float DetectionAngle
        {
            get
            {
                return _detectionAngle;
            }
        }

        public int DetectionLayer
        {
            get
            {
                return _detectionLayer;
            }
        }
    }
}