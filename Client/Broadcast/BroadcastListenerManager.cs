using System;
using Shared.Broadcast;

namespace Client.Broadcast
{
    public class BroadcastListenerManager : IDisposable
    {
        #region Fields
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly BroadcastListener _broadcastListener;
        #endregion
        #region Constructor
        public BroadcastListenerManager(int listenPort, bool startListening)
        {
            _broadcastListener = new BroadcastListener(listenPort);
            _broadcastListener.BroadcastReceived += BroadcastDataReceived;

            if (startListening) StartListening();
        }
        #endregion
        #region Events
        #region BroadcastReceived
        public class BroadcastReceivedEventArgs : EventArgs
        {
            public Broadcastpackage Broadcastpackage { get; private set; }
            public BroadcastReceivedEventArgs(Broadcastpackage broadcastpackage)
            {
                Broadcastpackage = broadcastpackage;
            }
        }
        public delegate void BroadcastReceivedHandler(object sender, BroadcastReceivedEventArgs eventArgs);
        public event BroadcastReceivedHandler BroadcastReceived;
        #endregion
        #endregion
        #region EventHandlers
        void BroadcastDataReceived(object sender, BroadcastListener.BroadcastReceivedEventArgs eventArgs)
        {
            try { if (BroadcastReceived != null) BroadcastReceived(this, new BroadcastReceivedEventArgs(new Broadcastpackage(eventArgs.Data))); }
            catch (Exception ex) { _logger.WarnException("Error raising BroadcastReceived", ex); }
        }
        #endregion
        #region Methods
        public void StartListening()
        {
            _broadcastListener.StartReceive();
        }

        public void Dispose()
        {
            _broadcastListener.Dispose();
        }
        #endregion
    }
}
