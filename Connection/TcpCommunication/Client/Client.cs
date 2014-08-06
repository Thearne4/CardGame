using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace BasicConnection.TcpCommunication.Client
{
    public class Client : IDisposable
    {
        #region Fields
        private readonly TcpClient _server;
        private readonly IPEndPoint _endPoint;
        private readonly int _buffersize;
        private bool _isDisposing;
        #endregion
        #region Properties
        public int Timeout { get; private set; }
        #endregion
        #region Constructors
        public Client(IPAddress remoteIP, int remotePort, int bufferSize = 4096, int receiveTimeout = 61000)
        {
            this._buffersize = bufferSize;
            this.Timeout = receiveTimeout;
            this._server = new TcpClient();
            this._endPoint = new IPEndPoint(remoteIP, remotePort);
        }
        #endregion
        #region Events
        public delegate void ServerDisconnectedHandler(TcpClient server);
        public event ServerDisconnectedHandler ServerDisconnected;

        public delegate void ServerMessageReceivedHandler(TcpClient server, byte[] message);
        public event ServerMessageReceivedHandler ServerMessageReceived;
        #endregion
        #region EventHandlers

        #endregion
        #region Methods
        public void Connect()
        {
            _server.Connect(_endPoint);

            Thread listenThread = new Thread(new ParameterizedThreadStart(HandleServerComm));
            listenThread.Start(_server);
        }

        public void SendToServer(byte[] data)
        {
            //if (data.Length > _buffersize)
            //    throw new ArgumentOutOfRangeException("data","Parameter 'data' exceeded maximum buffersize");
            if (!_server.Connected) return;

            NetworkStream serverStream = _server.GetStream();
            serverStream.Write(data, 0, data.Length);
            serverStream.Flush();
        }

        private void HandleServerComm(object parameter)
        {
            TcpClient server = (TcpClient)parameter;
            NetworkStream serverStream = server.GetStream();
            serverStream.ReadTimeout = Timeout;

            //byte[] message = new byte[buffersize];
            byte[] message = new byte[this._buffersize];
            int bytesRead;

            while (!_isDisposing)
            {
                bytesRead = 0;

                try
                {
                    bytesRead = serverStream.Read(message, 0, message.Length);
                    ReportServerMessageReceived(server, message, bytesRead);
                }
                catch (TimeoutException te) { Console.WriteLine("TCP Client timed out waiting for server message.\n\r" + te.Message); }
                catch (Exception)
                {
                    //socket error
                    ReportServerDisconnected(server);
                    break;
                }

                if (bytesRead == 0 || !server.Connected)
                {
                    ReportServerDisconnected(server);
                    break;
                }
            }

            server.Close();
        }

        private void ReportServerDisconnected(TcpClient server)
        {
            if (ServerDisconnected == null) return;
            try
            {
                ServerDisconnected(server);
            }
            catch (Exception) { }
            //exception during invokation of ServerDisconnected
        }
        private void ReportServerMessageReceived(TcpClient server, byte[] received, int bytesRead)
        {
            if (ServerMessageReceived == null) return;
            byte[] message = new byte[bytesRead];
            Array.Copy(received, message, bytesRead);

            try
            {
                ServerMessageReceived(server, message);
            }
            catch (Exception) { }
            //exception during invokation of ServerMessageReceived
        }

        public void Dispose()
        {
            this._isDisposing = true;
        }
        #endregion
    }
}