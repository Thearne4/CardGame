using System.Windows.Forms;
using Server;
using Server.Broadcast;
using Shared.Broadcast;

namespace TestUI
{
    public partial class FrmLanLobby : Form
    {
        private LanLobby _lanLobby;

        public FrmLanLobby()
        {
            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            _lanLobby = new LanLobby(1608, new Broadcastpackage(Broadcaster.GetMyIp(), 1608, "TestServer", "Description"));
        }
    }
}
