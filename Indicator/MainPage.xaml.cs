using Indicator.Model;
using Indicator.TCP;
using Indicator.Utill;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static Android.Provider.ContactsContract.CommonDataKinds;

namespace Indicator;

public partial class MainPage : ContentPage
{
    private MainViewModel viewModel;

    private string _ipAddress = "192.168.8.100";


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


        _ipAddress = ReadSavedJson();

        TCPSocketClient msg = new TCPSocketClient(_ipAddress);
        msg.Initialize();



        OnDataReceive();

        OnStatusReceive();
    }

    private void Setting_btn(object sender, EventArgs e)
    {
        // 버튼 클릭 이벤트 처리 로직을 여기에 추가
        // 예: DisplayAlert("알림", "버튼이 클릭되었습니다!", "확인");
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


    
   // 

    private void OnSendBtn(object sender, EventArgs e)
    {
        SendBackEndModel model = new SendBackEndModel();
        model.send_backend.eventValue = "pick_up";
        string json_body = Utill_.ObjectToJson(model);
        MessagingCenter.Send<object, string>(this, "TCP_DATA_Send", json_body);

        OpenPopup();
    }

    private async void OpenPopup()
    {
        // 저장된 JSON 데이터를 읽어옴
        string savedJson = ReadSavedJson();

        // 팝업에서 입력받을 데이터를 위한 Entry 생성
        Entry jsonEntry = new Entry
        {
            Placeholder = "IP 주소 입력 (예: 192.168.0.1)",
            Text = FormatIpAddress(savedJson) // 저장된 JSON 데이터를 Entry에 설정하면서 형식 변환
        };

        // "접속 IP" 라벨 추가
        Label ipLabel = new Label
        {
            Text = "접속 IP",
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), // 폰트 크기를 크게 설정
            FontAttributes = FontAttributes.Bold | FontAttributes.Italic // 두껍고 기울임 스타일 설정
        };

        // 팝업 생성
        ContentPage popupPage = new ContentPage
        {
            Content = new StackLayout
            {
                Children = {
                    ipLabel,
                    jsonEntry,
                    new Button { Text = "저장", Command = new Command(() => SaveConfig(FormatIpAddress(jsonEntry.Text))), Margin = new Thickness(0, 10, 0, 0) },
                    new Button { Text = "닫기", Command = new Command(async () => await Navigation.PopModalAsync()), Margin = new Thickness(0, 10, 0, 0) }
                }
            }
        };

        // 팝업 표시
        await Navigation.PushModalAsync(popupPage);
    }

    private string ReadSavedJson()
    {
        try
        {
            // 저장된 JSON 파일에서 데이터를 읽어옴
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "config.json");
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
        }
        catch (Exception ex)
        {
            // 읽기 중 오류가 발생한 경우 오류 메시지 표시
            DisplayAlert("오류", $"읽기 중 오류가 발생했습니다. 오류 메시지: {ex.Message}", "확인");
        }

        return string.Empty;
    }

    private void SaveConfig(string jsonText)
    {
        try
        {
            // JSON 문자열을 파일에 저장
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "config.json");
            File.WriteAllText(filePath, jsonText);

            // 성공적으로 저장되었음을 알리는 메시지 표시
            DisplayAlert("저장 성공", "Config가 성공적으로 저장되었습니다.", "확인");
            _ipAddress = jsonText;
            MessagingCenter.Send<object, string>(this, "IP_ADDRESS", _ipAddress);
            Navigation.PopModalAsync();



        }
        catch (Exception ex)
        {
            // 저장 중 오류가 발생한 경우 오류 메시지 표시
            DisplayAlert("오류", $"저장 중 오류가 발생했습니다. 오류 메시지: {ex.Message}", "확인");
        }
    }

    private string FormatIpAddress(string ipAddress)
    {
        // IP 주소 형식 변환 (점 4개로 고정)
        if (IPAddress.TryParse(ipAddress, out IPAddress ip))
        {
            return ip.ToString();
        }

        return string.Empty;
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

