using System;

namespace DiceMathsters.Domain.Game
{
    /// <summary>
    /// Defines the parameters that control target number generation difficulty.
    /// Theme-agnostic — display names (e.g. "Apprentice", "Cadet") are resolved
    /// separately at the presentation layer.
    /// </summary>
    public sealed class DifficultyProfile
    {
        /// <summary>
        /// Multiplier applied to the geometric mean to produce the lower bound
        /// of the target number range.
        /// </summary>
        public double LowerMultiplier { get; }

        /// <summary>
        /// Multiplier applied to the geometric mean to produce the upper bound
        /// of the target number range.
        /// </summary>
        public double UpperMultiplier { get; }

        /// <summary>
        /// Minimum value any die result is treated as when computing the
        /// geometric mean. Prevents low rolls (especially 1s) from collapsing
        /// the target range. Does not affect the actual dice values used in
        /// student expressions.
        /// </summary>
        public int ValueFloor { get; }

        /// <summary>
        /// Initializes a new <see cref="DifficultyProfile"/> with the given parameters.
        /// </summary>
        public DifficultyProfile(double lowerMultiplier, double upperMultiplier, int valueFloor)
        {
            if (lowerMultiplier <= 0)
                throw new ArgumentOutOfRangeException(nameof(lowerMultiplier), "Must be greater than zero.");
            if (upperMultiplier <= lowerMultiplier)
                throw new ArgumentOutOfRangeException(nameof(upperMultiplier), "Must be greater than lowerMultiplier.");
            if (valueFloor < 1)
                throw new ArgumentOutOfRangeException(nameof(valueFloor), "Must be at least 1.");

            LowerMultiplier = lowerMultiplier;
            UpperMultiplier = upperMultiplier;
            ValueFloor      = valueFloor;
        }

        // ----------------------------------------------------------------
        //  Built-in tiers
        // ----------------------------------------------------------------

        /// <summary>Tight range, forgiving targets. Suits new or younger players.</summary>
        public static readonly DifficultyProfile Easy     = new(0.75, 1.50, 2);

        /// <summary>Moderate range. Default for most classroom sessions.</summary>
        public static readonly DifficultyProfile Standard = new(0.50, 2.00, 2);

        /// <summary>Wider range, more unpredictable targets.</summary>
        public static readonly DifficultyProfile Advanced = new(0.25, 2.50, 2);

        /// <summary>Widest range. For experienced players seeking maximum challenge.</summary>
        public static readonly DifficultyProfile Expert   = new(0.25, 3.00, 2);
    }
}
