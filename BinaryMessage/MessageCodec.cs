using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinaryMessage.Exceptions;
using BinaryMessage.Extensions;
using BinaryMessage.Interfaces;

namespace BinaryMessage
{
    public class MessageCodec : IMessageCodec
    {
        public byte[] Encode(Message message)
        {
            try
            {
                ValidateMessage(message);
                
                var memoryStream = new MemoryStream();
                 
                 memoryStream.Write(Constants.StartSequence);
                 WriteHeaders(message, memoryStream);

                 if (message.payload.Length > Constants.MaxPayLoadSizeInBytes)
                 {
                     throw new BinaryMessageArgumentException($"Message payload size exceed {Constants.MaxPayLoadSizeInBytes}");
                 }
                 
                 memoryStream.Write(message.payload);
            
                 return memoryStream.ToArray();
            
            }
            catch (Exception e)
            {
                if (e is BinaryBaseException)
                {
                    throw e;
                }
                
                throw new BinaryMessageGeneralException("Exception occurs during encoding Message", e);
            }
        }

        private static void ValidateMessage(Message message)
        {
            if (message.headers == null || message.headers.Count == 0)
            {
                throw new BinaryMessageArgumentException("Headers are null or not exist");
            }
            if (message.payload == null || message.payload.Length == 0)
            {
                throw new BinaryMessageArgumentException("Payload is  null or not exist");
            }
            
            if (message.payload.Length > Constants.MaxPayLoadSizeInBytes)
            {
                throw new BinaryMessageArgumentException($"Payload exceeds {Constants.MaxPayLoadSizeInBytes} ");
            }
            
        }

        public Message Decode(byte[] data)
        {
            try
            {
                var memoryStream = new MemoryStream(data);
                var message = new Message();
            
                ValidateStartSequence(memoryStream);
                ReadHeaders(memoryStream, message);

                int payLoadSize = (int) (memoryStream.Capacity - memoryStream.Position);
                    
                if (payLoadSize > Constants.MaxPayLoadSizeInBytes)
                {
                    throw new BinaryMessageValidationException($"Payload size exceed {Constants.MaxPayLoadSizeInBytes}");
                }
                
                message.payload = new byte[payLoadSize];
                memoryStream.Read(message.payload, 0, payLoadSize);

                return message;
            
            }
            catch (Exception e)
            {
                throw new BinaryMessageGeneralException("Exception occurs during decoding Message", e);
            }
        }

        private static void ReadHeaders(Stream stream, Message message)
        {
            int headersCount = stream.ReadByte();

            if (headersCount > Constants.MaxHeadersCount)
            {
                throw new BinaryMessageValidationException($"Number of headers exceed {Constants.MaxHeadersCount}");
            }
            
            message.headers = new Dictionary<string, string>();

            for (int i = 0; i < headersCount; i++)
            {
                string key = ReadFixedASCIIStringFromStream(stream);
                string value = ReadFixedASCIIStringFromStream(stream);
                
                message.headers.Add(key, value);
            }
        }

        private static void ValidateStartSequence(Stream stream)
        {
            var startSequence = new byte[Constants.StartSequence.Length];
            stream.Read(startSequence);

            if (!startSequence.SequenceEqual(Constants.StartSequence))
            {
                throw new BinaryMessageValidationException("Input byte array does not contain valid start sequence, please check the data");
            }
        }

        private static void WriteHeaders(Message message, Stream stream)
        {
            if (message.headers.Count > Constants.MaxHeadersCount)
            {
                throw new BinaryMessageArgumentException(
                    $"Number of headers  exceed the limit. Limit is {Constants.MaxHeadersCount}");
            }

            stream.WriteByte(Convert.ToByte(message.headers.Count));
            
            foreach (var key in message.headers.Keys)
            {
                string value = message.headers[key];
                if (key.ContainNonASCIISequence() || value.ContainNonASCIISequence())
                {
                    throw new BinaryMessageArgumentException(
                        $"Header contains non ASCII sequence, key - {key}, value - {value}");
                }
                
                if (key.Length >  Constants.MaxKeyValueSizeInBytes || value.Length > Constants.MaxKeyValueSizeInBytes)
                {
                    throw new BinaryMessageArgumentException(
                        $"Header exceed the limit. Limit is {Constants.MaxKeyValueSizeInBytes} per key/value independently");
                }

                WriteFixedASCIIStringToStream(stream, key);
                WriteFixedASCIIStringToStream(stream, value);
            }
        }

        private static void WriteFixedASCIIStringToStream(Stream stream, string value)
        {
            byte[] valueBytes = Encoding.ASCII.GetBytes(value);
            ushort valueSize = Convert.ToUInt16(valueBytes.Length);

            stream.Write(BitConverter.GetBytes(valueSize));
            stream.Write(valueBytes);
        }
        
        private static string ReadFixedASCIIStringFromStream(Stream stream)
        {
            byte[] stringSizeBytes = new byte[2];
            stream.Read(stringSizeBytes);
            
            ushort stringSize = BitConverter.ToUInt16(stringSizeBytes);

            if (stringSize > Constants.MaxKeyValueSizeInBytes)
            {
                throw new BinaryMessageValidationException($"Header data exceed {Constants.MaxKeyValueSizeInBytes}");
            }
            
            byte[] stringBytes = new byte[stringSize];
            stream.Read(stringBytes);
            
            return Encoding.ASCII.GetString(stringBytes);
        }

    }
}