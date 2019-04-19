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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace Mate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainPage : Page
    {
        private StreamSocketListener streamSocketListener;
        public async void startUp()
        {
            var hostName = new HostName("localhost");
            var socket = new StreamSocket();

            streamSocketListener = new StreamSocketListener();

            streamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionReceived;
            await streamSocketListener.BindServiceNameAsync("26566").AsTask();
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

            var toast = new ToastNotification(content.GetXml());
            toast.ExpirationTime = DateTime.Now.AddMinutes(5);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public MainPage()
        {
            /*
            Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalFolder;

            async Task prepareAsync()
            {

                Windows.Storage.StorageFile statusFile =
                    await storageFolder.CreateFileAsync("status.txt",
                        Windows.Storage.CreationCollisionOption.ReplaceExisting);

                await Windows.Storage.FileIO.WriteTextAsync(statusFile, "0");
            };

            async void startUp()
            {
                await prepareAsync();

                Windows.Storage.StorageFile statusFile =
                    await storageFolder.GetFileAsync("status.txt");

                String status = await Windows.Storage.FileIO.ReadTextAsync(statusFile);
                if (status == "0")
                {
                    Debug.WriteLine("Initial status ok: " + status + ".");
                }
                else
                {
                    Debug.WriteLine("Initialization failure, aborting.");
                    System.Diagnostics.Debugger.Launch();
                }
            }

            */

            this.InitializeComponent();


            startUp();
        }
    }
}
