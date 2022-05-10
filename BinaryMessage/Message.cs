using System;
using System.Collections.Generic;

namespace BinaryMessage
{
    public class Message
    {
        public Dictionary<String, String> headers;
        public byte[] payload;
    }
}