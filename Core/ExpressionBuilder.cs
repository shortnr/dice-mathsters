using System;
using System.Collections.Generic;
using System.Text;

namespace DiceMathsters.Core
{
    public class ExpressionBuilder
    {
        public MathExpression BuildExpressionFromTokens(IReadOnlyList<Token> tokens)
        {
            bool lastTokenWasRightParenthesis = false;
            bool lastTokenWasNumber = false;

            Stack<MathExpression> output = new();
            Stack<Token> operators = new();
            foreach (var token in tokens)
            {
                if (token.Type == TokenType.Number)
                {
                    if (lastTokenWasNumber)
                        throw new Exception("Invalid expression: two numbers in a row without an operator.");

                    if (lastTokenWasRightParenthesis)
                        operators.Push(new Token(TokenType.Operator, "*"));
                    
                    output.Push(new UnaryMathExpression(int.Parse(token.Value)));

                    lastTokenWasRightParenthesis = false;
                    lastTokenWasNumber = true;
                }
                else if (token.Type == TokenType.Operator)
                {
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
                }
                else if (token.Type == TokenType.LeftParenthesis)
                {
                    if (lastTokenWasRightParenthesis || lastTokenWasNumber)
                        operators.Push(new Token(TokenType.Operator, "*"));
                    
                    operators.Push(token);

                    lastTokenWasRightParenthesis = false;
                    lastTokenWasNumber = false;
                }
                else if (token.Type == TokenType.RightParenthesis)
                {
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
                }
            }
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
