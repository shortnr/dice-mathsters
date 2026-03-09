using System;
using System.Collections.Generic;
using System.Linq;
using DiceMathsters.Domain.Dice;
using DiceMathsters.Domain.Expressions;
using DiceMathsters.Domain.Tokenizer;

namespace DiceMathsters.Domain.Game
{
    /// <summary>
    /// Validates that the numbers used in a submitted expression are consistent
    /// with the rolled <see cref="DiceSet"/> under a given <see cref="ValidationRules"/> policy.
    /// </summary>
    /// <remarks>
    /// Validation is purely structural — it checks which numeric values appear in
    /// the token stream against the multiset of rolled dice values. Expression
    /// correctness (syntax, evaluation) is handled separately by
    /// <see cref="ExpressionEvaluator"/>.
    /// </remarks>
    public static class ExpressionValidator
    {
        /// <summary>
        /// Validates an expression string against a rolled dice set.
        /// </summary>
        /// <param name="expression">The raw expression string submitted by the student.</param>
        /// <param name="diceSet">The dice set rolled for this round.</param>
        /// <param name="rules">The validation policy in effect for this session.</param>
        /// <returns>
        /// A <see cref="ValidationResult"/> indicating whether the expression is valid,
        /// and if not, why.
        /// </returns>
        public static ValidationResult Validate(
            string expression,
            DiceSet diceSet,
            ValidationRules rules)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (diceSet    == null) throw new ArgumentNullException(nameof(diceSet));
            if (rules      == null) throw new ArgumentNullException(nameof(rules));

            IReadOnlyList<Token> tokens;
            try
            {
                tokens = StringTokenizer.Tokenize(expression);
            }
            catch (Exception ex)
            {
                return ValidationResult.Fail($"Expression could not be parsed: {ex.Message}");
            }

            return Validate(tokens, diceSet, rules);
        }

        /// <summary>
        /// Validates a pre-tokenized expression against a rolled dice set.
        /// Use this overload when you have already tokenized the expression (e.g. for
        /// evaluation and validation in the same pipeline without tokenizing twice).
        /// </summary>
        /// <param name="tokens">Tokens produced by the tokenizer.</param>
        /// <param name="diceSet">The dice set rolled for this round.</param>
        /// <param name="rules">The validation policy in effect for this session.</param>
        public static ValidationResult Validate(
            IReadOnlyList<Token> tokens,
            DiceSet diceSet,
            ValidationRules rules)
        {
            if (tokens  == null) throw new ArgumentNullException(nameof(tokens));
            if (diceSet == null) throw new ArgumentNullException(nameof(diceSet));
            if (rules   == null) throw new ArgumentNullException(nameof(rules));

            // Extract numeric values from the token stream (ignore operators, parens, end).
            var usedNumbers = tokens
                .Where(t => t.Type == TokenType.Number)
                .Select(t => t.Num!.Value)
                .ToList();

            // Model each die as a separate slot with its own usage budget.
            // This correctly handles duplicate rolled values (e.g. two dice both showing 6).
            var slots = diceSet.Values
                .Select(v => new DieSlot(v, rules.MaxUsesPerDie))
                .ToArray();

            // Consume a slot for each number appearing in the expression.
            // Group slots into a queue per value so that uses are distributed
            // across all slots of the same value before any one slot is reused.
            // This ensures duplicate rolled values (e.g. two dice both showing 6)
            // are each treated as independent slots.
            var slotQueues = slots
                .GroupBy(s => s.Value)
                .ToDictionary(g => g.Key, g => new Queue<DieSlot>(g));

            // Consume a slot for each number appearing in the expression.
            foreach (int number in usedNumbers)
            {
                if (!slotQueues.TryGetValue(number, out var queue) || queue.Count == 0)
                {
                    return ValidationResult.Fail(
                        $"The value {number} was used more times than the rolled dice permit.");
                }

                var slot = queue.Dequeue();
                slot.RemainingUses--;

                // If this slot still has remaining uses, re-enqueue it at the back
                // so other slots of the same value get a turn first.
                if (slot.RemainingUses > 0)
                    queue.Enqueue(slot);
            }

            // If all dice must be used, verify every slot was touched at least once.
            // A slot that was never touched will still have RemainingUses == MaxUsesPerDie.
            if (rules.MustUseAllDice)
            {
                var unusedValues = slots
                    .Where(s => s.RemainingUses == rules.MaxUsesPerDie)
                    .Select(s => s.Value)
                    .ToList();

                if (unusedValues.Count > 0)
                {
                    var list = string.Join(", ", unusedValues);
                    return ValidationResult.Fail(
                        $"The following rolled values were not used: {list}.");
                }
            }

            return ValidationResult.Ok();
        }

        // ----------------------------------------------------------------
        //  Helpers
        // ----------------------------------------------------------------

        /// <summary>
        /// Mutable slot representing one die in the rolled set.
        /// Tracks how many more times this die's value may be used.
        /// </summary>
        private sealed class DieSlot
        {
            public int Value         { get; }
            public int RemainingUses { get; set; }

            public DieSlot(int value, int maxUses)
            {
                Value         = value;
                RemainingUses = maxUses;
            }
        }
    }
}
