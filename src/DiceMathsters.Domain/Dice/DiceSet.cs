using System;
using System.Collections.Generic;
using System.Linq;
using DiceMathsters.Domain.Random;

namespace DiceMathsters.Domain.Dice
{
    /// <summary>
    /// Represents the complete set of dice rolled at the start of a round.
    /// Provides the pool of values students must use to build their expressions.
    /// Immutable after creation.
    /// </summary>
    public sealed class DiceSet
    {
        private readonly IReadOnlyList<DiceRoll> _rolls;

        /// <summary>All rolls in this set, in the order they were rolled.</summary>
        public IReadOnlyList<DiceRoll> Rolls => _rolls;

        /// <summary>The integer values of each roll, in roll order.</summary>
        public IReadOnlyList<int> Values => _rolls.Select(r => r.Value).ToList();

        private DiceSet(IReadOnlyList<DiceRoll> rolls)
        {
            _rolls = rolls;
        }

        /// <summary>
        /// Rolls one die of each specified type, in the order given.
        /// Server-side only — clients receive values directly rather than rolling.
        /// </summary>
        /// <param name="diceTypes">The dice to include in this set.</param>
        /// <param name="random">Shared random provider for all rolls.</param>
        public static DiceSet Roll(IEnumerable<DiceType> diceTypes, IRandomProvider random)
        {
            if (diceTypes == null) throw new ArgumentNullException(nameof(diceTypes));
            if (random == null)    throw new ArgumentNullException(nameof(random));

            var rolls = diceTypes
                .Select(dt => DiceRoll.Roll(dt, random))
                .ToList();

            if (rolls.Count == 0)
                throw new ArgumentException("At least one die type must be provided.", nameof(diceTypes));

            return new DiceSet(rolls);
        }

        /// <summary>
        /// Constructs a DiceSet from predetermined roll values. Used client-side
        /// to reconstruct server-authoritative round state from received values.
        /// </summary>
        /// <param name="rolls">Pre-built DiceRoll instances.</param>
        public static DiceSet FromRolls(IEnumerable<DiceRoll> rolls)
        {
            if (rolls == null) throw new ArgumentNullException(nameof(rolls));

            var list = rolls.ToList();
            if (list.Count == 0)
                throw new ArgumentException("At least one roll must be provided.", nameof(rolls));

            return new DiceSet(list);
        }

        /// <summary>
        /// Rolls the default competitive set: one each of D4, D6, D8, D10, D12, D20.
        /// Server-side only.
        /// </summary>
        public static DiceSet RollStandard(IRandomProvider random)
        {
            return Roll(new[]
            {
                DiceType.D4,
                DiceType.D6,
                DiceType.D8,
                DiceType.D10,
                DiceType.D12,
                DiceType.D20
            }, random);
        }

        public override string ToString() =>
            string.Join(", ", _rolls.Select(r => r.ToString()));
    }
}
