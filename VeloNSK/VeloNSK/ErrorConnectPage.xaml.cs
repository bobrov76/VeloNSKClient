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
        links picture_lincs = new links();
        ConnectClass connectClass = new ConnectClass();
        public ErrorConnectPage()
        {
            DisplayAlert("", picture_lincs.LinksResourse() + "connect.gif", "Ok");
            //Gif.Source = SvgImageSource.FromResource(picture_lincs.LinksResourse() + "Del_text.png");
            //Gif.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "connect.gif");
            // AnimationView.Resources =  ImageSource.FromResource(picture_lincs.LinksResourse() + "connect.gif");
            if (connectClass.CheckConnection()) { Navigation.PopModalAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += async (s, e) => { if (connectClass.CheckConnection()) { await Navigation.PopModalAsync(); } };// обработка изменения состояния подключения
            InitializeComponent();
        }

    }
}