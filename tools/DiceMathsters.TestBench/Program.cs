using DiceMathsters.Application.Handlers;

namespace DiceMathsters.Testbench
{
    internal class Program
    {
        static void Main()
        {
            string expressionString = "-(3 * (4 + 2))";
            double result = EvaluateExpressionHandler.Evaluate(expressionString);
            Console.WriteLine($"Result of '{expressionString}' is: {result}");
        }
    }
}