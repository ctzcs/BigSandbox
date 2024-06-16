using UnityEngine;

namespace ScriptsBox.精度问题
{
    public class Decimal : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            decimal x = new decimal(100.3);
            
            decimal y = new decimal(111.2);
            print(x-y);

            float xf = 100.3f;
            float yf = 111.2f;
            print(xf-yf);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
