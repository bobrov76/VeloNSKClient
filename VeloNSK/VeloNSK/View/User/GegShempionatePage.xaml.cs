using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        private GetClientServise getClientServise = new GetClientServise();
        private RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        private DistantionsServise distantionsServise = new DistantionsServise();
        private LoginUsersService loginUsersService = new LoginUsersService();
        private links picture_lincs = new links();
        private Animations animations = new Animations();
        private ConnectClass connectClass = new ConnectClass();
        private Picker CompetentionsPicer;
        private Picker SelectDate;
        private int IDUser;
        private int IDCompitention;

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
            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());
            Back_Button.Clicked += async (s, e) =>
            {
                animations.Animations_Button(Back_Button);
                await Task.Delay(300);
                await Navigation.PopModalAsync();//Переход назад
            };

            Registrations_Button.Clicked += async (s, e) =>
            {
                await Criate();
            };
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage());
        }

        private async Task GetUser()
        {
            InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
            UserInfo_Lable.Text = "Участник: " + loginUsers.Fam.Replace(" ", string.Empty) + " " + loginUsers.Name + " " + loginUsers.Patronimic;
            IDUser = loginUsers.IdUsers;
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

        private async Task<bool> Date_TestAsync()
        {
            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            IEnumerable<Competentions> competentions = await competentionsServise.Get();
            var info = from k in competentions
                       join d in distantions on k.IdDistantion equals d.IdDistantion

                       select new
                       {
                           d.NameDistantion,
                           k.Date,
                           k.IdCompetentions
                       };
            DateTime selectd_date = Convert.ToDateTime(SelectDate.Items[SelectDate.SelectedIndex]);
            var picker_list = info.FirstOrDefault(x => x.NameDistantion == CompetentionsPicer.Items[CompetentionsPicer.SelectedIndex] && x.Date == selectd_date);

            if (picker_list.Date <= DateTime.Today.AddDays(2))
            {
                await DisplayAlert("Ошибка", "На эту дату запись уже недоступна", "Ок");
                Registrations_Button.IsEnabled = false;
                return false;
            }
            else
            {
                Registrations_Button.IsEnabled = true;
                return true;
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
                    await Date_TestAsync();
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

        public async Task Criate()
        {
            InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
            IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
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
            DateTime selectd_date = Convert.ToDateTime(SelectDate.Items[SelectDate.SelectedIndex]);
            var picker_list = info.FirstOrDefault(x => x.IdUser == loginUsers.IdUsers &&
            x.NameDistantion == CompetentionsPicer.Items[CompetentionsPicer.SelectedIndex] && x.Date == selectd_date);

            if (picker_list == null)
            {
                if (await Date_TestAsync())
                {
                    if (IDUser != 0 && IDCompitention != 0)
                    {
                        Participation participation = new Participation
                        {
                            IdCompetentions = IDCompitention,
                            IdUser = IDUser,
                            IdStatusVerification = true
                        };

                        await participationService.Add(participation);
                        await DownloadBage();
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        if (!await DisplayAlert("Ошибка", "Вы заполнили не все поля", "Заполнить", "Выйти")) { await Navigation.PopModalAsync(); }
                    }
                }
            }
            else
            {
                if (!await DisplayAlert("Предупреждение", "Вы уже зарегистрированы на эту дисциплину", "Зарегистрироваться на другую", "Выйти")) await Navigation.PopModalAsync();
            }
        }

        public async Task DownloadBage()
        {
            InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
            IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
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
                           p.IdParticipation,
                       };
            DateTime selectd_date = Convert.ToDateTime(SelectDate.Items[SelectDate.SelectedIndex]);
            var picker_list = info.FirstOrDefault(x => x.IdUser == loginUsers.IdUsers &&
            x.NameDistantion == CompetentionsPicer.Items[CompetentionsPicer.SelectedIndex] && x.Date == selectd_date);
            if (picker_list != null)
            {
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(IDUser.ToString()), "\"Id\"");
                content.Add(new StringContent("criate_daidg"), "\"masage\"");
                content.Add(new StringContent(picker_list.IdParticipation.ToString()), "\"Id_patisipant\"");
                var httpClient = new HttpClient();
                var servere_adres = "http://90.189.158.10/api/Doxs";
                var httpResponseMasage = await httpClient.PostAsync(servere_adres, content);
                var url = await httpResponseMasage.Content.ReadAsStringAsync();
                await DisplayAlert("", "Вы успешно зарегистрированы", "Ok");
                if (await DisplayAlert("", "Сохранить бэйдж", "Да", "Не сейчас"))
                {
                    var response = await httpClient.GetStreamAsync(url);
                    await response.SaveToLocalFolderAsync("Бэйдж" + $"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}" + ".docx");
                    await DisplayAlert("", "Бэйдж сохранен", "Ok");
                }
            }
        }

        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();

        private new void SizeChanged(object sender, EventArgs e)
        {
            if (size_form.GetWidthSize() >= 600)
            {
                Login_ColumnDefinition_Ziro.Width = new GridLength(1, GridUnitType.Star);
                Login_ColumnDefinition_Two.Width = new GridLength(1, GridUnitType.Star);
            }
            if (size_form.GetWidthSize() <= 550)
            {
                Login_ColumnDefinition_Ziro.Width = 0;
                Login_ColumnDefinition_Two.Width = 0;
            }
        }
    }
}