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
using VeloNSK.HelpClass.Validate;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadPage : PopupPage
    {
        links picture_lincs = new links();
        MessagingAPI messagingAPI = new MessagingAPI();
        ConnectClass connectClass = new ConnectClass();
        RegularValidate validation = new RegularValidate();
        public LoadPage()
        {
            InitializeComponent();
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы            
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };
            activity.IsEnabled = true;
            activity.IsRunning = true;
            activity.IsVisible = true;

        }
        public async Task Connect_ErrorAsync() { await Navigation.PushModalAsync(new ErrorConnectPage()); }
      
           
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