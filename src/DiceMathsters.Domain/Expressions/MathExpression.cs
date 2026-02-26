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

    internal abstract class MathExpression
    {
        internal abstract double Evaluate();
    }
}
