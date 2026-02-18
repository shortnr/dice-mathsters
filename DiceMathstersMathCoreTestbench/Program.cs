using DiceMathsters.Core;

namespace DiceMathstersMathCoreTestbench
{
    internal class Program
    {
        static void Main()
        {
            string expressionString = "-(3 * (4 + 2))";
            IReadOnlyList<Token> tokens = Tokenizer.Tokenize(expressionString);
            ExpressionBuilder expressionBuilder = new();
            MathExpression expression = expressionBuilder.BuildExpression(tokens);
            Console.WriteLine($"Result of '{expressionString}' is: {expression.Evaluate()}");
        }
    }
}