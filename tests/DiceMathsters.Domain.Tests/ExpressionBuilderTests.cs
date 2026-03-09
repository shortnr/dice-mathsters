using DiceMathsters.Domain.Expressions;
using DiceMathsters.Domain.Exceptions;

using Xunit;

namespace DiceMathsters.Domain.Tests.Expressions
{
    public class ExpressionBuilderTests
    {
        // ============================================================
        //  VALID EXPRESSIONS
        // ============================================================

        // ----- Literals -----
        [Theory]
        [InlineData("3", 3)]
        [InlineData("-3", -3)]
        public void ExpressionBuilder_Literals(string expressionString, double expected)
        {
            EvaluateAndAssert(expressionString, expected);
        }

        // ----- Unary Operators -----
        [Theory]
        [InlineData("2--3", 5)]
        [InlineData("2+-3", -1)]
        [InlineData("-(3 + 4)", -7)]
        [InlineData("-(2 + (3 + 4))", -9)]
        [InlineData("-2(3)", -6)]
        [InlineData("-(2)(3)", -6)]
        public void ExpressionBuilder_Unary(string expressionString, double expected)
        {
            EvaluateAndAssert(expressionString, expected);
        }

        // ----- Basic Binary Operators (+, -, *, /) -----
        [Theory]
        [InlineData("3 + 4 + 1", 8)]
        [InlineData("2 * 3 + 4", 10)]
        [InlineData("2 + 3 * 4", 14)]
        [InlineData("2 + 3 * 4 - 5", 9)]
        [InlineData("10 / 2 - 3", 2)]
        [InlineData("10 / (2 - 3)", -10)]
        [InlineData("8 / 4 / 2", 1)]  // left associative
        [InlineData("8 / 4 * 2", 4)]
        public void ExpressionBuilder_BasicBinaryOperators(string expressionString, double expected)
        {
            EvaluateAndAssert(expressionString, expected);
        }

        // ----- Exponentiation (precedence + right associativity) -----
        [Theory]
        [InlineData("2^2^3", 256)]       // right associative
        [InlineData("(2^2)^3", 64)]
        [InlineData("-2^2", -4)]         // unary lower than exponent
        [InlineData("(-2)^2", 4)]
        [InlineData("2^-3", 0.125)]
        [InlineData("2^-2^2", 0.0625)]
        public void ExpressionBuilder_Exponentiation(string expressionString, double expected)
        {
            EvaluateAndAssert(expressionString, expected);
        }

        // ----- Implicit Multiplication -----
        [Theory]
        [InlineData("(2+3)*(4+5)", 45)]
        [InlineData("(2 + 3)(4 + 5)", 45)]
        [InlineData("2(3 + 4)", 14)]
        [InlineData("(2 + 3)4", 20)]
        [InlineData("2(3)(4)", 24)]
        public void ExpressionBuilder_ImplicitMultiplication(string expressionString, double expected)
        {
            EvaluateAndAssert(expressionString, expected);
        }

        // ----- Mixed Precedence Stress Tests -----
        [Theory]
        [InlineData("3 + 4 * 2 / (1 - 5)", 1)]
        [InlineData("3 + 4 * 2 / (1 - 5) ^ 2", 3.5)]
        [InlineData("3 + 4 * 2 ^ 2 / (1 - 5) ^ 2", 4)]
        [InlineData("3 + (4 * 2) ^ 2 / (1 - 5) ^ 2", 7)]
        public void ExpressionBuilder_MixedPrecedence(string expressionString, double expected)
        {
            EvaluateAndAssert(expressionString, expected);
        }

        // ----- Deep Nesting -----
        [Theory]
        [InlineData("((((3))))", 3)]
        [InlineData("((2+3)*(1+(2)))", 15)]
        public void ExpressionBuilder_DeepNesting(string expressionString, double expected)
        {
            EvaluateAndAssert(expressionString, expected);
        }

        // ----- Special Numeric Edge Cases -----
        [Theory]
        [InlineData("0^0", 1)]
        [InlineData("0^-1", double.PositiveInfinity)]
        public void ExpressionBuilder_SpecialNumericCases(string expressionString, double expected)
        {
            EvaluateAndAssert(expressionString, expected);
        }


        // ============================================================
        //  INVALID EXPRESSIONS
        // ============================================================

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
        [InlineData("2^^3")]
        [InlineData("2()")]
        [InlineData(")")]
        [InlineData("(")]
        [InlineData("--")]
        [InlineData("^-2")]
        [InlineData("2^")]
        public void ExpressionBuilder_InvalidExpressions(string expressionString)
        {
            Assert.Throws<ExpressionException>(() => new ExpressionEvaluator().EvaluateExpression(expressionString));
        }


        // ============================================================
        //  Helper
        // ============================================================

        private static void EvaluateAndAssert(string expressionString, double expected)
        {
            Assert.Equal(expected, new ExpressionEvaluator().EvaluateExpression(expressionString), 10);
        }
    }
}
