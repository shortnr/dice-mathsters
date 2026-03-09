using DiceMathsters.Domain.Dice;
using DiceMathsters.Domain.Game;
using Xunit;

namespace DiceMathsters.Domain.Tests.Game
{
    /// <summary>
    /// Tests for <see cref="ExpressionValidator"/>.
    ///
    /// Helper: <see cref="Dice"/> builds a DiceSet from explicit (type, value) pairs
    /// without going through the random-roll path.
    /// </summary>
    public class ExpressionValidatorTests
    {
        // ----------------------------------------------------------------
        //  Test helper
        // ----------------------------------------------------------------

        /// <summary>
        /// Builds a DiceSet from a sequence of (DiceType, rolledValue) pairs.
        /// Keeps test cases readable without constructing a full roll pipeline.
        /// </summary>
        private static DiceSet MakeDiceSet(params (DiceType Type, int Value)[] rolls)
        {
            var diceRolls = rolls
                .Select(r => DiceRoll.FromValue(r.Type, r.Value))
                .ToList();
            return DiceSet.FromRolls(diceRolls);
        }

        /// <summary>Standard competitive set: D4=3, D6=5, D8=7, D10=9, D12=11, D20=15.</summary>
        private static DiceSet StandardSet() => MakeDiceSet(
            (DiceType.D4,  3),
            (DiceType.D6,  5),
            (DiceType.D8,  7),
            (DiceType.D10, 9),
            (DiceType.D12, 11),
            (DiceType.D20, 15));

        // ================================================================
        //  Strict (default) — must use every die exactly once
        // ================================================================

        [Fact]
        public void Strict_AllDiceUsedExactlyOnce_IsValid()
        {
            var result = ExpressionValidator.Validate("3 + 5 + 7 + 9 + 11 + 15", StandardSet(), ValidationRules.Strict);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Strict_MissingOneDie_Fails()
        {
            // Omits the 15 (D20)
            var result = ExpressionValidator.Validate("3 + 5 + 7 + 9 + 11", StandardSet(), ValidationRules.Strict);
            Assert.False(result.IsValid);
            Assert.Contains("15", result.FailureReason);
        }

        [Fact]
        public void Strict_ValueNotInDiceSet_Fails()
        {
            // 99 was never rolled
            var result = ExpressionValidator.Validate("3 + 5 + 7 + 9 + 11 + 99", StandardSet(), ValidationRules.Strict);
            Assert.False(result.IsValid);
            Assert.Contains("99", result.FailureReason);
        }

        [Fact]
        public void Strict_DieUsedTwice_Fails()
        {
            // 3 appears twice, 15 omitted
            var result = ExpressionValidator.Validate("3 + 3 + 7 + 9 + 11 + 15", StandardSet(), ValidationRules.Strict);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Strict_OrderOfOperationsExpression_IsValid()
        {
            // (3 + 5) * (9 - 7) + 11 - 15  — all six values present, each once
            var result = ExpressionValidator.Validate("(3 + 5) * (9 - 7) + 11 - 15", StandardSet(), ValidationRules.Strict);
            Assert.True(result.IsValid);
        }

        // ================================================================
        //  Strict — duplicate rolled values
        // ================================================================

        [Fact]
        public void Strict_TwoDiceWithSameValue_BothUsed_IsValid()
        {
            // Two D6s both showing 6
            var diceSet = MakeDiceSet((DiceType.D6, 6), (DiceType.D6, 6));
            var result = ExpressionValidator.Validate("6 + 6", diceSet, ValidationRules.Strict);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Strict_TwoDiceWithSameValue_OnlyOneUsed_Fails()
        {
            var diceSet = MakeDiceSet((DiceType.D6, 6), (DiceType.D6, 6));
            var result = ExpressionValidator.Validate("6", diceSet, ValidationRules.Strict);
            Assert.False(result.IsValid);
            Assert.Contains("6", result.FailureReason);
        }

        [Fact]
        public void Strict_TwoDiceWithSameValue_UsedThreeTimes_Fails()
        {
            var diceSet = MakeDiceSet((DiceType.D6, 6), (DiceType.D6, 6));
            var result = ExpressionValidator.Validate("6 + 6 + 6", diceSet, ValidationRules.Strict);
            Assert.False(result.IsValid);
        }

        // ================================================================
        //  SubsetAllowed — any subset of dice, each at most once
        // ================================================================

        [Fact]
        public void SubsetAllowed_UseAllDice_IsValid()
        {
            var result = ExpressionValidator.Validate("3 + 5 + 7 + 9 + 11 + 15", StandardSet(), ValidationRules.SubsetAllowed);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void SubsetAllowed_UseSubset_IsValid()
        {
            var result = ExpressionValidator.Validate("3 + 5", StandardSet(), ValidationRules.SubsetAllowed);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void SubsetAllowed_UseSingleDie_IsValid()
        {
            var result = ExpressionValidator.Validate("15", StandardSet(), ValidationRules.SubsetAllowed);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void SubsetAllowed_DieUsedTwice_Fails()
        {
            var result = ExpressionValidator.Validate("3 + 3", StandardSet(), ValidationRules.SubsetAllowed);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void SubsetAllowed_ValueNotInDiceSet_Fails()
        {
            var result = ExpressionValidator.Validate("3 + 42", StandardSet(), ValidationRules.SubsetAllowed);
            Assert.False(result.IsValid);
            Assert.Contains("42", result.FailureReason);
        }

        // ================================================================
        //  DoubleUse — all dice must appear, each usable up to twice
        // ================================================================

        [Fact]
        public void DoubleUse_AllDiceUsedOnce_IsValid()
        {
            var result = ExpressionValidator.Validate("3 + 5 + 7 + 9 + 11 + 15", StandardSet(), ValidationRules.DoubleUse);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void DoubleUse_SomeDiceUsedTwice_IsValid()
        {
            // 3 and 5 each used twice, rest once
            var result = ExpressionValidator.Validate("3 + 3 + 5 + 5 + 7 + 9 + 11 + 15", StandardSet(), ValidationRules.DoubleUse);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void DoubleUse_AllDiceUsedTwice_IsValid()
        {
            var result = ExpressionValidator.Validate("3 + 3 + 5 + 5 + 7 + 7 + 9 + 9 + 11 + 11 + 15 + 15", StandardSet(), ValidationRules.DoubleUse);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void DoubleUse_DieMissingEntirely_Fails()
        {
            // 15 never appears
            var result = ExpressionValidator.Validate("3 + 3 + 5 + 7 + 9 + 11", StandardSet(), ValidationRules.DoubleUse);
            Assert.False(result.IsValid);
            Assert.Contains("15", result.FailureReason);
        }

        [Fact]
        public void DoubleUse_DieUsedThreeTimes_Fails()
        {
            var result = ExpressionValidator.Validate("3 + 3 + 3 + 5 + 7 + 9 + 11 + 15", StandardSet(), ValidationRules.DoubleUse);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void DoubleUse_TwoDuplicateRolls_BothSlotsUsedOnce_IsValid()
        {
            // Two D6s both showing 6; each used once under DoubleUse
            var diceSet = MakeDiceSet((DiceType.D6, 6), (DiceType.D6, 6));
            var result = ExpressionValidator.Validate("6 + 6", diceSet, ValidationRules.DoubleUse);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void DoubleUse_TwoDuplicateRolls_FirstSlotUsedTwiceSecondOnce_IsValid()
        {
            // Two D6s both showing 6; 6 used three times total (2+1 across slots)
            var diceSet = MakeDiceSet((DiceType.D6, 6), (DiceType.D6, 6));
            var result = ExpressionValidator.Validate("6 + 6 + 6", diceSet, ValidationRules.DoubleUse);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void DoubleUse_TwoDuplicateRolls_UsedFourTimes_IsValid()
        {
            // Two D6s, each usable twice = four total uses of 6
            var diceSet = MakeDiceSet((DiceType.D6, 6), (DiceType.D6, 6));
            var result = ExpressionValidator.Validate("6 + 6 + 6 + 6", diceSet, ValidationRules.DoubleUse);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void DoubleUse_TwoDuplicateRolls_UsedFiveTimes_Fails()
        {
            var diceSet = MakeDiceSet((DiceType.D6, 6), (DiceType.D6, 6));
            var result = ExpressionValidator.Validate("6 + 6 + 6 + 6 + 6", diceSet, ValidationRules.DoubleUse);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void DoubleUse_TwoDuplicateRolls_OnlyUsedOnce_Fails()
        {
            // Second slot never touched
            var diceSet = MakeDiceSet((DiceType.D6, 6), (DiceType.D6, 6));
            var result = ExpressionValidator.Validate("6", diceSet, ValidationRules.DoubleUse);
            Assert.False(result.IsValid);
        }

        // ================================================================
        //  Relaxed — subset allowed, each die usable up to twice
        // ================================================================

        [Fact]
        public void Relaxed_SubsetUsedOnce_IsValid()
        {
            var result = ExpressionValidator.Validate("3 + 5", StandardSet(), ValidationRules.Relaxed);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Relaxed_SubsetWithDoubleUse_IsValid()
        {
            var result = ExpressionValidator.Validate("3 + 3 + 5 + 5", StandardSet(), ValidationRules.Relaxed);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Relaxed_DieUsedThreeTimes_Fails()
        {
            var result = ExpressionValidator.Validate("3 + 3 + 3", StandardSet(), ValidationRules.Relaxed);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Relaxed_ValueNotInDiceSet_Fails()
        {
            var result = ExpressionValidator.Validate("3 + 99", StandardSet(), ValidationRules.Relaxed);
            Assert.False(result.IsValid);
        }

        // ================================================================
        //  Unparseable expression
        // ================================================================

        [Fact]
        public void UnparseableExpression_Fails()
        {
            var result = ExpressionValidator.Validate("3 @ 5", StandardSet(), ValidationRules.Strict);
            Assert.False(result.IsValid);
            Assert.NotNull(result.FailureReason);
        }

        // ================================================================
        //  Null guard
        // ================================================================

        [Fact]
        public void NullExpression_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => ExpressionValidator.Validate((string)null!, StandardSet(), ValidationRules.Strict));
        }

        [Fact]
        public void NullDiceSet_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => ExpressionValidator.Validate("3 + 5", null!, ValidationRules.Strict));
        }

        [Fact]
        public void NullRules_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => ExpressionValidator.Validate("3 + 5", StandardSet(), null!));
        }
    }
}
