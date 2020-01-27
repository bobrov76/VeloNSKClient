using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.FilePicker;
using Rg.Plugins.Popup.Services;
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

namespace VeloNSK.View.Admin.Participations
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ParticipationsPage : ContentPage
    {
        private ResultParticipationServise resultParticipationServise = new ResultParticipationServise();
        private RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        private ParticipationService participationService = new ParticipationService();
        private CompetentionsServise competentionsServise = new CompetentionsServise();
        private CategoriYarsServise categoriYarsServise = new CategoriYarsServise();
        private DistantionsServise distantionsServise = new DistantionsServise();
        private GetClientServise getClientServise = new GetClientServise();
        private ConnectClass connectClass = new ConnectClass();
        private Animations animations = new Animations();
        private links picture_lincs = new links();
        private DateTime SelectedDate;
        private bool alive = true;
        private bool animate;

        public ParticipationsPage()
        {
            InitializeComponent();
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());
            Device.StartTimer(TimeSpan.FromSeconds(10), OnTimerTickAsync);
            showEmployeeAsync(true);

            Back_Button.Clicked += async (s, e) =>
            {
                animations.Animations_Button(Back_Button);
                await Task.Delay(300);
                await Navigation.PopModalAsync();//Переход назад
            };

            btnAddRecord.Clicked += async (s, e) =>
            {
                string res = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Добавить данные", "Импортировать данные", "Экспортировать данные");
                switch (res)
                {
                    case "Добавить данные":
                        animations.Animations_Button(btnAddRecord);
                        await Task.Delay(300);
                        int nul = 0;
                        await Navigation.PushModalAsync(new AddPaticipationsPage(nul), animate);
                        break;

                    case "Импортировать данные":
                        await Import();
                        break;

                    case "Экспортировать данные":
                        string res_export = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Скачать шаблон", "Экспортировать");
                        switch (res_export)
                        {
                            case "Скачать шаблон":
                                await DownloadSimple();
                                break;

                            case "Экспортировать":
                                await Export();
                                break;
                        }
                        break;
                }
            };

            PoiskLogin.TextChanged += async (s, e) =>
            {
                await Poisk("PoiskLogin");
            };

            PoiskNameDistans.TextChanged += async (s, e) =>
            {
                await Poisk("PoiskNameDistans");
            };

            PoiskStatus.TextChanged += async (s, e) =>
            {
                await Poisk("PoiskStatus");
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
                    showEmployeeAsync(false);
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
                case "PoiskNameDistans": info = info.Where(p => p.NameDistantion == PoiskNameDistans.Text || p.NameDistantion.StartsWith(PoiskNameDistans.Text)); break;
                case "PoiskLogin": info = info.Where(p => p.Login == PoiskLogin.Text || p.Login.StartsWith(PoiskLogin.Text)); break;
                case "PoiskDate": info = info.Where(p => p.Date == SelectedDate); break;
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
                            IEnumerable<ResultParticipant> participations = await resultParticipationServise.Get();
                            var selectad = participations.FirstOrDefault(p => p.IdParticipation == Convert.ToInt32(obj));
                            Participation Del_Participation = await participationService.Delete(Convert.ToInt32(obj));
                            if (selectad != null)
                            {
                                ResultParticipant Del_ResultPartisipation = await resultParticipationServise.Delete(selectad.IdResultParticipation);
                            }
                            await showEmployeeAsync(false);
                            await DisplayAlert("Уведомление", "Соревнование удалено", "Ok");
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

        private async Task Export()
        {
            var file = await CrossFilePicker.Current.PickFile();
            if (file != null)
            {
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(file.GetStream()), "\"files\"", $"\"{$"ExportDistans{DateTime.Now.ToString("ddMMyyyyhhmmss")}.xlsx"}\"");
                var httpClient = new HttpClient();
                var servere_adres = "http://90.189.158.10/api/Participation/";
                var httpResponseMasage = await httpClient.PostAsync(servere_adres, content);
                await httpResponseMasage.Content.ReadAsStringAsync();
                await DisplayAlert("", "Экспорт успешно выполнен", "Ok");
            }
        }

        private async Task DownloadSimple()
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.GetStreamAsync("http://90.189.158.10/Simple/PartisipantExportSimple.xlsx");
            await response.SaveToLocalFolderAsync("Шаблон для экспорта информации о соревновании" + $"{DateTime.Now.ToString("ddMMyyyyhhmmss")}" + ".xlsx");
            await DisplayAlert("", "Шаблон успешно сохранен", "Ok");
        }

        private async Task Import()
        {
            try
            {
                Main_RowDefinition_One.Height = 0;
                Main_RowDefinition_Activity.Height = new GridLength(1, GridUnitType.Star);
                activityIndicator.IsRunning = true;
                HttpClient client = getClientServise.GetClient();
                string result = await client.GetStringAsync("http://90.189.158.10/api/Participation/ExportParticipation");
                string path = JsonConvert.DeserializeObject<string>(result);
                path = path.Replace(" ", string.Empty);

                var response = await client.GetStreamAsync(path);
                var filePath = await response.SaveToLocalFolderAsync($"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.xlsx");

                await Task.Delay(3000);
                Main_RowDefinition_One.Height = new GridLength(1, GridUnitType.Star);
                Main_RowDefinition_Activity.Height = 0;
                activityIndicator.IsRunning = false;
                await DisplayAlert("", "Импорт успешно выполнен", "Ok");
            }
            catch { }
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