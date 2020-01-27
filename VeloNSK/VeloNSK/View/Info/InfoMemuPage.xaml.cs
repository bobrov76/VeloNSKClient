using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using VeloNSK.View.Gateri;
using VeloNSK.View.Info;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoMemuPage : ContentPage
    {
        private links picture_lincs = new links();
        private Animations animations = new Animations();
        private ConnectClass connectClass = new ConnectClass();
        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();

        public InfoMemuPage()
        {
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            InitializeComponent();
            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());
            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());

            Head_Button.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Head_Button);
                //await Task.Delay(1000);
                await Navigation.PopModalAsync();//Переход назад
            };
            Block_Button_One.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Block_Button_One);
                //await Task.Delay(1000);
                await Navigation.PushModalAsync(new PhotoGaleri());
            };
            Block_Button_Two.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Block_Button_Two);
                //await Task.Delay(1000);
                await Navigation.PushModalAsync(new InfoUsersPage());
            };
            Block_Button_Three.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Block_Button_Three);
                //await Task.Delay(1000);
                await Navigation.PushModalAsync(new InfoMapsPage());
            };
            Block_Button_Fore.Clicked += async (s, e) =>
             {
                 //animations.Animations_Button(Block_Button_Fore);
                 //await Task.Delay(1000);
                 await Navigation.PushModalAsync(new InfoContactsPage());
             };
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PopModalAsync();
        } //Переход на страницу с ошибкой интернет соединения

        private new void SizeChanged(object sender, EventArgs e)
        {
            if (size_form.GetHeightSize() < 800) Main_RowDefinition_Three.Height = 0;
            if (size_form.GetHeightSize() > 800) Main_RowDefinition_Three.Height = new GridLength(1, GridUnitType.Star);
            if (size_form.GetHeightSize() < 600) Main_RowDefinition_One.Height = 0;
            if (size_form.GetHeightSize() > 600) Main_RowDefinition_One.Height = new GridLength(1, GridUnitType.Star);
        }
    }
}