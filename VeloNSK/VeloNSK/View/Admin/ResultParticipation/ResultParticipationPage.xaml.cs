using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.FilePicker;
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

namespace VeloNSK.View.Admin.ResultParticipation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultParticipationPage : ContentPage
    {
        private ParticipationService participationService = new ParticipationService();
        private CompetentionsServise competentionsServise = new CompetentionsServise();
        private RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        private ResultParticipationServise resultParticipationServise = new ResultParticipationServise();
        private DistantionsServise distantionsServise = new DistantionsServise();
        private GetClientServise getClientServise = new GetClientServise();
        private ConnectClass connectClass = new ConnectClass();
        private links picture_lincs = new links();
        private Animations animations = new Animations();
        private bool animate;
        private bool alive = true;
        private DateTime SelectedDate;
        private DateTime ID_Time;

        public ResultParticipationPage()
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
                await Task.Delay(300);
                await Navigation.PopModalAsync();//Переход назад
            };

            btnAddRecord.Clicked += async (s, e) =>
            {
                string res = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Добавить", "Импортировать", "Экспортировать", "Сформировать отчет", "Статистика");
                switch (res)
                {
                    case "Добавить":
                        animations.Animations_Button(btnAddRecord);
                        await Task.Delay(300);
                        int nul = 0;
                        await Navigation.PushModalAsync(new AddResultParticipationPagePage(nul), animate);
                        break;

                    case "Импортировать":
                        await Import();
                        break;

                    case "Экспортировать":
                        string res_select = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Скачать шаблон", "Экспортировать");
                        switch (res_select)
                        {
                            case "Скачать шаблон":
                                await DownloadSimple();
                                break;

                            case "Экспортировать":
                                await Export();
                                break;
                        }
                        break;

                    case "Сформировать результирующий отчет":
                        await Otchet(1);
                        break;

                    case "Статистика":
                        await Navigation.PushModalAsync(new StatisticsPage());
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
                await Poisk("PoiskMesto");
            };

            ItogTime_Entry.TextChanged += async (s, e) =>
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
            };
        }

        private async Task Get_Time()
        {
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
                           i.Login,
                           r.IdResultParticipation
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
                           i.Login,
                           c.Date,
                           p.IdStatusVerification,
                           r.IdResultParticipation,
                       };

            switch (filtr)
            {
                case "PoiskLogin": info = info.Where(p => p.Login == PoiskLogin.Text || p.Login.StartsWith(PoiskLogin.Text)); break;
                case "PoiskNameDistans": info = info.Where(p => p.NameDistantion == PoiskNameDistans.Text || p.NameDistantion.StartsWith(PoiskNameDistans.Text)); break;
                case "PoiskMesto":
                    string str = PoiskStatus.Text;
                    int num;
                    bool isNum = int.TryParse(str, out num);
                    if (isNum)
                        info = info.Where(p => p.Mesto == Convert.ToInt32(str));
                    else
                        PoiskStatus.Text = "";
                    break;

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
                           i.Login,
                           r.IdResultParticipation
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
                obj = obj.Substring(obj.LastIndexOf(',') + 1).Replace("IdResultParticipation = ", string.Empty).Replace("}", string.Empty);

                string res = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Обновить данные", "Удалить данные");
                switch (res)
                {
                    // case "Подробнее":
                    // await PopupNavigation.Instance.PushAsync(new MoreInfoParticipationsPage());
                    //    break;

                    case "Обновить данные":
                        await Navigation.PushModalAsync(new AddResultParticipationPagePage(Convert.ToInt32(obj)), animate);
                        break;

                    case "Удалить данные":
                        bool result = await DisplayAlert("Подтвердить действие", "Вы хотите удалить элемент?", "Да", "Нет");
                        if (result == true)
                        {
                            ResultParticipant Del = await resultParticipationServise.Delete(Convert.ToInt32(obj));
                            await showEmployeeAsync();
                            await DisplayAlert("Уведомление", "Итог соревнования удален", "Ok");
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
                var servere_adres = "http://90.189.158.10/api/Resultpartisipations/";
                var httpResponseMasage = await httpClient.PostAsync(servere_adres, content);
                await httpResponseMasage.Content.ReadAsStringAsync();
                await DisplayAlert("", "Экспорт успешно выполнен", "Ok");
            }
        }

        private async Task DownloadSimple()
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.GetStreamAsync("http://90.189.158.10/Simple/ResultPatisipantSimple.xlsx");
            await response.SaveToLocalFolderAsync("Шаблон для экспорта информации о итогах соревнования" + $"{DateTime.Now.ToString("ddMMyyyyhhmmss")}" + ".xlsx");
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
                string result = await client.GetStringAsync("http://90.189.158.10/api/Resultpartisipations/ExportParticipation");
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

        private async Task Otchet(int id_pat)
        {
            if (id_pat != 0)
            {
                var content = new MultipartFormDataContent();
                content.Add(new StringContent("0"), "\"Id\"");
                content.Add(new StringContent("criate_itog"), "\"masage\"");
                content.Add(new StringContent(id_pat.ToString()), "\"Id_patisipant\"");
                var httpClient = new HttpClient();
                var servere_adres = "http://90.189.158.10/api/Doxs";
                var httpResponseMasage = await httpClient.PostAsync(servere_adres, content);
                var url = await httpResponseMasage.Content.ReadAsStringAsync();
                var response = await httpClient.GetStreamAsync(url);
                await response.SaveToLocalFolderAsync("Итог соревнования" + $"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}" + ".docx");
                await DisplayAlert("", "Инофрмация сохранена", "Ok");
            }
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