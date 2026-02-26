using System;

namespace DiceMathsters.Domain.Exceptions
{
    internal class ExpressionException : Exception
    {
        public ExpressionException(string message) : base(message) { }
    }
}