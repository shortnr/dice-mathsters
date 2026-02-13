using System;
using System.Collections.Generic;
using System.Text;

namespace DiceMathsters.Core
{
    public class Tokenizer
    {
        public IReadOnlyList<Token> Tokenize(string expression)
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
                    _tokens.Add(new Token(TokenType.Number, number.ToString()));
                }
                else if ("+-*/^".Contains(current))
                {
                    _tokens.Add(new Token(TokenType.Operator, current.ToString()));
                    i++;
                }
                else if (current == '(')
                {
                    _tokens.Add(new Token(TokenType.LeftParenthesis, current.ToString()));
                    i++;
                }
                else if (current == ')')
                {
                    _tokens.Add(new Token(TokenType.RightParenthesis, current.ToString()));
                    i++;
                }
                else
                {
                    throw new Exception($"Unexpected character: {current}");
                }
            }
            return _tokens;
        }
    }
}
