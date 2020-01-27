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
        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();
        private ConnectClass connectClass = new ConnectClass();
        private Animations animations = new Animations();
        private links picture_lincs = new links();
        private bool animate;
        private int ID;

        public PersonalAccountPage(int id, bool IsAdmin)
        {
            InitializeComponent();
            Get(id);
            if (IsAdmin)
            {
                Redact_Button.IsVisible = false;
                Profile_Lable.Text = "Профиль участника";
            }
            else
            {
                Redact_Button.IsVisible = true;
                Profile_Lable.Text = "Мой профиль";
            }

            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };
            Redact_Button.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "redact_user.jpg");
            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());

            Head_Button.Clicked += async (s, e) =>
            {
                animations.Animations_Button(Head_Button);
                await Task.Delay(300);
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

        private async Task Get(int id)
        {
            try
            {
                InfoUser loginUsers = await registrationUsersService.Get_user_id(id);
                IEnumerable<UserHelth> userHelths = await registrationUsersService.get_hels_status();
                ID = loginUsers.IdUsers;
                if (loginUsers.Isman) { Pol_Lable.Text += "Мужской"; }
                else { Pol_Lable.Text += "Женский"; }
                userHelths = userHelths.Where(p => p.IdHealth == loginUsers.IdHelth);
                foreach (UserHelth userHelth in userHelths)
                {
                    StatusHels_Lable.Text += userHelth.NameHealth;
                }
                if (loginUsers.Logo != null)
                {
                    User_Image.Source = new UriImageSource
                    {
                        CachingEnabled = true,
                        CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                        Uri = new Uri(loginUsers.Logo)
                    };
                }
                else
                {
                    User_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "nophotouser.png");
                }
                FIO_Lable.Text += loginUsers.Fam + " " + loginUsers.Name + " " + loginUsers.Patronimic;
                Yars_Lable.Text += loginUsers.Years;
                Email_Lable.Text += loginUsers.Email;
                Login_Lable.Text += loginUsers.Login;
            }
            catch { }
        }
    }
}