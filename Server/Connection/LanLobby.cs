using System.Timers;
using Server.Broadcast;
using Shared.Broadcast;

namespace Server
{
    public class LanLobby
    {
        #region Field
        private readonly Broadcaster _broadcaster;
        private readonly Timer _broadcastTimer;
        #endregion
        #region Properties
        public Broadcastpackage Broadcastpackage { get; set; }
        #endregion
        #region Constructor
        public LanLobby(int port, Broadcastpackage broadcastpackage, int broadcastInterval = 5000, bool startBroadcasting = true)
        {
            _broadcastTimer = new Timer(broadcastInterval);
            _broadcastTimer.Elapsed += BroadcastTimerElapsed;

            this.Broadcastpackage = broadcastpackage;

            _broadcaster = new Broadcaster(port);

            if (startBroadcasting) StartBroadcast();
        }
        #endregion
        #region EventHandlers
        void BroadcastTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _broadcaster.Send(this.Broadcastpackage.ToData());
        }
        #endregion
        #region Methods
        public void StartBroadcast()
        {
            if (!_broadcastTimer.Enabled) _broadcastTimer.Start();
        }
        public void StopBroadcast()
        {
            if (_broadcastTimer.Enabled) _broadcastTimer.Stop();
        }

        //public static Broadcastpackage GetBroadcastPackage(){}
        #endregion
    }
}
