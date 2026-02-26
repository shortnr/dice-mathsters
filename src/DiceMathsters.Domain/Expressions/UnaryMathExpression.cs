namespace DiceMathsters.Domain.Expressions
{
    internal class UnaryMathExpression : MathExpression
    {
        internal float Value { get; }

        internal UnaryMathExpression(int value)
        {
            Value = value;
        }

        internal override double Evaluate() => Value;
    }
}