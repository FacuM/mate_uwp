using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Activation;
using Windows.Networking.Connectivity;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace Mate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainPage : Page
    {
        private StreamSocketListener mStreamSocketListener;
        private TextBlock mServerStatusTextBlock;
        private const int SERVER_PORT = 26566;

        public async void startUp()
        {
            var hostName = new HostName("localhost");
            var socket = new StreamSocket();

            mStreamSocketListener = new StreamSocketListener();

            mStreamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionReceived;
            await mStreamSocketListener.BindServiceNameAsync(SERVER_PORT.ToString()).AsTask();
        }

        private void StreamSocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = "¡Tenés un mate!"
                        },

                        new AdaptiveText()
                        {
                            Text = "Vení a buscarlo."
                        }
                    }
                }
            };

            ToastContent content = new ToastContent()
            {
                Visual = visual
            };

            var toast = new ToastNotification(content.GetXml())
            {
                ExpirationTime = DateTime.Now.AddMinutes(5)
            };
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public MainPage()
        {
            this.InitializeComponent();

            mServerStatusTextBlock = (TextBlock)FindName("ServerStatusTextBlock");


            var internetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
            if (internetConnectionProfile.NetworkAdapter == null)
            {
                mServerStatusTextBlock.Text = "Para poder ejecutar esta aplicación, tenés que conectarte a una red.";
            }
            else
            {
                startUp();

                String SERVER_IP = null;
                foreach (HostName hostName in NetworkInformation.GetHostNames())
                {
                    if (hostName.Type == HostNameType.Ipv4)
                    {
                        SERVER_IP = hostName.ToString();
                        break;
                    }
                }

                mServerStatusTextBlock.Text = "El servidor está ejecutándose en " + SERVER_IP + ":" + SERVER_PORT + ".";
            }
        }
    }
}
