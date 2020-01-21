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

namespace VeloNSK.View.Admin.Participations.Compitentions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCompitentionsPage : ContentPage
    {
        private links picture_lincs = new links();
        private Lincs server_lincs = new Lincs();
        private Animations animations = new Animations();
        private ConnectClass connectClass = new ConnectClass();
        private DistantionsServise distantionsServise = new DistantionsServise();
        private CompetentionsServise competentionsServise = new CompetentionsServise();
        private DateTime Time;
        private int id_Distantion;
        private Picker picker;

        public AddCompitentionsPage(int id)
        {
            InitializeComponent();
            get_infa(id);
            Criate_Picer();
            Time_Picrt.MinimumDate = DateTime.Now.AddDays(3);
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());//Устанавливаем фон
            Users_Fon_Images.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "UserFon.png");

            Back_Button.Clicked += async (s, e) =>
            {
                animations.Animations_Button(Back_Button);
                await Task.Delay(1000);
                await Navigation.PopModalAsync();//Переход назад
            };
            Time_Picrt.MinimumDate = DateTime.Today;
            Time_Picrt.DateSelected += (s, e) =>
            {
                Time = e.NewDate;
            };

            Registrations_Button.Clicked += async (s, e) =>
            {
                if (id != 0) { await Update(id); }
                else
                {
                    IEnumerable<Competentions> competentions = await competentionsServise.Get();
                    IEnumerable<Distantion> distantions = await distantionsServise.Get();
                    var info = from d in distantions
                               join i in competentions on d.IdDistantion equals i.IdDistantion
                               select new
                               {
                                   d.NameDistantion,
                                   i.Date,
                               };
                    info = info.Where(p => p.NameDistantion == picker.Items[picker.SelectedIndex] && p.Date == Time);
                    int res = info.Count();
                    if (res == 0)
                    {
                        await Criate();
                    }
                    else { await DisplayAlert("Ошибка", "Компетенция с такими параметрами существует", "Ok"); }
                }
            };
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage());
        }

        private async Task Criate_Picer()
        {
            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            picker = new Picker { Margin = new Thickness(0, -15, 10, 0) };
            foreach (var item in distantions)
            {
                picker.Items.Add(item.NameDistantion);
            }

            picker.SelectedIndexChanged += async (s, e) =>
            {
                IEnumerable<Distantion> distantion = await distantionsServise.Get();
                var info = distantions.FirstOrDefault(p => p.NameDistantion == picker.Items[picker.SelectedIndex]);
                id_Distantion = info.IdDistantion;
            };

            Name_Picer.Children.Add(picker);
        }

        private async Task get_infa(int id)
        {
            Competentions competentions = await competentionsServise.Get_ID(id);
            IEnumerable<Distantion> distantions = await distantionsServise.Get();
            var info = distantions.FirstOrDefault(p => p.IdDistantion == competentions.IdDistantion);
            if (id != 0)
            {
                Time_Picrt.Date = competentions.Date;
                for (int i = 0; i < picker.Items.Count; i++)
                {
                    if (info.Discriptions == picker.Items[picker.SelectedIndex])
                    {
                        picker.SelectedIndex = picker.SelectedIndex;
                    }
                    break;
                }
                Head_Lable.Text = "Редактирование компетенции";
            }
        }

        public async Task Update(int id)
        {
            if (Time != null && id_Distantion != 0)
            {
                Competentions competentions = new Competentions
                {
                    IdCompetentions = id,
                    Date = Time,
                    IdDistantion = id_Distantion
                };
                await competentionsServise.Update(competentions);
                await Navigation.PopModalAsync();
            }
            else
            {
                if (!await DisplayAlert("Ошибка", "Вы заполнили не все поля", "Заполнить", "Выйти")) { await Navigation.PopModalAsync(); }
            }
        }

        public async Task Criate()
        {
            if (Time != null && id_Distantion != 0)
            {
                Competentions competentions = new Competentions
                {
                    Date = Time,
                    IdDistantion = id_Distantion
                };
                await competentionsServise.Add(competentions);

                if (!await DisplayAlert("", "Добавить еще одну дистанцию", "Да", "Нет")) { await Navigation.PopModalAsync(); }
                else
                {
                    Time_Picrt.MinimumDate = DateTime.Today;
                }
            }
            else
            {
                if (!await DisplayAlert("Ошибка", "Вы заполнили не все поля", "Заполнить", "Выйти")) { await Navigation.PopModalAsync(); }
            }
        }
    }
}