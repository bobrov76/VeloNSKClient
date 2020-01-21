using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.HelpClass.Connected;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoMaraphonPage : ContentPage
    {
        private const string APP_PATH = "http://90.189.158.10";
        private static string token;
        ConnectClass connectClass = new ConnectClass();
        public InfoMaraphonPage()
        {
            InitializeComponent();

            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы            
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };


            Login_Button.Clicked += Login_Button_Clicked;            
        }
        public async Task Connect_ErrorAsync() { await Navigation.PopModalAsync(); } //Переход на страницу с ошибкой интернет соединения

        private void Login_Button_Clicked(object sender, EventArgs e)
        {
            Dictionary<string, string> tokenDictionary = GetTokenDictionary(Login_Entry.Text, Password_Entry.Text);
            if (tokenDictionary["access_token"] == "") { DisplayAlert("", "ну ты и дебил", "ok"); }
            else { token = tokenDictionary["access_token"]; }            
            DisplayAlert("", token, "ok");
            DisplayAlert("", GetUserInfo(token), "ok");
            //var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string url = "http://90.189.158.10/ImagesGaleri/1.jpg";

            //try
            //{
            //    WebClient client = new WebClient();
            //    client.DownloadFileAsync(new Uri(url), documents + @"\qqqwww.jpg");//  client.DownloadFile(url, filepath);

            //}
            //catch (Exception e1)
            //{
            //    DisplayAlert("",e1.Message.ToString(),"ok");   
            //}
        }

        // получение токена
        static Dictionary<string, string> GetTokenDictionary(string userName, string password)
        {
            try
            {

                var pairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>( "grant_type", "password"),
                    new KeyValuePair<string, string>( "username", userName ),
                    new KeyValuePair<string, string> ( "password", password )
                };
                var content = new FormUrlEncodedContent(pairs);

                using (var client = new HttpClient())
                {
                    var response =
                        client.PostAsync(APP_PATH + "/token", content).Result;
                    var result = response.Content.ReadAsStringAsync().Result;

                    // Десериализация полученного JSON-объекта
                    Dictionary<string, string> tokenDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                    return tokenDictionary;
                }
            }
            catch {
                Dictionary<string, string> countries = new Dictionary<string, string>(1);
                countries.Add("access_token", "");
                return countries;
            }

        }

        // создаем http-клиента с токеном 
        static HttpClient CreateClient(string accessToken = "")
        {
            var client = new HttpClient();
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }
            return client;
        }

        // получаем информацию о клиенте 
        static string GetUserInfo(string token)
        {
            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(APP_PATH + "/api/LoginUsers/getlogin").Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        // обращаемся по маршруту api/values 
        static string GetValues(string token)
        {
            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(APP_PATH + "/api/LoginUsers/getrole").Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }       
    }
}