using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise.Model;
using VeloNSK.APIServise.Servise;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using VeloNSK.View;
using VeloNSK.View.User;
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
        private LoginUsersService loginUsersService = new LoginUsersService();
        private bool animate;

        public UserPage()
        {
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            InitializeComponent();
            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());

            Head_Button.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Head_Button);
                //await Task.Delay(300);
                await Navigation.PushModalAsync(new MainPage(), animate);
            };
            Block_Button_Main_Profil.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Block_Button_Main_Profil);
                //await Task.Delay(300);
                InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
                await Navigation.PushModalAsync(new PersonalAccountPage(loginUsers.IdUsers, false), animate);
            };

            Block_Button_Main_One.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Block_Button_Main_One);
                //await Task.Delay(300);
                await Navigation.PushModalAsync(new GegShempionatePage(), animate);
            };
            Block_Button_Main_Two.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Block_Button_Main_Two);
                //await Task.Delay(300);
                await Navigation.PushModalAsync(new MyPatisipantPage(), animate);
            };

            Block_Button_Main_Three.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Block_Button_Main_Three);
                //await Task.Delay(300);
                await Navigation.PushModalAsync(new MyResultPatisipantPage(), animate);
            };
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage(), animate);
        }
    }
}