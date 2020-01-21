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

namespace VeloNSK.View.Admin.Participations.Distanse
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DistantionsPage : ContentPage
    {
        
        DistantionsServise distantionsServise = new DistantionsServise();
        ConnectClass connectClass = new ConnectClass();
        links picture_lincs = new links();
        Animations animations = new Animations();
        bool animate;
        bool alive = true;
        public DistantionsPage()
        {
            InitializeComponent();



            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы            
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            Fon.BackgroundImageSource = ImageSource.FromResource(picture_lincs.GetFon());
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
                await Navigation.PushModalAsync(new AddDistantionsPage(nul), animate);
            };
            Device.StartTimer(TimeSpan.FromSeconds(10), OnTimerTick);
        }

        private bool OnTimerTick()
        {
            showEmployeeAsync();
            return alive;
        }


        private async Task showEmployeeAsync()
        {
            IEnumerable<Distantion> infoUsers = await distantionsServise.Get();
            var res = infoUsers.ToList();
            lstData.ItemsSource = res;
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
                            await showEmployeeAsync();
                        }
                        break;
                }
                lstData.SelectedItem = null;
            }
        }
        public async Task Connect_ErrorAsync() { await Navigation.PushModalAsync(new ErrorConnectPage(), animate); }
    }
}