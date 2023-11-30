using AndroidX.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Indicator.TCP
{
    class TCPSocketClient
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private CancellationTokenSource cancellationTokenSource;


        public TCPSocketClient()
        {

           
        }

        public void Initialize()
        {
            ConnectTimer();


            MessagingCenter.Subscribe<object, string>(this, "TCP_DATA_Send", (sender, message) =>
            {
                SendMessage(message);

            });

        }

        private void ConnectTimer()
        {
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                // do something every 60 seconds
                Device.BeginInvokeOnMainThread(() =>
                {

                    if (tcpClient != null && tcpClient.Connected == false)
                    {
                        tcpClient.Close();
                        tcpClient = null;
                    }

                    ConnectToServer();
                });


                StatusCheck();

                return true; // runs again, or false to stop
            });
        }
        int _OlineCheckCnt = 0;


        private void StatusCheck()
        {
            if (_OlineCheckCnt < 5)
            {
                //DisConnect Status
                MessagingCenter.Send<object, string>(this, "TCP_STATUS_Receive", "disconnect");

                _OlineCheckCnt = 0;
            }
            else
            {
                //Connect Status
                MessagingCenter.Send<object, string>(this, "TCP_STATUS_Receive", "connect");
            }

            _OlineCheckCnt--;
        }

        

            

        private async Task ConnectToServer()
        {
            try
            {
                if (tcpClient != null)
                {
                    return;
                }

                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync("192.168.8.100", 8051);

                if (tcpClient.Connected)
                {
                    networkStream = tcpClient.GetStream();
                    cancellationTokenSource = new CancellationTokenSource();
                    _ = Task.Run(() => ReceiveData(cancellationTokenSource.Token));
                }
            }
            catch (Exception ex)
            {

            }

        }

        private async Task SendMessage(string msg)
        {
            try
            {
                if (tcpClient != null && tcpClient.Connected)
                {
                    byte[] data = Encoding.UTF8.GetBytes(msg);
                    await networkStream.WriteAsync(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task ReceiveData(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                    if (bytesRead > 0)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);


                        MessagingCenter.Send<object, string>(this, "TCP_DATA_Receive", receivedMessage);
                    
                        _OlineCheckCnt = 7;
                    }

                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ReceiveData Exception {ex.Message}");
                tcpClient.Close();
                tcpClient = null;
            }
        }
    }
}
