using DiceMathsters.Domain.Dice;
using DiceMathsters.Domain.Game;
using DiceMathsters.Domain.Random;

namespace DiceMathsters.Application.Handlers
{
    /// <summary>
    /// Produces a <see cref="RoundSetup"/> from a game configuration.
    /// Server-side only — clients receive the resulting state directly.
    /// </summary>
    public static class RoundSetupHandler
    {
        /// <summary>
        /// Rolls the standard dice set, generates a target number, and packages
        /// the result into a <see cref="RoundSetup"/> ready for distribution
        /// to clients.
        /// </summary>
        /// <param name="profile">Controls the range of the generated target number.</param>
        /// <param name="rules">The dice usage policy for this round.</param>
        /// <param name="operations">The operators available to students this round.</param>
        /// <param name="random">Shared random provider — consumed first by the dice
        /// roll, then by target number generation.</param>
        /// <returns>A fully initialised <see cref="RoundSetup"/>.</returns>
        public static RoundSetup Setup(
            DifficultyProfile profile,
            ValidationRules rules,
            AllowedOperations operations,
            IRandomProvider randomProvider)
        {
            ArgumentNullException.ThrowIfNull(profile);
            ArgumentNullException.ThrowIfNull(rules);
            ArgumentNullException.ThrowIfNull(operations);
            ArgumentNullException.ThrowIfNull(randomProvider);

            var rolls = DiceSet.RollStandard(randomProvider);
            int target = TargetNumberGenerator.Generate(rolls.Values, profile, randomProvider);

            return new RoundSetup(rolls, target, rules, profile, operations);
        }
    }
}
