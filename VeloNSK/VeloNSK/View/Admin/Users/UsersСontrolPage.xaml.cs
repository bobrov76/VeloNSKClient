using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.FilePicker;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise.Model;
using VeloNSK.APIServise.Servise;
using VeloNSK.Context;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using VeloNSK.View;
using VeloNSK.View.Admin;
using VeloNSK.View.Admin.Users;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsersСontrolPage : ContentPage
    {
        private RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        private ConnectClass connectClass = new ConnectClass();
        private links picture_lincs = new links();
        private GetClientServise getClientServise = new GetClientServise();
        private Animations animations = new Animations();
        private bool animate;
        private UsersContext db;

        public UsersСontrolPage()
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
                //await Task.Delay(1000);
                await Navigation.PopModalAsync();//Переход назад
            };

            btnAddRecord.Clicked += async (s, e) =>
            {
                string res = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Добавить данные", "Импортировать данные", "Экспортировать данные");
                switch (res)
                {
                    case "Добавить данные":
                        int nul = 0;
                        await Navigation.PushModalAsync(new AddUsersPage(nul, "Admin"), animate);
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
                //animations.Animations_Button(btnAddRecord);
                //await Task.Delay(1000);
            };

            PoiskLogin.TextChanged += async (s, e) =>
            {
                try
                {
                    await Poisk("PoiskLogin");
                }
                catch { }
            };

            PoiskName.TextChanged += async (s, e) =>
            {
                try
                {
                    await Poisk("PoiskName");
                }
                catch { }
            };

            PoiskFam.TextChanged += async (s, e) =>
            {
                try
                {
                    await Poisk("PoiskFam");
                }
                catch { }
            };
        }

        private async Task Poisk(string filtr)
        {
            IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
            switch (filtr)
            {
                case "PoiskLogin": infoUsers = infoUsers.Where(p => p.Login == PoiskLogin.Text || p.Login.StartsWith(PoiskLogin.Text)); break;
                case "PoiskName": infoUsers = infoUsers.Where(p => p.Name == PoiskName.Text || p.Name.StartsWith(PoiskName.Text)); break;
                case "PoiskFam": infoUsers = infoUsers.Where(p => p.Fam == PoiskFam.Text || p.Fam.StartsWith(PoiskFam.Text)); break;
            }
            var res = infoUsers.ToList();
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
            YesRecords.Height = 0;
            NoRecords.Height = 0;
            RowDefinitionActivity.Height = new GridLength(1, GridUnitType.Star);
            activityIndicator.IsRunning = true;
            IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
            IEnumerable<UserHelth> userHelths = await registrationUsersService.get_hels_status();
            var info = from p in infoUsers
                       join u in userHelths on p.IdHelth equals u.IdHealth

                       select new
                       {
                           p.Email,
                           p.Fam,
                           p.Isman,
                           p.Login,
                           p.Years,
                           p.Rol,
                           p.Patronimic,
                           p.Name,
                           u.NameHealth
                       };
            var res = infoUsers.ToList();
            if (res.Count != 0)
            {
                lstData.ItemsSource = res;
                YesRecords.Height = new GridLength(1, GridUnitType.Star);
                NoRecords.Height = 0;
                await Task.Delay(1000);
                RowDefinitionActivity.Height = 0;
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
                InfoUser obj = (InfoUser)e.SelectedItem;
                string res = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Подробнее", "Обновить данные", "Удалить данные");
                switch (res)
                {
                    case "Обновить данные":
                        await Navigation.PushModalAsync(new AddUsersPage(obj.IdUsers, "Admin"), animate);
                        break;

                    case "Удалить данные":
                        bool result = await DisplayAlert("Подтвердить действие", "Вы хотите удалить элемент?", "Да", "Нет");
                        if (result == true)
                        {
                            InfoUser DeliteUsers = await registrationUsersService.Delete(obj.IdUsers);
                            string folder_name = obj.Login.Replace("+", string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);

                            using (var client = getClientServise.GetClient())
                            {
                                string results = await client.GetStringAsync("http://90.189.158.10/api/Folder/" + folder_name);//Создать контроллер
                                string masage = results;
                            }
                            await showEmployeeAsync();
                        }
                        break;

                    case "Подробнее":
                        await Navigation.PushModalAsync(new PersonalAccountPage(obj.IdUsers, true), animate);
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
                var servere_adres = "http://90.189.158.10/api/Exel/";
                var httpResponseMasage = await httpClient.PostAsync(servere_adres, content);
                await httpResponseMasage.Content.ReadAsStringAsync();
                await DisplayAlert("", "Экспорт успешно выполнен", "Ok");
            }
        }

        private async Task DownloadSimple()
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.GetStreamAsync("http://90.189.158.10/Simple/UserExportSimple.xlsx");
            await response.SaveToLocalFolderAsync("Шаблон для экспорта информации о участниках" + $"{DateTime.Now.ToString("ddMMyyyyhhmmss")}" + ".xlsx");
            await DisplayAlert("", "Шаблон успешно сохранен", "Ok");
        }

        private async Task Import()
        {
            try
            {
                HttpClient client = getClientServise.GetClient();
                string result = await client.GetStringAsync("http://90.189.158.10/api/Exel/ExportUser");
                string path = JsonConvert.DeserializeObject<string>(result);
                path = path.Replace(" ", string.Empty);

                var response = await client.GetStreamAsync(path);
                var filePath = await response.SaveToLocalFolderAsync($"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.xlsx");

                await DisplayAlert("", "Импорт успешно выполнен", "Ok");
            }
            catch { }
        }

        private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
        }
    }
}