﻿using Plugin.Connectivity;
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
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK.View.Admin.Participations
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ParticipationsPage : ContentPage
    {
        private ParticipationService participationService = new ParticipationService();
        private CompetentionsServise competentionsServise = new CompetentionsServise();
        private RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        private CategoriYarsServise categoriYarsServise = new CategoriYarsServise();
        private DistantionsServise distantionsServise = new DistantionsServise();
        private ConnectClass connectClass = new ConnectClass();
        private links picture_lincs = new links();
        private Animations animations = new Animations();
        private bool animate;
        private bool alive = true;
        private DateTime SelectedDate;

        public ParticipationsPage()
        {
            InitializeComponent();
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());

            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            showEmployeeAsync();
            Device.StartTimer(TimeSpan.FromSeconds(10), OnTimerTickAsync);

            Back_Button.Clicked += async (s, e) =>
            {
                animations.Animations_Button(Back_Button);
                await Task.Delay(1000);
                await Navigation.PopModalAsync();//Переход назад
            };

            btnAddRecord.Clicked += async (s, e) =>
            {
                animations.Animations_Button(btnAddRecord);
                await Task.Delay(1000);
                int nul = 0;
                await Navigation.PushModalAsync(new AddPaticipationsPage(nul), animate);
            };

            PoiskLogin.TextChanged += async (s, e) =>
            {
                if (PoiskLogin.Text != null)
                {
                    await Poisk("PoiskLogin");
                }
            };

            PoiskNameDistans.TextChanged += async (s, e) =>
            {
                if (PoiskNameDistans.Text != null)
                {
                    await Poisk("PoiskNameDistans");
                }
            };

            PoiskStatus.TextChanged += async (s, e) =>
            {
                if (PoiskStatus.Text != null && PoiskStatus.Text != "")
                {
                    await Poisk("PoiskStatus");
                }
            };

            PoiskDate.MinimumDate = DateTime.Today;

            PoiskDate.DateSelected += async (s, e) =>
            {
                if (e.NewDate != null)
                {
                    SelectedDate = e.NewDate;
                    await Poisk("PoiskDate");
                }
            };
        }

        private async Task Get_Time()
        {
            IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
            IEnumerable<CategoriYars> categoriYars = await categoriYarsServise.Get();
            IEnumerable<Participation> participations = await participationService.Get();
            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            IEnumerable<Competentions> competentions = await competentionsServise.Get();
            var info = from p in participations
                       join k in competentions on p.IdCompetentions equals k.IdCompetentions
                       join d in distantions on k.IdDistantion equals d.IdDistantion
                       join i in infoUsers on p.IdUser equals i.IdUsers
                       select new
                       {
                           p.IdUser,
                           i.Login,
                           k.Date,
                           p.IdStatusVerification,
                           d.NameDistantion,
                       };
            var res = info.ToList();
            if (counts < res.Count || counts > res.Count)
            {
                lstData.ItemsSource = res;
                counts = res.Count;
            }
        }

        private int counts = 0;

        private bool OnTimerTickAsync()
        {
            Get_Time();

            return alive;
        }

        private async Task Poisk(string filtr)
        {
            IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
            IEnumerable<CategoriYars> categoriYars = await categoriYarsServise.Get();
            IEnumerable<Participation> participations = await participationService.Get();
            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            IEnumerable<Competentions> competentions = await competentionsServise.Get();
            var info = from p in participations
                       join k in competentions on p.IdCompetentions equals k.IdCompetentions
                       join d in distantions on k.IdDistantion equals d.IdDistantion
                       join i in infoUsers on p.IdUser equals i.IdUsers
                       select new
                       {
                           p.IdUser,
                           i.Login,
                           k.Date,
                           p.IdStatusVerification,
                           d.NameDistantion,
                       };

            switch (filtr)
            {
                case "PoiskLogin": info = info.Where(p => p.Login == PoiskLogin.Text || p.Login.StartsWith(PoiskLogin.Text)); break;
                case "PoiskNameDistans": info = info.Where(p => p.NameDistantion == PoiskNameDistans.Text || p.NameDistantion.StartsWith(PoiskNameDistans.Text)); break;
                case "PoiskStatus":
                    if (PoiskStatus.Text.Substring(0, 1).ToString() == "T" || PoiskStatus.Text.Substring(0, 1).ToString() == "t")
                    {
                        info = info.Where(p => p.IdStatusVerification == true);
                    }
                    else if (PoiskStatus.Text.Substring(0, 1).ToString() == "F" || PoiskStatus.Text.Substring(0, 1).ToString() == "f")
                    {
                        info = info.Where(p => p.IdStatusVerification == false);
                    }

                    break;

                case "PoiskDate": info = info.Where(p => p.Date == SelectedDate); break;
            }
            var res = info.ToList();
            if (res.Count != 0)
            {
                lstData.ItemsSource = res;
                YesRecords.Height = new GridLength(1, GridUnitType.Star);
                NoRecords.Height = 0;
            }
            else
            {
                lstData.ItemsSource = res;
                YesRecords.Height = 0;
                NoRecords.Height = new GridLength(1, GridUnitType.Star);
            }
        }

        private async Task showEmployeeAsync()
        {
            Main_RowDefinition_One.Height = 0;
            Main_RowDefinition_Activity.Height = new GridLength(1, GridUnitType.Star);
            activityIndicator.IsRunning = true;
            IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
            IEnumerable<CategoriYars> categoriYars = await categoriYarsServise.Get();
            IEnumerable<Participation> participations = await participationService.Get();
            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            IEnumerable<Competentions> competentions = await competentionsServise.Get();
            var info = from p in participations
                       join k in competentions on p.IdCompetentions equals k.IdCompetentions
                       join d in distantions on k.IdDistantion equals d.IdDistantion
                       join i in infoUsers on p.IdUser equals i.IdUsers
                       select new
                       {
                           i.Login,
                           k.Date,
                           p.IdStatusVerification,
                           d.NameDistantion,
                           p.IdParticipation
                       };
            var res = info.ToList();
            counts = res.Count;
            if (res.Count != 0)
            {
                lstData.ItemsSource = res;
                YesRecords.Height = new GridLength(1, GridUnitType.Star);
                NoRecords.Height = 0;
                await Task.Delay(3000);
                Main_RowDefinition_One.Height = new GridLength(1, GridUnitType.Star);
                Main_RowDefinition_Activity.Height = 0;
                activityIndicator.IsRunning = false;
            }
            else
            {
                lstData.ItemsSource = res;
                YesRecords.Height = 0;
                NoRecords.Height = new GridLength(1, GridUnitType.Star);
            }
        }

        private async void lstData_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                string obj = e.SelectedItem.ToString();
                obj = obj.Substring(obj.LastIndexOf(',') + 1).Replace("IdParticipation = ", string.Empty).Replace("}", string.Empty);
                DisplayAlert("", obj.ToString(), "Ok");
                string res = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Подробнее", "Обновить данные", "Удалить данные");
                switch (res)
                {
                    case "Подробнее":
                        await PopupNavigation.Instance.PushAsync(new MoreInfoParticipationsPage());
                        break;

                    case "Обновить данные":
                        await Navigation.PushModalAsync(new AddPaticipationsPage(Convert.ToInt32(obj)), animate);
                        break;

                    case "Удалить данные":
                        bool result = await DisplayAlert("Подтвердить действие", "Вы хотите удалить элемент?", "Да", "Нет");
                        if (result == true)
                        {
                            Distantion Del_Distantion = await distantionsServise.Delete(Convert.ToInt32(obj));
                            await showEmployeeAsync();
                        }
                        break;
                }
                lstData.SelectedItem = null;
            }
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage(), animate);
        }
    }
}