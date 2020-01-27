using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using VeloNSK.View.Admin.Users;
using Xamarin.Forms;

namespace VeloNSK
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private links picture_lincs = new links();
        private Lincs server_lincs = new Lincs();
        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();
        private int counter = 0, counter1 = 1, counter2 = 2, counter3 = 3, counter4 = 4, counter5 = 5;
        private string[] image_string = new string[11];
        private ConnectClass connectClass = new ConnectClass();
        private Animations animations = new Animations();
        private bool animate;
        private HttpClient _client;
        private bool alive = true;

        public MainPage()
        {
            InitializeComponent();
            if (!App.Current.Properties.TryGetValue("Login", out object Login))
            {
                App.Current.Properties.Add("Login", "");
            }

            if (!App.Current.Properties.TryGetValue("Password", out object Password))
            {
                App.Current.Properties.Add("Password", "");
            }
            if (!App.Current.Properties.TryGetValue("token", out object token))
            {
                App.Current.Properties.Add("token", "");
            }
            if (!App.Current.Properties.TryGetValue("pin_code", out object pin_code))
            {
                App.Current.Properties.Add("pin_code", "");
            }
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += async (s, e) =>
            {
                if (!connectClass.CheckConnection()) await Connect_ErrorAsync();
                else GetImageListAsync();
            };

            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());

            Head_Image.Source = ImageSource.FromResource(picture_lincs.GetLogo());

            _client = new HttpClient();
            //Device.StartTimer(TimeSpan.FromSeconds(20), OnTimerTick);
            Slider_Left_Bt.Clicked += (s, e) => Slider_Left_Function();
            Slider_Right_Bt.Clicked += (s, e) => Slider_Right_Function();
            Head_Button.Clicked += async (s, e) =>
            {
                alive = false;
                await LoginAsync();
            };
            Block_Button_One.Clicked += async (s, e) =>
            {
                alive = false;
                await Navigation.PushModalAsync(new AddUsersPage(0, "MainPage"), animate);
            };
            Block_Button_Tho.Clicked += async (s, e) =>
            {
                alive = false;
                await LoginAsync();
            };
            Block_Button_Three.Clicked += async (s, e) =>
            {
                alive = false;
                await Navigation.PushModalAsync(new InfoMemuPage(), animate);
            };

            GetImageListAsync();
        }

        //private bool OnTimerTick()
        //{
        //    Slider_Right_Function();
        //    return alive;
        //}

        private async void GetImageListAsync()
        {
            try
            {
                string requestUri = "http://90.189.158.10/api/Images";
                string result = await _client.GetStringAsync(requestUri);
                var result_msg = JsonConvert.DeserializeObject<string[]>(result);
                image_string = result_msg;
                img();
            }
            catch { }
        }

        public async Task LoginAsync()
        {
            string Login = App.Current.Properties["Login"].ToString();
            string Password = App.Current.Properties["Password"].ToString();
            if (Login != "" && Password != "")
            {
                await Navigation.PushModalAsync(new PinLoginPage());
            }
            else
            {
                await Navigation.PushModalAsync(new LoginPage());
            }
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage());
        }

        private new void SizeChanged(object sender, EventArgs e)
        {
            if (size_form.GetHeightSize() < 800) Main_RowDefinition_Three.Height = 0;
            if (size_form.GetHeightSize() > 800) Main_RowDefinition_Three.Height = new GridLength(1, GridUnitType.Star);
            if (size_form.GetHeightSize() < 700) Main_RowDefinition_Fore.Height = 0;
            if (size_form.GetHeightSize() > 700) Main_RowDefinition_Fore.Height = 200;
            if (size_form.GetHeightSize() < 600) Main_RowDefinition_One.Height = 0;
            if (size_form.GetHeightSize() > 600) Main_RowDefinition_One.Height = new GridLength(1, GridUnitType.Star);
        }

        private void img()
        {
            animated();
            Slid0.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new Uri(image_string[image_string.Length - 5])
            };

            Slid1.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[image_string.Length - 4])
            };

            Slid2.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[image_string.Length - 3])
            };

            Slid3.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[image_string.Length - 2])
            };

            Slid4.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[image_string.Length - 1])
            };

            Slid5.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[image_string.Length - 1])
            };
        }

        private async void Flip(Image imageOne)
        {
            imageOne.Opacity = 0;
            await imageOne.FadeTo(1, 2000);
            uint timeout = 5000;
            imageOne.RotationY = -360;
            await imageOne.RotateYTo(-180, timeout, Easing.SpringOut);
        }

        private void animated()
        {
            Flip(Slid0);
            Flip(Slid1);
            Flip(Slid2);
            Flip(Slid3);
            Flip(Slid4);
            Flip(Slid5);
        }

        private void Slider_Right_Function() //Перелистывание слайдера на право
        {
            if (counter >= image_string.Length) counter = 0;

            Slid0.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[counter++])
            };

            if (counter1 >= image_string.Length) counter1 = 0;

            Slid1.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[counter1++])
            };

            if (counter2 >= image_string.Length) counter2 = 0;
            Slid2.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[counter2++])
            };

            if (counter3 >= image_string.Length) counter3 = 0;
            Slid3.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[counter3++])
            };

            if (counter4 >= image_string.Length) counter4 = 0;
            Slid4.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[counter4++])
            };

            if (counter5 >= image_string.Length) counter5 = 0;
            Slid5.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[counter5++])
            };
            animated();
        }

        private void Slider_Left_Function()//Перелистывание слайдера на лево
        {
            if (counter <= 0) counter = image_string.Length;

            Slid0.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[--counter])
            };

            Slid1.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[--counter1])
            };
            if (counter1 <= 0) counter1 = image_string.Length;

            Slid2.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[--counter2])
            };
            if (counter2 <= 0) counter2 = image_string.Length;

            Slid3.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[--counter3])
            };

            if (counter3 <= 0) counter3 = image_string.Length;
            Slid4.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[--counter4])
            };

            if (counter4 <= 0) counter4 = image_string.Length;

            Slid5.Source = new UriImageSource
            {
                CachingEnabled = true,
                CacheValidity = new System.TimeSpan(2, 0, 0, 0),
                Uri = new System.Uri(image_string[--counter5])
            };
            if (counter5 <= 0) counter5 = image_string.Length;
            animated();
        }
    }
}