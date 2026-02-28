using System;

namespace DiceMathsters.Domain.Exceptions
{
    public class ExpressionException : Exception
    {
        public ExpressionException(string message) : base(message) { }
    }
}