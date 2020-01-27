using Plugin.Connectivity;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK.View.Autorization
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewPinPage : PopupPage
    {
        private links picture_lincs = new links();
        private ConnectClass connectClass = new ConnectClass();
        private Animations animations = new Animations();

        public NewPinPage()
        {
            InitializeComponent();

            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            OctocatImage.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "replispasswd.png");
            CloseImage.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "close_circle_button.png");
            DisplayAlert("Предупреждение", "Pin-code должен состоять из 4 цифр", "Ok");
            GetMasageButton.Clicked += (s, e) => SetMail();
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage());
        }

        private void SetMail()//Pin code
        {
            if (Pin_Entry.Text.Length == 4)
            {
                if (int.TryParse(Pin_Entry.Text, out int num))
                {
                    if (int.TryParse(Pin_Repid_Entry.Text, out int nums))
                    {
                        if (Pin_Entry.Text != null && Pin_Repid_Entry.Text != null && Pin_Entry.Text == Pin_Repid_Entry.Text)
                        {
                            App.Current.Properties["pin_code"] = Pin_Repid_Entry.Text;
                            CloseAllPopup();
                        }
                        else
                        {
                            animations.Animations_Entry(Pin_Repid_Entry);
                            Pin_Repid_Entry.Text = "";
                            Pin_Entry.Text = "";
                        }
                    }
                    else
                    {
                        animations.Animations_Entry(Pin_Repid_Entry);
                        Pin_Repid_Entry.Text = "";
                    }
                }
                else
                {
                    animations.Animations_Entry(Pin_Entry);
                    Pin_Entry.Text = "";
                }
            }
            else
            {
                animations.Animations_Entry(Pin_Repid_Entry);
                Pin_Repid_Entry.Text = "";
                Pin_Entry.Text = "";
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
        }

        protected override async Task OnAppearingAnimationEndAsync()
        {
            if (!IsAnimationEnabled)
                return;

            var translateLength = 400u;

            await Task.WhenAll(
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