using DiceMathsters.Application;

namespace DiceMathsters.MathCore.Testbench
{
    internal class Program
    {
        static void Main()
        {
            string expressionString = "-(3 * (4 + 2))";
            double result = ExpressionEvaluator.Evaluate(expressionString);
            Console.WriteLine($"Result of '{expressionString}' is: {result}");
        }
    }
}