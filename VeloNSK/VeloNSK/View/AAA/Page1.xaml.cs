using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise.Model;
using VeloNSK.APIServise.Servise;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using VeloNSK.View.AAA;
using VeloNSK.View.Admin.Participations;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        private ParticipationService participationService = new ParticipationService();
        private RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        private CategoriYarsServise categoriYarsServise = new CategoriYarsServise();
        private DistantionsServise distantionsServise = new DistantionsServise();
        private ConnectClass connectClass = new ConnectClass();
        private links picture_lincs = new links();
        private Animations animations = new Animations();

        public Page1()
        {
            InitializeComponent();
            SelectedwsProrerty();
        }

        private async Task SelectedwsProrerty()
        {
            IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
            IEnumerable<CategoriYars> categoriYars = await categoriYarsServise.Get();
            IEnumerable<Participation> participations = await participationService.Get();
            IEnumerable<Distantion> distantions = await distantionsServise.Get();

            //var info = from p in participations
            //           join d in distantions on p.IdDistans equals d.IdDistantion
            //           join c in categoriYars on p.IdCategoriYars equals c.IdCategori
            //           join i in infoUsers on p.IdUser equals i.IdUsers
            //           select new
            //           {
            //               p.Date,
            //               p.IdStatusVerification,
            //               d.NameDistantion,
            //               c.Ot,
            //               c.Do,
            //               i.Name,
            //               i.Patronimic,
            //               i.Login,
            //               p.IdDistans
            //           };

            //var res = info.ToList();
            //lstData.ItemsSource = res;
        }

        private async void lstData_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                string obj = e.SelectedItem.ToString();
                obj = obj.Substring(obj.LastIndexOf(',') + 1).Replace("IdDistans = ", string.Empty).Replace("}", string.Empty);
                await DisplayAlert("", obj.ToString(), "Ok");
            }
        }
    }
}