using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Messaging;
using VeloNSK.HelpClass.Style;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoContactsPage : ContentPage
    {
        private links picture_lincs = new links();
        private ConnectClass connectClass = new ConnectClass();
        private MessagingAPI messagingAPI = new MessagingAPI();
        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();

        public InfoContactsPage()
        {
            InitializeComponent();
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            App.Current.Properties["ID_Form"] = "InfoMemuPage";//ID формы для навигации

            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());
            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());

            Head_Button.Clicked += async (s, e) => await Navigation.PopModalAsync();
            Block_Button_One.Clicked += (s, e) => SetPhoneCell();
            Block_Button_Two.Clicked += (s, e) => PopupNavigation.Instance.PushAsync(new GetMasagesPopupPage());
        }

        private async Task Connect_ErrorAsync()
        {
            await Navigation.PopModalAsync();
        } //Переход на страницу с ошибкой интернет соединения

        private void SetPhoneCell()
        {
            messagingAPI.MakePhoneCall("+79876543210");
        }

        private new void SizeChanged(object sender, EventArgs e)
        {
            if (size_form.GetHeightSize() < 700) Main_RowDefinition_Three.Height = new GridLength(0.3, GridUnitType.Star);
            if (size_form.GetHeightSize() > 700) Main_RowDefinition_Three.Height = new GridLength(1, GridUnitType.Star);
        }
    }
}