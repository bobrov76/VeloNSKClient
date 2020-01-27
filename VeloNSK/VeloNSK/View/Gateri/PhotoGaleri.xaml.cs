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
        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();
        private links picture_lincs = new links();
        private ConnectClass connectClass = new ConnectClass();
        private string[] images;

        public PhotoGaleri()
        {
            InitializeComponent();
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());
            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());
            _client = new HttpClient();
            LoadingAsync();

            Save_More_Button.Clicked += async (s, e) =>
            {
                for (int i = 0; i < images.Length; i++)
                {
                    await DownloadAndSaveImage(images[i]);
                }
            };
            Head_Button.Clicked += async (s, e) => { await Navigation.PopModalAsync(); };
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
            await OnAppearing();
            await Task.Delay(400);
            Main_RowDefinition_Two.Height = new GridLength(1, GridUnitType.Star);
            RowDefinitionActivity.Height = 0;
            activityIndicator.IsRunning = false;
        }

        public async Task OnAppearing()
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
            catch { }
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
            catch { }
        }

        private new void SizeChanged(object sender, EventArgs e)
        {
            double width = size_form.GetWidthSize();
            double height = size_form.GetHeightSize();
            if (width > height)
            {
                if (Device.Idiom == TargetIdiom.Phone)
                {
                    Main_RowDefinition_Ziro.Height = 0;
                    Main_RowDefinition_Fore.Height = 0;
                    Head_Image.IsVisible = false;
                    Head_Lable.IsVisible = false;
                    Head_Button.IsVisible = false;
                    Hend_BoxView.IsVisible = false;
                }
            }
            else
            {
                if (Device.Idiom == TargetIdiom.Phone)
                {
                    Main_RowDefinition_Ziro.Height = 70;
                    Main_RowDefinition_Fore.Height = 40;
                    Head_Image.IsVisible = true;
                    Head_Lable.IsVisible = true;
                    Head_Button.IsVisible = true;
                    Hend_BoxView.IsVisible = true;
                }
            }
        }
    }
}