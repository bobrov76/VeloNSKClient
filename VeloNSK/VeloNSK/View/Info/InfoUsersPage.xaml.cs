using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise.Servise;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK.View.Info
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoUsersPage : ContentPage
    {
        private links picture_lincs = new links();
        private ConnectClass connectClass = new ConnectClass();
        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();
        private HttpClient _client;
        private string pdfUrl = "";

        public InfoUsersPage()
        {
            _client = new HttpClient();

            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());
            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            InitializeComponent();
            LoadingAsync();
            Save_Button.Clicked += async (s, e) => { await DownloadAndSaveImage(pdfUrl); };
            Head_Button.Clicked += async (s, e) => { await Navigation.PopModalAsync(); };
        }

        private async Task LoadingAsync()
        {
            Main_RowDefinition_Two.Height = 0;
            Main_RowDefinition_Activiti.Height = new GridLength(1, GridUnitType.Star);
            activityIndicator.IsRunning = true;
            string pdfUrl = "http://90.189.158.10/folders/TrebovanieOfUsers.pdf";
            var googleUrl = "http://drive.google.com/viewerng/viewer?embedded=true&url=";
            if (Device.RuntimePlatform == Device.iOS)
            {
                InfoUser_WebView.Source = pdfUrl;
            }
            else if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.UWP)
            {
                InfoUser_WebView.Source = new UrlWebViewSource() { Url = googleUrl + pdfUrl };
            }
            await Task.Delay(500);
            Main_RowDefinition_Two.Height = new GridLength(1, GridUnitType.Star);
            Main_RowDefinition_Activiti.Height = 0;
            activityIndicator.IsRunning = false;
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PopModalAsync();
        } //Переход на страницу с ошибкой интернет соединения

        private async Task DownloadAndSaveImage(string get_path)
        {
            try
            {
                using (var response = await _client.GetStreamAsync(get_path))
                {
                    var filePath = await response.SaveToLocalFolderAsync($"Требования{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.jpg");
                    await DisplayAlert("", filePath, "Ok");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DownloadAndSaveImage Exception: {ex}");
            }
        }

        private new void SizeChanged(object sender, EventArgs e)
        {
            if (size_form.GetHeightSize() < 600) Main_RowDefinition_One.Height = 0;
            if (size_form.GetHeightSize() > 600) Main_RowDefinition_One.Height = 60;
            if (size_form.GetHeightSize() < 600) Main_RowDefinition_Fore.Height = 0;
            if (size_form.GetHeightSize() > 600) Main_RowDefinition_Fore.Height = 60;
        }
    }
}