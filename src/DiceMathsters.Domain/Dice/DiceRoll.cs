using System;
using DiceMathsters.Domain.Random;

namespace DiceMathsters.Domain.Dice
{
    /// <summary>
    /// Represents a single die roll: the type of die and the value that was rolled.
    /// Immutable — create a new instance for each roll.
    /// </summary>
    public sealed class DiceRoll
    {
        /// <summary>The type of die that was rolled.</summary>
        public DiceType DiceType { get; }

        /// <summary>The value produced by the roll. Always in [1, (int)DiceType].</summary>
        public int Value { get; }

        private DiceRoll(DiceType diceType, int value)
        {
            DiceType = diceType;
            Value    = value;
        }

        /// <summary>
        /// Rolls a single die of the given type using the provided random provider.
        /// Server-side only — clients receive values directly rather than rolling.
        /// </summary>
        /// <param name="diceType">The type of die to roll.</param>
        /// <param name="random">The random provider to use.</param>
        public static DiceRoll Roll(DiceType diceType, IRandomProvider random)
        {
            if (random == null) throw new ArgumentNullException(nameof(random));

            int faces = (int)diceType;
            int value = random.Next(1, faces); // [1, faces]
            return new DiceRoll(diceType, value);
        }

        /// <summary>
        /// Creates a DiceRoll with a predetermined value. Useful for testing and
        /// replaying round state received from the server.
        /// </summary>
        /// <param name="diceType">The type of die.</param>
        /// <param name="value">The face value. Must be in [1, (int)diceType].</param>
        public static DiceRoll FromValue(DiceType diceType, int value)
        {
            int faces = (int)diceType;
            if (value < 1 || value > faces)
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"Value {value} is out of range for {diceType} (1–{faces}).");

            return new DiceRoll(diceType, value);
        }

        public override string ToString() => $"{DiceType}={Value}";
    }
}
