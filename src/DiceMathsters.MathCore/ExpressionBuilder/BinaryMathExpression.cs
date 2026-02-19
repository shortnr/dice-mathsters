using System;

namespace DiceMathsters.Core
{
    public class BinaryMathExpression : MathExpression
    {
        private readonly OperatorType type;
        private readonly MathExpression opA;
        private readonly MathExpression opB;

        public double Result { get; set; }

        public BinaryMathExpression(OperatorType type, MathExpression opA, MathExpression opB)
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
                OperatorType.Add => resA + resB,
                OperatorType.Subtract => resA - resB,
                OperatorType.Multiply => resA * resB,
                OperatorType.Divide => resB == 0 ? throw new DivideByZeroException("Cannot divide by zero.") : resA / resB,
                OperatorType.Power => Math.Pow(resA, resB),
                _ => throw new NotImplementedException()
            };
        }
    }
}
