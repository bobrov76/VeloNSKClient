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

namespace VeloNSK.View.Admin.Participations.Compitentions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompitentionsPage : ContentPage
    {
        private CompetentionsServise competentionsServise = new CompetentionsServise();
        private DistantionsServise distantionsServise = new DistantionsServise();
        private ParticipationService participationService = new ParticipationService();
        private ResultParticipationServise resultParticipationServise = new ResultParticipationServise();
        private ConnectClass connectClass = new ConnectClass();
        private links picture_lincs = new links();
        private Animations animations = new Animations();
        private bool animate;
        private bool alive = true;
        private DateTime SelectedDate;

        public CompitentionsPage()
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
                await Task.Delay(1000);
                await Navigation.PopModalAsync();//Переход назад
            };

            btnAddRecord.Clicked += async (s, e) =>
            {
                animations.Animations_Button(btnAddRecord);
                await Task.Delay(1000);
                int nul = 0;
                await Navigation.PushModalAsync(new AddCompitentionsPage(nul), animate);
            };

            PoiskName.TextChanged += async (s, e) =>
            {
                try
                {
                    if (PoiskName.Text != null)
                    {
                        await Poisk("PoiskName");
                    }
                }
                catch { }
            };
            PoiskDate.MinimumDate = DateTime.Today;

            PoiskDate.DateSelected += async (s, e) =>
            {
                try
                {
                    if (e.NewDate != null)
                    {
                        SelectedDate = e.NewDate;
                        await Poisk("PoiskDate");
                    }
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

        private async Task showEmployeeAsync()
        {
            Main_RowDefinition_One.Height = 0;
            Main_RowDefinition_Activity.Height = new GridLength(1, GridUnitType.Star);
            activityIndicator.IsRunning = true;
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
                            // IEnumerable<Competentions> competentions = await competentionsServise.Get();

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
    }
}