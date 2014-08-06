using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestUI
{
    public partial class FrmLan : Form
    {
        public FrmLan()
        {
            InitializeComponent();

            this.CenterToParent();
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            var frmLanBrowser =new FrmLanBrowser();
            frmLanBrowser.ShowDialog();
        }

        private void btnHost_Click(object sender, EventArgs e)
        {
            var frmLanLobby = new FrmLanLobby();
            frmLanLobby.ShowDialog();
        }
    }
}
