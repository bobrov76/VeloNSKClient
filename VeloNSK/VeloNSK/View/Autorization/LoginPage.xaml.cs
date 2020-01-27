using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise;
using VeloNSK.APIServise.Model;
using VeloNSK.APIServise.Servise;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using VeloNSK.HelpClass.Validate;
using VeloNSK.View;
using VeloNSK.View.Autorization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private links picture_lincs = new links();
        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();
        private LoginUsersService loginUsersService = new LoginUsersService();
        private ConnectClass connectClass = new ConnectClass();
        private Animations animations = new Animations();
        private JWTServise JWT = new JWTServise();
        private Hash hash = new Hash();
        private bool alive = false;
        private DateTime endTime;
        private DateTime start_time;
        private bool animate;

        public LoginPage()
        {
            InitializeComponent();
            if (!connectClass.CheckConnection()) Connect_ErrorAsync();//Проверка интернет соединения

            string PIN = App.Current.Properties["pin_code"].ToString();

            if (PIN.Length > 0)
            {
                AutoLogin_CheckBox.IsChecked = true;
            }
            else { AutoLogin_CheckBox.IsChecked = false; }
            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());
            password_status_image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "anvisible_password.png");
            Users_Fon_Images.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "UserFon.png");
            Password_Entry.Text = "";

            AutoLogin_CheckBox.CheckedChanged += async (s, e) => { await AutoLogin_CheckedChangedAsync(); };

            Head_Lable.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
            Login_Button.Clicked += async (s, e) =>
            {
                animations.Animations_Button(Login_Button);
                await Login_Async();
            };
            TapGestureRecognizer tapGesture = new TapGestureRecognizer //Распознание жеста косания
            {
                NumberOfTapsRequired = 1
            };
            tapGesture.Tapped += async (s, e) =>//Обработка касания
            {
                try
                {
                    if (Login_Entry.Text.Length == 17 && Login_Entry.Text.Length > 0)
                    {
                        string id = await GetClient();
                        if (id != null)
                        {
                            Error_Password_Lable.TextColor = Color.Red;
                            await Task.Delay(300);
                            await PopupNavigation.Instance.PushAsync(new DoubleAuthPage(Convert.ToInt32(id), "ReplisPasswd"));
                        }
                        else { await DisplayAlert("Ошибка", "Логин не указан,необходимо указать логин", "Ok"); }
                    }
                    else
                    {
                        await DisplayAlert("Предупреждение", "Необходимо указать логин", "Ok");
                    }
                }
                catch { await DisplayAlert("Предупреждение", "Необходимо указать логин", "Ok"); }
            };
            Passwd_Lable.GestureRecognizers.Add(tapGesture);
            Back_Button.Clicked += async (s, e) =>
            {
                animations.Animations_Button(Back_Button);
                await Task.Delay(300);
                await Navigation.PopModalAsync();//Переход назад
            };
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };// обработка изменения состояния подключения
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage(), animate);
        }

        public async Task<string> GetClient()
        {
            RegistrationUsersService registrationUsersService = new RegistrationUsersService();
            IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
            var user = infoUsers.FirstOrDefault(x => x.Login == Login_Entry.Text);
            string ID = user.IdUsers.ToString();
            return ID;
        }

        private bool double_out = false;
        private int count_click = 0;

        private async Task Login_Async()//Запрос к серверу на получение данных для авторизации
        {
            try
            {
                if (Login_Entry.Text.Length != 0 && Password_Entry.Text.Length != 0)
                {
                    Dictionary<string, string> tokenDictionary = JWT.GetTokenDictionary(Login_Entry.Text, hash.GetHash(Password_Entry.Text));
                    if (tokenDictionary["access_token"] != "")
                    {
                        App.Current.Properties["token"] = tokenDictionary["access_token"];
                        InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
                        App.Current.Properties["rol_user"] = loginUsers.Rol;
                        if (!double_out)
                        {
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
                        else
                        {
                            string id = await GetClient();
                            if (id != null)
                            {
                                await PopupNavigation.Instance.PushAsync(new DoubleAuthPage(Convert.ToInt32(id), "DobleOuth"));
                            }
                        }
                    }
                    else
                    {
                        count_click++;
                        animations.Animations_Entry(Password_Entry);
                        Error_Password_RowDefinition.Height = 30;
                        Error_Password_Lable.Text = "Неверные данные!";
                    }
                }
                if (count_click >= 5)//
                {
                    Login_Button.IsEnabled = false;
                    alive = true;
                    start_time = DateTime.UtcNow;
                    endTime = start_time.AddSeconds(30); //AddMinutes(1);
                    Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
                    double_out = true;
                    Password_Entry.Text = "";
                }
                else if (count_click >= 2)
                {
                    Error_Password_RowDefinition.Height = 30;
                    Error_Password_Lable.Text = "Осталось : " + (5 - count_click).ToString() + " попытки";
                }
                else if (count_click >= 0)
                {
                    Error_Password_RowDefinition.Height = 30;
                    Error_Password_Lable.Text = "Поле Login или пароль незаполно";
                }
            }
            catch { await DisplayAlert("Ошибка", "Поля незаполнены", "Ok"); }
        }

        private bool OnTimerTick()//Обработка времени на выполнение
        {
            TimeSpan remainingTime = endTime - DateTime.UtcNow;
            Error_Password_RowDefinition.Height = 30;
            Error_Password_Lable.Text = "Повторить авторизацию вы можете через : " + (remainingTime.Minutes + ":" + remainingTime.Seconds).ToString();
            if (remainingTime.Minutes == 0 && remainingTime.Seconds == 0)
            {
                Error_Password_RowDefinition.Height = 0;
                Error_Password_Lable.Text = "";
                Login_Button.IsEnabled = true;
                count_click = 0;
                return alive = false;
            }
            return alive;
        }

        private new void SizeChanged(object sender, EventArgs e) //Стилизация
        {
            if (size_form.GetHeightSize() > size_form.GetWidthSize())
            {
                Login_ColumnDefinition_Ziro.Width = 20;
                Login_ColumnDefinition_One.Width = new GridLength(1, GridUnitType.Star);
                Login_ColumnDefinition_Two.Width = 20;
            }
            else
            {
                Login_ColumnDefinition_Ziro.Width = new GridLength(1, GridUnitType.Star);
                Login_ColumnDefinition_One.Width = 400;
                Login_ColumnDefinition_Two.Width = new GridLength(1, GridUnitType.Star);
            }
        }

        private bool status_password = false;

        private void Password_IsVisible(object sender, EventArgs e)
        {
            if (status_password == false)
            {
                password_status_image.Source = ImageSource.FromResource("VeloNSK.Resours.visible_passwprd.png");
                Password_Entry.IsPassword = false;
                status_password = true;
            }
            else
            {
                password_status_image.Source = ImageSource.FromResource("VeloNSK.Resours.anvisible_password.png");
                Password_Entry.IsPassword = true;
                status_password = false;
            }
        }

        private async Task AutoLogin_CheckedChangedAsync()
        {
            if (AutoLogin_CheckBox.IsChecked == true && Login_Entry.Text != "" && Password_Entry.Text != "")
            {
                App.Current.Properties["Login"] = Login_Entry.Text;
                App.Current.Properties["Password"] = Password_Entry.Text;
                await PopupNavigation.Instance.PushAsync(new NewPinPage());
            }
            else
            {
                AutoLogin_CheckBox.IsChecked = false;
                await DisplayAlert("Предупреждение", "Login или пароль отсутствует", "Ok");
                App.Current.Properties["Login"] = "";
                App.Current.Properties["Password"] = "";
                App.Current.Properties["pin_code"] = "";
            }
        }
    }
}