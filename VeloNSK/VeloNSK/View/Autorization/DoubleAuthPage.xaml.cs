using Plugin.Connectivity;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
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

namespace VeloNSK.View.Autorization
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DoubleAuthPage : PopupPage
    {
        private DoubleAuthenticationService doubleAuthenticationService = new DoubleAuthenticationService();

        private bool alive = false;
        private DateTime endTime;
        private DateTime start_time;
        private string get_cod;
        private Animations animations = new Animations();

        private delegate void Connect_Error();

        private links picture_lincs = new links();
        private ConnectClass connectClass = new ConnectClass();
        private bool animate;
        private RegularValidate regularValidate = new RegularValidate();

        public DoubleAuthPage(int ID, string status)
        {
            InitializeComponent();
            DisplayAlert("", ID.ToString(), "");
            switch (status)
            {
                case "ReplisPasswd": Name_Lable.Text = "Изменение пароля"; PinCode_Entry.IsVisible = false; break;
                case "DobleOuth": Name_Lable.Text = "Двухфакторная аутентификация"; PinCode_Entry.IsVisible = true; break;
            }
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            OctocatImage.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "replispasswd.png");
            CloseImage.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "close_circle_button.png");

            GetMasageButton.Clicked += async (s, a) =>
            {
                switch (status)
                {
                    case "ReplisPasswd": await ReplasePasswordAsync(ID); break;
                    case "DobleOuth": await DoubleAuthAsync(ID); break;
                }
            };
            //  Authenticatio_Password_Entry.TextChanged += async (s, e) => { await Authenticatio_Password_Entry_TextChangedAsync(); };
        }

        private async Task<string> ReplasePasswordAsync(int ID)
        {
            try
            {
                string respond = "";
                if (regularValidate.Vadidation(Email_Entry.Text, @"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)") == false)
                {
                    await DisplayAlert("Ошибка", "E-mail введен некорректно", "Ok");
                    animations.Animations_Entry(Email_Entry);
                    Email_Entry.Text = "";
                }
                else
                {
                    if (await GetClient(ID) == Email_Entry.Text)
                    {
                        respond = await doubleAuthenticationService.Post(ID.ToString(), "ReplisPassword");
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Пользователя с таким логином не существует", "Ok");
                    }
                }
                return respond;
            }
            catch { return "Error"; }
        }

        private async Task<string> DoubleAuthAsync(int ID)
        {
            string passwd = "";
            if (regularValidate.Vadidation(Email_Entry.Text, @"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)") == false)
            {
                await DisplayAlert("Ошибка", "E-mail введен некорректно", "Ok");
                animations.Animations_Entry(Email_Entry);
                Email_Entry.Text = "";
            }
            else
            {
                if (await GetClient(ID) == Email_Entry.Text)
                {
                    passwd = await doubleAuthenticationService.Post(ID.ToString(), "DoubleAutentifity");
                    alive = true;
                    start_time = DateTime.UtcNow;
                    endTime = start_time.AddMinutes(2);
                    Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
                    PinCode_Entry.IsEnabled = true;
                    Email_Entry.IsEnabled = false;

                    if (PinCode_Entry.Text.Length == 4)
                    {
                        if (passwd == PinCode_Entry.Text)
                        {
                            alive = false;//Остановка таймера

                            if (App.Current.Properties.TryGetValue("rol_user", out object rol_user))
                            {
                                switch ((short)rol_user)
                                {
                                    case 1: await Navigation.PushModalAsync(new AdminPage()); break;
                                    case 2: await Navigation.PushModalAsync(new UserPage()); break;
                                }
                            }
                        }
                        else
                        {
                            animations.Animations_Entry(PinCode_Entry);
                            PinCode_Entry.Text = "";
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", "Пользователя с таким логином не существует", "Ok");
                }
            }
            return "";
        }

        public async Task<string> GetClient(int ID)
        {
            RegistrationUsersService registrationUsersService = new RegistrationUsersService();
            IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
            var user = infoUsers.FirstOrDefault(x => x.IdUsers == ID);
            return user.Email;
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage(), animate);
        }//Страница ошибки соединения

        private bool OnTimerTick()//Обработка времени на выполнение
        {
            TimeSpan remainingTime = endTime - DateTime.UtcNow;
            Timer_Lable.Text = (remainingTime.Minutes + ":" + remainingTime.Seconds).ToString();
            if (remainingTime.Minutes == 0 && remainingTime.Seconds == 0)
            {
                Timer_Lable.Text = "Время вышло попробуйте заново";
                get_cod = "";
                // Authenticatio_Password_Entry.IsEnabled = false;
                // Email_Button.IsEnabled = true;
                Email_Entry.IsEnabled = true;
                return alive = false;
            }
            return alive;
        }

        protected override async Task OnAppearingAnimationEndAsync()
        {
            if (!IsAnimationEnabled)
                return;

            var translateLength = 400u;

            await Task.WhenAll(
                // MasageEditor.TranslateTo(0, 0, easing: Easing.SpringOut, length: translateLength),
                // MasageEditor.FadeTo(1),
                (new Func<Task>(async () =>
                {
                    await Task.Delay(200);
                }))());

            await Task.WhenAll(
                CloseImage.FadeTo(1),
                CloseImage.ScaleTo(1, easing: Easing.SpringOut),
                CloseImage.RotateTo(0),
                GetMasageButton.ScaleTo(1),
                GetMasageButton.FadeTo(1));
        }

        protected override async Task OnDisappearingAnimationBeginAsync()
        {
            if (!IsAnimationEnabled)
                return;

            var taskSource = new TaskCompletionSource<bool>();

            var currentHeight = FrameContainer.Height;

            await Task.WhenAll(
                //   MasageEditor.FadeTo(0),
                GetMasageButton.FadeTo(0));

            FrameContainer.Animate("HideAnimation", d =>
            {
                FrameContainer.HeightRequest = d;
            },
            start: currentHeight,
            end: 170,
            finished: async (d, b) =>
            {
                await Task.Delay(300);
                taskSource.TrySetResult(true);
            });

            await taskSource.Task;
        }

        private async void OnLogin(object sender, EventArgs e)
        {
            var loadingPage = new GetMasagesPopupPage();
            await Navigation.PushPopupAsync(loadingPage);
            await Task.Delay(2000);
            await Navigation.RemovePopupPageAsync(loadingPage);
            await Navigation.PushPopupAsync(new GetMasagesPopupPage());
        }

        private void OnCloseButtonTapped(object sender, EventArgs e)
        {
            CloseAllPopup();
        }

        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();

            return false;
        }

        private async void CloseAllPopup()
        {
            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}