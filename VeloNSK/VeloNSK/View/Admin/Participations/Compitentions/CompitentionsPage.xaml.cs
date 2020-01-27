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

namespace VeloNSK.View.Admin.Participations.Compitentions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompitentionsPage : ContentPage
    {
        private ResultParticipationServise resultParticipationServise = new ResultParticipationServise();
        private ParticipationService participationService = new ParticipationService();
        private CompetentionsServise competentionsServise = new CompetentionsServise();
        private DistantionsServise distantionsServise = new DistantionsServise();
        private GetClientServise getClientServise = new GetClientServise();
        private ConnectClass connectClass = new ConnectClass();
        private Animations animations = new Animations();
        private links picture_lincs = new links();
        private DateTime SelectedDate;
        private bool alive = true;
        private bool animate;

        public CompitentionsPage()
        {
            InitializeComponent();
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            showEmployeeAsync(true);

            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());
            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            Device.StartTimer(TimeSpan.FromSeconds(10), OnTimerTickAsync);

            Back_Button.Clicked += async (s, e) =>
            {
                animations.Animations_Button(Back_Button);
                await Task.Delay(1000);
                await Navigation.PopModalAsync();//Переход назад
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

            btnAddRecord.Clicked += async (s, e) =>
            {
                string res = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Добавить данные", "Импортировать данные", "Экспортировать данные");
                switch (res)
                {
                    case "Добавить данные":
                        animations.Animations_Button(btnAddRecord);
                        await Task.Delay(300);
                        int nul = 0;
                        await Navigation.PushModalAsync(new AddCompitentionsPage(nul), animate);
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

            PoiskName.TextChanged += async (s, e) =>
            {
                try
                {
                    await Poisk("PoiskName");
                }
                catch { }
            };
        }

        private async Task Get_Time()
        {
            IEnumerable<Competentions> competentions = await competentionsServise.Get();
            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            var info = from d in distantions
                       join i in competentions on d.IdDistantion equals i.IdDistantion
                       select new
                       {
                           d.NameDistantion,
                           i.Date,
                           i.IdCompetentions
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
            IEnumerable<Competentions> competentions = await competentionsServise.Get();
            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            var info = from d in distantions
                       join i in competentions on d.IdDistantion equals i.IdDistantion
                       select new
                       {
                           d.NameDistantion,
                           i.Date,
                           i.IdCompetentions
                       };

            switch (filtr)
            {
                case "PoiskName": info = info.Where(p => p.NameDistantion == PoiskName.Text || p.NameDistantion.StartsWith(PoiskName.Text)); break;
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
            IEnumerable<Competentions> competentions = await competentionsServise.Get();
            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            var info = from d in distantions
                       join i in competentions on d.IdDistantion equals i.IdDistantion
                       select new
                       {
                           d.NameDistantion,
                           i.Date,
                           i.IdCompetentions
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
                obj = obj.Substring(obj.LastIndexOf(',') + 1).Replace("IdCompetentions = ", string.Empty).Replace("}", string.Empty);

                string res = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Обновить данные", "Удалить данные");
                switch (res)
                {
                    case "Обновить данные":
                        await Navigation.PushModalAsync(new AddCompitentionsPage(Convert.ToInt32(obj)), animate);
                        break;

                    case "Удалить данные":
                        bool result = await DisplayAlert("Подтвердить действие", "Вы хотите удалить элемент?", "Да", "Нет");
                        if (result == true)
                        {
                            IEnumerable<Participation> participations = await participationService.Get();
                            Competentions Del_compitentions = await competentionsServise.Delete(Convert.ToInt32(obj));
                            var selectad = participations.FirstOrDefault(p => p.IdCompetentions == Convert.ToInt32(obj));
                            if (selectad != null)
                            {
                                int id_part = selectad.IdParticipation;
                                IEnumerable<ResultParticipant> res_participations = await resultParticipationServise.Get();
                                Participation Del_Participation = await participationService.Delete(id_part);
                                var res_selectad = res_participations.FirstOrDefault(p => p.IdParticipation == id_part);
                                if (res_selectad != null)
                                {
                                    int id_res_part = res_selectad.IdResultParticipation;
                                    ResultParticipant Del_ResultPartisipation = await resultParticipationServise.Delete(id_res_part);
                                }
                            }
                            await showEmployeeAsync(false);
                            await DisplayAlert("Уведомление", "Компетенция успешно удалена", "Ok");
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
                content.Add(new StreamContent(file.GetStream()), "\"files\"", $"\"{$"ExportCompitention{DateTime.Now.ToString("ddMMyyyyhhmmss")}.xlsx"}\"");
                var httpClient = new HttpClient();
                var servere_adres = "http://90.189.158.10/api/Competentious/";
                var httpResponseMasage = await httpClient.PostAsync(servere_adres, content);
                await httpResponseMasage.Content.ReadAsStringAsync();
                await DisplayAlert("", "Экспорт успешно выполнен", "Ok");
            }
        }

        private async Task DownloadSimple()//
        {
            HttpClient client = getClientServise.GetClient();
            string result = await client.GetStringAsync("http://90.189.158.10/api/Competentious/ExportSimple");
            var itog = JsonConvert.DeserializeObject<string>(result);

            var response = await client.GetStreamAsync(itog);
            await response.SaveToLocalFolderAsync("Шаблон для компетенции" + $"{DateTime.Now.ToString("ddMMyyyyhhmmss")}" + ".xlsx");
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
                string result = await client.GetStringAsync("http://90.189.158.10/api/Competentious/ExportCompetentious");
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