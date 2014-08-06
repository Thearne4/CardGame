using System;
using System.Linq;
using System.Net.Sockets;
using System.Net;

namespace Server.Broadcast
{
    public class Broadcaster
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        readonly Socket _broadcastSocket;
        readonly IPEndPoint _broadcastEndPoint;

        public IPAddress MyIp { get; private set; }
        public IPAddress BroadcastIp { get; private set; }
        public int BroadcastPort { get; private set; }

        /// <summary>
        /// Create a new broadcast server.
        /// </summary>
        /// <param name="port">The port to wich the broadcast is sent.</param>
        public Broadcaster(int port)
        {
            this._broadcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.MyIp = GetMyIp();
            this.BroadcastIp = GetBroadcastAddressFor(MyIp);
            //this.BroadcastIp = GetCorrectBroadcastAddressFor(Ip,);
            this.BroadcastPort = port;
            this._broadcastEndPoint = new IPEndPoint(BroadcastIp, BroadcastPort);
        }

        /// <summary>
        /// Send data to broadcastEndPoint.
        /// </summary>
        /// <param name="sendData">The data to send in the broadcast in bytes.</param>
        /// <returns>true if sending was successful</returns>
        public bool Send(byte[] sendData)
        {
            try
            {
                _broadcastSocket.SendTo(sendData, _broadcastEndPoint);
                return true;
            }
            catch (Exception sendException)
            {
                _logger.ErrorException("Error sending data trough broadcastsocket", sendException);
                //Console.WriteLine("Broadcaster Exception during send: {0}", sendException.Message);
                return false;
            }
        }

        public static IPAddress GetCorrectBroadcastAddressFor(IPAddress local, IPAddress subnetMask)
        {
            byte[] localbytes = local.GetAddressBytes();
            byte[] subnet = subnetMask.GetAddressBytes();
            byte[] broadcastbytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                broadcastbytes[i] = (byte)(localbytes[i] | (subnet[i] ^ 255));
            }
            return new IPAddress(broadcastbytes);
        }
        public static IPAddress GetBroadcastAddressFor(IPAddress local)
        {
            byte[] broadcast = local.GetAddressBytes();
            broadcast[3] = 255;
            return new IPAddress(broadcast);
        }
        public static IPAddress GetMyIp()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            var myIps = host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();
            return myIps[0] ?? null;
        }
    }
}