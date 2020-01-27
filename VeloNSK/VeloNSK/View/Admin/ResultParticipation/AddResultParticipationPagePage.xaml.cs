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

namespace VeloNSK.View.Admin.ResultParticipation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddResultParticipationPagePage : ContentPage
    {
        private ResultParticipationServise resultParticipationServise = new ResultParticipationServise();
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
        private Picker SelectDate;
        private int ID_Partisipant = 0;
        private DateTime ID_Time;

        public AddResultParticipationPagePage(int id)
        {
            InitializeComponent();
            Criate_LoginPicer(false);
            Criate_SelectCompitentions(false, false);
            Info_Editor.IsEnabled = false;
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());
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
                    await Criate_SelectCompitentions(true, false);
                }
            };

            ItogTime_Entry.TextChanged += (s, e) =>
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

        private async Task Criate_LoginPicer(bool status)
        {
            IEnumerable<Participation> participations = await participationService.Get();
            IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
            IEnumerable<Competentions> competentions = await competentionsServise.Get();
            var info = from p in participations
                       join i in infoUsers on p.IdUser equals i.IdUsers
                       join c in competentions on p.IdCompetentions equals c.IdCompetentions
                       select new
                       {
                           i.Login,
                           i.Name,
                           i.Fam,
                           i.Patronimic,
                           i.Email,
                           i.Years,
                           i.Isman,
                           i.IdHelth,
                           p.IdParticipation,
                           c.Date
                       };
            LoginPicer = new Picker { Margin = new Thickness(10, -10, 10, 5) };
            if (status)
            {
                info = info.Where(p => p.Login == PoiskLogin.Text || p.Login.StartsWith(PoiskLogin.Text));
            }
            foreach (var item in info)
            {
                DateTime today = DateTime.Now;
                DateTime start = item.Date;
                if (today <= start && today.AddMonths(3) >= start)
                {
                    LoginPicer.Items.Add(item.Login);
                }
            }

            LoginPicer.SelectedIndexChanged += async (s, e) =>
            {
                IEnumerable<UserHelth> userHelths = await registrationUsersService.get_hels_status();
                var picker_list = info.FirstOrDefault(x => x.Login == LoginPicer.Items[LoginPicer.SelectedIndex].ToString());
                var picker_list_Hels = userHelths.FirstOrDefault(x => x.IdHealth == picker_list.IdHelth);
                Criate_SelectCompitentions(true, true);
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
        }

        private async Task Criate_SelectCompitentions(bool status, bool selectad)
        {
            SelectDate = new Picker { Margin = new Thickness(10, -10, 10, 5) };
            if (status)
            {
                if (selectad)
                {
                    SelectDate.IsEnabled = true;
                    IEnumerable<Participation> participations = await participationService.Get();
                    IEnumerable<Distantion> distantions = await distantionsServise.Get();
                    IEnumerable<InfoUser> infoUsers = await registrationUsersService.Get_user();
                    IEnumerable<Competentions> competentions = await competentionsServise.Get();
                    var info = from p in participations
                               join c in competentions on p.IdCompetentions equals c.IdCompetentions
                               join d in distantions on c.IdDistantion equals d.IdDistantion
                               join i in infoUsers on p.IdUser equals i.IdUsers
                               select new
                               {
                                   p.IdParticipation,
                                   d.NameDistantion,
                                   c.Date,
                                   d.Discriptions,
                                   i.Login
                               };
                    info = info.Where(p => p.Login == LoginPicer.Items[LoginPicer.SelectedIndex]);

                    foreach (var item in info)
                    {
                        SelectDate.Items.Add(item.NameDistantion);
                    }

                    SelectDate.SelectedIndexChanged += (s, e) =>
                    {
                        var picker_list = info.FirstOrDefault(x => x.NameDistantion == SelectDate.Items[SelectDate.SelectedIndex].ToString());
                        Time_Lable.Text = picker_list.Date.ToShortDateString();
                        Info_Editor.Text = "";
                        Info_Editor.Text = "Название: " + picker_list.NameDistantion + "\r\n";
                        Info_Editor.Text += "Дата проведения: " + picker_list.Date.ToShortDateString() + "\r\n";
                        Info_Editor.Text += "Описание: " + picker_list.Discriptions + "\r\n";
                    };

                    SelectDate.SelectedIndexChanged += async (s, e) =>
                    {
                        var picker_list = info.FirstOrDefault(x => x.NameDistantion == SelectDate.Items[SelectDate.SelectedIndex].ToString());
                        ID_Partisipant = picker_list.IdParticipation;
                    };
                }
                else
                {
                    SelectDate.IsEnabled = false;
                }
            }
            else
            {
                SelectDate.IsEnabled = false;
            }
            GridMain.Children.Add(SelectDate, 1, 4);
        }

        public async Task Update(int id)
        {
            if (ID_Partisipant != 0)
            {
                //await Positions(ID_Partisipant);
                IEnumerable<ResultParticipant> resultParticipations = await resultParticipationServise.Get();
                var info = resultParticipations.FirstOrDefault(x => x.IdResultParticipation == id);
                ResultParticipant resultParticipant = new ResultParticipant
                {
                    IdResultParticipation = id,
                    IdParticipation = ID_Partisipant,
                    Mesto = info.Mesto,
                    ResultTime = ID_Time
                };

                await resultParticipationServise.Update(resultParticipant);
                await Navigation.PopModalAsync();
            }
            else
            {
                if (!await DisplayAlert("Ошибка", "Вы заполнили не все поля", "Заполнить", "Выйти")) { await Navigation.PopModalAsync(); }
            }
        }

        public async Task Criate()
        {
            if (ID_Partisipant != 0)
            {
                //await Positions(ID_Partisipant);
                ResultParticipant resultParticipant = new ResultParticipant
                {
                    IdParticipation = ID_Partisipant,
                    Mesto = Convert.ToInt32(Mesto_Entry.Text),
                    ResultTime = ID_Time
                };

                await resultParticipationServise.Add(resultParticipant);

                if (!await DisplayAlert("", "Добавить еще одну запись", "Да", "Нет")) { await Navigation.PopModalAsync(); }
            }
            else
            {
                if (!await DisplayAlert("Ошибка", "Вы заполнили не все поля", "Заполнить", "Выйти")) { await Navigation.PopModalAsync(); }
            }
        }

        //public async Task Positions(int id)
        //{
        //    IEnumerable<ResultParticipant> resultParticipants = await resultParticipationServise.Get();
        //    IEnumerable<Participation> participations = await participationService.Get();
        //    IEnumerable<Distantion> distantions = await distantionsServise.Get();
        //    IEnumerable<Competentions> competentions = await competentionsServise.Get();
        //    var info = from p in participations
        //               join c in competentions on p.IdCompetentions equals c.IdCompetentions
        //               join d in distantions on c.IdDistantion equals d.IdDistantion
        //               join res in resultParticipants on p.IdParticipation equals res.IdParticipation
        //               select new
        //               {
        //                   res.IdResultParticipation,
        //                   p.IdParticipation,
        //                   c.Date,
        //                   d.IdDistantion,
        //               };
        //    // у нас есть IdParticipation
        //    info = info.Where(p => p.IdParticipation == id).OrderBy(x => x.Date);
        //    int i = 1;
        //    foreach (var item in info)
        //    {
        //        //ResultParticipant resultParticipant = new ResultParticipant
        //        //{
        //        //    IdResultParticipation = item.IdResultParticipation,
        //        //    IdParticipation = item.IdDistantion,
        //        //    Mesto = i,
        //        //    ResultTime = item.Date
        //        //};
        //        //  await resultParticipationServise.Update(resultParticipant);
        //        i++;
        //    }
        //}

        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();

        private new void SizeChanged(object sender, EventArgs e) //Стилизация
        {
            if (size_form.GetHeightSize() > size_form.GetWidthSize())
            {
                Login_ColumnDefinition_Ziro.Width = 10;
                Login_ColumnDefinition_One.Width = new GridLength(1, GridUnitType.Star);
                Login_ColumnDefinition_Two.Width = 10;
            }
            else
            {
                Login_ColumnDefinition_Ziro.Width = new GridLength(1, GridUnitType.Star);
                Login_ColumnDefinition_One.Width = 560;
                Login_ColumnDefinition_Two.Width = new GridLength(1, GridUnitType.Star);
            }
        }
    }
}