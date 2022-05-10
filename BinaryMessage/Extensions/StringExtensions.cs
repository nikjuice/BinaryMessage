using System.Text;

namespace BinaryMessage.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainNonASCIISequence(this string value)
        {
            return Encoding.UTF8.GetByteCount(value) != value.Length;
        }
    }

}