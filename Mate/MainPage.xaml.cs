using System;
using Windows.UI.Xaml.Controls;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.ApplicationModel.Background;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using Windows.Networking.Sockets;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.Storage.Streams;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace Mate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainPage : Page
    {
        private string mSocketId = "MateServer";
        private int mTransferOwnershipCount = 0;

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

        private async void TransferOwnership(StreamSocketListener streamSocketListener)
        {
            await streamSocketListener.CancelIOAsync();

            var dataWriter = new DataWriter();
            ++mTransferOwnershipCount;
            dataWriter.WriteInt32(mTransferOwnershipCount);
            var context = new SocketActivityContext(dataWriter.DetachBuffer());
            streamSocketListener.TransferOwnership(mSocketId, context);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            TransferOwnership(mStreamSocketListener);
            deferral.Complete();
        }

        private async void startServerAsync()
        {
            Debug.WriteLine("Starting server...");

            mStreamSocketListener = new StreamSocketListener();
            mStreamSocketListener.EnableTransferOwnership(mTask.TaskId, SocketActivityConnectedStandbyAction.DoNotWake);

            mStreamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionReceived;
            await mStreamSocketListener.BindServiceNameAsync(SERVER_PORT.ToString()).AsTask();

            if (mStreamSocketListener == null)
            {
                mServerStatusTextBlock.Text = "Hubo un problema al iniciar el servidor.";
            }
            else
            {
                mServerStatusTextBlock.Text = "El servidor está ejecutándose en " + SERVER_IP + ".";
            }

            Debug.WriteLine("Startup completed.");
        }

        private StreamSocketListener       mStreamSocketListener;
        private TextBlock                  mServerStatusTextBlock;
        private BackgroundTaskRegistration mTask;

        private string                     SERVER_IP               = null;
        private const int                  SERVER_PORT             = 26566;

        public MainPage()
        {
            this.InitializeComponent();

            var socket = new StreamSocket();
            var socketTaskBuilder = new BackgroundTaskBuilder()
            {
                Name = "MateNotificationListener",
                TaskEntryPoint = "Tasks.BackgroundTaskServerRunner"
            };

            var trigger = new SocketActivityTrigger();
            socketTaskBuilder.SetTrigger(trigger);

            // Some background tasks could be kept running after shutdown (cause Windows), lets unregister them.
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                task.Value.Unregister(true);
            }

            // Then, register it.
            mTask = socketTaskBuilder.Register();

            mServerStatusTextBlock = (TextBlock)FindName("ServerStatusTextBlock");

            var internetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
            foreach (HostName hostName in NetworkInformation.GetHostNames())
            {
                if (hostName.Type == HostNameType.Ipv4)
                {
                    SERVER_IP = hostName.ToString();
                    break;
                }
            }

            if (internetConnectionProfile.NetworkAdapter == null)
            {
                mServerStatusTextBlock.Text = "Para poder ejecutar esta aplicación, tenés que conectarte a una red.";
            }
            else
            {
                startServerAsync();
            }
        }
    }
}