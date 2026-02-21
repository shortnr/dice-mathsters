using DiceMathsters.Core;

namespace DiceMathsters.Application
{
    /// <summary>
    /// Provides static methods for evaluating mathematical expressions represented as tokens or strings.
    /// </summary>
    /// <remarks>This class is sealed and cannot be inherited. It offers two static methods for evaluating
    /// expressions: one that accepts a list of tokens and another that takes a string representation of the expression.
    /// Both methods utilize an internal expression builder to construct and evaluate the expression. Use this class to
    /// obtain the numerical result of a mathematical expression without needing to manually parse or evaluate the
    /// expression yourself.</remarks>
    public sealed class EvaluateExpressionHandler
    {
        /// <summary>
        /// Evaluates a mathematical expression represented by a sequence of tokens and returns the computed result.
        /// </summary>
        /// <remarks>The method requires that the token sequence forms a valid mathematical expression. If
        /// the tokens are invalid or do not represent a complete expression, an exception may be thrown during
        /// evaluation.</remarks>
        /// <param name="tokens">A read-only list of tokens that define the mathematical expression to evaluate. Each token must be valid and
        /// contribute to a well-formed expression.</param>
        /// <returns>The result of the evaluated expression as a double.</returns>
        public static double Handle(IReadOnlyList<Token> tokens)
        {
            var expressionBuilder = new ExpressionBuilder();
            var expression = expressionBuilder.BuildExpression(tokens);
            var result = expression.Evaluate();
            return result;
        }

        /// <summary>
        /// Evaluates a mathematical expression provided as a string and returns the computed result as a double.
        /// </summary>
        /// <remarks>This method parses the input string into tokens and constructs an evaluable
        /// expression. Ensure that the input string adheres to the expected format to avoid evaluation
        /// errors.</remarks>
        /// <param name="expressionString">The string representation of the mathematical expression to evaluate. The expression must be properly
        /// formatted and may include numbers, operators, and parentheses.</param>
        /// <returns>The result of the evaluated expression as a double.</returns>
        public static double HandleFromString(string expressionString)
        {
            var tokens = Tokenizer.Tokenize(expressionString);
            var expressionBuilder = new ExpressionBuilder();
            var expression = expressionBuilder.BuildExpression(tokens);
            var result = expression.Evaluate();
            return result;
        }
    }
}
