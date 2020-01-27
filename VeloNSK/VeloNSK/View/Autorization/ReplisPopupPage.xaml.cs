using Newtonsoft.Json;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise.Servise;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Messaging;
using VeloNSK.HelpClass.Style;
using VeloNSK.HelpClass.Validate;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK.View.Autorization
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReplisPopupPage : PopupPage
    {
        private links picture_lincs = new links();
        private MessagingAPI messagingAPI = new MessagingAPI();
        private ConnectClass connectClass = new ConnectClass();
        private RegularValidate validation = new RegularValidate();

        public ReplisPopupPage(string ID)
        {
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            InitializeComponent();
            OctocatImage.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "replispasswd.png");
            CloseImage.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "close_circle_button.png");
            GetMasageButton.Clicked += (s, e) => SetMailAsync(ID);
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage());
        }

        private async Task SetMailAsync(string ID)//Отправка письма
        {
            if (ID != null)
            {
                if (!validation.Vadidation(MasageEditor.Text, @"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)"))
                {
                    GetClientServise getClientServise = new GetClientServise();
                    HttpClient client = getClientServise.GetClient();
                    string result = await client.GetStringAsync("http://90.189.158.10/api/GetMail/replasepassword/" + ID);
                    await DisplayAlert("", JsonConvert.DeserializeObject<string>(result), "Ok");
                }
                else
                {
                    await DisplayAlert("Ошибка", "E-mail введен неверно", "Ok");
                }
            }
        }

        protected override void OnAppearingAnimationBegin()
        {
            base.OnAppearingAnimationBegin();

            FrameContainer.HeightRequest = -1;

            if (!IsAnimationEnabled)
            {
                CloseImage.Rotation = 0;
                CloseImage.Scale = 1;
                CloseImage.Opacity = 1;

                GetMasageButton.Scale = 1;
                GetMasageButton.Opacity = 1;

                return;
            }

            CloseImage.Rotation = 30;
            CloseImage.Scale = 0.3;
            CloseImage.Opacity = 0;

            GetMasageButton.Scale = 0.3;
            GetMasageButton.Opacity = 0;

            MasageEditor.TranslationX = MasageEditor.TranslationX = -10;
            MasageEditor.Opacity = MasageEditor.Opacity = 0;
        }

        protected override async Task OnAppearingAnimationEndAsync()
        {
            if (!IsAnimationEnabled)
                return;

            var translateLength = 400u;

            await Task.WhenAll(
                MasageEditor.TranslateTo(0, 0, easing: Easing.SpringOut, length: translateLength),
                MasageEditor.FadeTo(1),
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
                MasageEditor.FadeTo(0),
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
            var loadingPage = new ReplisPopupPage(null);
            await Navigation.PushPopupAsync(loadingPage);
            await Task.Delay(2000);
            await Navigation.RemovePopupPageAsync(loadingPage);
            await Navigation.PushPopupAsync(new ReplisPopupPage(null));
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