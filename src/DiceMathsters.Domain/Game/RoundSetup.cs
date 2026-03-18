using DiceMathsters.Domain.Dice;
using System;

namespace DiceMathsters.Domain.Game
{
    /// <summary>
    /// Represents the complete server-authoritative state for a single round.
    /// Produced by <see cref="RoundSetupHandler"/> and distributed to all
    /// clients at the start of each round.
    /// </summary>
    /// <remarks>
    /// Immutable after creation. All fields are set server-side — clients
    /// reconstruct round state from received values rather than generating
    /// their own. Clients will only use this data for UI display and hints,
    /// never for validation or scoring, which are always performed server-side.
    /// </remarks>
    public sealed class RoundSetup
    {
        /// <summary>The dice rolled for this round. Students must use these
        /// values to construct their expressions.</summary>
        public DiceSet Dice { get; }

        /// <summary>The target number students are trying to reach.</summary>
        public int Target { get; }

        /// <summary>The validation policy in effect for this round, controlling
        /// how dice values must be used in submitted expressions.</summary>
        public ValidationRules Rules { get; }

        /// <summary>The difficulty profile used to generate the target number,
        /// controlling the range of possible targets relative to the rolled
        /// dice values.</summary>
        public DifficultyProfile Profile { get; }

        /// <summary>The operators students are permitted to use when building
        /// their expressions this round.</summary>
        public AllowedOperations Operations { get; }

        /// <summary>
        /// Initializes a new instance of the RoundSetup class with the specified
        /// dice set, target value, validation rules, difficulty profile, and
        /// allowed operations.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if dice, rules, profile,
        /// or operations is null.</exception>
        public RoundSetup(
            DiceSet dice,
            int target,
            ValidationRules rules,
            DifficultyProfile profile,
            AllowedOperations operations)
        {
            Dice = dice ?? throw new ArgumentNullException(nameof(dice));
            Target = target;
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));
            Operations = operations ?? throw new ArgumentNullException(nameof(operations));
        }
    }
}