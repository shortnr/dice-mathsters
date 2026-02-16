using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceMathsters.Core
{
    public class UnaryMathExpression : MathExpression
    {
        public float Value { get; }

        public UnaryMathExpression(int value)
        {
            Value = value;
        }

        public override double Evaluate() => Value;
    }
}