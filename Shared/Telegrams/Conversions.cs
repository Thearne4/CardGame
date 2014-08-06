using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared.Telegrams
{
    static class Conversions
    {
        static Conversions()
        {
            CreateCrcTable();
        }

        public static IEnumerable<byte> ToData(short input) { return BitConverter.IsLittleEndian ? BitConverter.GetBytes(input).Reverse() : BitConverter.GetBytes(input); }
        public static IEnumerable<byte> ToData(int input) { return BitConverter.IsLittleEndian ? BitConverter.GetBytes(input).Reverse() : BitConverter.GetBytes(input); }
        public static IEnumerable<byte> ToData(long input) { return BitConverter.IsLittleEndian ? BitConverter.GetBytes(input).Reverse() : BitConverter.GetBytes(input); }
        public static byte ToData(char input) { return Encoding.ASCII.GetBytes(new[] { input })[0]; }

        #region CRC
        private const ushort CrcPolynominal = 0xA001;
        private static ushort[] _crcTable;
        private static void CreateCrcTable()
        {
            _crcTable = new ushort[256];
            for (ushort i = 0; i < _crcTable.Length; ++i)
            {
                ushort value = 0;
                var temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ CrcPolynominal);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                _crcTable[i] = value;
            }
        }
        public static IEnumerable<byte> Crc16(IEnumerable<byte> data)
        {
            return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Crc16ToUshort(data)) : BitConverter.GetBytes(Crc16ToUshort(data)).Reverse();
        }

        private static ushort Crc16ToUshort(IEnumerable<byte> data)
        {
            ushort crc = 0;
            foreach (var index in data.Select(t => (byte)(crc ^ t)))
            {
                crc = (ushort)((crc >> 8) ^ _crcTable[index]);
            }
            return crc;
        }
        #endregion
    }
}
