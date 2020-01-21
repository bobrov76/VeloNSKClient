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
using VeloNSK.View.Admin.Users;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalAccountPage : ContentPage
    {
        private RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        private LoginUsersService loginUsersService = new LoginUsersService();
        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();
        private ConnectClass connectClass = new ConnectClass();
        private links picture_lincs = new links();
        private Animations animations = new Animations();
        private bool animate;
        private int ID;

        public PersonalAccountPage()
        {
            InitializeComponent();
            Get();
            User_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());
            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            Redact_Button.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "redact_user.jpg");

            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            Head_Button.Clicked += async (s, e) =>
            {
                animations.Animations_Button(Head_Button);
                await Task.Delay(1000);
                await Navigation.PopModalAsync();
            };

            Redact_Button.Clicked += async (s, e) =>
            {
                await Navigation.PushModalAsync(new AddUsersPage(ID, "Redacting"), animate);
            };
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage());
        }

        private new void SizeChanged(object sender, EventArgs e)
        {
            double width = size_form.GetWidthSize();
            double height = size_form.GetHeightSize();
            if (width > height)
            {
                outerStack.Orientation = StackOrientation.Horizontal;
            }
            else
            {
                outerStack.Orientation = StackOrientation.Vertical;
            }
        }

        private async Task Get()
        {
            InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
            IEnumerable<UserHelth> userHelths = await registrationUsersService.get_hels_status();
            ID = loginUsers.IdUsers;
            if (loginUsers.Isman) { Pol_Lable.Text += "Мужской"; }
            else { Pol_Lable.Text += "Женский"; }
            userHelths = userHelths.Where(p => p.IdHealth == loginUsers.IdHelth);
            foreach (UserHelth userHelth in userHelths)
            {
                StatusHels_Lable.Text += userHelth.NameHealth;
            }
            FIO_Lable.Text += loginUsers.Fam + " " + loginUsers.Name + " " + loginUsers.Patronimic;
            Email_Lable.Text += loginUsers.Email;
            Login_Lable.Text += loginUsers.Login;
            User_Image.Source = new UriImageSource
            {
                CachingEnabled = false,
                Uri = new System.Uri(loginUsers.Logo)
            };
        }
    }
}