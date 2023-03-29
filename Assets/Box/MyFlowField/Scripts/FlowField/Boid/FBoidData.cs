namespace MyFlowField
{
    public class FBoidData
    {
        private  float _detectionRadius;
        private  float _detectionAngle;
        private float _riskDistance;
        private int _detectionLayer;

        public FBoidData(float detectionRadius, float detectionAngle,float riskColl)
        {
            _detectionRadius = detectionRadius;
            _detectionAngle = detectionAngle;
            _riskDistance = riskColl;
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

        public float RiskDistance
        {
            get
            {
                return _riskDistance;
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