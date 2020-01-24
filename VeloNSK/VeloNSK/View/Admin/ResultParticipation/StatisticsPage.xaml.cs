using System.Collections.Generic;
using Microcharts;
using SkiaSharp;
using Xamarin.Forms;
using Entry = Microcharts.Entry;
using Xamarin.Forms.Xaml;

namespace VeloNSK.View.Admin.ResultParticipation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatisticsPage : ContentPage
    {
        private List<Entry> entries = new List<Entry>
        {
            new Entry(200)
            {
                Color=SKColor.Parse("#FF1943"),
                Label ="January",
                ValueLabel = "200"
            },
            new Entry(400)
            {
                Color = SKColor.Parse("00BFFF"),
                Label = "March",
                ValueLabel = "400"
            },
            new Entry(-100)
            {
                Color =  SKColor.Parse("#00CED1"),
                Label = "Octobar",
                ValueLabel = "-100"
            },
            };

        public StatisticsPage()
        {
            InitializeComponent();

            Chart1.Chart = new RadialGaugeChart() { Entries = entries };
            Chart2.Chart = new LineChart() { Entries = entries };
            Chart3.Chart = new DonutChart() { Entries = entries };
            Chart4.Chart = new BarChart() { Entries = entries };
            Chart5.Chart = new PointChart() { Entries = entries };
            //Chart6.Chart = new RadarChart() { Entries = entries };
        }
    }
}