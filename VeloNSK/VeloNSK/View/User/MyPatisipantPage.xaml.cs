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
    public partial class MyPatisipantPage : ContentPage
    {
        private ResultParticipationServise resultParticipationServise = new ResultParticipationServise();
        private RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        private ParticipationService participationService = new ParticipationService();
        private CompetentionsServise competentionsServise = new CompetentionsServise();
        private CategoriYarsServise categoriYarsServise = new CategoriYarsServise();
        private DistantionsServise distantionsServise = new DistantionsServise();
        private LoginUsersService loginUsersService = new LoginUsersService();
        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();
        private GetClientServise getClientServise = new GetClientServise();
        private ConnectClass connectClass = new ConnectClass();
        private Animations animations = new Animations();
        private links picture_lincs = new links();
        private DateTime SelectedDate;
        private bool alive = true;
        private bool animate;

        public MyPatisipantPage()
        {
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            InitializeComponent();
            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());
            showEmployeeAsync(true);

            Back_Button.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Back_Button);
                //await Task.Delay(300);
                await Navigation.PopModalAsync();//Переход назад
            };

            PoiskNameDistans.TextChanged += async (s, e) =>
            {
                await Poisk("PoiskNameDistans");
            };

            PoiskDate.TextChanged += async (s, e) =>
            {
                if (PoiskDate.Text.Length == 10)
                {
                    string a = PoiskDate.Text;
                    int day = Convert.ToInt32(a.Remove(2, 8));
                    int mouns = Convert.ToInt32(a.Remove(0, 3).Remove(2, 5));
                    int yars = Convert.ToInt32(a.Remove(0, 6));
                    SelectedDate = new DateTime(yars, mouns, day);
                    await Poisk("PoiskDate");
                }
                else
                {
                    await showEmployeeAsync(false);
                }
            };
        }

        private async Task Poisk(string filtr)
        {
            InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
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
                           i.IdUsers,
                           k.Date,
                           p.IdStatusVerification,
                           d.NameDistantion,
                           p.IdParticipation
                       };
            info = info.Where(x => x.IdUsers == loginUsers.IdUsers);
            switch (filtr)
            {
                case "PoiskNameDistans": info = info.Where(p => p.NameDistantion == PoiskNameDistans.Text || p.NameDistantion.StartsWith(PoiskNameDistans.Text)); break;
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

        private async Task showEmployeeAsync(bool time)
        {
            if (time)
            {
                Main_RowDefinition_One.Height = 0;
                Main_RowDefinition_Activity.Height = new GridLength(1, GridUnitType.Star);
                activityIndicator.IsRunning = true;
            }
            InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
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
                           i.IdUsers,
                           k.Date,
                           p.IdStatusVerification,
                           d.NameDistantion,
                           p.IdParticipation
                       };
            info = info.Where(x => x.IdUsers == loginUsers.IdUsers);
            var res = info.ToList();
            if (res.Count != 0)
            {
                lstData.ItemsSource = res;
                if (time)
                {
                    YesRecords.Height = new GridLength(1, GridUnitType.Star);
                    NoRecords.Height = 0;
                    await Task.Delay(3000);
                    Main_RowDefinition_One.Height = new GridLength(1, GridUnitType.Star);
                    Main_RowDefinition_Activity.Height = 0;
                    activityIndicator.IsRunning = false;
                }
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
                string res = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Отказаться от участия в соревнование", "Получить бэйдж");
                switch (res)
                {
                    case "Отказаться от участия в соревнование":
                        InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
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
                                       i.IdUsers,
                                       k.Date,
                                       p.IdStatusVerification,
                                       d.NameDistantion,
                                       p.IdParticipation
                                   };
                        info = info.Where(x => x.IdUsers == loginUsers.IdUsers);
                        var picker_list = info.FirstOrDefault(x => x.IdParticipation == Convert.ToInt32(obj));

                        if (picker_list.Date <= DateTime.Today.AddDays(2))
                        {
                            await DisplayAlert("Уведомление", "К сожалению отказаться от участия уже нельзя", "Ok");
                        }
                        else
                        {
                            await participationService.Delete(Convert.ToInt32(obj));
                            await showEmployeeAsync(false);
                            await DisplayAlert("Уведомление", "Вы успешно отказались от участия", "Ok");
                        }

                        break;

                    case "Получить бэйдж":
                        await DownloadBage(Convert.ToInt32(obj));

                        break;
                }
                lstData.SelectedItem = null;
            }
        }

        public async Task DownloadBage(int id_pat)
        {
            InfoUser loginUsers = await loginUsersService.Get(App.Current.Properties["token"].ToString());
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
                           i.IdUsers,
                           k.Date,
                           p.IdStatusVerification,
                           d.NameDistantion,
                           p.IdParticipation
                       };
            info = info.Where(x => x.IdUsers == loginUsers.IdUsers);
            var picker_list = info.FirstOrDefault(x => x.IdParticipation == id_pat);
            if (picker_list != null)
            {
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(picker_list.IdUsers.ToString()), "\"Id\"");
                content.Add(new StringContent("criate_daidg"), "\"masage\"");
                content.Add(new StringContent(picker_list.IdParticipation.ToString()), "\"Id_patisipant\"");
                var httpClient = new HttpClient();
                var servere_adres = "http://90.189.158.10/api/Doxs";
                var httpResponseMasage = await httpClient.PostAsync(servere_adres, content);
                var url = await httpResponseMasage.Content.ReadAsStringAsync();
                var response = await httpClient.GetStreamAsync(url);
                await response.SaveToLocalFolderAsync("Бэйдж" + $"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}" + ".docx");
                await DisplayAlert("", "Бэйдж сохранен", "Ok");
            }
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage(), animate);
        }

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