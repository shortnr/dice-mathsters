using System;
using System.Collections.Generic;

namespace DiceMathsters.Domain.ExpressionBuilder
{
    public class ParserState
    {
        private readonly IReadOnlyList<Token> _tokens;
        public int Position { get; private set; } = 0;
        
        public ParserState(IReadOnlyList<Token> tokens)
        {
            _tokens = tokens;
        }

        public void Reset() => Position = 0;

        public Token Current => Position < _tokens.Count ? _tokens[Position] : _tokens[^1];
        public Token Previous => Position > 0 ? _tokens[Position - 1] : _tokens[^1];
        public Token Next => Position + 1 < _tokens.Count ? _tokens[Position + 1] : _tokens[^1];

        public Token Advance()
        {
            var token = Current;
            if (Position < _tokens.Count)
                Position++;
            return token;
        }

        public bool Match(params OperatorType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        public bool IsCurrentNumber()
        {
            return Current.Type == TokenType.Number;
        }

        public bool IsCurrentLeftParenthesis()
        {
            return Current.Type == TokenType.LeftParenthesis;
        }

        public bool Check(OperatorType type)
        {
            return Current.Op == type;
        }

        public Token Consume(TokenType type, string errorMessage)
        {
            if (Current.Type == type)
                return Advance();

            throw new Exception(errorMessage);
        }

        public bool Done => Position >= _tokens.Count - 1;
    }

    public class ExpressionBuilder
    {
        public MathExpression BuildExpression(IReadOnlyList<Token> tokens)
        {
            var state = new ParserState(tokens);

            var expr = ParseExpression(state);

            if (!state.Done)
                throw new Exception("Unexpected tokens at end.");

            return expr;
        }

        private MathExpression ParseExpression(ParserState state)
        {
            var expr = ParseTerm(state);

            while (state.Match(OperatorType.Add, OperatorType.Subtract))
            {
                var op = state.Previous;
                var right = ParseTerm(state);

                expr = new BinaryMathExpression(op.Op.GetValueOrDefault(), expr, right);
            }

            return expr;
        }

        private MathExpression ParseTerm(ParserState state)
        {
            var expr = ParseUnary(state);

            while (true)
            {
                if (state.Match(OperatorType.Multiply, OperatorType.Divide))
                {
                    var op = state.Previous;
                    var right = ParseUnary(state);
                    expr = new BinaryMathExpression(op.Op.GetValueOrDefault(), expr, right);
                }
                else if (state.IsCurrentNumber() || state.IsCurrentLeftParenthesis())
                {
                    var right = ParseUnary(state);
                    expr = new BinaryMathExpression(OperatorType.Multiply, expr, right);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private MathExpression ParsePower(ParserState state)
        {
            var left = ParsePrimary(state);

            if (state.Match(OperatorType.Power))
            {
                var op = state.Previous;

                var right = ParseUnary(state);

                return new BinaryMathExpression(op.Op.GetValueOrDefault(), left, right);
            }

            return left;
        }

        private MathExpression ParseUnary(ParserState state)
        {
            if (state.Match(OperatorType.Subtract))
            {
                return new NegatedMathExpression(ParseUnary(state));
            }

            return ParsePower(state);
        }

        private MathExpression ParsePrimary(ParserState state)
        {
            if (state.IsCurrentNumber())
            {
                var numberToken = state.Current;
                state.Advance();
                return new UnaryMathExpression(numberToken.Num.GetValueOrDefault());
            }

            if (state.IsCurrentLeftParenthesis())
            {
                state.Advance();
                var expr = ParseExpression(state);
                state.Consume(TokenType.RightParenthesis,
                    "Expected ')' after expression.");
                return expr;
            }

            throw new Exception("Unexpected token.");
        }
    }
}
