using System;

namespace DiceMathsters.Domain.Game
{
    /// <summary>
    /// Scores a student's submitted expression result against the round's target number.
    /// </summary>
    /// <remarks>
    /// Scoring uses an EaseOutSine curve so that near-misses still earn meaningful
    /// points while large misses drop off more steeply. The curve maps a normalised
    /// distance ratio in [0, 1] to a score in [1, 100].
    ///
    /// Distance is normalised against the target number itself, so a miss equal to
    /// the target value is treated as the worst-case miss. Any miss beyond that is
    /// clamped to the same floor.
    ///
    /// Invalid submissions (those that fail <see cref="ExpressionValidator"/>) score 0.
    /// Valid submissions always score at least 1, even on a complete miss.
    /// A perfect match scores 100.
    /// </remarks>
    public static class RoundScorer
    {
        /// <summary>Maximum score awarded for a perfect match.</summary>
        public const int MaxScore = 100;

        /// <summary>Minimum score awarded for any valid submission.</summary>
        public const int MinScore = 1;

        /// <summary>Score awarded for an invalid submission.</summary>
        public const int InvalidScore = 0;

        /// <summary>
        /// Scores a valid submission result against the target number.
        /// </summary>
        /// <param name="target">The target number for the round. Must be >= 1.</param>
        /// <param name="result">The evaluated result of the student's expression.</param>
        /// <returns>
        /// An integer score in [<see cref="MinScore"/>, <see cref="MaxScore"/>].
        /// </returns>
        public static int ScoreSubmission(int target, double result)
        {
            if (target < 1)
                throw new ArgumentOutOfRangeException(nameof(target), "Target must be at least 1.");

            double distance = Math.Abs(result - target);

            // Normalise distance against the target value.
            // A miss equal to the target is the worst-case (ratio = 1.0).
            // Anything beyond is clamped to 1.0.
            double ratio = Math.Min(distance / target, 1.0);

            // EaseOutSine: f(t) = sin(t * π/2)
            // At t=0 (perfect): f=0 → score = MaxScore
            // At t=1 (max miss): f=1 → score = MinScore
            double eased = Math.Sin(ratio * Math.PI / 2.0);

            // Map eased value [0, 1] → score [MaxScore, MinScore]
            double raw = MaxScore - eased * (MaxScore - MinScore);

            return (int)Math.Round(raw);
        }
    }
}
