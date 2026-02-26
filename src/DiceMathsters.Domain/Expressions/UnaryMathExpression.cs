namespace DiceMathsters.Domain.Expressions
{
    public class UnaryMathExpression : MathExpression
    {
        public float Value { get; }

        public UnaryMathExpression(int value)
        {
            Value = value;
        }

        public override double Evaluate() => Value;
    }
}