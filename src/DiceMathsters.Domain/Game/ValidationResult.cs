using System;

namespace DiceMathsters.Domain.Game
{
    /// <summary>
    /// The result of an <see cref="ExpressionValidator"/> check.
    /// Either a success or a failure with a human-readable reason.
    /// </summary>
    public sealed class ValidationResult
    {
        /// <summary>Whether the expression passed validation.</summary>
        public bool IsValid { get; }

        /// <summary>
        /// When <see cref="IsValid"/> is <c>false</c>, a description of why
        /// validation failed. <c>null</c> on success.
        /// </summary>
        public string? FailureReason { get; }

        private ValidationResult(bool isValid, string? failureReason)
        {
            IsValid       = isValid;
            FailureReason = failureReason;
        }

        /// <summary>Creates a passing result.</summary>
        public static ValidationResult Ok() => new(true, null);

        /// <summary>Creates a failing result with a reason.</summary>
        /// <param name="reason">A human-readable explanation of the failure.</param>
        public static ValidationResult Fail(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Failure reason must not be empty.", nameof(reason));

            return new(false, reason);
        }

        public override string ToString() =>
            IsValid ? "Valid" : $"Invalid: {FailureReason}";
    }
}
