using System.Text;

namespace BinaryMessage
{
    public static class Constants
    {
        public static readonly byte[] StartSequence =  Encoding.ASCII.GetBytes("sme");
        public static int MaxHeadersCount =  63;
        public static int MaxKeyValueSizeInBytes =  1023;
        public static int MaxPayLoadSizeInBytes =  262144;
    }
}