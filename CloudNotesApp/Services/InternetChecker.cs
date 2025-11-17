using System;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace CloudNotesApp.Services
{
    public class InternetChecker : INotifyPropertyChanged
    {
        private bool _isConnected;
        private ImageSource _networkImageSource;

        public event PropertyChangedEventHandler PropertyChanged;

        public InternetChecker()
        {

            _isConnected = CheckInternetConnection();
            NetworkImageSource = _isConnected ? "online.png" : "offline.png";
            Task.Run(() => MonitorInternetConnection());
        }


        public ImageSource NetworkImageSource
        {
            get => _networkImageSource;
            private set
            {
                if (_networkImageSource != value)
                {
                    _networkImageSource = value;
                    OnPropertyChanged(nameof(NetworkImageSource));
                }
            }
        }

        public bool CheckInternetConnection()
        {
            try
            {
                using Ping ping = new Ping();
                PingReply reply = ping.Send("www.google.com");
                return reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return false;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void MonitorInternetConnection()
        {
            while (true)
            {
                bool newStatus = CheckInternetConnection();
                if (newStatus != _isConnected)
                {
                    _isConnected = newStatus;
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        NetworkImageSource = _isConnected ? "online.png" : "offline.png";
                    });
                }
                Thread.Sleep(1000);
            }
        }
    }
}
