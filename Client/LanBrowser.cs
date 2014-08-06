using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using Client.Broadcast;
using Shared.Broadcast;

namespace Client
{
    public class LanBrowser : INotifyPropertyChanged, IDisposable
    {
        #region Fields
        private readonly BroadcastListenerManager _broadcastListener;
        private readonly ObservableCollection<Broadcastpackage> _broadcastpackages;

        private readonly Timer _checkServerBroadcastTimes;
        #endregion
        #region Properties
        public ObservableCollection<Broadcastpackage> Broadcastpackages
        {
            get { return _broadcastpackages; }
            //private set
            //{
            //    var original = _broadcastpackages;
            //    _broadcastpackages = value;
            //    if (_broadcastpackages != original) OnPropertyChanged("Broadcastpackages");
            //}
        }
        #endregion
        #region Constructors
        public LanBrowser(int listenPort, bool startListening = true)
        {
            _broadcastpackages = new ObservableCollection<Broadcastpackage>();
            _broadcastpackages.CollectionChanged += BroadcastpackagesCollectionChanged;

            _broadcastListener = new BroadcastListenerManager(listenPort, startListening);
            _broadcastListener.BroadcastReceived += BroadcastReceived;

            _checkServerBroadcastTimes = new Timer(10000);
            _checkServerBroadcastTimes.Elapsed += CheckServerBroadcastTimesElapsed;
            _checkServerBroadcastTimes.Start();
        }

        ~LanBrowser()
        {
            this.Dispose();
        }
        #endregion
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region EventHandlers
        private void BroadcastReceived(object sender, BroadcastListenerManager.BroadcastReceivedEventArgs eventArgs)
        {
            if (!Broadcastpackages.Contains(eventArgs.Broadcastpackage))
                Broadcastpackages.Add(eventArgs.Broadcastpackage);
            else
                Broadcastpackages.Single(o => o.Equals(eventArgs.Broadcastpackage)).LastReceivedBroadcast = DateTime.Now;
        }

        void BroadcastpackagesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Broadcastpackages");
        }

        void CheckServerBroadcastTimesElapsed(object sender, ElapsedEventArgs e)
        {
            List<Broadcastpackage> old = Broadcastpackages.Where(o => DateTime.Now.Subtract(o.LastReceivedBroadcast) > new TimeSpan(0, 0, 30)).ToList();

            foreach (Broadcastpackage oldBcp in old) Broadcastpackages.Remove(oldBcp);
        }
        #endregion
        #region Methods

        public void StartListening()
        {
            _broadcastListener.StartListening();
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        
        public void Dispose()
        {
            _checkServerBroadcastTimes.Stop();
            _checkServerBroadcastTimes.Dispose();

            _broadcastListener.Dispose();
        }
    }
}