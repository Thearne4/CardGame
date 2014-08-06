using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Shared.Broadcast
{
    public static class ByteConversions
    {
        public static bool LittleEndian { get { return BitConverter.IsLittleEndian; } }

        public static byte[] ToByte(System.Net.IPAddress input)
        {
            return input.GetAddressBytes();
        }
        public static byte[] ToByte(int input)
        {
            return LittleEndian ?
                BitConverter.GetBytes(input).Reverse().ToArray() :
                BitConverter.GetBytes(input);
        }
        public static byte[] ToByte(string input)
        {
            return Encoding.ASCII.GetBytes(input);
        }

        public static System.Net.IPAddress ToIpAddress(byte[] data)
        {
            return new System.Net.IPAddress(data);
        }
        public static int ToInt(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (data.Length != 4) throw new InvalidDataException("data should be 4 bytes");
            return BitConverter.ToInt32(LittleEndian ? data.Reverse().ToArray() : data, 0);
        }
        public static string ToString(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }
    }
}
