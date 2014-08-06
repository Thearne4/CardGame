using System;
using System.Net;
using System.Net.Sockets;

namespace Client.Broadcast
{
    internal class BroadcastListener : IDisposable
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly int _listenport;
        private UdpClient _udpListener;
        private System.Threading.Thread _receiveThread;
        private bool _disposing;

        public event BroadcastReceivedDelegate BroadcastReceived;
        public delegate void BroadcastReceivedDelegate(object sender, BroadcastReceivedEventArgs eventArgs);
        public class BroadcastReceivedEventArgs : EventArgs
        {
            public IPEndPoint Sender { get; private set; }
            public byte[] Data { get; private set; }

            public BroadcastReceivedEventArgs(IPEndPoint sender, byte[] data)
            {
                Sender = sender;
                Data = data;
            }
        }

        public int ReceiveTimeout { get; private set; }

        /// <summary>
        /// Creates a new broadcast listener.
        /// </summary>
        /// <param name="port">The port on wich to listen</param>
        /// <param name="receiveTimeout"></param>
        public BroadcastListener(int port, int receiveTimeout = 61000)
        {
            _listenport = port;
            _disposing = false;
            ReceiveTimeout = receiveTimeout;
        }

        public void StartReceive()
        {
            _udpListener = new UdpClient(_listenport) { Client = { ReceiveTimeout = ReceiveTimeout } };

            _receiveThread = new System.Threading.Thread(ListenForData) { Name = "Listen for broadcasts on port " + _listenport + " thread" };
            _receiveThread.Start();
        }
        private void ListenForData()
        {
            try
            {
                while (!_disposing && _udpListener != null)
                {
                    try
                    {
                        if (_udpListener.Available <= 0)
                        {
                            System.Threading.Thread.Sleep(10);
                            continue;
                        }
                        var sender = new IPEndPoint(IPAddress.Any, _listenport);
                        byte[] receiveData = _udpListener.Receive(ref sender);

                        if (BroadcastReceived != null)
                            ReportBroadcastReceived(sender, receiveData);
                    }
                    catch (TimeoutException te) { Logger.WarnException("TCP Client timed out waiting for server message.", te); }
                    catch (SocketException se)
                    {
                        if (se.ErrorCode == 10060 || se.ErrorCode == 10004) Logger.WarnException("A SocketException was thrown while trying to receive broadcasts", se);
                        else { Logger.ErrorException("An SocketException was thrown while trying to receive broadcasts", se); throw se; }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("An exception was thrown while trying to receive broadcasts", ex);

                throw;
            }
            if (_udpListener != null) _udpListener.Close();
        }

        private void ReportBroadcastReceived(IPEndPoint sender, byte[] receiveData)
        {
            try { BroadcastReceived(this, new BroadcastReceivedEventArgs(sender, receiveData)); }
            catch (Exception ex) { Logger.WarnException("Error invoking BroadcastReceived event", ex); }
        }

        public void Dispose()
        {
            _disposing = true;
        }
    }
}
