namespace DiceMathsters.Domain.Expressions
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
