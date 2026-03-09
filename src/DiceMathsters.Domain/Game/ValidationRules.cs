using System;

namespace DiceMathsters.Domain.Game
{
    /// <summary>
    /// Controls how strictly a submitted expression must use the rolled dice values.
    /// Configured per game session; passed into <see cref="ExpressionValidator"/>.
    /// </summary>
    public sealed class ValidationRules
    {
        /// <summary>
        /// Whether every die in the rolled set must appear at least once in the expression.
        /// When <c>true</c>, expressions that omit any rolled value are rejected.
        /// When <c>false</c>, a subset of the rolled values is acceptable.
        /// Default: <c>true</c>.
        /// </summary>
        public bool MustUseAllDice { get; }

        /// <summary>
        /// The maximum number of times a single rolled value (by position) may be used
        /// in the expression. Must be at least 1.
        /// Default: <c>1</c> (each die used exactly once).
        /// </summary>
        public int MaxUsesPerDie { get; }

        /// <summary>
        /// Initializes a new <see cref="ValidationRules"/> instance.
        /// </summary>
        /// <param name="mustUseAllDice">
        /// Whether all rolled dice must appear in the expression.
        /// </param>
        /// <param name="maxUsesPerDie">
        /// How many times each individual die value may be used. Must be >= 1.
        /// </param>
        public ValidationRules(bool mustUseAllDice = true, int maxUsesPerDie = 1)
        {
            if (maxUsesPerDie < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(maxUsesPerDie), "Must be at least 1.");

            MustUseAllDice = mustUseAllDice;
            MaxUsesPerDie  = maxUsesPerDie;
        }

        // ----------------------------------------------------------------
        //  Built-in presets
        // ----------------------------------------------------------------

        /// <summary>
        /// Each die must be used exactly once. The default competitive ruleset.
        /// </summary>
        public static readonly ValidationRules Strict =
            new(mustUseAllDice: true, maxUsesPerDie: 1);

        /// <summary>
        /// Students may use any subset of the rolled dice; unused dice are fine.
        /// Useful for younger players or introductory sessions.
        /// </summary>
        public static readonly ValidationRules SubsetAllowed =
            new(mustUseAllDice: false, maxUsesPerDie: 1);

        /// <summary>
        /// Each die may be used up to twice; all dice must still appear at least once.
        /// Adds flexibility for harder target numbers.
        /// </summary>
        public static readonly ValidationRules DoubleUse =
            new(mustUseAllDice: true, maxUsesPerDie: 2);

        /// <summary>
        /// Any subset of dice, each usable up to twice. Most permissive preset.
        /// </summary>
        public static readonly ValidationRules Relaxed =
            new(mustUseAllDice: false, maxUsesPerDie: 2);
    }
}
