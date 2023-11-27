using Indicator.Model;
using Indicator.Utill;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Text;

namespace Indicator;

public partial class MainPage : ContentPage
{

    private TcpClient tcpClient;
    private NetworkStream networkStream;
    private CancellationTokenSource cancellationTokenSource;
    private MainViewModel viewModel;




    public MainPage()
    {
        InitializeComponent();
        viewModel  = new MainViewModel();
        BindingContext = viewModel;

        ConnectTimer();
       
        viewModel.LeftStatusLabel = "Offline";
        viewModel.RightStatusLabel = "Offline";
        viewModel.back_ground_src = "screen_inactive.png";

        viewModel.QR_Label = "-";
        viewModel.LeftWeightLabel = "-";
        viewModel.RightWeightLabel = "-";
        viewModel.Vision_h_Label = "-";
        viewModel.TotalWeight_Label = "-";


    }

    int _OlineCheckCnt = 0;


    private void StatusCheck()
    {
        if (_OlineCheckCnt < 5)
        {
            //DisConnect Status
            viewModel.LeftStatusLabel = "Offline";
            viewModel.RightStatusLabel = "Offline";
            viewModel.LeftColorLabel = new Color(255, 0, 0); //Red
            viewModel.RightColorLabel = new Color(255, 0, 0); //Red

            viewModel.back_ground_src = "screen_inactive.png";


            viewModel.QR_Label = "-";
            viewModel.LeftWeightLabel = "-";
            viewModel.RightWeightLabel = "-";
            viewModel.Vision_h_Label = "-";
            viewModel.TotalWeight_Label = "-";




            _OlineCheckCnt = 0;
        }
        else
        {
            //Connect Status
            viewModel.LeftStatusLabel = "Online";
            viewModel.RightStatusLabel = "Online";
            viewModel.LeftColorLabel = new Color(173, 255, 47); //GreenYellow
            viewModel.RightColorLabel = new Color(173, 255, 47); //GreenYellow


            viewModel.back_ground_src = "screen_active.png";
        }

        _OlineCheckCnt--;
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
    


    private void OnConnetBtn(object sender, EventArgs e)
	{
        //ConnectToServer();
    }

    private void OnSendBtn(object sender, EventArgs e)
    {
        SendBackEndModel model = new SendBackEndModel();
        model.send_backend.eventValue = "pick_up";
        string json_body = Utill_.ObjectToJson(model);
        SendMessage(json_body);
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
                    ParaseJsonUI(receivedMessage);

                    _OlineCheckCnt = 10;
                }

              //  Thread.Sleep(100);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ReceiveData Exception {ex.Message}");
            tcpClient.Close();
            tcpClient = null;
        }
    }

    private  bool IsValidJson(string json)
    {
        try
        {
            JToken.Parse(json);
            return true;
        }
        catch (JsonReaderException)
        {
            return false;
        }
    }

    private void ParaseJsonUI(string recvstr)
    {
        try
        {
            if(IsValidJson(recvstr))
            {
                JObject jObject = JObject.Parse(recvstr);
                JObject outerObject = jObject["forlift_status"] as JObject; ;

                Console.WriteLine(recvstr);
                if (outerObject.ContainsKey("QR") == false)
                {

                    return;
                }

                if (outerObject.ContainsKey("weightLeft") == false)
                {

                    return;
                }

                if (outerObject.ContainsKey("weightRight") == false)
                {

                    return;
                }

                if (outerObject.ContainsKey("visionHeight") == false)
                {

                    return;
                }

                if (outerObject.ContainsKey("weightTotal") == false)
                {

                    return;
                }


                string _qr = jObject["forlift_status"]["QR"].ToString();
                string _weightLeft = jObject["forlift_status"]["weightLeft"].ToString() + " KG";
                string _weightRight = jObject["forlift_status"]["weightRight"].ToString() + " KG";
                string _visionHeight = jObject["forlift_status"]["visionHeight"].ToString() + " cm";
                string _weightTotal = jObject["forlift_status"]["weightTotal"].ToString() + " KG";

                viewModel.QR_Label = _qr;
                viewModel.LeftWeightLabel = _weightLeft;
                viewModel.RightWeightLabel = _weightRight;
                viewModel.Vision_h_Label = _visionHeight;
                viewModel.TotalWeight_Label = _weightTotal;
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"UI Exception {ex.Message}");
        }   

    }
   
}

