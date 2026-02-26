using System;
using System.Collections.Generic;
using System.Text;
using DiceMathsters.Domain.Expressions;

namespace DiceMathsters.Domain.Tokenizer
{
    internal class StringTokenizer
    {
        internal static IReadOnlyList<Token> Tokenize(string expression)
        {
            List<Token> _tokens = new();

            int i = 0;
            while (i < expression.Length)
            {
                char current = expression[i];
                if (char.IsWhiteSpace(current))
                {
                    i++;
                    continue;
                }
                if (char.IsDigit(current))
                {
                    StringBuilder number = new();
                    while (i < expression.Length && char.IsDigit(expression[i]))
                    {
                        number.Append(expression[i]);
                        i++;
                    }
                    _tokens.Add(Token.Number(int.Parse(number.ToString())));
                }
                else if (current == '+')
                {
                    _tokens.Add(Token.Operator(OperatorType.Add));
                    i++;
                }
                else if (current == '-')
                {
                    _tokens.Add(Token.Operator(OperatorType.Subtract));
                    i++;
                }
                else if (current == '*')
                {
                    _tokens.Add(Token.Operator(OperatorType.Multiply));
                    i++;
                }
                else if (current == '/')
                {
                    _tokens.Add(Token.Operator(OperatorType.Divide));
                    i++;
                }
                else if (current == '^')
                {
                    _tokens.Add(Token.Operator(OperatorType.Power));
                    i++;
                }
                else if (current == '(')
                {
                    _tokens.Add(Token.LeftParen());
                    i++;
                }
                else if (current == ')')
                {
                    _tokens.Add(Token.RightParen());
                    i++;
                }
                else
                {
                    throw new Exception($"Unexpected character: {current}");
                }
            }

            _tokens.Add(Token.End());

            return _tokens;
        }
    }
}
