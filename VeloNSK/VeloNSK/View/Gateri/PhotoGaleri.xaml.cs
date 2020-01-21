using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise.Servise;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK.View.Gateri
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoGaleri : ContentPage
    {
        private HttpClient _client;
        private links picture_lincs = new links();
        private ConnectClass connectClass = new ConnectClass();
        private string[] images;

        public PhotoGaleri()
        {
            InitializeComponent();

            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            Fon.BackgroundImageSource = ImageSource.FromResource(picture_lincs.GetFon());
            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());

            Save_More_Button.Clicked += async (s, e) =>
            {
                for (int i = 0; i < images.Length; i++)
                {
                    await DownloadAndSaveImage(images[i]);
                }
            };
            Head_Button.Clicked += async (s, e) => { await Navigation.PopModalAsync(); };

            _client = new HttpClient();
            LoadingAsync();
            OnAppearing();
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PopModalAsync();
        } //Переход на страницу с ошибкой интернет соединения

        private async Task LoadingAsync()
        {
            Main_RowDefinition_Two.Height = 0;
            RowDefinitionActivity.Height = new GridLength(1, GridUnitType.Star);
            activityIndicator.IsRunning = true;
            await Task.Delay(3000);
            Main_RowDefinition_Two.Height = new GridLength(1, GridUnitType.Star);
            RowDefinitionActivity.Height = 0;
            activityIndicator.IsRunning = false;
        }

        public async void OnAppearing()
        {
            base.OnAppearing();
            Thickness posLeft = new Thickness(5, 5, 5, 15);
            string[] images = await GetImageListAsync();
            if (images != null)
            {
                for (int i = 0; i < images.Length; i++)
                {
                    var image = new ImageButton
                    {
                        Aspect = Aspect.Fill,
                        WidthRequest = 200,
                        HeightRequest = 200,
                        MinimumHeightRequest = 200,
                        MinimumWidthRequest = 200,
                        Margin = new Thickness(5, 5, 5, 15),

                        Source = ImageSource.FromUri(new Uri(images[i]))
                    };
                    image.Clicked += async (s, e) =>
                    {
                        if (await DisplayAlert("Сохранение", "Вы хотите сохранить данное изображение", "Да", "Нет"))
                        {
                            await DownloadAndSaveImage((s as ImageButton).Source.ToString().Replace("Uri: ", string.Empty));
                        }
                    };
                    wrapLayout.Children.Add(image);
                }
            }
        }

        private async Task<string[]> GetImageListAsync()
        {
            try
            {
                string requestUri = "http://90.189.158.10/api/Images";
                string result = await _client.GetStringAsync(requestUri);
                return JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\tERROR: {ex.Message}");
            }

            return null;
        }

        private async Task DownloadAndSaveImage(string get_path)
        {
            try
            {
                using (var response = await _client.GetStreamAsync(get_path))
                {
                    var filePath = await response.SaveToLocalFolderAsync($"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.jpg");
                    await DisplayAlert("", filePath, "Ok");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DownloadAndSaveImage Exception: {ex}");
            }
        }
    }
}