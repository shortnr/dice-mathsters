namespace DiceMathsters.Domain.Random
{
    /// <summary>
    /// Abstraction over random number generation. Used server-side only —
    /// clients reconstruct game state from values distributed by the server
    /// rather than generating their own.
    /// </summary>
    public interface IRandomProvider
    {
        /// <summary>
        /// Returns a random integer in [minInclusive, maxInclusive].
        /// </summary>
        int Next(int minInclusive, int maxInclusive);
    }
}
