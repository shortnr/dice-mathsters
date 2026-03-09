using System;
using System.Collections.Generic;

namespace DiceMathsters.Domain.Random
{
    /// <summary>
    /// Test implementation of <see cref="IRandomProvider"/> that returns a
    /// predetermined sequence of values, cycling when exhausted.
    /// Allows dice roll tests to be fully deterministic without mocking frameworks.
    /// </summary>
    public sealed class FixedRandomProvider : IRandomProvider
    {
        private readonly IReadOnlyList<int> _values;
        private int _index;

        /// <summary>
        /// Initializes with a sequence of values to return in order.
        /// </summary>
        /// <param name="values">
        /// Values returned by successive calls to <see cref="Next"/>. Cycles
        /// back to the first value once the sequence is exhausted.
        /// </param>
        public FixedRandomProvider(params int[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException("At least one value must be provided.", nameof(values));

            _values = values;
            _index  = 0;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Range validation is still enforced so that tests surface out-of-range
        /// values rather than silently accepting them.
        /// </remarks>
        public int Next(int minInclusive, int maxInclusive)
        {
            if (minInclusive > maxInclusive)
                throw new ArgumentOutOfRangeException(
                    nameof(minInclusive),
                    $"minInclusive ({minInclusive}) must be <= maxInclusive ({maxInclusive}).");

            int value = _values[_index % _values.Count];
            _index++;

            if (value < minInclusive || value > maxInclusive)
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"Fixed value {value} is outside the requested range [{minInclusive}, {maxInclusive}].");

            return value;
        }
    }
}
