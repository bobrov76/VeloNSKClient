using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise;
using VeloNSK.APIServise.Model;
using VeloNSK.APIServise.Servise;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK.View.Admin.Participations
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddPaticipationsPage : ContentPage
    {
        private ParticipationService participationService = new ParticipationService();
        private CompetentionsServise competentionsServise = new CompetentionsServise();
        private RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        private CategoriYarsServise categoriYarsServise = new CategoriYarsServise();
        private DistantionsServise distantionsServise = new DistantionsServise();
        private links picture_lincs = new links();
        private Lincs server_lincs = new Lincs();
        private Animations animations = new Animations();
        private ConnectClass connectClass = new ConnectClass();
        private Picker LoginPicer;
        private Picker CompetentionsPicer;
        private Picker SelectDate;
        private string Date;
        private int IDUser;
        private int IDCompitention;
        private DateTime dateTimeTest;

        public AddPaticipationsPage(int id)
        {
            InitializeComponent();
            // get_infa(id);
            Criate_LoginPicer(false);
            Criate_CompetentionsPicer(false);
            Criate_SelectDate(false);
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            // Fon.BackgroundImageSource = ImageSource.FromResource(picture_lincs.GetFon()); ;//Устанавливаем фон
            Users_Fon_Images.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "UserFon.png");

            Back_Button.Clicked += async (s, e) =>
            {
                animations.Animations_Button(Back_Button);
                await Task.Delay(1000);
                await Navigation.PopModalAsync();//Переход назад
            };
            PoiskLogin.TextChanged += async (s, e) =>
            {
                if (PoiskLogin.Text != null)
                {
                    await Criate_LoginPicer(true);
                }
            };

            PoiskCompetentions.TextChanged += async (s, e) =>
            {
                if (PoiskCompetentions.Text != null)
                {
                    await Criate_CompetentionsPicer(true);
                }
            };

            Registrations_Button.Clicked += async (s, e) =>
            {
                if (id != 0) { await Update(id); }
                else { await Criate(); }
            };
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage());
        }

        private async Task Date_TestAsync()
        {
            IEnumerable<Participation> participations = await participationService.Get();
            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            IEnumerable<Competentions> competentions = await competentionsServise.Get();
            var info = from p in participations
                       join k in competentions on p.IdCompetentions equals k.IdCompetentions
                       join d in distantions on k.IdDistantion equals d.IdDistantion
                       select new
                       {
                           d.NameDistantion,
                           k.Date,
                       };

            var picker_list = info.FirstOrDefault(x => x.NameDistantion == CompetentionsPicer.Items[CompetentionsPicer.SelectedIndex].ToString());
            if (picker_list.Date >= DateTime.Today.AddDays(2))
            {
                await DisplayAlert("Ошибка", "На эту дату запись уже недоступна", "Ок");
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
                    SelectDate.Items.Add(item.Date.ToShortDateString().ToString());
                }
                SelectDate.SelectedIndexChanged += async (s, e) =>
                {
                    var picker_list = info.Where(
                        x => x.NameDistantion == CompetentionsPicer.Items[CompetentionsPicer.SelectedIndex] &&
                        x.Date == Convert.ToDateTime(SelectDate.Items[SelectDate.SelectedIndex]));
                    foreach (var item in picker_list)
                    {
                        IDCompitention = Convert.ToInt32(item.IdCompetentions);
                    }
                };
            }
            else
            {
                SelectDate.IsEnabled = false;
                SelectDate.Items.Add(DateTime.Today.ToString("d"));
            }
            GridMain.Children.Add(SelectDate, 1, 5);
        }

        private async Task Criate_LoginPicer(bool status)
        {
            IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
            LoginPicer = new Picker { Margin = new Thickness(10, -10, 10, 5) };
            if (status)
            {
                infoUsers = infoUsers.Where(p => p.Login == PoiskLogin.Text || p.Login.StartsWith(PoiskLogin.Text));
            }
            foreach (var item in infoUsers)
            {
                LoginPicer.Items.Add(item.Login);
            }

            LoginPicer.SelectedIndexChanged += async (s, e) =>
            {
                IEnumerable<UserHelth> userHelths = await registrationUsersService.get_hels_status();
                var picker_list = infoUsers.FirstOrDefault(x => x.Login == LoginPicer.Items[LoginPicer.SelectedIndex].ToString());
                var picker_list_Hels = userHelths.FirstOrDefault(x => x.IdHealth == picker_list.IdHelth);
                IDUser = picker_list.IdUsers;
                Info_Editor.Text = "";
                Info_Editor.Text = "Login: " + picker_list.Login + "\r\n";
                Info_Editor.Text += "E-mail: " + picker_list.Email + "\r\n";
                Info_Editor.Text += "ФИО: " + picker_list.Fam + " " + picker_list.Name + " " + picker_list.Patronimic + "\r\n";
                Info_Editor.Text += "Возраст: " + picker_list.Years + "\r\n";
                Info_Editor.Text += "Статус здоровья: " + picker_list_Hels.NameHealth + "\r\n";
                if (picker_list.Isman == true) { Info_Editor.Text += "Пол: Мужской" + "\r\n"; }
                else { Info_Editor.Text += "Пол: Женский" + "\r\n"; }
            };

            GridMain.Children.Add(LoginPicer, 1, 2);
            // LoginPicer.SelectedIndex = 1;
        }

        private async Task Criate_CompetentionsPicer(bool status)
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
                           d.Discriptions
                       };
            if (status)
            {
                info = info.Where(p => p.NameDistantion == PoiskCompetentions.Text || p.NameDistantion.StartsWith(PoiskCompetentions.Text));
            }
            CompetentionsPicer = new Picker { Margin = new Thickness(10, -10, 10, 5) };
            foreach (var item in info)
            {
                CompetentionsPicer.Items.Add(item.NameDistantion);
            }

            CompetentionsPicer.SelectedIndexChanged += async (s, e) =>
            {
                var picker_list = info.FirstOrDefault(x => x.NameDistantion == CompetentionsPicer.Items[CompetentionsPicer.SelectedIndex].ToString());
                await Criate_SelectDate(true);
                Info_Editor.Text = "";
                Info_Editor.Text = "Название: " + picker_list.NameDistantion + "\r\n";
                Info_Editor.Text += "Дата проведения: " + picker_list.Date + "\r\n";
                Info_Editor.Text += "Описание: " + picker_list.Discriptions + "\r\n";
            };

            GridMain.Children.Add(CompetentionsPicer, 1, 4);
            //  CompetentionsPicer.SelectedIndex = 1;
        }

        private async Task get_infa(int id)
        {
            Competentions competentions = await competentionsServise.Get_ID(id);
            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            var info = distantions.FirstOrDefault(p => p.IdDistantion == competentions.IdDistantion);
            if (id != 0)
            {
            }
        }

        public async Task Update(int id)
        {
            if (IDUser != 0 && IDCompitention != 0 && Autentification_CheckBox.IsChecked)
            {
                bool verifi = false;
                if (Autentification_CheckBox.IsChecked)
                {
                    verifi = true;
                }
                else { verifi = false; }

                Participation participation = new Participation
                {
                    IdParticipation = id,
                    IdCompetentions = IDCompitention,
                    IdUser = IDUser,
                    IdStatusVerification = verifi
                };

                await participationService.Update(participation);
                await Navigation.PopModalAsync();
            }
            else
            {
                if (!await DisplayAlert("Ошибка", "Вы заполнили не все поля", "Заполнить", "Выйти")) { await Navigation.PopModalAsync(); }
            }
        }

        public async Task Criate()
        {
            if (IDUser != 0 && IDCompitention != 0 && Autentification_CheckBox.IsChecked)
            {
                bool verifi = false;
                if (Autentification_CheckBox.IsChecked)
                {
                    verifi = true;
                }
                else { verifi = false; }

                Participation participation = new Participation
                {
                    IdCompetentions = IDCompitention,
                    IdUser = IDUser,
                    IdStatusVerification = verifi
                };

                await participationService.Add(participation);

                if (!await DisplayAlert("", "Добавить еще одну запись", "Да", "Нет")) { await Navigation.PopModalAsync(); }
            }
            else
            {
                if (!await DisplayAlert("Ошибка", "Вы заполнили не все поля", "Заполнить", "Выйти")) { await Navigation.PopModalAsync(); }
            }
        }

        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();

        private new void SizeChanged(object sender, EventArgs e) //Стилизация
        {
            if (size_form.GetHeightSize() > size_form.GetWidthSize())
            {
                Login_ColumnDefinition_Ziro.Width = 20;
                Login_ColumnDefinition_One.Width = new GridLength(1, GridUnitType.Star);
                Login_ColumnDefinition_Two.Width = 20;
            }
            else
            {
                Login_ColumnDefinition_Ziro.Width = new GridLength(1, GridUnitType.Star);
                Login_ColumnDefinition_One.Width = 500;
                Login_ColumnDefinition_Two.Width = new GridLength(1, GridUnitType.Star);
            }
        }
    }
}