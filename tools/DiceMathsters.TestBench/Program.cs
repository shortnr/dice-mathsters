using DiceMathsters.Application.Handlers;
using DiceMathsters.Domain.Random;

namespace DiceMathsters.Testbench
{
    internal class Program
    {
        static void Main()
        {
            // ---- Expression evaluator ----
            string expressionString = "-(3 * (4 + 2))";
            double result = EvaluateExpressionHandler.Evaluate(expressionString);
            Console.WriteLine($"Result of '{expressionString}' is: {result}");

            Console.WriteLine();

            // ---- Dice roll ----
            var random = new SystemRandomProvider();
            var set = DiceSetHandler.RollStandard(random);

            Console.WriteLine("Standard dice roll:");
            foreach (var roll in set.Rolls)
                Console.WriteLine($"  {roll.DiceType}: {roll.Value}");

            Console.WriteLine();

            // ---- Round-trip: simulate server sending values to client ----
            var message = set.Rolls.Select(r => (r.DiceType, r.Value)).ToArray();
            var reconstructed = DiceSetHandler.FromServerMessage(message);

            Console.WriteLine("Reconstructed from server message:");
            foreach (var roll in reconstructed.Rolls)
                Console.WriteLine($"  {roll.DiceType}: {roll.Value}");

            Console.WriteLine();
            Console.WriteLine($"Round-trip match: {set.Values.SequenceEqual(reconstructed.Values)}");
        }
    }
}