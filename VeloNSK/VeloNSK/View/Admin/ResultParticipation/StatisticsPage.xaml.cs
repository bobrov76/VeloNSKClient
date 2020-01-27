using System.Collections.Generic;
using Microcharts;
using SkiaSharp;
using Xamarin.Forms;
using Entry = Microcharts.Entry;
using Xamarin.Forms.Xaml;
using VeloNSK.APIServise.Servise;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using VeloNSK.APIServise.Model;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace VeloNSK.View.Admin.ResultParticipation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatisticsPage : ContentPage
    {
        private ParticipationService participationService = new ParticipationService();
        private CompetentionsServise competentionsServise = new CompetentionsServise();
        private RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        private ResultParticipationServise resultParticipationServise = new ResultParticipationServise();
        private DistantionsServise distantionsServise = new DistantionsServise();

        private async Task Get()
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
                           d.NameDistantion,
                           i.Login,
                           r.IdResultParticipation
                       };

            var groups = from p in info
                         group p by p.NameDistantion into g
                         select new
                         {
                             g.Key,
                             Count = g.Count()
                         };

            List<Entry> entries = new List<Entry>(groups.Count());

            int k = 0;
            string[] color = new string[] { "#dbf720", "#42d62b", "#cf2934", "#f20acb", "#9090e8", "#183bed", "#926eae", "#FF1943", "#ab5b68", "#0af2bc", "#cac4b0", "#fa0f0f", "#18ed54" };
            foreach (var item in groups)
            {
                if (color.Length == k++)
                {
                    k = 0;
                }
                entries.Add(new Entry(item.Count)
                {
                    Color = SKColor.Parse(color[k]),
                    Label = item.Key,
                    ValueLabel = item.Count.ToString()
                });
            }
            Chart2.Chart = new LineChart() { Entries = entries };
            // Chart4.Chart = new BarChart() { Entries = entries };
        }

        public StatisticsPage()
        {
            Get();
            InitializeComponent();
            Back_Button.Clicked += async (s, e) =>
            {
                await Navigation.PopModalAsync();//Переход назад
            };
        }
    }
}