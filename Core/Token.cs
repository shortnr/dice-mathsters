using System;
using System.Collections.Generic;
using System.Text;

namespace DiceMathsters.Core
{
    public enum TokenType
    {
        Number,
        Operator,
        LeftParenthesis,
        RightParenthesis
    }
    public class Token
    {
            public TokenType Type { get; }
            public string Value { get; }
    
            public Token(TokenType type, string value)
            {
                Type = type;
                Value = value;
        }
    }
}
