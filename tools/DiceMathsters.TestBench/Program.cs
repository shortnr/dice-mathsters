using DiceMathsters.Application;

namespace DiceMathstersMathCoreTestbench
{
    internal class Program
    {
        static void Main()
        {
            string expressionString = "-(3 * (4 + 2))";
            double result = EvaluateExpressionHandler.HandleFromString(expressionString);
            Console.WriteLine($"Result of '{expressionString}' is: {result}");
        }
    }
}