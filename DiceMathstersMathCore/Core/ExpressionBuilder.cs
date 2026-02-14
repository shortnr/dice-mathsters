using System;
using System.Collections.Generic;
using System.Text;

namespace DiceMathsters.Core
{
    public class ExpressionBuilder
    {
        /// <summary>
        /// Builds a mathematical expression tree from a sequence of tokens representing numbers, operators, and
        /// parentheses. Uses the Shunting Yard algorithm to handle operator precedence and associativity.
        /// </summary>
        /// <remarks>
        /// The method processes tokens according to standard operator precedence and supports
        /// implicit multiplication (e.g., between a number and a parenthesis). Ensure that the input tokens form a
        /// syntactically valid expression to avoid exceptions.
        /// </remarks>
        /// <param name="tokens">
        /// The list of tokens to process. The tokens must represent a valid mathematical expression using supported
        /// operators and parentheses.
        /// </param>
        /// <returns>
        /// A MathExpression that represents the constructed mathematical expression based on the provided tokens.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown if the expression is invalid, such as when parentheses are mismatched, there are insufficient
        /// operands for an operator, or an unsupported token type is encountered.
        /// </exception>
        public MathExpression BuildExpressionFromTokens(IReadOnlyList<Token> tokens)
        {
            // Track the last token type to handle implicit multiplication and validate the expression structure
            bool lastTokenWasRightParenthesis = false;
            bool lastTokenWasNumber = false;

            // Stacks for output expressions and operators
            Stack<MathExpression> output = new();
            Stack<Token> operators = new();

            // Process each token in the input list
            foreach (var token in tokens)
            {
                // Handle the token based on its type
                switch (token.Type)
                {
                    // If the token is a number, create a UnaryMathExpression and push it to the output stack
                    case TokenType.Number:
                        if (lastTokenWasNumber)
                            throw new Exception("Invalid expression: two numbers in a row without an operator.");

                        if (lastTokenWasRightParenthesis)
                            operators.Push(new Token(TokenType.Operator, "*"));

                        output.Push(new UnaryMathExpression(int.Parse(token.Value)));

                        lastTokenWasRightParenthesis = false;
                        lastTokenWasNumber = true;
                        break;

                    // If the token is an operator, pop operators from the stack to the output stack based on precedence
                    case TokenType.Operator:
                        while (operators.Count > 0 && operators.Peek().Type == TokenType.Operator &&
                           GetPrecedence(operators.Peek().Value) >= GetPrecedence(token.Value))
                        {
                            var op = operators.Pop();
                            try
                            {
                                var right = output.Pop();
                                var left = output.Pop();
                                output.Push(new BinaryMathExpression(GetOperationType(op.Value), left, right));
                            }
                            catch
                            {
                                throw new Exception("Invalid expression: not enough operands for operator " + op.Value);
                            }
                        }
                        operators.Push(token);

                        lastTokenWasRightParenthesis = false;
                        lastTokenWasNumber = false;
                        break;

                    // If the token is a left parenthesis, push it to the operator stack. Handle implicit multiplication if necessary.
                    case TokenType.LeftParenthesis:
                        if (lastTokenWasRightParenthesis || lastTokenWasNumber)
                            operators.Push(new Token(TokenType.Operator, "*"));

                        operators.Push(token);

                        lastTokenWasRightParenthesis = false;
                        lastTokenWasNumber = false;
                        break;

                    // If the token is a right parenthesis, pop operators to the output stack until a left parenthesis is encountered
                    case TokenType.RightParenthesis:
                        while (operators.Count > 0 && operators.Peek().Type != TokenType.LeftParenthesis)
                        {
                            var op = operators.Pop();
                            try
                            {
                                var right = output.Pop();
                                var left = output.Pop();
                                output.Push(new BinaryMathExpression(GetOperationType(op.Value), left, right));
                            }
                            catch
                            {
                                throw new Exception("Invalid expression: not enough operands for operator " + op.Value);
                            }
                        }
                        if (operators.Count == 0 || operators.Peek().Type != TokenType.LeftParenthesis)
                        {
                            throw new Exception("Mismatched parentheses.");
                        }
                        operators.Pop(); // Pop the left parenthesis

                        lastTokenWasRightParenthesis = true;
                        lastTokenWasNumber = false;
                        break;

                    // If the token type is not recognized, throw an exception
                    default:
                        throw new Exception($"Invalid token type: {token.Type}");
                }
            }

            // After processing all tokens, fill the output stack with any remaining operators and their operands
            while (operators.Count > 0)
            {
                var op = operators.Pop();
                try
                {
                    var right = output.Pop();
                    var left = output.Pop();
                    output.Push(new BinaryMathExpression(GetOperationType(op.Value), left, right));
                }
                catch
                {
                    throw new Exception($"Invalid expression: not enough operands for operator {op}");
                }
            }
            try
            {
                return output.Pop();
            }
            catch
            {
                throw new Exception("Invalid expression: no output expression.");
            }
        }

        /// <summary>
        /// Determines the precedence level of the specified mathematical operator.
        /// </summary>
        /// <remarks>
        /// Operators with higher precedence values are evaluated before those with lower values.
        /// Operators with the same precedence value are evaluated according to their associativity rules, which are not
        /// determined by this method.
        /// </remarks>
        /// <param name="op">
        /// The operator for which to determine precedence. Valid values include "+", "-", "*", "/", and "^".
        /// </param>
        /// <returns>
        /// An integer representing the precedence level of the operator. Returns 0 if the operator is not recognized.
        /// </returns>
        private int GetPrecedence(string op)
        {
            return op switch
            {
                "+" => 1,
                "-" => 1,
                "*" => 2,
                "/" => 2,
                "^" => 3,
                _ => 0
            };
        }

        /// <summary>
        /// Determines the corresponding operation type for a given operator string.
        /// </summary>
        /// <remarks>
        /// Use this method to map a string representation of a mathematical operator to its
        /// corresponding <see cref="OperationType"/> enumeration value. Ensure that the input string matches one of the
        /// supported operators to avoid exceptions.
        /// </remarks>
        /// <param name="op">
        /// The operator string representing the mathematical operation. Valid values are "+", "-", "*", "/", and "^".
        /// </param>
        /// <returns>
        /// The <see cref="OperationType"/> value that represents the specified operator.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown if <paramref name="op"/> is not a recognized operator.
        /// </exception>
        private OperationType GetOperationType(string op)
        {
            return op switch
            {
                "+" => OperationType.Add,
                "-" => OperationType.Subtract,
                "*" => OperationType.Multiply,
                "/" => OperationType.Divide,
                "^" => OperationType.Exponentiate,
                _ => throw new Exception($"Invalid operator: {op}")
            };
        }
    }
}
