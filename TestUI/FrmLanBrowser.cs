using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Client;
using Shared.Broadcast;

namespace TestUI
{
    public partial class FrmLanBrowser : Form,IDisposable
    {
        private LanBrowser _lanBrowser;

        public BindingList<Broadcastpackage> Broadcastpackages { get; private set; }

        public FrmLanBrowser()
        {
            InitializeComponent();
            Initialize();

            this.CenterToParent();
        }

        private void Initialize()
        {
            this.Broadcastpackages = new BindingList<Broadcastpackage>();
            dgvLanBrowser.DataSource = Broadcastpackages;

             _lanBrowser = new LanBrowser(1608, false);
            _lanBrowser.PropertyChanged += LanBrowserPropertyChanged;
            _lanBrowser.StartListening();
        }

        void LanBrowserPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is LanBrowser)) return;
            if (e.PropertyName != "Broadcastpackages") return;

            UpdateBroadcastList(((LanBrowser)sender).Broadcastpackages);
        }

        private void UpdateBroadcastList(IEnumerable<Broadcastpackage> newList)
        {
            if (dgvLanBrowser.InvokeRequired)
                dgvLanBrowser.Invoke(new Action<IEnumerable<Broadcastpackage>>(UpdateBroadcastList), newList);
            else
            {
                var toAdd = newList.Except(this.Broadcastpackages);
                var toRemove = this.Broadcastpackages.Except(newList).ToList();

                foreach (var broadcastpackage in toRemove)
                    this.Broadcastpackages.Remove(broadcastpackage);
                foreach (var broadcastpackage in toAdd)
                    this.Broadcastpackages.Add(broadcastpackage);
            }
        }

        private void dgvLanBrowser_SelectionChanged(object sender, EventArgs e)
        {
            if (!(sender is DataGridView)) return;
            var dgv = (DataGridView) sender;
            btnConnect.Enabled = dgv.SelectedRows.Count == 1;
        }
        
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if(!(sender is DataGridView))return;

            
        }


        void IDisposable.Dispose()
        {
            if(_lanBrowser!=null)_lanBrowser.Dispose();
            base.Dispose();
        }
    }
}
