
using UnityEngine;

namespace ScriptsBox.Interpreter
{
    public class Client:MonoBehaviour
    {
        private void Start()
        {
            NumberExpression numberExpression1 = new NumberExpression(1);
            NumberExpression numberExpression2 = new NumberExpression(2);
            AdditionExpression additionExpression = new AdditionExpression(numberExpression1, numberExpression2);
            print(additionExpression.Evaluate());
        }
    }
}