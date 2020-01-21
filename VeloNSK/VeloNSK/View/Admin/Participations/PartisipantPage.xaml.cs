using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class PartisipantPage : ContentPage
    {
        private ParticipationService participationService = new ParticipationService();
        private CompetentionsServise competentionsServise = new CompetentionsServise();
        private RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        private CategoriYarsServise categoriYarsServise = new CategoriYarsServise();
        private DistantionsServise distantionsServise = new DistantionsServise();
        private ConnectClass connectClass = new ConnectClass();
        private links picture_lincs = new links();
        private Animations animations = new Animations();

        private bool animate;
        private bool alive = true;

        public PartisipantPage()
        {
            InitializeComponent();
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            //Fon.BackgroundImageSource = ImageSource.FromResource(picture_lincs.GetFon());
            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            showEmployeeAsync("OrderByName");
            Back_Button.Clicked += async (s, e) =>
            {
                animations.Animations_Button(Back_Button);
                await Task.Delay(1000);
                await Navigation.PopModalAsync();//Переход назад
            };
            bool AnimateStatus = false;
            rotate_name.Clicked += async (s, e) =>
            {
                if (AnimateStatus)
                {
                    await showEmployeeAsync("OrderByName");
                    await rotate_name.RotateTo(360, 700);
                    rotate_name.Rotation = 0;
                    AnimateStatus = false;
                }
                else
                {
                    await showEmployeeAsync("OrderByDescendingName");
                    await rotate_name.RotateTo(180, 700);
                    rotate_name.Rotation = 180;
                    AnimateStatus = true;
                }
            };
            rotate_login.Clicked += async (s, e) =>
            {
                if (AnimateStatus)
                {
                    await showEmployeeAsync("OrderByLogin");
                    await rotate_login.RotateTo(360, 700);
                    rotate_login.Rotation = 0;
                    AnimateStatus = false;
                }
                else
                {
                    await showEmployeeAsync("OrderByDescendingLogin");
                    await rotate_login.RotateTo(180, 700);
                    rotate_login.Rotation = 180;
                    AnimateStatus = true;
                }
            };
            rotate_distanse.Clicked += async (s, e) =>
            {
                if (AnimateStatus)
                {
                    await showEmployeeAsync("OrderByDistans");
                    await rotate_distanse.RotateTo(360, 700);
                    rotate_distanse.Rotation = 0;
                    AnimateStatus = false;
                }
                else
                {
                    await showEmployeeAsync("OrderByDescendingDistans");
                    await rotate_distanse.RotateTo(180, 700);
                    rotate_distanse.Rotation = 180;
                    AnimateStatus = true;
                }
            };
            rotate_date.Clicked += async (s, e) =>
            {
                if (AnimateStatus)
                {
                    await showEmployeeAsync("OrderByDate");
                    await rotate_date.RotateTo(360, 700);
                    rotate_date.Rotation = 0;
                    AnimateStatus = false;
                }
                else
                {
                    await showEmployeeAsync("OrderByDescendingDate");
                    await rotate_date.RotateTo(180, 700);
                    rotate_date.Rotation = 180;
                    AnimateStatus = true;
                }
            };
            rotate_yars.Clicked += async (s, e) =>
            {
                if (AnimateStatus)
                {
                    await showEmployeeAsync("OrderByYars");
                    await rotate_yars.RotateTo(360, 700);
                    rotate_yars.Rotation = 0;
                    AnimateStatus = false;
                }
                else
                {
                    await showEmployeeAsync("OrderByDescendingYars");
                    await rotate_yars.RotateTo(180, 700);
                    rotate_yars.Rotation = 180;
                    AnimateStatus = true;
                }
            };

            bool _collapsed = true;
            btnAddRecord.Clicked += async (s, e) =>
            {
                if (_collapsed)
                {
                    //for (int i = 0; i < 150; i++)
                    //{
                    //    await Task.Delay(5);
                    //    AddUser_RowDefinition.Height= i;
                    //}
                    //AddUser_One.Height = 30;
                    //AddUser_Two.Height = 40;
                    //AddUser_Thre.Height = 40;
                    _collapsed = false;
                }
                else
                {
                    //for (int i = 150; i >= 0; i--)
                    //{
                    //    await Task.Delay(10);
                    //    AddUser_RowDefinition.Height = i;
                    //}
                    _collapsed = true;
                    //AddUser_One.Height = 0;
                    //AddUser_Two.Height = 0;
                    //AddUser_Thre.Height = 0;
                }
            };

            PoiskLogin.TextChanged += async (s, e) => { await SelectedwsProrerty(PoiskLogin.Text, "PoiskLogin"); };
            PoiskNameUser.TextChanged += async (s, e) => { await SelectedwsProrerty(PoiskNameUser.Text, "PoiskNameUser"); };
            PoiskDate.TextChanged += async (s, e) => { await SelectedwsProrerty(PoiskDate.Text, ""); };
            PoiskYars.TextChanged += async (s, e) => { await SelectedwsProrerty(PoiskYars.Text, ""); };
            PoiskDistans.TextChanged += async (s, e) => { await SelectedwsProrerty(PoiskDistans.Text, "PoiskDistans"); };
            PoiskVerificate.TextChanged += async (s, e) => { await SelectedwsProrerty(PoiskVerificate.Text, ""); };

            //btnAddRecord.Clicked += async (s, e) =>
            //{
            //    animations.Animations_Button(btnAddRecord);
            //    await Task.Delay(1000);
            //    int nul = 0;
            //    await Navigation.PushModalAsync(new AddDistantionsPage(nul), animate);
            //};
            //Device.StartTimer(TimeSpan.FromSeconds(10), OnTimerTick);
        }

        //private bool OnTimerTick()
        //{
        //    showEmployeeAsync();
        //    return alive;
        //}

        private async Task SelectedwsProrerty(string masage, string PropertySelections)
        {
            //IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
            //IEnumerable<CategoriYars> categoriYars = await categoriYarsServise.Get();
            //IEnumerable<Participation> participations = await participationService.Get();
            //IEnumerable<Distantion> distantions = await distantionsServise.Get();

            //var info = from p in participations
            //           join d in distantions on  equals d.IdDistantion
            //           join c in categoriYars on p.IdCategoriYars equals c.IdCategori
            //           join i in infoUsers on p.IdUser equals i.IdUsers
            //           select new
            //           {
            //               p.Date,
            //               p.IdStatusVerification,
            //               d.NameDistantion,
            //               c.Ot,
            //               c.Do,
            //               i.Name,
            //               i.Patronimic,
            //               i.Login
            //           };

            //switch (PropertySelections)
            //{
            //    case "PoiskLogin": info = info.Where(p => p.Login == masage || p.Login.StartsWith(masage)); break;

            //    case "PoiskNameUser": info = info.Where(p => p.Name == masage || p.Login.StartsWith(masage)); break;

            //    case "PoiskDistans": info = info.Where(p => p.NameDistantion == masage || p.Login.StartsWith(masage)); break;

            //    case "PoiskDate": info = info.Where(p => p.Date == Convert.ToDateTime(masage) || p.Login.StartsWith(masage)); break;

            //    case "PoiskYars": info = info.Where(p => p.Ot == Convert.ToInt16(masage) || p.Login.StartsWith(masage)); break;

            //    case "PoiskVerificate":
            //        {
            //            if (masage.Substring(0, 1) != "T" || masage.Substring(0, 1) != "t")
            //            {
            //                info = info.Where(c => c.IdStatusVerification == true); break;
            //            }
            //            else if (masage.Substring(0, 1) != "F" || masage.Substring(0, 1) != "f")
            //            {
            //                info = info.Where(c => c.IdStatusVerification == false); break;
            //            }
            //            else { break; }
            //        }

            //    case "PoiskLoginAndName": info = info.Where(c => c.Login.StartsWith(PoiskLogin.Text) && c.Name.StartsWith(PoiskNameUser.Text)); break;
            //}

            //var res = info.ToList();
            //if (res != null)
            //{
            //    lstData.ItemsSource = res;
            //}
        }

        private bool isRowEven;

        private void Cell_OnAppearing(object sender, EventArgs e)
        {
            if (this.isRowEven)
            {
                var viewCell = (ViewCell)sender;
                if (viewCell.View != null)
                {
                    viewCell.View.BackgroundColor = Color.FromHex("#F2F2F2");
                }
                else { viewCell.View.BackgroundColor = Color.FromHex("#FFFFFF"); }
            }

            this.isRowEven = !this.isRowEven;
        }

        private async Task showEmployeeAsync(string FiltrStatus)
        {
            Main_RowDefinition_One.Height = 0;
            Main_RowDefinition_Activity.Height = new GridLength(1, GridUnitType.Star);
            //activityIndicator.IsRunning = true;
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
                           p.IdParticipation,
                           i.Login,
                           i.Name,
                           i.Patronimic,
                           k.Date,
                           p.IdStatusVerification,
                           d.NameDistantion,
                       };

            switch (FiltrStatus)
            {
                case "OrderByDescendingName":
                    info = info.OrderByDescending(p => p.Name);
                    break;

                case "OrderByName":
                    info = info.OrderBy(p => p.Name);
                    break;

                case "OrderByDescendingLogin":
                    info = info.OrderByDescending(p => p.Login);
                    break;

                case "OrderByLogin":
                    info = info.OrderBy(p => p.Login);
                    break;

                case "OrderByDescendingYars":
                    // info = info.OrderByDescending(p => p.Ot);
                    break;

                case "OrderByYars":
                    //  info = info.OrderBy(p => p.Ot);
                    break;

                case "OrderByDescendingDistans":
                    info = info.OrderByDescending(p => p.NameDistantion);
                    break;

                case "OrderByDistans":
                    info = info.OrderBy(p => p.NameDistantion);
                    break;

                case "OrderByDescendingDate":
                    info = info.OrderByDescending(p => p.Date);
                    break;

                case "OrderByDate":
                    info = info.OrderBy(p => p.Date);
                    break;
            }
            var res = info.ToList();
            if (res.Count != 0)
            {
                //lstData.ItemsSource = res;
                //YesRecords.Height = new GridLength(1, GridUnitType.Star);
                //NoRecords.Height = 0;
                //await Task.Delay(3000);
                //Main_RowDefinition_One.Height = new GridLength(1, GridUnitType.Star);
                //Main_RowDefinition_Activity.Height = 0;
                //activityIndicator.IsRunning = false;
            }
            else
            {
                //lstData.ItemsSource = res;
                //YesRecords.Height = 0;
                //NoRecords.Height = new GridLength(1, GridUnitType.Star);
            }
        }

        private async void lstData_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var obj = e.SelectedItem;

                string res = await DisplayActionSheet("Выберите операцию", "Отмена", null, "Обновить данные", "Удалить данные");
                switch (res)
                {
                    case "Обновить данные":
                        // await Navigation.PushModalAsync(new AddDistantionsPage(obj.IdDistantion), animate);
                        break;

                    case "Удалить данные":
                        bool result = await DisplayAlert("Подтвердить действие", "Вы хотите удалить элемент?", "Да", "Нет");
                        if (result == true)
                        {
                            //  Participation Del_Participation = await participationService.Delete(obj.IdParticipation);
                            await showEmployeeAsync("OrderByName");
                        }
                        break;
                }
                //lstData.SelectedItem = null;
            }
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage(), animate);
        }
    }
}