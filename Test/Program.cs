using System;
using Client;

namespace TestClient
{
    class Program
    {
        static LanBrowser _lanBrowser;

        static void Main(string[] args)
        {
            _lanBrowser = new LanBrowser(1608);
            _lanBrowser.PropertyChanged += lanBrowser_PropertyChanged;

            Console.ReadLine();

            _lanBrowser.Dispose();
        }

        static void lanBrowser_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            foreach (var broadcastpackage in _lanBrowser.Broadcastpackages)
            {
                Console.WriteLine(broadcastpackage);
            }
            Console.WriteLine("----");
        }
    }
}
