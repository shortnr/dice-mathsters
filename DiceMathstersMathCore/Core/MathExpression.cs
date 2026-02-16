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
}
