using System;
using System.Collections.Generic;
using System.Linq;
using DiceMathsters.Domain.Dice;
using DiceMathsters.Domain.Game;
using DiceMathsters.Domain.Random;
using Xunit;

namespace DiceMathsters.Domain.Tests
{
    public class TargetNumberGeneratorTests
    {
        // ============================================================
        //  DifficultyProfile — construction validation
        // ============================================================

        [Fact]
        public void DifficultyProfile_ValidConstruction_Succeeds()
        {
            var profile = new DifficultyProfile(0.5, 2.0, 2);

            Assert.Equal(0.5, profile.LowerMultiplier);
            Assert.Equal(2.0, profile.UpperMultiplier);
            Assert.Equal(2,   profile.ValueFloor);
        }

        [Theory]
        [InlineData(0.0,  2.0, 2)]   // lower must be > 0
        [InlineData(-0.5, 2.0, 2)]   // lower negative
        [InlineData(2.0,  2.0, 2)]   // upper must be > lower
        [InlineData(2.0,  1.0, 2)]   // upper < lower
        [InlineData(0.5,  2.0, 0)]   // floor must be >= 1
        [InlineData(0.5,  2.0, -1)]  // floor negative
        public void DifficultyProfile_InvalidConstruction_Throws(
            double lower, double upper, int floor)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DifficultyProfile(lower, upper, floor));
        }

        [Fact]
        public void DifficultyProfile_BuiltInTiers_HaveExpectedValues()
        {
            Assert.Equal(0.75, DifficultyProfile.Easy.LowerMultiplier);
            Assert.Equal(1.50, DifficultyProfile.Easy.UpperMultiplier);

            Assert.Equal(0.50, DifficultyProfile.Standard.LowerMultiplier);
            Assert.Equal(2.00, DifficultyProfile.Standard.UpperMultiplier);

            Assert.Equal(0.25, DifficultyProfile.Advanced.LowerMultiplier);
            Assert.Equal(2.50, DifficultyProfile.Advanced.UpperMultiplier);

            Assert.Equal(0.25, DifficultyProfile.Expert.LowerMultiplier);
            Assert.Equal(3.00, DifficultyProfile.Expert.UpperMultiplier);

            // All built-in tiers share the same floor for now
            Assert.Equal(2, DifficultyProfile.Easy.ValueFloor);
            Assert.Equal(2, DifficultyProfile.Standard.ValueFloor);
            Assert.Equal(2, DifficultyProfile.Advanced.ValueFloor);
            Assert.Equal(2, DifficultyProfile.Expert.ValueFloor);
        }

        // ============================================================
        //  GeometricMean — value floor behaviour
        // ============================================================

        [Fact]
        public void GeometricMean_AllOnesWithFloor_UsesFloorInsteadOfOne()
        {
            // Without floor: GM of [1,1,1] = 1.0
            // With floor 2: GM of [2,2,2] = 2.0
            var values = new[] { 1, 1, 1 };
            double mean = TargetNumberGenerator.ComputeGeometricMean(values, valueFloor: 2);

            Assert.Equal(2.0, mean, precision: 10);
        }

        [Fact]
        public void GeometricMean_ValuesAboveFloor_AreUnaffected()
        {
            // GM of [4, 6, 8, 10, 12, 20] ≈ 8.76
            var values   = new[] { 4, 6, 8, 10, 12, 20 };
            double mean  = TargetNumberGenerator.ComputeGeometricMean(values, valueFloor: 2);
            double expected = Math.Pow(4.0 * 6 * 8 * 10 * 12 * 20, 1.0 / 6);

            Assert.Equal(expected, mean, precision: 10);
        }

        [Fact]
        public void GeometricMean_MixedWithOnes_ClampsOnlyOnes()
        {
            // 1 is clamped to 2; 6 and 8 are unaffected
            var values   = new[] { 1, 6, 8 };
            double mean  = TargetNumberGenerator.ComputeGeometricMean(values, valueFloor: 2);
            double expected = Math.Pow(2.0 * 6 * 8, 1.0 / 3);

            Assert.Equal(expected, mean, precision: 10);
        }

        // ============================================================
        //  TargetNumberGenerator.Generate — range correctness
        // ============================================================

        [Fact]
        public void Generate_ResultWithinExpectedRange_Easy()
        {
            // GM of [4,6,8,10,12,20] ≈ 8.76
            // Easy: lower = round(8.76 * 0.75) = 7, upper = round(8.76 * 1.5) = 13
            var values  = new[] { 4, 6, 8, 10, 12, 20 };
            var profile = DifficultyProfile.Easy;
            double mean = TargetNumberGenerator.ComputeGeometricMean(values, profile.ValueFloor);
            int lower   = Math.Max(1, (int)Math.Round(mean * profile.LowerMultiplier));
            int upper   = (int)Math.Round(mean * profile.UpperMultiplier);

            // Run many times to exercise the range
            var random = new SystemRandomProvider();
            for (int i = 0; i < 500; i++)
            {
                int target = TargetNumberGenerator.Generate(values, profile, random);
                Assert.InRange(target, lower, upper);
            }
        }

        [Fact]
        public void Generate_ResultWithinExpectedRange_Expert()
        {
            var values  = new[] { 4, 6, 8, 10, 12, 20 };
            var profile = DifficultyProfile.Expert;
            double mean = TargetNumberGenerator.ComputeGeometricMean(values, profile.ValueFloor);
            int lower   = Math.Max(1, (int)Math.Round(mean * profile.LowerMultiplier));
            int upper   = (int)Math.Round(mean * profile.UpperMultiplier);

            var random = new SystemRandomProvider();
            for (int i = 0; i < 500; i++)
            {
                int target = TargetNumberGenerator.Generate(values, profile, random);
                Assert.InRange(target, lower, upper);
            }
        }

        [Fact]
        public void Generate_ExpertRangeWiderThanEasyRange()
        {
            var values   = new[] { 4, 6, 8, 10, 12, 20 };
            int easySpan = RangeSpan(values, DifficultyProfile.Easy);
            int expertSpan = RangeSpan(values, DifficultyProfile.Expert);

            Assert.True(expertSpan > easySpan,
                $"Expert span ({expertSpan}) should be wider than Easy span ({easySpan}).");
        }

        [Fact]
        public void Generate_AllOnesRolled_TargetAtLeastOne()
        {
            // Even with all 1s rolled, target must be >= 1
            var values = new[] { 1, 1, 1, 1, 1, 1 };
            var random = new SystemRandomProvider();

            for (int i = 0; i < 200; i++)
            {
                int target = TargetNumberGenerator.Generate(values, DifficultyProfile.Easy, random);
                Assert.True(target >= 1, $"Target was {target}, expected >= 1.");
            }
        }

        [Fact]
        public void Generate_FromDiceSet_MatchesRawValues()
        {
            // Confirm the DiceSet overload produces the same range as the raw values overload
            var rolls = new[]
            {
                DiceRoll.FromValue(DiceType.D4,  3),
                DiceRoll.FromValue(DiceType.D6,  5),
                DiceRoll.FromValue(DiceType.D20, 17)
            };
            var diceSet = DiceSet.FromRolls(rolls);
            var values  = new[] { 3, 5, 17 };
            var profile = DifficultyProfile.Standard;

            double meanFromSet    = TargetNumberGenerator.ComputeGeometricMean(diceSet.Values, profile.ValueFloor);
            double meanFromValues = TargetNumberGenerator.ComputeGeometricMean(values, profile.ValueFloor);

            Assert.Equal(meanFromValues, meanFromSet, precision: 10);
        }

        // ============================================================
        //  TargetNumberGenerator.Generate — null / empty guards
        // ============================================================

        [Fact]
        public void Generate_NullDiceSet_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                TargetNumberGenerator.Generate((DiceSet)null!, DifficultyProfile.Standard, new SystemRandomProvider()));
        }

        [Fact]
        public void Generate_NullProfile_Throws()
        {
            var rolls   = new[] { DiceRoll.FromValue(DiceType.D6, 3) };
            var diceSet = DiceSet.FromRolls(rolls);

            Assert.Throws<ArgumentNullException>(() =>
                TargetNumberGenerator.Generate(diceSet, null!, new SystemRandomProvider()));
        }

        [Fact]
        public void Generate_NullRandom_Throws()
        {
            var rolls   = new[] { DiceRoll.FromValue(DiceType.D6, 3) };
            var diceSet = DiceSet.FromRolls(rolls);

            Assert.Throws<ArgumentNullException>(() =>
                TargetNumberGenerator.Generate(diceSet, DifficultyProfile.Standard, null!));
        }

        [Fact]
        public void Generate_EmptyValues_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                TargetNumberGenerator.Generate(
                    Array.Empty<int>(), DifficultyProfile.Standard, new SystemRandomProvider()));
        }

        // ============================================================
        //  Helper
        // ============================================================

        private static int RangeSpan(IReadOnlyList<int> values, DifficultyProfile profile)
        {
            double mean = TargetNumberGenerator.ComputeGeometricMean(values, profile.ValueFloor);
            int lower   = Math.Max(1, (int)Math.Round(mean * profile.LowerMultiplier));
            int upper   = (int)Math.Round(mean * profile.UpperMultiplier);
            return upper - lower;
        }
    }
}
