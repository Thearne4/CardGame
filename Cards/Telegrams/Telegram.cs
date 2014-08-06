using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Shared.Telegrams
{
    public abstract class Telegram
    {
        public bool FromServer { get; private set; }
        public TelegramType Type { get; private set; }

        public abstract IEnumerable<byte> ContentData();
        public byte[] ToData()
        {
            var data = ContentData();

            return new byte[] { 0x1 }
                .Concat(Conversions.ToData(data.Count()))
                .Concat(Conversions.Crc16(data))
                .Concat(Type.ToData())
                .Concat(new byte[] { 0x4 })
                .ToArray();
        }

    }

    public class TelegramType
    {
        public char Char1 { get; set; }
        public char Char2 { get; set; }
        public char Char3 { get; set; }

        public string Description { get; set; }

        public TelegramType(char char1, char char2, char char3, string description = null)
        {
            this.Char1 = char1;
            this.Char2 = char2;
            this.Char3 = char3;

            this.Description = description;
        }

        public IEnumerable<byte> ToData()
        {
            return new[] { Conversions.ToData(Char1), Conversions.ToData(Char2), Conversions.ToData(Char3) };
        }

        static TelegramType()
        {
            KnownTypes = new Dictionary<string, TelegramType>
                {
                    { "CHT", new TelegramType('C', 'H', 'T',"Chat Message") },
                    {"PIR",new TelegramType('P','I','R',"Player Info Request")},
                    {"PS ",new TelegramType('P','S',' ',"Player Status")}
                };
        }
        public static Dictionary<string, TelegramType> KnownTypes;

        public static TelegramType FromString(string telegramType, string description = null) { if (telegramType.Length != 3)throw new InvalidDataException(); return new TelegramType(telegramType[0], telegramType[1], telegramType[2], description); }
    }
}
