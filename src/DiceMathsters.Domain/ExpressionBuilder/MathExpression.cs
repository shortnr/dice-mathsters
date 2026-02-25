namespace DiceMathsters.Domain.ExpressionBuilder
{
    public enum OperatorType
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Power
    }

    public abstract class MathExpression
    {
        public abstract double Evaluate();
    }
}
