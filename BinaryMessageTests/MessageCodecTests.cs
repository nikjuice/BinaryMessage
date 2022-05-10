using System.Collections.Generic;
using System.Text;
using BinaryMessage;
using BinaryMessage.Exceptions;
using BinaryMessage.Interfaces;
using NUnit.Framework;

namespace BinaryMessageTests
{
    public class MessageCodecTests
    {
        private IMessageCodec _messageCodec;
        [SetUp]
        public void Setup()
        {
            _messageCodec = new MessageCodec();
        }
        [Test]
        public void ShouldEncodeSimpleMessageCorrectly()
        {
            var result = _messageCodec.Encode(GetCorrectSimpleMessage());
            Assert.AreEqual(result, GetCorrectMessageBytes());
        }

        [Test]
        public void ShouldThrowArgumentExceptionForLongHeader()
        {
            var message = new Message
            {
                headers = GetTooBigHeadersMock(),
                payload = Encoding.UTF8.GetBytes("test")
            };

            Assert.Throws<BinaryMessageArgumentException>(() => _messageCodec.Encode(message));
        }
        [Test]
        public void ShouldThrowArgumentExceptionForTooManyHeaders()
        {
            var message = new Message
            {
                headers = GetManyHeadersMock(65),
                payload = Encoding.UTF8.GetBytes("test")
            };

            Assert.Throws<BinaryMessageArgumentException>(() => _messageCodec.Encode(message));
        }


        [Test]
        public void ShouldThrowArgumentExceptionForUTF8Header()
        {
            var message = new Message
            {
                headers = GetUTF8HeadersMock(),
                payload = Encoding.UTF8.GetBytes("test")
            };

            Assert.Throws<BinaryMessageArgumentException>(() => _messageCodec.Encode(message));
        }
        
        [Test]
        public void ShouldThrowArgumentExceptionForEmptyHeaders()
        {
            var message = new Message
            {
                payload = Encoding.UTF8.GetBytes("test")
            };

            Assert.Throws<BinaryMessageArgumentException>(() => _messageCodec.Encode(message));
        }
        
        [Test]
        public void ShouldThrowArgumentExceptionForEmptyPayLoad()
        {
            var message = new Message
            {
                headers = GetCorrectHeadersMock()
            };

            Assert.Throws<BinaryMessageArgumentException>(() => _messageCodec.Encode(message));
        }
        
        [Test]
        public void ShouldThrowArgumentExceptionForTooBigPayLoad()
        {
            var message = new Message
            {
                headers = GetCorrectHeadersMock(),
                payload = Encoding.UTF8.GetBytes(new string('A', 400000))
            };

            Assert.Throws<BinaryMessageArgumentException>(() => _messageCodec.Encode(message));
        }
        
        [Test]
        public void ShouldDecodeBasicMessage()
        {
            var payloadString = "Testing payload";
            var message = new Message
            {
                headers = new Dictionary<string, string>
                {
                    { "Version", "3.32" },
                    { "Type", "test2" }
                },
                payload = Encoding.UTF8.GetBytes(payloadString)
            };

            var result = _messageCodec.Encode(message);

            var messageDecoded = _messageCodec.Decode(result);
            
            Assert.AreEqual(message.headers, messageDecoded.headers);
            Assert.AreEqual(payloadString, Encoding.UTF8.GetString(messageDecoded.payload));
            
        }
        
        [Test]
        public void ShouldEncodeDecodeCorrectlyWithMaxAmountOfHeaders()
        {
            var headersMock = GetManyHeadersMock(63);
            var payloadString = "Testing payload";
            
            var message = new Message
            {
                headers = headersMock,
                payload = Encoding.UTF8.GetBytes(payloadString)
            };
            
            var result = _messageCodec.Encode(message);

            var messageDecoded = _messageCodec.Decode(result);
            
            Assert.AreEqual(headersMock, messageDecoded.headers);
            Assert.AreEqual(payloadString, Encoding.UTF8.GetString(messageDecoded.payload));
        }
        
        private Message GetCorrectSimpleMessage()
        {
            return new Message()
            {
                headers = new Dictionary<string, string>()
                {
                    ["Version"] = "3.12",
                    ["Type"] = "Direct"
                },
                payload = Encoding.ASCII.GetBytes("test")

            };
        }

        private static byte[] GetCorrectMessageBytes()
        {
            return new byte[]
            {
                0x73, 0x6D, 0x65, 0x2, 0x7, 0x0, 0x56, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x4, 0x0, 0x33, 0x2E, 0x31,
                0x32, 0x4, 0x0, 0x54, 0x79, 0x70, 0x65, 0x6, 0x0, 0x44, 0x69, 0x72, 0x65, 0x63, 0x74, 0x74, 0x65, 0x73,
                0x74
            };

        }
        private static Dictionary<string, string> GetTooBigHeadersMock()
        {
            return new Dictionary<string, string>()
            {
                ["Version"] = "3.12",
                ["Type"] = new string('A', 1024),
                ["Test"] = "Hello",
            };
        }
        
        private static Dictionary<string, string> GetCorrectHeadersMock()
        {
            return new Dictionary<string, string>()
            {
                ["Version"] = "3.12",
                ["Type"] = "control"
            };
        }
        private static Dictionary<string, string> GetUTF8HeadersMock()
        {
            return new Dictionary<string, string>()
            {
                ["Version"] = "3.12",
                ["Type"] = "Ý",
                ["Test"] = "Hello",
            };
        }
        
        private static Dictionary<string, string> GetManyHeadersMock(int count)
        {
            var headers = new Dictionary<string, string>();
            for (int i = 0; i < count; i++)
            {
                headers.Add($"test{i}",$"test{i}");
            }

            return headers;
        }
        
    }
}