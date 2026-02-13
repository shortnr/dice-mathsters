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

        public float Result { get; set; }

        public BinaryMathExpression(OperationType type, MathExpression opA, MathExpression opB)
        {
            this.type = type;
            this.opA = opA;
            this.opB = opB;
        }

        public override double Evaluate()
        {
            return type switch
            {
                OperationType.Add => opA.Evaluate() + opB.Evaluate(),
                OperationType.Subtract => opA.Evaluate() - opB.Evaluate(),
                OperationType.Multiply => opA.Evaluate() * opB.Evaluate(),
                OperationType.Divide => opB.Evaluate() == 0 ? throw new DivideByZeroException("Cannot divide by zero.") : opA.Evaluate() / opB.Evaluate(),
                OperationType.Exponentiate => Math.Pow(opA.Evaluate(), opB.Evaluate()),
                _ => throw new NotImplementedException()
            };
        }
    }
}
