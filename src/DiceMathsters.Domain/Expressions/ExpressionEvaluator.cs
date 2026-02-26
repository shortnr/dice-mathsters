using System.Collections.Generic;
using DiceMathsters.Domain.Tokenizer;

namespace DiceMathsters.Domain.Expressions
{
    public class ExpressionEvaluator
    {
        public double EvaluateExpression(string expr)
        {
            var tokens = StringTokenizer.Tokenize(expr);
            return EvaluateExpression(tokens);
        }

        public double EvaluateExpression(IReadOnlyList<Token> tokens)
        {
            var expression = new ExpressionBuilder().Build(tokens);
            return expression.Evaluate();
        }
    }
}
