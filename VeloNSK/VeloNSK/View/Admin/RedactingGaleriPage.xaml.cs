using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.Media;
using Plugin.Media.Abstractions;
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

namespace VeloNSK.View.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    
    public partial class RedactingGaleriPage : ContentPage
    {
        HttpClient _client;
        links picture_lincs = new links();
        ConnectClass connectClass = new ConnectClass();
        private MediaFile _mediaFile;
        string[] images;
        public RedactingGaleriPage()
        {
            InitializeComponent();
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы            
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            Fon.BackgroundImageSource = ImageSource.FromResource(picture_lincs.GetFon());
            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());


            Save_More_Button.Clicked += async (s, e) =>
            {
                if (images.Length!=0)
                {
                    for (int i = 0; i < images.Length; i++)
                    {
                        await DownloadAndSaveImage(images[i]);
                    }
                }               

            };
            Head_Button.Clicked += async (s, e) => { await Navigation.PopModalAsync(); };
            Save_NewPicture_Button.Clicked += async (s, e) => { await getPhotoingaleriAsync(); };
            _client = new HttpClient(); 
            OnAppearing();
        }
        public async Task Connect_ErrorAsync() { await Navigation.PopModalAsync(); } //Переход на страницу с ошибкой интернет соединения


        // выбор фото
        private async Task getPhotoingaleriAsync()
        {
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                _mediaFile = await CrossMedia.Current.PickPhotoAsync();
                if (_mediaFile == null)
                {
                    return;
                }                 

                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(_mediaFile.GetStream()), "\"files\"", $"\"{_mediaFile.Path.Remove(0, (_mediaFile.Path.LastIndexOf(@"\")))}\"");
                content.Add(new StringContent(""), "\"Id\"");
                var httpClient = new HttpClient();
                var servere_adres = "http://90.189.158.10/api/Folder/galeri";
                var httpResponseMasage = await httpClient.PostAsync(servere_adres, content);
                var url_image = await httpResponseMasage.Content.ReadAsStringAsync();
                OnAppearing();
            } 
        }



        public async void OnAppearing()
        {
            wrapLayout.Children.Clear();
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


        async Task<string[]> GetImageListAsync()
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