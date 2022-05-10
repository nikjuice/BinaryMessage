using System;

namespace BinaryMessage.Exceptions
{
    public class BinaryMessageValidationException: BinaryBaseException
    {
        public BinaryMessageValidationException(string message) : base(message)
        {

        }
    }
}