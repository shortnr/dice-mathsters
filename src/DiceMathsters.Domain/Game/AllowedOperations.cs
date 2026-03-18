using System;
namespace DiceMathsters.Domain.Game
{
    /// <summary>
    /// Defines which mathematical operators students may use when constructing
    /// expressions during a round.
    /// </summary>
    /// <remarks>
    /// Configured per game session alongside <see cref="ValidationRules"/> and
    /// <see cref="DifficultyProfile"/>. The three axes are independent — any
    /// combination is valid, though sensible pairings are expected to emerge
    /// from playtesting.
    ///
    /// At least one operation must be enabled; an all-false configuration is
    /// rejected at construction time.
    /// </remarks>
    public sealed class AllowedOperations
    {
        /// <summary>Whether the addition operator (+) is permitted.</summary>
        public bool Addition { get; }

        /// <summary>Whether the subtraction operator (-) is permitted.</summary>
        public bool Subtraction { get; }

        /// <summary>Whether the multiplication operator (*) is permitted.</summary>
        public bool Multiplication { get; }

        /// <summary>Whether the division operator (/) is permitted.</summary>
        public bool Division { get; }

        /// <summary>Whether the exponentiation operator (^) is permitted.</summary>
        public bool Exponentiation { get; }

        /// <summary>
        /// Initializes a new <see cref="AllowedOperations"/> instance.
        /// At least one operation must be enabled.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when all flags are <c>false</c>.
        /// </exception>
        public AllowedOperations(bool addition, bool subtraction,
            bool multiplication, bool division, bool exponentiation)
        {
            if (!addition && !subtraction && !multiplication && !division && !exponentiation)
                throw new ArgumentException(
                    "At least one operation must be allowed.");

            Addition = addition;
            Subtraction = subtraction;
            Multiplication = multiplication;
            Division = division;
            Exponentiation = exponentiation;
        }

        // ----------------------------------------------------------------
        //  Built-in presets
        // ----------------------------------------------------------------

        /// <summary>
        /// Addition and subtraction only. Suited to the youngest players or
        /// introductory sessions where multiplication has not yet been taught.
        /// </summary>
        public static readonly AllowedOperations AddSubtract =
            new(addition: true, subtraction: true,
                multiplication: false, division: false,
                exponentiation: false);

        /// <summary>
        /// Addition, subtraction, multiplication, and division. The default
        /// for most classroom sessions.
        /// </summary>
        public static readonly AllowedOperations Standard =
            new(addition: true, subtraction: true,
                multiplication: true, division: true,
                exponentiation: false);

        /// <summary>
        /// All five operators including exponentiation. For experienced players
        /// comfortable with order-of-operations reasoning.
        /// </summary>
        public static readonly AllowedOperations Full =
            new(addition: true, subtraction: true,
                multiplication: true, division: true,
                exponentiation: true);
    }
}