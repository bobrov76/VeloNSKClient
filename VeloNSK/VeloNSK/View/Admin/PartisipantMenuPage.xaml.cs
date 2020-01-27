using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using VeloNSK.View.Admin.Participations;
using VeloNSK.View.Admin.Participations.Compitentions;
using VeloNSK.View.Admin.Participations.Distanse;
using VeloNSK.View.Admin.ResultParticipation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK.View.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PartisipantMenuPage : ContentPage
    {
        private ConnectClass connectClass = new ConnectClass();
        private Animations animations = new Animations();
        private links picture_lincs = new links();
        private bool animate;

        public PartisipantMenuPage()
        {
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };
            InitializeComponent();

            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());

            Block_Button_Main_One.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Block_Button_Main_One);
                //await Task.Delay(300);
                await Navigation.PushModalAsync(new DistantionPage(), animate);
            };

            Block_Button_Main_Two.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Block_Button_Main_Two);
                //await Task.Delay(300);
                await Navigation.PushModalAsync(new CompitentionsPage(), animate);
            };

            Block_Button_Main_Three.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Block_Button_Main_Three);
                //await Task.Delay(300);
                await Navigation.PushModalAsync(new ParticipationsPage(), animate);
            };

            Block_Button_Main_Fore.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Block_Button_Main_Fore);
                //await Task.Delay(300);
                await Navigation.PushModalAsync(new ResultParticipationPage(), animate);//новое соревнование
            };

            Head_Button.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Head_Button);
                //await Task.Delay(300);
                await Navigation.PopModalAsync(animate);
            };
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage(), animate);
        }
    }
}