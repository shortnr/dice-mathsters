namespace DiceMathsters.Domain.Expressions
{
    internal class NegatedMathExpression : MathExpression
    {
        private readonly MathExpression expression;

        internal NegatedMathExpression(MathExpression expr)
        {
            expression = expr;
        }

        internal override double Evaluate()
        {
            return -expression.Evaluate();
        }
    }
}
