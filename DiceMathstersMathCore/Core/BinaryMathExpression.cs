using System;

namespace DiceMathsters.Core
{
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
