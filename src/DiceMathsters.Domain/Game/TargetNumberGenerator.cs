using System;
using System.Collections.Generic;
using System.Linq;
using DiceMathsters.Domain.Dice;
using DiceMathsters.Domain.Random;

namespace DiceMathsters.Domain.Game
{
    /// <summary>
    /// Generates a target number for a round based on the rolled dice values
    /// and a difficulty profile.
    /// </summary>
    /// <remarks>
    /// The target is a random integer within a range derived from the geometric
    /// mean of the dice values:
    /// <list type="bullet">
    ///   <item>Each die value is clamped to <see cref="DifficultyProfile.ValueFloor"/> before the mean is computed.</item>
    ///   <item>Lower bound = max(1, round(mean * <see cref="DifficultyProfile.LowerMultiplier"/>))</item>
    ///   <item>Upper bound = round(mean * <see cref="DifficultyProfile.UpperMultiplier"/>)</item>
    ///   <item>A random integer is selected uniformly from [lower, upper].</item>
    /// </list>
    /// Server-side only — clients receive the target value directly.
    /// </remarks>
    public static class TargetNumberGenerator
    {
        /// <summary>
        /// Generates a target number from a <see cref="DiceSet"/>.
        /// </summary>
        public static int Generate(DiceSet diceSet, DifficultyProfile profile, IRandomProvider random)
        {
            if (diceSet  == null) throw new ArgumentNullException(nameof(diceSet));
            if (profile  == null) throw new ArgumentNullException(nameof(profile));
            if (random   == null) throw new ArgumentNullException(nameof(random));

            return Generate(diceSet.Values, profile, random);
        }

        /// <summary>
        /// Generates a target number from a raw list of dice values.
        /// Useful for testing without constructing a full <see cref="DiceSet"/>.
        /// </summary>
        public static int Generate(IReadOnlyList<int> diceValues, DifficultyProfile profile, IRandomProvider random)
        {
            if (diceValues == null || diceValues.Count == 0)
                throw new ArgumentException("At least one dice value must be provided.", nameof(diceValues));
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            if (random  == null) throw new ArgumentNullException(nameof(random));

            double geometricMean = ComputeGeometricMean(diceValues, profile.ValueFloor);

            int lower = Math.Max(1, (int)Math.Round(geometricMean * profile.LowerMultiplier));
            int upper = Math.Max(lower, (int)Math.Round(geometricMean * profile.UpperMultiplier));

            return random.Next(lower, upper);
        }

        /// <summary>
        /// Computes the geometric mean of the given values, clamping each to
        /// <paramref name="valueFloor"/> before multiplication.
        /// </summary>
        internal static double ComputeGeometricMean(IReadOnlyList<int> values, int valueFloor)
        {
            double logSum = values
                .Select(v => Math.Max(v, valueFloor))
                .Sum(v => Math.Log(v));

            return Math.Exp(logSum / values.Count);
        }
    }
}
