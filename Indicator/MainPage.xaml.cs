using Indicator.Model;
using Indicator.TCP;
using Indicator.Utill;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Text;

namespace Indicator;

public partial class MainPage : ContentPage
{
    private MainViewModel viewModel;
    
    public MainPage()
    {
        InitializeComponent();
        viewModel  = new MainViewModel(); //binding
        BindingContext = viewModel;
        viewModel.LeftStatusLabel = "Offline";
        viewModel.RightStatusLabel = "Offline";
        viewModel.back_ground_src = "screen_inactive.png";

        viewModel.QR_Label = "-";
        viewModel.LeftWeightLabel = "-";
        viewModel.RightWeightLabel = "-";
        viewModel.Vision_h_Label = "-";
        viewModel.TotalWeight_Label = "-";


        TCPSocketClient msg = new TCPSocketClient();
        msg.Initialize();


        OnDataReceive();

        OnStatusReceive();
    }

    private void OnDataReceive()
    {
        MessagingCenter.Subscribe<object, string>(this, "TCP_DATA_Receive", (sender, message) =>
        {
            ParaseJsonUI(message);
        });
    }


    private void OnStatusReceive()
    {
        MessagingCenter.Subscribe<object, string>(this, "TCP_STATUS_Receive", (sender, message) =>
        {
            StatusDisplay(message);
        });
    }




    private void StatusDisplay(string status)
    {
        if (status == "disconnect")
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

        }
        if (status == "connect")
        {
            //Connect Status
            viewModel.LeftStatusLabel = "Online";
            viewModel.RightStatusLabel = "Online";
            viewModel.LeftColorLabel = new Color(173, 255, 47); //GreenYellow
            viewModel.RightColorLabel = new Color(173, 255, 47); //GreenYellow
            viewModel.back_ground_src = "screen_active.png";
        }

    }


    private void OnSendBtn(object sender, EventArgs e)
    {
        SendBackEndModel model = new SendBackEndModel();
        model.send_backend.eventValue = "pick_up";
        string json_body = Utill_.ObjectToJson(model);
        MessagingCenter.Send<object, string>(this, "TCP_DATA_Send", json_body);
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

