using FFImageLoading.Svg.Forms;
using Plugin.Connectivity;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorConnectPage : ContentPage
    {
        private links picture_lincs = new links();
        private ConnectClass connectClass = new ConnectClass();

        public ErrorConnectPage()
        {
            if (connectClass.CheckConnection()) { Navigation.PopModalAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += async (s, e) => { if (connectClass.CheckConnection()) { await Navigation.PopModalAsync(); } };// обработка изменения состояния подключения
            InitializeComponent();
            Error_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "ErrirConnect.png");
        }
    }
}