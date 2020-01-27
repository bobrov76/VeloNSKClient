using Plugin.Connectivity;
using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace VeloNSK.View.Admin.Participations.Distanse
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DistantionPage : ContentPage
    {
        private CompetentionsServise competentionsServise = new CompetentionsServise();
        private DistantionsServise distantionsServise = new DistantionsServise();
        private ParticipationService participationService = new ParticipationService();
        private ResultParticipationServise resultParticipationServise = new ResultParticipationServise();
        private GetClientServise getClientServise = new GetClientServise();
        private ConnectClass connectClass = new ConnectClass();
        private links picture_lincs = new links();
        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();
        private Animations animations = new Animations();
        private HttpClient _client;
        private bool animate;
        private bool alive = true;

        /// <summary>
        ///
        /// </summary>
        public DistantionPage()
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
                string res = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Добавить данные", "Импортировать данные", "Экспортировать данные");
                switch (res)
                {
                    case "Добавить данные":

                        int nul = 0;
                        await Navigation.PushModalAsync(new AddDistantionsPage(nul), animate);
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
            PoiskDiscript.TextChanged += async (s, e) =>
            {
                try
                {
                    await Poisk("PoiskDiscript");
                }
                catch { }
            };
            PoiskLengs.TextChanged += async (s, e) =>
            {
                try
                {
                    await Poisk("PoiskLengs");
                }
                catch { }
            };
        }

        private async Task Get_Time()
        {
            IEnumerable<Distantion> infoUsers = await distantionsServise.Get();
            var res = infoUsers.ToList();
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
            try
            {
                IEnumerable<Distantion> infoUsers = await distantionsServise.Get();
                switch (filtr)
                {
                    case "PoiskName": infoUsers = infoUsers.Where(p => p.NameDistantion == PoiskName.Text || p.NameDistantion.StartsWith(PoiskName.Text)); break;
                    case "PoiskDiscript": infoUsers = infoUsers.Where(p => p.Discriptions == PoiskDiscript.Text || p.Discriptions.StartsWith(PoiskDiscript.Text)); break;
                    case "PoiskLengs":

                        int num;
                        if (int.TryParse(PoiskLengs.Text, out num))
                        {
                            infoUsers = infoUsers.Where(p => p.Lengs == Convert.ToInt16(PoiskLengs.Text));
                        }
                        break;
                }
                if (infoUsers.ToList() != null)
                {
                    var res = infoUsers.ToList();
                    lstData.ItemsSource = res;
                    YesRecords.Height = new GridLength(1, GridUnitType.Star);
                    NoRecords.Height = 0;
                }
                else
                {
                    // lstData.ItemsSource = res;
                    YesRecords.Height = 0;
                    NoRecords.Height = new GridLength(1, GridUnitType.Star);
                }
            }
            catch { }
        }

        private async Task showEmployeeAsync()
        {
            Main_RowDefinition_One.Height = 0;
            Main_RowDefinition_Activity.Height = new GridLength(1, GridUnitType.Star);
            activityIndicator.IsRunning = true;
            IEnumerable<Distantion> infoUsers = await distantionsServise.Get();
            var res = infoUsers.ToList();
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
                Distantion obj = (Distantion)e.SelectedItem;
                string res = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Обновить данные", "Удалить данные");
                switch (res)
                {
                    case "Обновить данные":
                        await Navigation.PushModalAsync(new AddDistantionsPage(obj.IdDistantion), animate);
                        break;

                    case "Удалить данные":
                        bool result = await DisplayAlert("Подтвердить действие", "Вы хотите удалить элемент?", "Да", "Нет");
                        if (result == true)
                        {
                            Distantion Del_Distantion = await distantionsServise.Delete(obj.IdDistantion);
                            IEnumerable<Competentions> competentions = await competentionsServise.Get();
                            var selectad_del_compitentions = competentions.FirstOrDefault(p => p.IdDistantion == obj.IdDistantion);
                            if (selectad_del_compitentions != null)
                            {
                                Competentions Del_compitentions = await competentionsServise.Delete(selectad_del_compitentions.IdCompetentions);
                                IEnumerable<Participation> participations = await participationService.Get();
                                var selectad = participations.FirstOrDefault(p => p.IdCompetentions == selectad_del_compitentions.IdCompetentions);
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
                            }
                            await showEmployeeAsync();
                            await DisplayAlert("Уведомление", "Дистанция успешно удалена", "Ok");
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

                var servere_adres = "http://90.189.158.10/api/Distans/";
                var httpResponseMasage = await httpClient.PostAsync(servere_adres, content);
                var url_image = await httpResponseMasage.Content.ReadAsStringAsync();
                await DisplayAlert("", "Экспорт успешно выполнен", "Ok");
            }
        }

        private async Task DownloadSimple()
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.GetStreamAsync("http://90.189.158.10/Simple/Шаблон для дистанций.xlsx");
            var filePath = await response.SaveToLocalFolderAsync("Шаблон для дистанций.xlsx");
            await DisplayAlert("", "Шаблон успешно сохранен", "Ok");
        }

        private async Task Import()
        {
            try
            {
                Main_RowDefinition_One.Height = 0;
                Main_RowDefinition_Activity.Height = new GridLength(1, GridUnitType.Star);
                activityIndicator.IsRunning = true;

                string path = await distantionsServise.GetExport();
                path = path.Replace(" ", string.Empty);

                HttpClient client = getClientServise.GetClient();
                var response = await client.GetStreamAsync(path);
                var filePath = await response.SaveToLocalFolderAsync($"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.xlsx");

                await Task.Delay(3000);
                Main_RowDefinition_One.Height = new GridLength(1, GridUnitType.Star);
                Main_RowDefinition_Activity.Height = 0;
                activityIndicator.IsRunning = false;
                await DisplayAlert("", "Импорт успешно выполнен", "Ok");
                await DisplayAlert("", filePath, "Ok");
            }
            catch { }
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