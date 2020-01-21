using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using VeloNSK.View;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPage : ContentPage
    {
        private links picture_lincs = new links();
        private ConnectClass connectClass = new ConnectClass();
        private Animations animations = new Animations();
        private bool animate;

        public UserPage()
        {
            InitializeComponent();

            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            Fon.BackgroundImageSource = ImageSource.FromResource(picture_lincs.GetFon()); //Устанавливаем фон
            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());
            //Fon_Picture.Source= ImageSource.FromResource(picture_lincs.LinksResourse()+ "user_fon.jpg");
            //User_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "nophotouser.png");
            //Redact_User.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "redact_user.jpg");
            //Maim_Fon.IconImageSource = ImageSource.FromResource(picture_lincs.GetLogo());

            Head_Button.Clicked += async (s, e) =>
            {
                animations.Animations_Button(Head_Button);
                await Task.Delay(1000);
                await Navigation.PushModalAsync(new MainPage(), animate);
            };
            Block_Button_Main_Profil.Clicked += async (s, e) =>
            {
                animations.Animations_Button(Block_Button_Main_Profil);
                await Task.Delay(1000);
                await Navigation.PushModalAsync(new PersonalAccountPage(), animate);
            };
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage(), animate);
        }
    }
}