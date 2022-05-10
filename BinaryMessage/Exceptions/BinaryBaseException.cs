using System;

namespace BinaryMessage.Exceptions
{
    public abstract class BinaryBaseException : Exception
    {
        public BinaryBaseException(string message) : base(message)
        {
            
        }
        public BinaryBaseException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}