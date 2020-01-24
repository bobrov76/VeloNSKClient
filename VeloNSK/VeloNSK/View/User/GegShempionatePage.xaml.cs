using Plugin.Connectivity;
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
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK.View.User
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GegShempionatePage : ContentPage
    {
        private ParticipationService participationService = new ParticipationService();
        private CompetentionsServise competentionsServise = new CompetentionsServise();
        private RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        private CategoriYarsServise categoriYarsServise = new CategoriYarsServise();
        private DistantionsServise distantionsServise = new DistantionsServise();
        private LoginUsersService loginUsersService = new LoginUsersService();
        private links picture_lincs = new links();
        private Lincs server_lincs = new Lincs();
        private Animations animations = new Animations();
        private ConnectClass connectClass = new ConnectClass();
        private Picker LoginPicer;
        private Picker CompetentionsPicer;
        private Picker SelectDate;
        private string Date;
        private int IDUser;
        private int IDCompitention;
        private DateTime dateTimeTest;

        public GegShempionatePage()
        {
            InitializeComponent();
            GetUser();
            Criate_CompetentionsPicer();
            // Criate_CompetentionsPicer(false);
            Criate_SelectDate(false);
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            Users_Fon_Images.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "UserFon.png");

            Back_Button.Clicked += async (s, e) =>
            {
                animations.Animations_Button(Back_Button);
                await Task.Delay(300);
                await Navigation.PopModalAsync();//Переход назад
            };

            Registrations_Button.Clicked += async (s, e) =>
            {
                //if (id != 0) { await Update(id); }
                //else { await Criate(); }
            };
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage());
        }

        private async Task GetUser()
        {
            InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
            UserInfo_Lable.Text = loginUsers.Fam.Replace(" ", string.Empty) + " " + loginUsers.Name + " " + loginUsers.Patronimic;
        }

        private async Task Criate_CompetentionsPicer()
        {
            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            IEnumerable<Competentions> competentions = await competentionsServise.Get();
            var info = from k in competentions
                       join d in distantions on k.IdDistantion equals d.IdDistantion
                       select new
                       {
                           k.IdCompetentions,
                           d.NameDistantion,
                           k.Date,
                           d.Discriptions,
                       };
            var groups = from p in info
                         group p by p.NameDistantion into g
                         select new
                         {
                             g.Key,
                             Count = g.Count()
                         };
            // InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
            //  info = info.Where(p => p.IdUsers == loginUsers.IdUsers);
            CompetentionsPicer = new Picker { Margin = new Thickness(10, -10, 10, 5) };
            foreach (var item in groups)
            {
                CompetentionsPicer.Items.Add(item.Key);
            }

            CompetentionsPicer.SelectedIndexChanged += async (s, e) =>
            {
                var picker_list = info.FirstOrDefault(x => x.NameDistantion == CompetentionsPicer.Items[CompetentionsPicer.SelectedIndex].ToString());
                await Criate_SelectDate(true);
                Info_Editor.Text = "";
                Info_Editor.Text = "Название: " + picker_list.NameDistantion + "\r\n";
                Info_Editor.Text += "Описание: " + picker_list.Discriptions + "\r\n";
            };
            GridMain.Children.Add(CompetentionsPicer, 1, 2);
        }

        private async Task Date_TestAsync()
        {
            IEnumerable<Participation> participations = await participationService.Get();
            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            IEnumerable<Competentions> competentions = await competentionsServise.Get();
            var info = from p in participations
                       join k in competentions on p.IdCompetentions equals k.IdCompetentions
                       join d in distantions on k.IdDistantion equals d.IdDistantion
                       select new
                       {
                           d.NameDistantion,
                           k.Date,
                       };

            var picker_list = info.FirstOrDefault(x => x.NameDistantion == CompetentionsPicer.Items[CompetentionsPicer.SelectedIndex].ToString());
            if (picker_list.Date >= DateTime.Today.AddDays(1))
            {
                await DisplayAlert("Ошибка", "На эту дату запись уже недоступна", "Ок");
            }
        }

        private async Task Criate_SelectDate(bool status)
        {
            SelectDate = new Picker { Margin = new Thickness(10, -10, 10, 5) };

            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            IEnumerable<Competentions> competentions = await competentionsServise.Get();

            if (status)
            {
                SelectDate.IsEnabled = true;
                var info = from k in competentions
                           join d in distantions on k.IdDistantion equals d.IdDistantion

                           select new
                           {
                               d.NameDistantion,
                               k.Date,
                               k.IdCompetentions
                           };
                info = info.Where(x => x.NameDistantion == CompetentionsPicer.Items[CompetentionsPicer.SelectedIndex]);
                foreach (var item in info)
                {
                    SelectDate.Items.Add(item.Date.ToString("f"));
                }
                SelectDate.SelectedIndexChanged += async (s, e) =>
                {
                    var picker_list = info.Where(
                        x => x.NameDistantion == CompetentionsPicer.Items[CompetentionsPicer.SelectedIndex] &&
                        x.Date == Convert.ToDateTime(SelectDate.Items[SelectDate.SelectedIndex]));
                    foreach (var item in picker_list)
                    {
                        IDCompitention = Convert.ToInt32(item.IdCompetentions);
                    }
                };
            }
            else
            {
                SelectDate.IsEnabled = false;
                SelectDate.Items.Add(DateTime.Today.ToString("f"));
            }
            GridMain.Children.Add(SelectDate, 1, 3);
        }

        //public async Task Criate()
        //{
        //    if (IDUser != 0 && IDCompitention != 0 && Autentification_CheckBox.IsChecked)
        //    {
        //        bool verifi = false;
        //        if (Autentification_CheckBox.IsChecked)
        //        {
        //            verifi = true;
        //        }
        //        else { verifi = false; }

        //        Participation participation = new Participation
        //        {
        //            IdCompetentions = IDCompitention,
        //            IdUser = IDUser,
        //            IdStatusVerification = verifi
        //        };

        //        await participationService.Add(participation);

        //        if (!await DisplayAlert("", "Добавить еще одну запись", "Да", "Нет")) { await Navigation.PopModalAsync(); }
        //    }
        //    else
        //    {
        //        if (!await DisplayAlert("Ошибка", "Вы заполнили не все поля", "Заполнить", "Выйти")) { await Navigation.PopModalAsync(); }
        //    }
        //}
    }
}