namespace BinaryMessage.Interfaces
{
    public interface IMessageCodec
    {
        public byte[] Encode(Message message);
        public Message Decode(byte[] data);
    }
}