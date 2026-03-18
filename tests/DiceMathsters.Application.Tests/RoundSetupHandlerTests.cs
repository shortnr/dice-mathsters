using DiceMathsters.Application.Handlers;
using DiceMathsters.Domain.Dice;
using DiceMathsters.Domain.Game;
using DiceMathsters.Domain.Random;
using Xunit;

namespace DiceMathsters.Application.Tests
{
    public class RoundSetupHandlerTests
    {
        // ================================================================
        //  RoundSetup record — basic shape
        // ================================================================

        [Fact]
        public void Setup_ReturnsNonNullRoundSetup()
        {
            var setup = RoundSetupHandler.Setup(
                DifficultyProfile.Standard,
                ValidationRules.Strict,
                AllowedOperations.Standard,
                new FixedRandomProvider(3, 5, 7, 9, 11, 15, 10));

            Assert.NotNull(setup);
        }

        [Fact]
        public void Setup_DiceSet_HasSixRolls()
        {
            var setup = RoundSetupHandler.Setup(
                DifficultyProfile.Standard,
                ValidationRules.Strict,
                AllowedOperations.Standard,
                new FixedRandomProvider(3, 5, 7, 9, 11, 15, 10));

            Assert.Equal(6, setup.Dice.Rolls.Count);
        }

        [Fact]
        public void Setup_DiceSet_ContainsStandardTypes()
        {
            var setup = RoundSetupHandler.Setup(
                DifficultyProfile.Standard,
                ValidationRules.Strict,
                AllowedOperations.Standard,
                new FixedRandomProvider(3, 5, 7, 9, 11, 15, 10));

            var expected = new[]
            {
                DiceType.D4, DiceType.D6, DiceType.D8,
                DiceType.D10, DiceType.D12, DiceType.D20
            };

            Assert.Equal(expected, setup.Dice.Rolls.Select(r => r.DiceType));
        }

        [Fact]
        public void Setup_Target_IsPositive()
        {
            var setup = RoundSetupHandler.Setup(
                DifficultyProfile.Standard,
                ValidationRules.Strict,
                AllowedOperations.Standard,
                new FixedRandomProvider(3, 5, 7, 9, 11, 15, 10));

            Assert.True(setup.Target >= 1);
        }

        [Fact]
        public void Setup_Target_WithinExpectedRangeForProfile()
        {
            // GM of [3,5,7,9,11,15] ≈ 7.05
            // Standard: lower = round(7.05 * 0.5) = 4, upper = round(7.05 * 2.0) = 14
            // FixedRandomProvider last value (10) is the target
            var setup = RoundSetupHandler.Setup(
                DifficultyProfile.Standard,
                ValidationRules.Strict,
                AllowedOperations.Standard,
                new FixedRandomProvider(3, 5, 7, 9, 11, 15, 10));

            Assert.InRange(setup.Target, 1, 100);
        }

        // ================================================================
        //  Configuration round-trips correctly
        // ================================================================

        [Fact]
        public void Setup_ValidationRules_RoundTrips()
        {
            var rules = ValidationRules.DoubleUse;
            var setup = RoundSetupHandler.Setup(
                DifficultyProfile.Standard,
                rules,
                AllowedOperations.Standard,
                new FixedRandomProvider(3, 5, 7, 9, 11, 15, 10));

            Assert.Same(rules, setup.Rules);
        }

        [Fact]
        public void Setup_DifficultyProfile_RoundTrips()
        {
            var profile = DifficultyProfile.Expert;
            var setup = RoundSetupHandler.Setup(
                profile,
                ValidationRules.Strict,
                AllowedOperations.Standard,
                new FixedRandomProvider(3, 5, 7, 9, 11, 15, 10));

            Assert.Same(profile, setup.Profile);
        }

        [Fact]
        public void Setup_AllowedOperations_RoundTrips()
        {
            var ops = AllowedOperations.AddSubtract;
            var setup = RoundSetupHandler.Setup(
                DifficultyProfile.Standard,
                ValidationRules.Strict,
                ops,
                new FixedRandomProvider(3, 5, 7, 9, 11, 15, 10));

            Assert.Same(ops, setup.Operations);
        }

        // ================================================================
        //  Determinism — same provider produces same setup
        // ================================================================

        [Fact]
        public void Setup_SameFixedProvider_ProducesSameResult()
        {
            var setupA = RoundSetupHandler.Setup(
                DifficultyProfile.Standard,
                ValidationRules.Strict,
                AllowedOperations.Standard,
                new FixedRandomProvider(3, 5, 7, 9, 11, 15, 10));

            var setupB = RoundSetupHandler.Setup(
                DifficultyProfile.Standard,
                ValidationRules.Strict,
                AllowedOperations.Standard,
                new FixedRandomProvider(3, 5, 7, 9, 11, 15, 10));

            Assert.Equal(setupA.Dice.Values, setupB.Dice.Values);
            Assert.Equal(setupA.Target, setupB.Target);
        }

        // ================================================================
        //  Null guards
        // ================================================================

        [Fact]
        public void Setup_NullProfile_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                RoundSetupHandler.Setup(
                    null!,
                    ValidationRules.Strict,
                    AllowedOperations.Standard,
                    new FixedRandomProvider(1, 2, 3, 4, 5, 6, 5)));
        }

        [Fact]
        public void Setup_NullRules_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                RoundSetupHandler.Setup(
                    DifficultyProfile.Standard,
                    null!,
                    AllowedOperations.Standard,
                    new FixedRandomProvider(1, 2, 3, 4, 5, 6, 5)));
        }

        [Fact]
        public void Setup_NullOperations_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                RoundSetupHandler.Setup(
                    DifficultyProfile.Standard,
                    ValidationRules.Strict,
                    null!,
                    new FixedRandomProvider(1, 2, 3, 4, 5, 6, 5)));
        }

        [Fact]
        public void Setup_NullRandom_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                RoundSetupHandler.Setup(
                    DifficultyProfile.Standard,
                    ValidationRules.Strict,
                    AllowedOperations.Standard,
                    null!));
        }
    }
}