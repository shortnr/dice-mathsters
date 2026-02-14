using System;
using System.Collections.Generic;
using System.Text;

namespace DiceMathsters.Core
{
    public enum OperationType
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Exponentiate
    }

    public abstract class MathExpression
    {
        public abstract double Evaluate();
    }

    public class UnaryMathExpression : MathExpression
    {
        public float Value { get; }

        public UnaryMathExpression(int value)
        {
            Value = value;
        }

        public override double Evaluate() => Value;
    }

    public class BinaryMathExpression : MathExpression
    {
        private readonly OperationType type;
        private readonly MathExpression opA;
        private readonly MathExpression opB;

        public double Result { get; set; }

        public BinaryMathExpression(OperationType type, MathExpression opA, MathExpression opB)
        {
            this.type = type;
            this.opA = opA;
            this.opB = opB;
        }

        public override double Evaluate()
        {
            double resA = opA.Evaluate();
            double resB = opB.Evaluate();

            return type switch
            {
                OperationType.Add => resA + resB,
                OperationType.Subtract => resA - resB,
                OperationType.Multiply => resA * resB,
                OperationType.Divide => resB == 0 ? throw new DivideByZeroException("Cannot divide by zero.") : resA / resB,
                OperationType.Exponentiate => Math.Pow(resA, resB),
                _ => throw new NotImplementedException()
            };
        }
    }
}
