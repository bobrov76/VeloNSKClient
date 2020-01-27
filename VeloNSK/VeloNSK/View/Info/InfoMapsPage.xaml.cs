using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise.Model;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace VeloNSK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoMapsPage : ContentPage
    {
        private links picture_lincs = new links();
        private ConnectClass connectClass = new ConnectClass();
        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();

        public InfoMapsPage()
        {
            InitializeComponent();
            InfoUser_WebView.Source = "http://90.189.158.10/Maps/index.html";
            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());
            LoadingAsync();

            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            Head_Button.Clicked += async (s, e) => { await Navigation.PopModalAsync(); };
        }

        private async Task LoadingAsync()
        {
            Main_RowDefinition_Two.Height = 0;
            Main_RowDefinition_Activiti.Height = new GridLength(1, GridUnitType.Star);
            activityIndicator.IsRunning = true;
            await Task.Delay(3000);
            Main_RowDefinition_Two.Height = new GridLength(1, GridUnitType.Star);
            Main_RowDefinition_Activiti.Height = 0;
            activityIndicator.IsRunning = false;
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PopModalAsync();
        } //Переход на страницу с ошибкой интернет соединения

        private new void SizeChanged(object sender, EventArgs e)
        {
            if (size_form.GetHeightSize() < size_form.GetWidthSize())
            {
                Main_RowDefinition_Ziro.Height = 0;
                Main_RowDefinition_Fore.Height = 0;
                Main_RowDefinition_One.Height = 0;
            }
            else
            {
                Main_RowDefinition_Ziro.Height = 70;
                Main_RowDefinition_Fore.Height = 60;
                Main_RowDefinition_One.Height = 30;
            }
        }
    }
}