using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace BasicConnection.TcpCommunication.Server
{
    public class Server : IDisposable
    {
        #region Fields
        private TcpListener _tcpListener;

        private readonly List<TcpClient> _clients;

        private readonly int _buffersize;
        private bool _isDisposing;

        #endregion
        #region Properties
        public List<TcpClient> Clients { get { return _clients; } }

        public int Timeout { get; private set; }
        public int ListenPort { get; set; }
        #endregion
        #region Constructors
        public Server(int listenPort, int bufferSize = 4096, int receiveTimeout = 61000)
        {
            this._buffersize = bufferSize;
            this.Timeout = receiveTimeout;
            this.ListenPort = listenPort;
            this._clients = new List<TcpClient>();
        }
        #endregion
        #region Events
        public delegate void ClientConnectedHandler(TcpClient client);
        public event ClientConnectedHandler ClientConnected;

        public delegate void ClientDisconnectedHandler(TcpClient client);
        public event ClientDisconnectedHandler ClientDisconnected;

        public delegate void ClientMessageReceivedHandler(TcpClient client, byte[] message);
        public event ClientMessageReceivedHandler ClientMessageReceived;
        #endregion
        #region EventHandlers

        #endregion
        #region Methods
        public void Start()
        {
            (new Thread(new ThreadStart(ListenForClients))).Start();
        }

        private void ListenForClients()
        {
            this._tcpListener = new TcpListener(System.Net.IPAddress.Any, this.ListenPort);
            this._tcpListener.Start();

            while (!_isDisposing)
            {
                TcpClient client = null;
                try
                {
                    //blocks until a client has connected to the server
                    client = this._tcpListener.AcceptTcpClient();
                }
                catch (Exception) { }//catchall

                if (client == null) continue;

                //create a thread to handle communication 
                //with connected client
                Thread clientThread = new Thread(new System.Threading.ParameterizedThreadStart(HandleClientComm));
                //clientThread.Start(new object[2] { client, buffersizeobj });

                ReportClientConnected(client);

                _clients.Add(client);

                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object parameter)
        {
            //TcpClient client = (TcpClient)parameters[0];
            //int buffersize = (int)parameters[1];
            TcpClient client = (TcpClient)parameter;
            NetworkStream clientStream = client.GetStream();
            clientStream.ReadTimeout = Timeout;

            //byte[] message = new byte[buffersize];
            byte[] message = new byte[this._buffersize];
            int bytesRead;

            while (!_isDisposing)
            {
                bytesRead = 0;

                try
                {
                    bytesRead = clientStream.Read(message, 0, message.Length);
                    ReportClientMessageReceived(client, message, bytesRead);
                }
                catch (TimeoutException te) { Console.WriteLine("TCP Client timed out waiting for server message.\n\r" + te.Message); }
                catch (Exception)
                {
                    //socket error
                    ReportClientDisconnected(client);
                    break;
                }

                if (bytesRead == 0 || !client.Connected)
                {
                    ReportClientDisconnected(client);
                    break;
                }
            }

            client.Close();
            if (this._clients.Contains(client))
                this._clients.Remove(client);
        }

        private void ReportClientConnected(TcpClient client)
        {
            if (ClientConnected == null) return;
            try
            {
                ClientConnected(client);
            }
            catch (Exception) { }
            //exception during invokation of ClientConnected            
        }
        private void ReportClientDisconnected(TcpClient client)
        {
            if (ClientDisconnected == null) return;
            try
            {
                ClientDisconnected(client);
            }
            catch (Exception) { }
            //exception during invokation of ClientDisconnected            
        }
        private void ReportClientMessageReceived(TcpClient client, byte[] received, int bytesRead)
        {
            if (ClientMessageReceived == null) return;
            byte[] message = new byte[bytesRead];
            Array.Copy(received, message, bytesRead);

            try
            {
                ClientMessageReceived(client, message);
            }
            catch (Exception) { }
            //exception during invokation of ClientMessageReceived            
        }

        public void SendToAll(byte[] data)
        {
            //if (data.Length > _buffersize)
            //    throw new ArgumentOutOfRangeException("data","Parameter 'data' exceeded maximum buffersize");

            foreach (TcpClient client in _clients)
            {
                if (!client.Connected) continue;

                NetworkStream clientStream = client.GetStream();
                clientStream.Write(data, 0, data.Length);
                clientStream.Flush();

            }
        }
        public void SendToClient(TcpClient client, byte[] data)
        {
            //if (data.Length > _buffersize)
            //    throw new ArgumentOutOfRangeException("data","Parameter 'data' exceeded maximum buffersize");
            if (!client.Connected) return;

            NetworkStream clientStream = client.GetStream();
            clientStream.Write(data, 0, data.Length);
            clientStream.Flush();

        }
        public void SendToClient(int clientIndex, byte[] data)
        {
            if (clientIndex < _clients.Count && clientIndex > 0)
                SendToClient(_clients[clientIndex], data);
            else
                throw new ArgumentOutOfRangeException("clientIndex", "Parameter 'clientIndex' does not correspond to an existing TcpClient");
        }

        public void DisconnectClient(TcpClient client)
        {
            client.Close();
        }

        public void Dispose()
        {
            this._tcpListener.Stop();
            this._isDisposing = true;
        }
        #endregion
    }
}