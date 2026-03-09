using DiceMathsters.Application.Handlers;
using DiceMathsters.Domain.Game;
using DiceMathsters.Domain.Random;

// Informal scratchpad for manual exploration. Not part of the production system.
namespace DiceMathsters.Testbench
{
    internal class Program
    {
        static void Main()
        {
            var random = new SystemRandomProvider();

            Section("Expression Evaluator");
            {
                string expr = "-(3 * (4 + 2))";
                double result = EvaluateExpressionHandler.Evaluate(expr);
                Console.WriteLine($"  '{expr}' = {result}");
            }

            Section("Dice Roll + Server Round-Trip");
            {
                var set = DiceSetHandler.RollStandard(random);

                Console.WriteLine("  Rolled:");
                foreach (var roll in set.Rolls)
                    Console.WriteLine($"    {roll.DiceType,-5} : {roll.Value}");

                var message      = set.Rolls.Select(r => (r.DiceType, r.Value)).ToArray();
                var reconstructed = DiceSetHandler.FromServerMessage(message);
                Console.WriteLine($"\n  Round-trip match: {set.Values.SequenceEqual(reconstructed.Values)}");
            }

            Section("Target Number Generation");
            {
                var set = DiceSetHandler.RollStandard(random);

                Console.WriteLine("  Dice values: " + string.Join(", ", set.Values));
                Console.WriteLine();

                foreach (var (label, profile) in new[]
                {
                    ("Easy",     DifficultyProfile.Easy),
                    ("Standard", DifficultyProfile.Standard),
                    ("Advanced", DifficultyProfile.Advanced),
                    ("Expert",   DifficultyProfile.Expert),
                })
                {
                    int target = TargetNumberGenerator.Generate(set, profile, random);
                    Console.WriteLine($"  {label,-10} : target {target}");
                }
            }

            Section("Expression Validation");
            {
                var set = DiceSetHandler.RollStandard(random);

                Console.WriteLine("  Dice values: " + string.Join(", ", set.Values));
                Console.WriteLine();

                // Build a valid expression using all dice values joined by +
                string validExpr   = string.Join(" + ", set.Values);
                // Repeat the first value to force a violation
                string invalidExpr = string.Join(" + ", set.Values) + $" + {set.Values[0]}";
                // Omit the last die
                string subsetExpr  = string.Join(" + ", set.Values.Take(set.Values.Count - 1));

                PrintValidation("All dice, Strict",          validExpr,   set, ValidationRules.Strict);
                PrintValidation("Repeated die, Strict",      invalidExpr, set, ValidationRules.Strict);
                PrintValidation("Subset, Strict",            subsetExpr,  set, ValidationRules.Strict);
                PrintValidation("Subset, SubsetAllowed",     subsetExpr,  set, ValidationRules.SubsetAllowed);
                PrintValidation("Repeated die, DoubleUse",   invalidExpr, set, ValidationRules.DoubleUse);
                PrintValidation("Unparseable expression",    "4 @ 2",     set, ValidationRules.Strict);
            }

            Section("Round Scoring");
            {
                var set    = DiceSetHandler.RollStandard(random);
                int target = TargetNumberGenerator.Generate(set, DifficultyProfile.Standard, random);

                Console.WriteLine($"  Dice values: {string.Join(", ", set.Values)}");
                Console.WriteLine($"  Target:      {target}");
                Console.WriteLine();

                // Build a few expressions with varying accuracy
                var candidates = new[]
                {
                    ("Perfect match",    (double)target),
                    ("Off by 1",         (double)(target + 1)),
                    ("Off by 2",         (double)(target - 2)),
                    ("Off by 10%",       target * 1.10),
                    ("Off by 25%",       target * 1.25),
                    ("Off by 50%",       target * 1.50),
                    ("Off by 100%",      target * 2.00),
                    ("Way off (×5)",     target * 5.00),
                };

                foreach (var (label, result) in candidates)
                {
                    int score = RoundScorer.ScoreSubmission(target, result);
                    Console.WriteLine($"  {label,-20} result={result,8:F1}  score={score,3}");
                }

                Console.WriteLine();

                // Now validate + score a real expression built from the dice
                string expr       = string.Join(" + ", set.Values);
                double exprResult = EvaluateExpressionHandler.Evaluate(expr);
                var validation    = ExpressionValidator.Validate(expr, set, ValidationRules.Strict);
                int exprScore     = validation.IsValid
                    ? RoundScorer.ScoreSubmission(target, exprResult)
                    : RoundScorer.InvalidScore;

                Console.WriteLine($"  Expression:  '{expr}'");
                Console.WriteLine($"  Result:      {exprResult}");
                Console.WriteLine($"  Valid:       {validation.IsValid}");
                Console.WriteLine($"  Score:       {exprScore}");
            }
        }

        // ----------------------------------------------------------------
        //  Helpers
        // ----------------------------------------------------------------

        static void Section(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"── {title} {'─'.Repeat(Math.Max(0, 50 - title.Length))}");
        }

        static void PrintValidation(string label, string expr, DiceMathsters.Domain.Dice.DiceSet set, ValidationRules rules)
        {
            var result = ExpressionValidator.Validate(expr, set, rules);
            string status = result.IsValid ? "✓ Valid" : $"✗ {result.FailureReason}";
            Console.WriteLine($"  [{label}]");
            Console.WriteLine($"    '{expr}'");
            Console.WriteLine($"    {status}");
            Console.WriteLine();
        }
    }

    internal static class CharExtensions
    {
        internal static string Repeat(this char c, int count) => new(c, count);
    }
}
