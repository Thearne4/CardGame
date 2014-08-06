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
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();

            this.CenterToParent();
        }

        private void btnLan_Click(object sender, EventArgs e)
        {
            var frmLan = new FrmLan();
            frmLan.ShowDialog();
        }
    }
}
