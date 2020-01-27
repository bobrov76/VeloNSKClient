using Plugin.Connectivity;
using Plugin.Fingerprint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise.Model;
using VeloNSK.APIServise.Servise;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using VeloNSK.HelpClass.Validate;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PinLoginPage : ContentPage
    {
        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();
        private ConnectClass connectClass = new ConnectClass();
        private LoginUsersService loginUsersService = new LoginUsersService();
        private JWTServise JWT = new JWTServise();
        private links picture_lincs = new links();
        private Hash hash = new Hash();
        private bool animate;
        private string PIN_Code = "";
        private bool alive = true;

        public PinLoginPage()
        {
            InitializeComponent();

            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            Fingers();
            Get_User();
            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());
            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            //Button_del.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Del_text.png");
            //One_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circle_PIN.png");
            //Two_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circle_PIN.png");
            //Three_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circle_PIN.png");
            //Fore_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circle_PIN.png");

            Exit_Button.Clicked += async (s, e) =>
            {
                App.Current.Properties["Login"] = "";
                App.Current.Properties["Password"] = "";
                App.Current.Properties["pin_code"] = "";
                await Navigation.PushModalAsync(new LoginPage());
            };

            Button1.Clicked += (s, e) => PIN_Validate(Button1.Text);
            Button2.Clicked += (s, e) => PIN_Validate(Button2.Text);
            Button3.Clicked += (s, e) => PIN_Validate(Button3.Text);
            Button4.Clicked += (s, e) => PIN_Validate(Button4.Text);
            Button5.Clicked += (s, e) => PIN_Validate(Button5.Text);
            Button6.Clicked += (s, e) => PIN_Validate(Button6.Text);
            Button7.Clicked += (s, e) => PIN_Validate(Button7.Text);
            Button8.Clicked += (s, e) => PIN_Validate(Button8.Text);
            Button9.Clicked += (s, e) => PIN_Validate(Button9.Text);
            Button0.Clicked += (s, e) => PIN_Validate(Button0.Text);
            Button_del.Clicked += (s, e) => PIN_Del();
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage());
        }

        private int count_click = 0;

        public async Task Get_User()
        {
            InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
            PIN_Lable.Text = loginUsers.Name + " " + loginUsers.Patronimic;
        }

        private void PIN_Validate(string text)
        {
            if (PIN_Code.Length < 4)
            {
                PIN_Code += text;
                switch (PIN_Code.Length)
                {
                    case 1: One_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circl_PIN_Color.png"); break;
                    case 2: Two_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circl_PIN_Color.png"); break;
                    case 3: Three_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circl_PIN_Color.png"); break;
                    case 4: Fore_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circl_PIN_Color.png"); break;
                }
            }
            if (PIN_Code.Length == 4)
            {
                if (PIN_Code == App.Current.Properties["pin_code"].ToString())
                {
                    Login_Async();
                }
                else
                {
                    ErrorPin();
                    Task.Delay(10000);
                    count_click++;
                    PIN_Code = "";
                }
                if (count_click >= 5)//
                {
                    Navigation.PushModalAsync(new LoginPage(), animate);
                }
            }
        }

        private void PIN_Del()
        {
            if (PIN_Code.Length > 0)
            {
                PIN_Code = PIN_Code.Remove(PIN_Code.Length - 1);
                PIN_Lable.Text = PIN_Code;
                switch (PIN_Code.Length)
                {
                    case 0: One_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circle_PIN.png"); break;
                    case 1: Two_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circle_PIN.png"); break;
                    case 2: Three_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circle_PIN.png"); break;
                    case 3: Fore_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circle_PIN.png"); break;
                }
            }
        }

        private async Task Login_Async()//Запрос к серверу на получение данных для авторизации
        {
            try
            {
                string Login = App.Current.Properties["Login"].ToString();
                string Password = App.Current.Properties["Password"].ToString();
                if (Password != null && Login != null && Password != "" && Login != "")
                {
                    Dictionary<string, string> tokenDictionary = JWT.GetTokenDictionary(Login, hash.GetHash(Password));

                    App.Current.Properties["token"] = tokenDictionary["access_token"];
                    InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
                    App.Current.Properties["rol_user"] = loginUsers.Rol;
                    alive = false;
                    switch (App.Current.Properties["rol_user"])
                    {
                        case "Admin":
                            await Navigation.PushModalAsync(new AdminPage(), animate);
                            break;

                        case "User":
                            await Navigation.PushModalAsync(new UserPage(), animate);
                            break;
                    }
                }
            }
            catch { }
        }

        private async void Fingers()
        {
            await Task.Delay(2000);
            var sss = new System.Threading.CancellationToken();
            var scan = await CrossFingerprint.Current.AuthenticateAsync("Приложите палец к сканеру отпечатков", sss);
            if (scan.Authenticated)
            {
                await Login_Async();
            }
        }

        private async void ErrorPin()
        {
            uint timeout = 50;

            await One_Image.TranslateTo(-15, 0, timeout);
            await Two_Image.TranslateTo(-15, 0, timeout);
            await Three_Image.TranslateTo(-15, 0, timeout);
            await Fore_Image.TranslateTo(-15, 0, timeout);

            await One_Image.TranslateTo(15, 0, timeout);
            await Two_Image.TranslateTo(15, 0, timeout);
            await Three_Image.TranslateTo(15, 0, timeout);
            await Fore_Image.TranslateTo(15, 0, timeout);

            await One_Image.TranslateTo(-10, 0, timeout);
            await Two_Image.TranslateTo(-10, 0, timeout);
            await Three_Image.TranslateTo(-10, 0, timeout);
            await Fore_Image.TranslateTo(-10, 0, timeout);

            await One_Image.TranslateTo(10, 0, timeout);
            await Two_Image.TranslateTo(10, 0, timeout);
            await Three_Image.TranslateTo(10, 0, timeout);
            await Fore_Image.TranslateTo(10, 0, timeout);

            await One_Image.TranslateTo(-5, 0, timeout);
            await Two_Image.TranslateTo(-5, 0, timeout);
            await Three_Image.TranslateTo(-5, 0, timeout);
            await Fore_Image.TranslateTo(-5, 0, timeout);

            await One_Image.TranslateTo(5, 0, timeout);
            await Two_Image.TranslateTo(5, 0, timeout);
            await Three_Image.TranslateTo(5, 0, timeout);
            await Fore_Image.TranslateTo(5, 0, timeout);

            One_Image.TranslationX = 0;
            Two_Image.TranslationX = 0;
            Three_Image.TranslationX = 0;
            Fore_Image.TranslationX = 0;
            One_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circle_PIN.png");
            Two_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circle_PIN.png");
            Three_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circle_PIN.png");
            Fore_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "Circle_PIN.png");
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
    }
}