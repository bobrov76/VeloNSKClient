using Microsoft.EntityFrameworkCore;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise.Model;
using VeloNSK.APIServise.Servise;
using VeloNSK.Context;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
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

        // DummyDataProvider dummyData = new DummyDataProvider();
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
                animations.Animations_Button(Back_Button);
                await Task.Delay(1000);
                await Navigation.PopModalAsync();//Переход назад
            };

            btnAddRecord.Clicked += async (s, e) =>
            {
                animations.Animations_Button(btnAddRecord);
                await Task.Delay(1000);
                int nul = 0;
                await Navigation.PushModalAsync(new AddUsersPage(nul, "Admin"), animate);
            };

            PoiskLogin.TextChanged += async (s, e) =>
            {
                if (PoiskLogin.Text.Length != 0)
                {
                    await Poisk("PoiskLogin");
                }
            };

            PoiskEmail.TextChanged += async (s, e) =>
            {
                if (PoiskEmail.Text.Length != 0)
                {
                    await Poisk("PoiskEmail");
                }
            };

            PoiskName.TextChanged += async (s, e) =>
            {
                if (PoiskName.Text.Length != 0)
                {
                    await Poisk("PoiskName");
                }
            };

            PoiskFam.TextChanged += async (s, e) =>
            {
                if (PoiskFam.Text.Length != 0)
                {
                    await Poisk("PoiskFam");
                }
            };

            PoiskPatronimic.TextChanged += async (s, e) =>
            {
                if (PoiskPatronimic.Text.Length != 0)
                {
                    await Poisk("PoiskPatronimic");
                }
            };

            PoiskRol.TextChanged += async (s, e) =>
            {
                if (PoiskRol.Text.Length != 0)
                {
                    await Poisk("PoiskRol");
                }
            };

            PoiskYars.TextChanged += async (s, e) =>
            {
                if (PoiskYars.Text.Length != 0)
                {
                    await Poisk("PoiskYars");
                }
            };

            PoiskStatusHels.TextChanged += async (s, e) =>
            {
                if (PoiskStatusHels.Text.Length != 0)
                {
                    await Poisk("PoiskStatusHels");
                }
            };
        }

        private async Task Poisk(string filtr)
        {
            IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
            switch (filtr)
            {
                case "PoiskLogin": infoUsers = infoUsers.Where(p => p.Login == PoiskLogin.Text || p.Login.StartsWith(PoiskLogin.Text)); break;
                case "PoiskName": infoUsers = infoUsers.Where(p => p.Name == PoiskName.Text || p.Name.StartsWith(PoiskName.Text)); break;

                case "PoiskFam": infoUsers = infoUsers.Where(p => p.Fam == PoiskFam.Text || p.Login.StartsWith(PoiskFam.Text)); break;

                case "PoiskEmail": infoUsers = infoUsers.Where(p => p.Email == PoiskEmail.Text || p.Name.StartsWith(PoiskEmail.Text)); break;

                case "PoiskPatronimic": infoUsers = infoUsers.Where(p => p.Patronimic == PoiskPatronimic.Text || p.Login.StartsWith(PoiskPatronimic.Text)); break;

                //case "PoiskPol": infoUsers = infoUsers.Where(p => p.Isman == PoiskName.Text || p.Name.StartsWith(PoiskName.Text)); break;

                case "PoiskRol": infoUsers = infoUsers.Where(p => p.Rol == PoiskRol.Text || p.Login.StartsWith(PoiskRol.Text)); break;
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
                string res = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Обновить данные", "Удалить данные");
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
                }
                lstData.SelectedItem = null;
            }
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage(), animate);
        }

        private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
        }
    }
}