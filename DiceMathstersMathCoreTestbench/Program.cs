using DiceMathsters.Core;

namespace DiceMathstersMathCoreTestbench
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string expressionString = "-(3 * (4 + 2))";
            Tokenizer tokenizer = new();
            IReadOnlyList<Token> tokens = tokenizer.Tokenize(expressionString);
            ExpressionBuilder expressionBuilder = new();
            MathExpression expression = expressionBuilder.BuildExpressionFromTokens(tokens);
            Console.WriteLine($"Result of '{expressionString}' is: {expression.Evaluate()}");
        }
    }
}