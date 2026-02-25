namespace DiceMathsters.Domain.ExpressionBuilder
{
    public class NegatedMathExpression : MathExpression
    {
        private readonly MathExpression expression;

        public NegatedMathExpression(MathExpression expr)
        {
            expression = expr;
        }

        public override double Evaluate()
        {
            return -expression.Evaluate();
        }
    }
}
