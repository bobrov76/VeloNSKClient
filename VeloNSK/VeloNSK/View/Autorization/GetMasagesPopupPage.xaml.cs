using Plugin.Connectivity;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Messaging;
using VeloNSK.HelpClass.Style;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GetMasagesPopupPage : PopupPage
    {
        private links picture_lincs = new links();
        private MessagingAPI messagingAPI = new MessagingAPI();
        private ConnectClass connectClass = new ConnectClass();

        public GetMasagesPopupPage()
        {
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };
            InitializeComponent();

            OctocatImage.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "masage_picture.png");
            CloseImage.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "close_circle_button.png");
            GetMasageButton.Clicked += (s, e) => SetMail();
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage());
        }

        private void SetMail()//Отправка письма
        {
            messagingAPI.SendEmail("velo.nsk2020@mail.ru", "Письмо от пользователя", MasageEditor.Text);
            CloseAllPopup();
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