﻿using Indicator.Model;
using Indicator.TCP;
using Indicator.Utill;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;

namespace Indicator;


public partial class MainPage : ContentPage
{
    private MainViewModel viewModel;

    private string _ipAddress = "192.168.8.100";

    private string _languageCode;


    public MainPage()
    {
        InitializeComponent();
        viewModel  = new MainViewModel(); //binding
        BindingContext = viewModel;

        var (ipAddress, languageCode) = ReadSavedJson();
        _ipAddress = ipAddress;
        _languageCode = languageCode;


        SetLanguage(_languageCode);

     //   string key = $"string_offline.{_languageCode}";
     //   string offlineString = Application.Current.Resources[key] as string;



        viewModel.LeftStatusLabel = Resources[$"string_offline.{_languageCode}"].ToString();
        viewModel.RightStatusLabel = Resources[$"string_offline.{_languageCode}"].ToString();
        viewModel.back_ground_src = "screen_inactive.png";

        viewModel.QR_Label = "-";
        viewModel.LeftWeightLabel = "-";
        viewModel.RightWeightLabel = "-";
        viewModel.Vision_h_Label = "-";
        viewModel.TotalWeight_Label = "-";


 
        if((_ipAddress == "")  || (_ipAddress == ""))
        {
            _ipAddress = "192.168.8.100";
            _languageCode = "en";
            SaveConfig(_ipAddress, _languageCode);
        }


        TCPSocketClient msg = new TCPSocketClient(_ipAddress);
        msg.Initialize();



        OnDataReceive();

        OnStatusReceive();


    
    }

    


    private void SetLanguage(string languageCode)
    {
        _languageCode = languageCode;

        // 언어에 맞는 리소스 키를 사용하여 텍스트 갱신
        Resources["label_QRInfo"] = Resources[$"label_QRInfo.{_languageCode}"];
        Resources["label_Height"] = Resources[$"label_Height.{_languageCode}"];
        Resources["Button_SendPlatform"] = Resources[$"Button_SendPlatform.{_languageCode}"];
        string temp = Resources[$"string_offline.{_languageCode}"].ToString();
        viewModel.LeftStatusLabel = temp;
        viewModel.RightStatusLabel = Resources[$"string_offline.{_languageCode}"].ToString();
    }


    private void Setting_btn(object sender, EventArgs e)
    {
        OpenPopup();
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
            viewModel.LeftStatusLabel = Resources[$"string_offline.{_languageCode}"].ToString();
            viewModel.RightStatusLabel = Resources[$"string_offline.{_languageCode}"].ToString();
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
            viewModel.LeftStatusLabel = Resources[$"string_online.{_languageCode}"].ToString();
            viewModel.RightStatusLabel = Resources[$"string_online.{_languageCode}"].ToString();
            viewModel.LeftColorLabel = new Color(173, 255, 47); //GreenYellow
            viewModel.RightColorLabel = new Color(173, 255, 47); //GreenYellow
            viewModel.back_ground_src = "screen_active.png";
        }

    }


    RadioButton koreanRadioButton;
    RadioButton englishRadioButton;
    RadioButton japaneseRadioButton;

   // 

    private void OnSendBtn(object sender, EventArgs e)
    {
        SendBackEndModel model = new SendBackEndModel();
        model.send_backend.eventValue = "pick_up";
        string json_body = Utill_.ObjectToJson(model);
        MessagingCenter.Send<object, string>(this, "TCP_DATA_Send", json_body);
    }

    private async void OpenPopup()
    {
        // 저장된 JSON 데이터를 읽어옴
        var (ipAddress, languageCode) = ReadSavedJson();
        _languageCode = languageCode;
        _ipAddress = ipAddress;



        // 팝업에서 입력받을 데이터를 위한 Entry 생성
        Entry jsonEntry = new Entry
        {
            Placeholder = "(ex: 192.168.0.1)",
            Text = FormatIpAddress(_ipAddress) // 저장된 JSON 데이터를 Entry에 설정하면서 형식 변환
        };

        // "접속 IP" 라벨 추가
        Label ipLabel = new Label
        {
            Text = Resources[$"string_ip_connect_info.{_languageCode}"].ToString(),
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), // 폰트 크기를 크게 설정
            FontAttributes = FontAttributes.Bold | FontAttributes.Italic // 두껍고 기울임 스타일 설정
        };


        Label LanguageLabel = new Label
        {
            Text = Resources[$"string_select_language.{_languageCode}"].ToString(),
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), // 폰트 크기를 크게 설정
            FontAttributes = FontAttributes.Bold | FontAttributes.Italic // 두껍고 기울임 스타일 설정
        };


        // 언어 선택을 위한 RadioButton 추가
        koreanRadioButton = new RadioButton { Content = "한국어", GroupName = "Language", IsChecked = true };
        englishRadioButton = new RadioButton { Content = "English", GroupName = "Language" };
        japaneseRadioButton = new RadioButton { Content = "日本語", GroupName = "Language" };


        koreanRadioButton.IsChecked = false;
        englishRadioButton.IsChecked = false;
        japaneseRadioButton.IsChecked = false;

        if (_ipAddress == "ko")
        {
            koreanRadioButton.IsChecked = true;
        }
        else if (_ipAddress == "en")
        {
            englishRadioButton.IsChecked = true;
        }
        else
        {
            japaneseRadioButton.IsChecked = true;
        }



        // 팝업 생성
        ContentPage popupPage = new ContentPage
        {
            Content = new StackLayout
            {
                Children = {
                ipLabel,
                jsonEntry,
                LanguageLabel,
                koreanRadioButton,
                englishRadioButton,
                japaneseRadioButton,
                new Button { Text = Resources[$"string_save.{_languageCode}"].ToString(), Command = new Command(() => SaveConfig(FormatIpAddress(jsonEntry.Text), GetSelectedLanguage())), Margin = new Thickness(0, 10, 0, 0) },
                new Button { Text = Resources[$"string_close.{_languageCode}"].ToString(), Command = new Command(async () => await Navigation.PopModalAsync()), Margin = new Thickness(0, 10, 0, 0) }
            }
            }
        };

        // 팝업 표시
        await Navigation.PushModalAsync(popupPage);
    }

    private (string ipAddress, string languageCode) ReadSavedJson()
    {
        try
        {
            // 저장된 JSON 파일에서 데이터를 읽어옴
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "config.json");
            if (File.Exists(filePath))
            {
                string jsonText = File.ReadAllText(filePath);
                JObject json = JObject.Parse(jsonText);

                string ipAddress = json["ipAddress"]?.ToString();
                string languageCode = json["languageCode"]?.ToString();

                return (ipAddress, languageCode);
            }
        }
        catch (Exception ex)
        {
            // 읽기 중 오류가 발생한 경우 디폴트 값으로 저장
            SaveConfig("192.168.8.100", "en");

            // 디폴트 값 반환
            return ("192.168.8.100", "en");
        }

        return (string.Empty, string.Empty);
    }

    private void SaveConfig(string ipAddress, string languageCode)
    {
        try
        {
            // JSON 형식으로 저장할 객체 생성
            var config = new
            {
                ipAddress,
                languageCode
            };

            // JSON 문자열로 변환
            string jsonText = JsonConvert.SerializeObject(config);

            // JSON 문자열을 파일에 저장
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "config.json");
            File.WriteAllText(filePath, jsonText);

            // 성공적으로 저장되었음을 알리는 메시지 표시
            //DisplayAlert(Resources[$"string_save_success.{_languageCode}"].ToString(), Resources[$"string_save_success_info.{_languageCode}"].ToString(), Resources[$"string_ok.{_languageCode}"].ToString());

            _ipAddress = ipAddress;
            MessagingCenter.Send<object, string>(this, "IP_ADDRESS", _ipAddress);

            if (koreanRadioButton.IsChecked)
            {
                SetLanguage("ko");
            }
            else if (englishRadioButton.IsChecked)
            {
                SetLanguage("en");
            }
            else if (japaneseRadioButton.IsChecked)
            {
                SetLanguage("jp");
            }


            Navigation.PopModalAsync();



        }
        catch (Exception ex)
        {
            // 저장 중 오류가 발생한 경우 오류 메시지 표시
            //DisplayAlert("오류", $"저장 중 오류가 발생했습니다. 오류 메시지: {ex.Message}", "확인");
        }
    }




    private string GetSelectedLanguage()
    {
        if (koreanRadioButton.IsChecked)
        {
            return "ko";
        }
        else if (japaneseRadioButton.IsChecked)
        {
            return "jp";
        }
        else
        {
            return "en";
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

