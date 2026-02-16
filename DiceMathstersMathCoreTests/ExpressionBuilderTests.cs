using DiceMathsters.Core;
using Xunit;

namespace DiceMathstersCoreTests
{
    public class ExpressionBuilderTests
    {
        [Theory]
        [InlineData("3", 3)]
        [InlineData("-3", -3)]
        [InlineData("2--3", 5)]
        [InlineData("2+-3", -1)]
        [InlineData("-(3 + 4)", -7)]
        [InlineData("-(2 + (3 + 4))", -9)]
        [InlineData("2^-3", 0.125)]
        [InlineData("3 + 4 + 1", 8.0)]
        [InlineData("2 * 3 + 4", 10.0)]
        [InlineData("2 + 3 * 4", 14.0)]
        [InlineData("2 + 3 * 4 - 5", 9.0)]
        [InlineData("(2 + 3) * 4", 20.0)]
        [InlineData("10 / 2 - 3", 2.0)]
        [InlineData("10 / (2 - 3)", -10.0)]
        [InlineData("3 + 4 * 2 / (1 - 5)", 1.0)]
        [InlineData("3 + 4 * 2 / (1 - 5) ^ 2", 3.5)]
        [InlineData("3 + 4 * 2 ^ 2 / (1 - 5) ^ 2", 4.0)]
        [InlineData("3 + (4 * 2) ^ 2 / (1 - 5) ^ 2", 7.0)]
        [InlineData("(2+3)*(4+5)", 45.0)]
        [InlineData("(2 + 3)(4 + 5)", 45)]
        [InlineData("2(3 + 4)", 14)]
        [InlineData("(2 + 3)4", 20)]
        [InlineData("2(3)(4)", 24)]
        public void ExpressionBuilder_FromStringPass(string expressionString, double expectedResult)
        {
            Tokenizer tokenizer = new();
            IReadOnlyList<Token> tokens = tokenizer.Tokenize(expressionString);

            ExpressionBuilder expressionBuilder = new();
            MathExpression expression = expressionBuilder.BuildExpressionFromTokens(tokens);
            Assert.Equal(expectedResult, expression.Evaluate());
        }

        [Theory]
        [InlineData("3 + + 4")]
        [InlineData("2 * (3 + 4")]
        [InlineData("2 + 3) * 4")]
        [InlineData("2 +")]
        [InlineData("* 3 4")]
        [InlineData("(2 + 3")]
        [InlineData("2 + )3(")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("2-*3")]
        public void ExpressionBuilder_FromStringFail(string expressionString)
        {
            Tokenizer tokenizer = new();
            IReadOnlyList<Token> tokens = tokenizer.Tokenize(expressionString);
            ExpressionBuilder expressionBuilder = new();
            Assert.Throws<Exception>(() => expressionBuilder.BuildExpressionFromTokens(tokens));
        }
    }
}
