using DiceMathsters.Core;

namespace DiceMathsters.Application
{
    /// <summary>
    /// Provides static methods for evaluating mathematical expressions represented as strings or token lists.
    /// </summary>
    /// <remarks>Use the Evaluate methods to compute the result of an expression. The input expression must be
    /// valid; otherwise, exceptions may be thrown during evaluation. This class is thread-safe and can be used
    /// concurrently across multiple threads.</remarks>
    public static class ExpressionEvaluator
    {
        /// <summary>
        /// Evaluates a mathematical expression represented as a string and returns the computed result as a double.
        /// </summary>
        /// <remarks>This method uses a tokenizer to parse the expression before evaluation. Ensure that
        /// the expression does not contain unsupported characters or formats.</remarks>
        /// <param name="expression">The mathematical expression to evaluate. The expression must be valid and follow
        /// the syntax rules supported by the tokenizer.</param>
        /// <returns>The result of the evaluated expression as a double.</returns>
        public static double Evaluate(string expression)
            => Evaluate(Tokenizer.Tokenize(expression));

        /// <summary>
        /// Evaluates a mathematical expression represented by a list of tokens and returns the computed result.
        /// </summary>
        /// <remarks>This method constructs an expression from the provided tokens and evaluates it.
        /// Ensure that the tokens are correctly formatted to avoid evaluation errors.</remarks>
        /// <param name="tokens">The list of tokens representing the mathematical expression to evaluate. Each token
        /// must be valid and correspond to the expected syntax of the expression.</param>
        /// <returns>The result of the evaluated expression as a double.</returns>
        public static double Evaluate(IReadOnlyList<Token> tokens)
            => new ExpressionBuilder()
                .BuildExpression(tokens)
                .Evaluate();
    }
}
