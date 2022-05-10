using System;

namespace BinaryMessage.Exceptions
{
    public class BinaryMessageGeneralException : BinaryBaseException
    {
        public BinaryMessageGeneralException(string message) : base(message)
        {
            
        }
        public BinaryMessageGeneralException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}