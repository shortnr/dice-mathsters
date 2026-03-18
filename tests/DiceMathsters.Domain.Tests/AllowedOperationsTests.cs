using DiceMathsters.Domain.Game;
using Xunit;

namespace DiceMathsters.Domain.Tests.Game
{
    public class AllowedOperationsTests
    {
        // ================================================================
        //  Construction
        // ================================================================

        [Fact]
        public void Constructor_AllFlagsTrue_Succeeds()
        {
            var ops = new AllowedOperations(
                addition: true, subtraction: true,
                multiplication: true, division: true,
                exponentiation: true);

            Assert.True(ops.Addition);
            Assert.True(ops.Subtraction);
            Assert.True(ops.Multiplication);
            Assert.True(ops.Division);
            Assert.True(ops.Exponentiation);
        }

        [Fact]
        public void Constructor_AllFlagsFalse_Throws()
        {
            // At least one operation must be allowed — an empty set is nonsensical
            Assert.Throws<ArgumentException>(() =>
                new AllowedOperations(false, false, false, false, false));
        }

        // ================================================================
        //  Built-in presets
        // ================================================================

        [Fact]
        public void AddSubtract_OnlyAdditionAndSubtraction()
        {
            var ops = AllowedOperations.AddSubtract;

            Assert.True(ops.Addition);
            Assert.True(ops.Subtraction);
            Assert.False(ops.Multiplication);
            Assert.False(ops.Division);
            Assert.False(ops.Exponentiation);
        }

        [Fact]
        public void Standard_AddSubtractMultiplyDivide_NoExponentiation()
        {
            var ops = AllowedOperations.Standard;

            Assert.True(ops.Addition);
            Assert.True(ops.Subtraction);
            Assert.True(ops.Multiplication);
            Assert.True(ops.Division);
            Assert.False(ops.Exponentiation);
        }

        [Fact]
        public void Full_AllOperationsEnabled()
        {
            var ops = AllowedOperations.Full;

            Assert.True(ops.Addition);
            Assert.True(ops.Subtraction);
            Assert.True(ops.Multiplication);
            Assert.True(ops.Division);
            Assert.True(ops.Exponentiation);
        }
    }
}