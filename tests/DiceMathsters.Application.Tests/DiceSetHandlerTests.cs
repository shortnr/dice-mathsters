using System;
using System.Linq;
using DiceMathsters.Application.Handlers;
using DiceMathsters.Domain.Dice;
using DiceMathsters.Domain.Random;
using Xunit;

namespace DiceMathsters.Application.Tests
{
    public class DiceSetHandlerTests
    {
        // ============================================================
        //  Roll
        // ============================================================

        [Fact]
        public void Roll_ProducesCorrectDiceTypes()
        {
            var types  = new[] { DiceType.D4, DiceType.D6, DiceType.D8 };
            var random = new FixedRandomProvider(2, 4, 6);
            var set    = DiceSetHandler.Roll(types, random);

            Assert.Equal(types, set.Rolls.Select(r => r.DiceType));
        }

        [Fact]
        public void Roll_ProducesCorrectValues()
        {
            var types  = new[] { DiceType.D4, DiceType.D6, DiceType.D8 };
            var random = new FixedRandomProvider(2, 4, 6);
            var set    = DiceSetHandler.Roll(types, random);

            Assert.Equal(new[] { 2, 4, 6 }, set.Values);
        }

        [Fact]
        public void Roll_NullRandom_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                DiceSetHandler.Roll(new[] { DiceType.D6 }, null!));
        }

        [Fact]
        public void Roll_NullDiceTypes_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                DiceSetHandler.Roll(null!, new FixedRandomProvider(1)));
        }

        // ============================================================
        //  RollStandard
        // ============================================================

        [Fact]
        public void RollStandard_HasSixDice()
        {
            var set = DiceSetHandler.RollStandard(new FixedRandomProvider(1, 2, 3, 4, 5, 6));
            Assert.Equal(6, set.Rolls.Count);
        }

        [Fact]
        public void RollStandard_ContainsAllStandardTypes()
        {
            var set      = DiceSetHandler.RollStandard(new FixedRandomProvider(1, 2, 3, 4, 5, 6));
            var expected = new[] { DiceType.D4, DiceType.D6, DiceType.D8, DiceType.D10, DiceType.D12, DiceType.D20 };

            Assert.Equal(expected, set.Rolls.Select(r => r.DiceType));
        }

        [Fact]
        public void RollStandard_NullRandom_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                DiceSetHandler.RollStandard(null!));
        }

        // ============================================================
        //  FromServerMessage
        // ============================================================

        [Fact]
        public void FromServerMessage_ReconstructsCorrectly()
        {
            var message = new[]
            {
                (DiceType.D4,  3),
                (DiceType.D6,  5),
                (DiceType.D20, 17)
            };

            var set = DiceSetHandler.FromServerMessage(message);

            Assert.Equal(new[] { DiceType.D4, DiceType.D6, DiceType.D20 }, set.Rolls.Select(r => r.DiceType));
            Assert.Equal(new[] { 3, 5, 17 }, set.Values);
        }

        [Fact]
        public void FromServerMessage_AllDiceTypes_Reconstruct()
        {
            var message = new[]
            {
                (DiceType.D4,  4),
                (DiceType.D6,  6),
                (DiceType.D8,  8),
                (DiceType.D10, 10),
                (DiceType.D12, 12),
                (DiceType.D20, 20)
            };

            var set = DiceSetHandler.FromServerMessage(message);

            Assert.Equal(new[] { 4, 6, 8, 10, 12, 20 }, set.Values);
        }

        [Fact]
        public void FromServerMessage_InvalidValue_Throws()
        {
            // D6 can't have a value of 7
            var message = new[] { (DiceType.D6, 7) };

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                DiceSetHandler.FromServerMessage(message));
        }

        [Fact]
        public void FromServerMessage_NullInput_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                DiceSetHandler.FromServerMessage(null!));
        }

        [Fact]
        public void FromServerMessage_EmptyInput_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                DiceSetHandler.FromServerMessage(Array.Empty<(DiceType, int)>()));
        }

        // ============================================================
        //  Round-trip: Roll then reconstruct via FromServerMessage
        // ============================================================

        [Fact]
        public void RollThenReconstruct_ProducesIdenticalSet()
        {
            var random  = new FixedRandomProvider(3, 5, 7, 9, 11, 15);
            var rolled  = DiceSetHandler.RollStandard(random);

            // Simulate what the server would send
            var message = rolled.Rolls
                .Select(r => (r.DiceType, r.Value))
                .ToArray();

            var reconstructed = DiceSetHandler.FromServerMessage(message);

            Assert.Equal(rolled.Values, reconstructed.Values);
            Assert.Equal(
                rolled.Rolls.Select(r => r.DiceType),
                reconstructed.Rolls.Select(r => r.DiceType));
        }
    }
}
