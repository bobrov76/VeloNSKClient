using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise.Model;
using VeloNSK.APIServise.Servise;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK.View.User
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyResultPatisipantPage : ContentPage
    {
        private ParticipationService participationService = new ParticipationService();
        private CompetentionsServise competentionsServise = new CompetentionsServise();
        private LoginUsersService loginUsersService = new LoginUsersService();
        private RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        private ResultParticipationServise resultParticipationServise = new ResultParticipationServise();
        private DistantionsServise distantionsServise = new DistantionsServise();
        private ConnectClass connectClass = new ConnectClass();
        private links picture_lincs = new links();
        private Animations animations = new Animations();
        private bool animate;
        private DateTime ID_Time;

        public MyResultPatisipantPage()
        {
            InitializeComponent();
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());

            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            showEmployeeAsync();

            Back_Button.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Back_Button);
                //await Task.Delay(300);
                await Navigation.PopModalAsync();//Переход назад
            };

            PoiskNameDistans.TextChanged += async (s, e) =>
            {
                try
                {
                    await Poisk("PoiskNameDistans");
                }
                catch { }
            };

            ItogTime_Entry.TextChanged += async (s, e) =>
            {
                try
                {
                    if (ItogTime_Entry.Text.Length == 11)
                    {
                        string a = ItogTime_Entry.Text;
                        int hour = Convert.ToInt32(a.Remove(2, 9));
                        int min = Convert.ToInt32(a.Remove(0, 4).Remove(0, 5));
                        int sec = Convert.ToInt32(a.Remove(0, 6).Remove(2, 3));
                        int milisec = Convert.ToInt32(a.Remove(0, 9));
                        DateTime dateTime = DateTime.Now;
                        TimeSpan ts = new TimeSpan(1, hour, min, sec, milisec);
                        dateTime = dateTime.Date + ts;
                        ID_Time = dateTime;
                        await Poisk("PoiskDate");
                    }
                }
                catch { }
            };
        }

        private async Task Poisk(string filtr)
        {
            InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
            IEnumerable<ResultParticipant> resultParticipations = await resultParticipationServise.Get();
            IEnumerable<Participation> participations = await participationService.Get();
            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            IEnumerable<Competentions> competentions = await competentionsServise.Get();
            IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
            var info = from r in resultParticipations
                       join p in participations on r.IdParticipation equals p.IdParticipation
                       join c in competentions on p.IdCompetentions equals c.IdCompetentions
                       join d in distantions on c.IdDistantion equals d.IdDistantion
                       join i in infoUsers on p.IdUser equals i.IdUsers
                       select new
                       {
                           r.ResultTime,
                           r.Mesto,
                           d.NameDistantion,
                           c.Date,
                           i.IdUsers,
                           p.IdStatusVerification,
                           r.IdResultParticipation,
                       };
            info = info.Where(x => x.IdUsers == loginUsers.IdUsers);
            switch (filtr)
            {
                case "PoiskNameDistans": info = info.Where(p => p.NameDistantion == PoiskNameDistans.Text || p.NameDistantion.StartsWith(PoiskNameDistans.Text)); break;

                case "PoiskDate": info = info.Where(p => p.Date == ID_Time); break;
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

            InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
            IEnumerable<ResultParticipant> resultParticipations = await resultParticipationServise.Get();
            IEnumerable<Participation> participations = await participationService.Get();
            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            IEnumerable<Competentions> competentions = await competentionsServise.Get();
            IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
            var info = from r in resultParticipations
                       join p in participations on r.IdParticipation equals p.IdParticipation
                       join c in competentions on p.IdCompetentions equals c.IdCompetentions
                       join d in distantions on c.IdDistantion equals d.IdDistantion
                       join i in infoUsers on p.IdUser equals i.IdUsers
                       select new
                       {
                           r.ResultTime,
                           r.Mesto,
                           d.NameDistantion,
                           i.IdUsers,
                           r.IdResultParticipation
                       };
            info = info.Where(x => x.IdUsers == loginUsers.IdUsers);
            var res = info.ToList();
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
                obj = obj.Substring(obj.LastIndexOf(',') + 1).Replace("IdResultParticipation = ", string.Empty).Replace("}", string.Empty);

                string res = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Получить грамоту");
                switch (res)
                {
                    case "Получить грамоту":
                        await DownloadBage(Convert.ToInt32(obj));
                        break;
                }
                lstData.SelectedItem = null;
            }
        }

        public async Task DownloadBage(int id_pat)
        {
            InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
            if (id_pat != 0)
            {
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(loginUsers.IdUsers.ToString()), "\"Id\"");
                content.Add(new StringContent("criate_gramots"), "\"masage\"");
                content.Add(new StringContent(id_pat.ToString()), "\"Id_patisipant\"");
                var httpClient = new HttpClient();
                var servere_adres = "http://90.189.158.10/api/Doxs";
                var httpResponseMasage = await httpClient.PostAsync(servere_adres, content);
                var url = await httpResponseMasage.Content.ReadAsStringAsync();
                var response = await httpClient.GetStreamAsync(url);
                await response.SaveToLocalFolderAsync("Грамота" + $"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}" + ".docx");
                await DisplayAlert("", "Грамота сохранена", "Ok");
            }
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage(), animate);
        }

        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();

        private new void SizeChanged(object sender, EventArgs e)
        {
            double width = size_form.GetWidthSize();
            double height = size_form.GetHeightSize();
            if (width > height)
            {
                if (Device.Idiom == TargetIdiom.Phone)
                {
                    Main_RowDefinition_Ziro.Height = 0;
                    Main_RowDefinition_Three.Height = 0;
                    Head_Image.IsVisible = false;
                    Head_Lable.IsVisible = false;
                    Back_Button.IsVisible = false;
                    Hend_BoxView.IsVisible = false;
                }
            }
            else
            {
                if (Device.Idiom == TargetIdiom.Phone)
                {
                    Main_RowDefinition_Ziro.Height = 70;
                    Main_RowDefinition_Three.Height = 40;
                    Head_Image.IsVisible = true;
                    Head_Lable.IsVisible = true;
                    Back_Button.IsVisible = true;
                    Hend_BoxView.IsVisible = true;
                }
            }
        }
    }
}