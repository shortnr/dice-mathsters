using System;

namespace DiceMathsters.Domain.Random
{
    /// <summary>
    /// Production implementation of <see cref="IRandomProvider"/> backed by
    /// <see cref="System.Random"/>. Instantiate once and reuse to avoid
    /// seed collisions from rapid construction.
    /// </summary>
    public sealed class SystemRandomProvider : IRandomProvider
    {
        private readonly System.Random _random;

        /// <summary>
        /// Initializes with a time-based seed.
        /// </summary>
        public SystemRandomProvider()
        {
            _random = new System.Random();
        }

        /// <summary>
        /// Initializes with an explicit seed. Useful for deterministic
        /// test scenarios or future debug/replay tooling.
        /// </summary>
        public SystemRandomProvider(int seed)
        {
            _random = new System.Random(seed);
        }

        /// <inheritdoc/>
        public int Next(int minInclusive, int maxInclusive)
        {
            if (minInclusive > maxInclusive)
                throw new ArgumentOutOfRangeException(
                    nameof(minInclusive),
                    $"minInclusive ({minInclusive}) must be <= maxInclusive ({maxInclusive}).");

            return _random.Next(minInclusive, maxInclusive + 1);
        }
    }
}
