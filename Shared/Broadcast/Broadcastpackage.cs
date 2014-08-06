using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Shared.Broadcast
{
    public class Broadcastpackage : IEquatable<Broadcastpackage>
    {
        public const int ServerNameMaxLength = 50;
        public const int ServerDescriptionMaxLength = 500;

        private string _serverName;
        private string _description;

        public IPAddress IpAddress { get; private set; }
        public int Port { get; private set; }
        public string ServerName
        {
            get { return _serverName; }
            private set { if (value.Length > 50)throw new InvalidDataException("Name has to be 50 chars or less"); else _serverName = value; }
        }
        public string Description
        {
            get { return _description; }
            private set { if (value.Length > 500)throw new InvalidDataException("Name has to be 500 chars or less"); else _description = value; }
        }

        public DateTime LastReceivedBroadcast { get; set; }

        public Broadcastpackage(IPAddress ipAddress, int port, string serverName, string description)
        {
            this.IpAddress = ipAddress;
            this.Port = port;
            this.ServerName = serverName;
            this.Description = description;
        }

        public Broadcastpackage(byte[] data)
        {
            if (data.Length < 5) throw new InvalidDataException();
            int length = ByteConversions.ToInt(data.Take(4).ToArray());
            byte ipLength = data[4];
            if (length != ServerNameMaxLength + ServerDescriptionMaxLength + ipLength + 5) throw new InvalidDataException();


            this.IpAddress = ByteConversions.ToIpAddress(data.Skip(5).Take(ipLength).ToArray());
            this.Port = ByteConversions.ToInt(data.Skip(5 + ipLength).Take(4).ToArray());
            this.ServerName = ByteConversions.ToString(data.Skip(9 + ipLength).Take(ServerNameMaxLength).ToArray()).TrimStart();
            this.Description = ByteConversions.ToString(data.Skip(9 + ipLength + ServerNameMaxLength).Take(ServerDescriptionMaxLength).ToArray()).TrimStart();

            LastReceivedBroadcast = DateTime.Now;
        }

        public byte[] ToData()
        {
            byte[] ipBytes = ByteConversions.ToByte(this.IpAddress);
            var content = new[] { ByteConversions.ToByte(ipBytes.Length)[3] }
                .Concat(ipBytes)
                .Concat(ByteConversions.ToByte(this.Port))
                .Concat(ByteConversions.ToByte(this.ServerName.PadLeft(50)))
                .Concat(ByteConversions.ToByte(this.Description.PadLeft(500)));

            return ByteConversions.ToByte(content.Count())
            .Concat(content)
            .ToArray();
        }

        public bool Equals(Broadcastpackage other)
        {
            if (this.IpAddress.Equals(other.IpAddress) &&
                this.Port == other.Port &&
                this.ServerName == other.ServerName &&
                this.Description == other.Description)
                return true;
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0}({1}:{2})", this.ServerName, this.IpAddress, this.Port);
        }
    }
}
