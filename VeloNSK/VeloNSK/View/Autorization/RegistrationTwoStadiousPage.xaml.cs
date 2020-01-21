using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise.Model;
using VeloNSK.APIServise.Servise;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationTwoStadiousPage : ContentPage
    {
        links picture_lincs = new links();
        ConnectClass connectClass = new ConnectClass();
        RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        GetClientServise getClientServise = new GetClientServise();
        string logins, emails, passwords;      
        string user_hals;
        string[] hels_Array = new string[3];
        bool pol;
        private MediaFile _mediaFile;
       
        public RegistrationTwoStadiousPage(string login, string email, string password)
        {
            logins = login;
            passwords = password;
            emails = email;
            InitializeComponent();

            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы            
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            
            Fon.BackgroundImageSource = ImageSource.FromResource(picture_lincs.GetFon());
            User_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "nophotouser.png");
            hels_Picker.Items.Add("1");
            get_hels();
            //Status_hels_Picker.SelectedIndexChanged += async (s, e) => 
            //{
            //    IEnumerable<UserHelth> userHelths = await registrationUsersService.get_hels_status();
            //    var picker_list = userHelths.FirstOrDefault(x => x.NameHealth== Status_hels_Picker.Items[Status_hels_Picker.SelectedIndex].ToString());
            //    user_hals = picker_list.IdHealth.ToString();
            //};

            Pol_Picker.SelectedIndexChanged += (s, e) => 
            {
                if (Pol_Picker.Items[Pol_Picker.SelectedIndex].ToString() == "Мужской") { pol = true; }
                else { pol = false; }
            };

            Registrations_Button.Clicked += async(s, e) => { await reg_userAsync(); }; 
            
            Save_Picture_Button.Clicked += async(s, e) => 
            {
                if (await DisplayAlert("", "Сделать снимок ?", "Да", "Нет")) await takePhotoAsync();
                else await getPhotoingaleriAsync();       
            };


           
        }
        public async Task Connect_ErrorAsync() { await Navigation.PushModalAsync(new ErrorConnectPage()); }

        private async Task get_hels() 
        { 
            IEnumerable<UserHelth> userHelths = await registrationUsersService.get_hels_status();
            foreach (UserHelth getpicer in userHelths)
            {
                hels_Picker.Items.Add(getpicer.NameHealth.ToString());             
            }            
        }
        // выбор фото
        private async Task getPhotoingaleriAsync()
        {
                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    _mediaFile= await CrossMedia.Current.PickPhotoAsync();
                    if (_mediaFile==null)
                    {
                        return;
                    }                    
                    User_Image.Source = ImageSource.FromStream(() => { return _mediaFile.GetStream(); }); 
                }
        }
        // съемка фото
        private async Task takePhotoAsync()   
        {
                await CrossMedia.Current.Initialize();
                if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                {
                    _mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        SaveToAlbum = true,
                        Directory = "Sample",
                        Name = $"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.jpg"
                    });
                    if (_mediaFile == null)
                        return;                    
                    User_Image.Source = ImageSource.FromStream(() => { return _mediaFile.GetStream(); });
                await DisplayAlert("", _mediaFile.Path.Remove(0, (_mediaFile.Path.LastIndexOf(@"\"))) , "Ok");
                }   
        }
       
        public async Task reg_userAsync()
        {
           string ID_User = "";
            

            InfoUser infoUser = new InfoUser
            {                
                 Login =logins,
                 Password=passwords,
                 Rol="User",
                 Name = Name_Entry.Text, 
                 Fam =Fam_Entry.Text,
                 Patronimic = Patronymic_Entry.Text,
                 Years =17,
                 Email = emails,
                 Isman = true,
                 IdHelth = 1
            };           
            using (var client = getClientServise.GetClient())
            {
                var response = client.PostAsJsonAsync("http://90.189.158.10/api/UserInfoes/", infoUser).Result;
                var masage = JsonConvert.DeserializeObject<InfoUser>(await response.Content.ReadAsStringAsync());
                ID_User = masage.IdUsers.ToString();
                await DisplayAlert("", ID_User , "Ok");
            }

            using (var client = getClientServise.GetClient())
            {
                await DisplayAlert("", infoUser.IdUsers.ToString(), "Ok");
                string result = await client.GetStringAsync("http://90.189.158.10/api/Folder/"+ID_User);
                string masage = result;
                await DisplayAlert("", masage, "Ok");
            }           

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(_mediaFile.GetStream()), "\"files\"", $"\"{_mediaFile.Path.Remove(0, (_mediaFile.Path.LastIndexOf(@"\")))}\"");
            content.Add(new StringContent(ID_User), "\"Id\"");
            var httpClient = new HttpClient();
            var servere_adres = "http://90.189.158.10/api/Folder/";
            var httpResponseMasage = await httpClient.PostAsync(servere_adres, content);
            var url_image = await httpResponseMasage.Content.ReadAsStringAsync();
            await DisplayAlert("", url_image.ToString(), "Ok");

            InfoUser Url_logo = new InfoUser
            {
                Logo =url_image,
            };
            //using (var client = getClientServise.GetClient())
            //{
            //    var response = await client.PutAsync("http://90.189.158.10/api/UserInfoes/" + ID_User,
            //    new StringContent(JsonConvert.SerializeObject(Url_logo)));

            //    var aa = JsonConvert.DeserializeObject<InfoUser>(await response.Content.ReadAsStringAsync());
            //    await DisplayAlert("", aa.Logo.ToString(), "Ok");
            //}
           
        }
    }
}