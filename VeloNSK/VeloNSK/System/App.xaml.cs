using System;
using VeloNSK.View;
using VeloNSK.View.Admin;
using VeloNSK.View.Admin.Participations;
using VeloNSK.View.Admin.Participations.Compitentions;
using VeloNSK.View.Admin.Participations.Distanse;
using VeloNSK.View.Admin.ResultParticipation;
using VeloNSK.View.Admin.Users;
using VeloNSK.View.Gateri;
using VeloNSK.View.Info;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            int a = 0;
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}