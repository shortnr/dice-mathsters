using System;
using System.Linq;
using DiceMathsters.Domain.Dice;
using DiceMathsters.Domain.Random;
using Xunit;

namespace DiceMathsters.Domain.Tests.Dice
{
    public class DiceTests
    {
        // ============================================================
        //  DiceRoll.Roll — value always within die face range
        // ============================================================

        [Theory]
        [InlineData(DiceType.D4,  1, 4)]
        [InlineData(DiceType.D6,  1, 6)]
        [InlineData(DiceType.D8,  1, 8)]
        [InlineData(DiceType.D10, 1, 10)]
        [InlineData(DiceType.D12, 1, 12)]
        [InlineData(DiceType.D20, 1, 20)]
        public void DiceRoll_Roll_ValueInRange(DiceType diceType, int min, int max)
        {
            // SystemRandomProvider used here to verify real range behaviour
            var random = new SystemRandomProvider();
            for (int i = 0; i < 200; i++)
            {
                var roll = DiceRoll.Roll(diceType, random);
                Assert.Equal(diceType, roll.DiceType);
                Assert.InRange(roll.Value, min, max);
            }
        }

        [Fact]
        public void DiceRoll_Roll_ReturnsFixedValue()
        {
            // FixedRandomProvider verifies the value flows through correctly
            var random = new FixedRandomProvider(3);
            var roll   = DiceRoll.Roll(DiceType.D6, random);

            Assert.Equal(DiceType.D6, roll.DiceType);
            Assert.Equal(3, roll.Value);
        }

        [Fact]
        public void DiceRoll_Roll_NullRandom_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => DiceRoll.Roll(DiceType.D6, null!));
        }

        // ============================================================
        //  DiceRoll.FromValue — predetermined values
        // ============================================================

        [Theory]
        [InlineData(DiceType.D4,  1)]
        [InlineData(DiceType.D4,  4)]
        [InlineData(DiceType.D20, 1)]
        [InlineData(DiceType.D20, 20)]
        public void DiceRoll_FromValue_ValidBoundaries(DiceType diceType, int value)
        {
            var roll = DiceRoll.FromValue(diceType, value);
            Assert.Equal(diceType, roll.DiceType);
            Assert.Equal(value, roll.Value);
        }

        [Theory]
        [InlineData(DiceType.D6,   0)]  // below min
        [InlineData(DiceType.D6,   7)]  // above max
        [InlineData(DiceType.D4,  -1)]
        [InlineData(DiceType.D20, 21)]
        public void DiceRoll_FromValue_OutOfRange_Throws(DiceType diceType, int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DiceRoll.FromValue(diceType, value));
        }

        // ============================================================
        //  DiceSet.Roll
        // ============================================================

        [Fact]
        public void DiceSet_Roll_ProducesCorrectCount()
        {
            var types  = new[] { DiceType.D4, DiceType.D6, DiceType.D8 };
            var random = new FixedRandomProvider(2, 4, 6);
            var set    = DiceSet.Roll(types, random);

            Assert.Equal(3, set.Rolls.Count);
            Assert.Equal(3, set.Values.Count);
        }

        [Fact]
        public void DiceSet_Roll_DiceTypesPreservedInOrder()
        {
            var types  = new[] { DiceType.D4, DiceType.D6, DiceType.D8, DiceType.D10, DiceType.D12, DiceType.D20 };
            var random = new FixedRandomProvider(1, 2, 3, 4, 5, 6);
            var set    = DiceSet.Roll(types, random);

            for (int i = 0; i < types.Length; i++)
                Assert.Equal(types[i], set.Rolls[i].DiceType);
        }

        [Fact]
        public void DiceSet_Roll_ValuesMatchFixedProvider()
        {
            var types  = new[] { DiceType.D4, DiceType.D6, DiceType.D8 };
            var random = new FixedRandomProvider(3, 5, 7);
            var set    = DiceSet.Roll(types, random);

            Assert.Equal(new[] { 3, 5, 7 }, set.Values);
        }

        [Fact]
        public void DiceSet_Roll_AllValuesInRange()
        {
            var types  = new[] { DiceType.D4, DiceType.D6, DiceType.D8, DiceType.D10, DiceType.D12, DiceType.D20 };
            var random = new SystemRandomProvider();
            var set    = DiceSet.Roll(types, random);

            foreach (var roll in set.Rolls)
                Assert.InRange(roll.Value, 1, (int)roll.DiceType);
        }

        [Fact]
        public void DiceSet_Roll_EmptyDiceTypes_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                DiceSet.Roll(Array.Empty<DiceType>(), new FixedRandomProvider(1)));
        }

        [Fact]
        public void DiceSet_Roll_NullRandom_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                DiceSet.Roll(new[] { DiceType.D6 }, null!));
        }

        // ============================================================
        //  DiceSet.RollStandard
        // ============================================================

        [Fact]
        public void DiceSet_RollStandard_HasSixDice()
        {
            var set = DiceSet.RollStandard(new FixedRandomProvider(1, 2, 3, 4, 5, 6));
            Assert.Equal(6, set.Rolls.Count);
        }

        [Fact]
        public void DiceSet_RollStandard_ContainsAllStandardTypes()
        {
            var set      = DiceSet.RollStandard(new FixedRandomProvider(1, 2, 3, 4, 5, 6));
            var expected = new[] { DiceType.D4, DiceType.D6, DiceType.D8, DiceType.D10, DiceType.D12, DiceType.D20 };

            Assert.Equal(expected, set.Rolls.Select(r => r.DiceType));
        }

        // ============================================================
        //  DiceSet.FromRolls
        // ============================================================

        [Fact]
        public void DiceSet_FromRolls_PreservesValues()
        {
            var rolls = new[]
            {
                DiceRoll.FromValue(DiceType.D4,  3),
                DiceRoll.FromValue(DiceType.D6,  5),
                DiceRoll.FromValue(DiceType.D20, 17)
            };

            var set = DiceSet.FromRolls(rolls);

            Assert.Equal(new[] { 3, 5, 17 }, set.Values);
        }

        [Fact]
        public void DiceSet_FromRolls_Empty_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                DiceSet.FromRolls(Array.Empty<DiceRoll>()));
        }

        // ============================================================
        //  FixedRandomProvider
        // ============================================================

        [Fact]
        public void FixedRandomProvider_ReturnsValuesInSequence()
        {
            var provider = new FixedRandomProvider(1, 3, 5);

            Assert.Equal(1, provider.Next(1, 6));
            Assert.Equal(3, provider.Next(1, 6));
            Assert.Equal(5, provider.Next(1, 6));
        }

        [Fact]
        public void FixedRandomProvider_CyclesWhenExhausted()
        {
            var provider = new FixedRandomProvider(2, 4);

            Assert.Equal(2, provider.Next(1, 6));
            Assert.Equal(4, provider.Next(1, 6));
            Assert.Equal(2, provider.Next(1, 6)); // cycles
        }

        [Fact]
        public void FixedRandomProvider_ValueOutsideRange_Throws()
        {
            var provider = new FixedRandomProvider(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => provider.Next(1, 6));
        }

        [Fact]
        public void FixedRandomProvider_Empty_Throws()
        {
            Assert.Throws<ArgumentException>(() => new FixedRandomProvider());
        }
    }
}
