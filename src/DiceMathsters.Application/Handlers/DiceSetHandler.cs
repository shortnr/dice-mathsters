using System;
using System.Collections.Generic;
using System.Linq;
using DiceMathsters.Domain.Dice;
using DiceMathsters.Domain.Random;

namespace DiceMathsters.Application.Handlers
{
    /// <summary>
    /// Provides static methods for creating <see cref="DiceSet"/> instances.
    /// </summary>
    /// <remarks>
    /// Two distinct concerns are handled here:
    /// <list type="bullet">
    ///   <item>
    ///     <b>Server-side:</b> rolling new dice sets for a round via
    ///     <see cref="Roll"/> and <see cref="RollStandard"/>.
    ///   </item>
    ///   <item>
    ///     <b>Client-side:</b> reconstructing a <see cref="DiceSet"/> from
    ///     values received in a server message via <see cref="FromServerMessage"/>.
    ///   </item>
    /// </list>
    /// </remarks>
    public static class DiceSetHandler
    {
        /// <summary>
        /// Rolls one die of each specified type and returns the resulting
        /// <see cref="DiceSet"/>. Server-side only.
        /// </summary>
        /// <param name="diceTypes">The dice to include in the set.</param>
        /// <param name="random">The random provider to use for all rolls.</param>
        public static DiceSet Roll(IEnumerable<DiceType> diceTypes, IRandomProvider random)
        {
            if (diceTypes == null) throw new ArgumentNullException(nameof(diceTypes));
            if (random    == null) throw new ArgumentNullException(nameof(random));

            return DiceSet.Roll(diceTypes, random);
        }

        /// <summary>
        /// Rolls the standard competitive set (D4, D6, D8, D10, D12, D20)
        /// and returns the resulting <see cref="DiceSet"/>. Server-side only.
        /// </summary>
        /// <param name="random">The random provider to use for all rolls.</param>
        public static DiceSet RollStandard(IRandomProvider random)
        {
            if (random == null) throw new ArgumentNullException(nameof(random));

            return DiceSet.RollStandard(random);
        }

        /// <summary>
        /// Reconstructs a <see cref="DiceSet"/> from a server message containing
        /// die types and their corresponding rolled values. Client-side only.
        /// </summary>
        /// <param name="message">
        /// A sequence of (DiceType, value) pairs as received from the server.
        /// </param>
        public static DiceSet FromServerMessage(IEnumerable<(DiceType DiceType, int Value)> message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var rolls = message
                .Select(m => DiceRoll.FromValue(m.DiceType, m.Value))
                .ToList();

            return DiceSet.FromRolls(rolls);
        }
    }
}
