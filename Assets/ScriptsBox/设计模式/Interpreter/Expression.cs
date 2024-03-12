namespace ScriptsBox.Interpreter
{
    public class Expression
    {
        public virtual int Evaluate()
        {
            return 0;
        }
    }
    public class NumberExpression:Expression
    {
        private int m_Value;
        public NumberExpression(int value)
        { 
            m_Value = value;
        }

        public override int Evaluate()
        {
            return m_Value;
        }
    }

    public class AdditionExpression : Expression
    {
        private Expression m_Left;
        private Expression m_Right;
        public AdditionExpression(Expression left, Expression right)
        {
            m_Left = left;
            m_Right = right;
        }

        public override int Evaluate()
        {
            int left = m_Left.Evaluate();
            int right = m_Right.Evaluate();
            return left + right;
            
        }
    }
}
