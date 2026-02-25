using System;
using System.Collections.Generic;
using System.Text;
using DiceMathsters.Domain.ExpressionBuilder;

namespace DiceMathsters.Domain.Tokenizer
{
    public enum TokenType
    {
        Number,
        Operator,
        LeftParenthesis,
        RightParenthesis,
        End
    }

    public class Token
    {
        public TokenType Type { get; }
        public OperatorType? Op { get; }
        public int? Num { get; }

        private Token(TokenType type, OperatorType? op = null, int? number = null)
        {
            Type = type;
            Op = op;
            Num = number;
        }

        public static Token Number(int value)
            => new(TokenType.Number, number: value);

        public static Token Operator(OperatorType op)
            => new(TokenType.Operator, op: op);

        public static Token LeftParen()
            => new(TokenType.LeftParenthesis);

        public static Token RightParen()
            => new(TokenType.RightParenthesis);

        public static Token End()
            => new(TokenType.Operator, op: null);
    }
}
