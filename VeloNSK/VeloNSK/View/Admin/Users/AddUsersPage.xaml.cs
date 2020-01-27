using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise;
using VeloNSK.APIServise.Model;
using VeloNSK.APIServise.Servise;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using VeloNSK.HelpClass.Validate;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK.View.Admin.Users
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddUsersPage : ContentPage
    {
        private links picture_lincs = new links();
        private ConnectClass connectClass = new ConnectClass();
        private RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        private GetClientServise getClientServise = new GetClientServise();
        private string logins, emails, passwords;
        private IEnumerable<InfoUser> info;
        private Lincs server_lincs = new Lincs();
        private Animations animations = new Animations();
        private Get_Password get_Password = new Get_Password();
        private RegularValidate validation = new RegularValidate();
        private MediaFile _mediaFile;
        private IEnumerable<InfoUser> select_user;
        private Hash hash = new Hash();
        private bool animate;
        private int IDUser;
        private bool pol;
        private string RolUser;
        private Int16 user_hals;
        private Picker hels_Picker;

        public AddUsersPage(int id, string status_form)
        {
            InitializeComponent();

            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };
            get_users(id);
            GetPicerAsync();

            if (id == 0 && status_form == "MainPage" || status_form == "Redacting")
            {
                Rol_Picker.IsEnabled = false;
                RolUser = "User";
            }

            Pol_Picer.SelectedIndex = 1;
            Rol_Picker.SelectedIndex = 1;
            pol = true;

            User_Image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "nophotouser.png");
            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());
            //Users_Fon_Images.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "UserFon.png");
            password_status_image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "anvisible_password.png");//Инициализация статуса пароля
            password_status_image.Clicked += (s, e) => { Password_IsVisible(); };

            Pol_Picer.SelectedIndexChanged += (s, e) =>
            {
                if (Pol_Picer.Items[Pol_Picer.SelectedIndex].ToString() == "Мужской") { pol = true; }
                else { pol = false; }
            };
            Rol_Picker.SelectedIndexChanged += (s, e) =>
            {
                if (Rol_Picker.Items[Rol_Picker.SelectedIndex].ToString() == "Администратор") { RolUser = "Admin"; }
                else { RolUser = "User"; }
            };

            Registrations_Button.Clicked += async (s, e) =>
            {
                if (id == 0)
                {
                    await reg_userAsync(status_form);
                }
                else
                {
                    await update_userAsync(id);
                }
            };

            Save_Picture_Button.Clicked += async (s, e) =>
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    await getPhotoingaleriAsync();
                }
                else
                {
                    if (await DisplayAlert("", "Сделать снимок ?", "Да", "Нет")) await takePhotoAsync();
                    else await getPhotoingaleriAsync();
                }
            };

            Back_Button.Clicked += async (s, e) =>
            {
                //animations.Animations_Button(Back_Button);
                //await Task.Delay(1000);
                await Navigation.PopModalAsync();//Переход назад
            };
            Generate_Password_CheckBox.CheckedChanged += (s, e) =>
            {
                if (!Generate_Password_CheckBox.IsChecked)
                {
                    Password_Entry.Text = "";
                    Password_Replay_Entry.Text = "";
                }
                else
                {
                    string password = get_Password.GetPassword();
                    Password_Entry.Text = password;
                    Password_Replay_Entry.Text = password;
                }
            };
            Login_Entry.TextChanged += async (s, e) =>
            {
                if (Login_Entry.Text.Length == 17 && id == 0)
                {
                    IEnumerable<InfoUser> info = await registrationUsersService.Get_user();
                    var get = info.FirstOrDefault(x => x.Login == Login_Entry.Text);
                    if (get != null)
                    {
                        Error_Login_RowDefinition.Height = 40;
                        Error_Login_Lable.Text = "Пользоватьль с таким логином уже существует";
                    }
                    else
                    {
                        Error_Login_RowDefinition.Height = 0;
                        Error_Login_Lable.Text = "";
                    }
                }
            };
            Fam_Entry.TextChanged += (s, e) =>
            {
                if (Fam_Entry.Text != null && Fam_Entry.Text.Length == 1)
                {
                    Fam_Entry.Text = Fam_Entry.Text.Substring(0, 1).ToUpper() + Fam_Entry.Text.Substring(1);
                }
            };
            Name_Entry.TextChanged += (s, e) =>
            {
                if (Name_Entry.Text != null && Name_Entry.Text.Length == 1)
                {
                    Name_Entry.Text = Name_Entry.Text.Substring(0, 1).ToUpper() + Name_Entry.Text.Substring(1);
                }
            };
            Patronymic_Entry.TextChanged += (s, e) =>
            {
                if (Patronymic_Entry.Text != null && Patronymic_Entry.Text.Length == 1)
                {
                    Patronymic_Entry.Text = Patronymic_Entry.Text.Substring(0, 1).ToUpper() + Patronymic_Entry.Text.Substring(1);
                }
            };
            Yars_Entry.TextChanged += (s, e) =>
            {
                if (!Int32.TryParse(Yars_Entry.Text, out int num))
                {
                    Yars_Entry.Text = "";
                }
            };
            Password_Entry.TextChanged += (s, e) =>
            {
            };
            Password_Replay_Entry.TextChanged += (s, e) =>
            {
            };
        }

        private async Task GetPicerAsync()
        {
            IEnumerable<UserHelth> userHelths = await registrationUsersService.get_hels_status();
            hels_Picker = new Picker { Margin = new Thickness(60, -15, 10, 0) };
            foreach (var item in userHelths)
            {
                hels_Picker.Items.Add(item.NameHealth);
            }
            hels_Picker.SelectedIndexChanged += (s, e) =>
            {
                var picker_list = userHelths.FirstOrDefault(x => x.NameHealth == hels_Picker.Items[hels_Picker.SelectedIndex].ToString());
                user_hals = picker_list.IdHealth;
            };
            Main_Grid.Children.Add(hels_Picker, 1, 13);
            hels_Picker.SelectedIndex = 1;
        }

        private bool status_password = false;

        private void Password_IsVisible()
        {
            if (status_password == false)
            {
                password_status_image.Source = ImageSource.FromResource("VeloNSK.Resours.visible_passwprd.png");
                Password_Entry.IsPassword = false;
                Password_Replay_Entry.IsPassword = false;
                status_password = true;
            }
            else
            {
                password_status_image.Source = ImageSource.FromResource("VeloNSK.Resours.anvisible_password.png");
                Password_Entry.IsPassword = true;
                Password_Replay_Entry.IsPassword = true;
                status_password = false;
            }
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage());
        }

        private async Task get_users(int id)
        {
            InfoUser infoUsers = await registrationUsersService.Get_user_id(id);

            if (id != 0)
            {
                Head_Lable.Text = "Редактирование профиля";
                Grid_One.Height = 0;
                Grid_Two.Height = 0;
                Grid_Fore.Height = 0;
                Time_Lable_Picer.IsVisible = true;
                Rol_Picker.IsVisible = false;
                if (infoUsers.Rol == "Admin") { Time_Lable_Picer.Text = "Администратор"; }
                else { Time_Lable_Picer.Text = "Пользователь"; }
                pol = infoUsers.Isman;
                user_hals = infoUsers.IdHelth;
                Login_Entry.IsVisible = false;
                Login_Lable_Entry.IsVisible = true;
                Login_Lable_Entry.Text = infoUsers.Login.ToString();
                Email_Entry.IsVisible = false;
                Email_Lable_Entry.IsVisible = true;
                Email_Lable_Entry.Text = infoUsers.Email;
                RolUser = infoUsers.Rol;
                Name_Entry.Text = infoUsers.Name;
                Fam_Entry.Text = infoUsers.Fam;
                Patronymic_Entry.Text = infoUsers.Patronimic;
                Yars_Entry.Text = infoUsers.Years.ToString();
                User_Image.Source = new UriImageSource
                {
                    CachingEnabled = false,
                    Uri = new System.Uri(infoUsers.Logo)
                };
                if (infoUsers.Rol == "Admin") { Rol_Picker.SelectedIndex = 1; }
                else { Rol_Picker.SelectedIndex = 2; }
                if (infoUsers.Isman) { Pol_Picer.SelectedIndex = 1; }
                else { Pol_Picer.SelectedIndex = 2; }
                IEnumerable<UserHelth> userHelths = await registrationUsersService.get_hels_status();
                var picker_list = userHelths.FirstOrDefault(x => x.IdHealth == infoUsers.IdHelth);

                foreach (var item in hels_Picker.Items)
                {
                    if (hels_Picker.Items[hels_Picker.SelectedIndex].ToString() == picker_list.NameHealth) { hels_Picker.SelectedIndex = hels_Picker.SelectedIndex; }
                }
            }
        }

        private bool photo_status = false;

        // выбор фото
        private async Task getPhotoingaleriAsync()
        {
            try
            {
                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    _mediaFile = await CrossMedia.Current.PickPhotoAsync();
                    if (_mediaFile == null)
                    {
                        return;
                    }
                    User_Image.Source = ImageSource.FromStream(() => { return _mediaFile.GetStream(); });
                    photo_status = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // съемка фото
        private async Task takePhotoAsync()
        {
            try
            {
                await CrossMedia.Current.Initialize();
                if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                {
                    _mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        SaveToAlbum = true,
                        //Directory = "Sample",
                        Name = $"{DateTime.Now.ToString("ddMMyyyyhhmmss")}.jpg"
                    });
                    if (_mediaFile == null)
                        return;
                    User_Image.Source = ImageSource.FromStream(() => { return _mediaFile.GetStream(); });
                    photo_status = true;
                }
            }
            catch { await DisplayAlert("Предупреждение", "К сожалению в данный момент сьемка невозможна", "Ok"); }
        }

        public async Task update_userAsync(int id)
        {
            try
            {
                if (RolUser != null && Name_Entry.Text != null && Fam_Entry.Text != null && Patronymic_Entry.Text != null && Yars_Entry.Text != null)

                {
                    if (Convert.ToInt32(Yars_Entry.Text) >= 14)
                    {
                        Login_RowDefinition_One.Height = 0;
                        Main_RowDefinition_Activity.Height = 1000;
                        activityIndicator.IsRunning = true;
                        InfoUser infoUsers = await registrationUsersService.Get_user_id(id);
                        InfoUser infoUser;
                        if (photo_status)
                        {
                            string folder_name = Login_Entry.Text.Replace("+", string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
                            var content = new MultipartFormDataContent();
                            content.Add(new StreamContent(_mediaFile.GetStream()), "\"files\"", $"\"{_mediaFile.Path.Remove(0, (_mediaFile.Path.LastIndexOf(@"\")))}\"");
                            content.Add(new StringContent(folder_name), "\"Id\"");
                            var httpClient = new HttpClient();
                            var servere_adres = "http://90.189.158.10/api/Folder/";
                            var httpResponseMasage = await httpClient.PostAsync(servere_adres, content);
                            var url_image = await httpResponseMasage.Content.ReadAsStringAsync();
                            infoUser = new InfoUser
                            {
                                IdUsers = id,
                                Password = infoUsers.Password,
                                Email = Email_Lable_Entry.Text,
                                Login = Login_Lable_Entry.Text,
                                Rol = RolUser,
                                Name = Name_Entry.Text,
                                Fam = Fam_Entry.Text,
                                Patronimic = Patronymic_Entry.Text,
                                Years = Convert.ToInt16(Yars_Entry.Text),
                                Isman = pol,
                                IdHelth = user_hals,
                                Logo = url_image
                            };
                        }
                        else
                        {
                            infoUser = new InfoUser
                            {
                                IdUsers = id,
                                Password = infoUsers.Password,
                                Email = Email_Lable_Entry.Text,
                                Login = Login_Lable_Entry.Text,
                                Rol = RolUser,
                                Name = Name_Entry.Text,
                                Fam = Fam_Entry.Text,
                                Patronimic = Patronymic_Entry.Text,
                                Years = Convert.ToInt16(Yars_Entry.Text),
                                Isman = pol,
                                IdHelth = user_hals,
                                Logo = infoUsers.Logo
                            };
                        }
                        await registrationUsersService.Update(infoUser);
                        await Task.Delay(1000);
                        Login_RowDefinition_One.Height = 1000;
                        Main_RowDefinition_Activity.Height = 0;
                        activityIndicator.IsRunning = false;
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await DisplayAlert("Предупреждение", "Минимальный возраст для регистации 14 лет", "ОK");
                    }
                }
                else
                {
                    if (!await DisplayAlert("Ошибка", "Вы заполнили не все поля", "Заполнить", "Выйти")) { await Navigation.PopModalAsync(); }
                }
            }
            catch { }
        }

        public async Task reg_userAsync(string status_form)
        {
            try
            {
                if (Password_Entry.Text == Password_Replay_Entry.Text)
                {
                    if (Login_Entry.Text != null && Password_Entry.Text != null && RolUser != null && Name_Entry.Text != null &&
                                   Fam_Entry.Text != null && Patronymic_Entry.Text != null && Yars_Entry.Text != null &&
                                   Email_Entry.Text != null)
                    {
                        if (Convert.ToInt32(Yars_Entry.Text) >= 14)
                        {
                            if (_mediaFile != null)
                            {
                                if (!validation.Vadidation(Email_Entry.Text, @"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)"))
                                {
                                    Error_Email_RowDefinition.Height = 40;
                                    Error_Email_Lable.Text = "E-mail не соответствует требованию";
                                }
                                else
                                {
                                    Login_RowDefinition_One.Height = 0;
                                    Main_RowDefinition_Activity.Height = 1000;
                                    activityIndicator.IsRunning = true;
                                    Error_Email_RowDefinition.Height = 0;
                                    Error_Email_Lable.Text = "";

                                    string folder_name = Login_Entry.Text.Replace("+", string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);

                                    using (var client = getClientServise.GetClient())
                                    {
                                        string result = await client.GetStringAsync("http://90.189.158.10/api/Folder/" + folder_name);
                                        string masage = result;
                                    }

                                    var content = new MultipartFormDataContent();
                                    string patchs = "";
                                    if (Device.RuntimePlatform == Device.Android) { patchs = _mediaFile.Path.Remove(0, (_mediaFile.Path.LastIndexOf("/"))); }
                                    else if (Device.RuntimePlatform == Device.UWP) { patchs = _mediaFile.Path.Remove(0, (_mediaFile.Path.LastIndexOf(@"\"))); }
                                    content.Add(new StreamContent(_mediaFile.GetStream()), "\"files\"", $"\"{patchs}\"");
                                    content.Add(new StringContent(folder_name), "\"Id\"");
                                    var httpClient = new HttpClient();

                                    var servere_adres = "http://90.189.158.10/api/Folder/";
                                    var httpResponseMasage = await httpClient.PostAsync(servere_adres, content);
                                    var url_image = await httpResponseMasage.Content.ReadAsStringAsync();
                                    string Hach = hash.GetHash(Password_Entry.Text);
                                    InfoUser infoUser = new InfoUser
                                    {
                                        Login = Login_Entry.Text,
                                        Password = Hach,
                                        Rol = RolUser,
                                        Name = Name_Entry.Text,
                                        Fam = Fam_Entry.Text,
                                        Patronimic = Patronymic_Entry.Text,
                                        Years = Convert.ToInt16(Yars_Entry.Text),
                                        Email = Email_Entry.Text,
                                        Isman = pol,
                                        IdHelth = user_hals,
                                        Logo = url_image
                                    };
                                    using (var client = getClientServise.GetClient())
                                    {
                                        var response = client.PostAsJsonAsync("http://90.189.158.10/api/UserInfoes/", infoUser).Result;
                                        var masage = JsonConvert.DeserializeObject<InfoUser>(await response.Content.ReadAsStringAsync());
                                        if (status_form == "MainPage")
                                        {
                                            await Task.Delay(1000);
                                            Login_RowDefinition_One.Height = 1000;
                                            Main_RowDefinition_Activity.Height = 0;
                                            activityIndicator.IsRunning = false;
                                            await DisplayAlert("Успешная регистрация", "Вы успешно зарегистрованы", "Ok");
                                            await Navigation.PushModalAsync(new LoginPage());
                                        }
                                        if (status_form == "Admin")
                                        {
                                            await Task.Delay(1000);
                                            Login_RowDefinition_One.Height = 1000;
                                            Main_RowDefinition_Activity.Height = 0;
                                            activityIndicator.IsRunning = false;
                                            if (await DisplayAlert("", "Зарегистрировать еще одного пользователя?", "Да", "Нет"))
                                            {
                                                Login_Entry.Text = null;
                                                Password_Entry.Text = null;
                                                RolUser = null;
                                                Name_Entry.Text = null;
                                                Fam_Entry.Text = null;
                                                Patronymic_Entry.Text = null;
                                                Yars_Entry.Text = null;
                                                Email_Entry.Text = null;
                                                url_image = null;
                                            }
                                            else { await Navigation.PopModalAsync(); }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                await DisplayAlert("Предупреждение", "Пожалуйста выберите изображение", "ОK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Предупреждение", "Минимальный возраст для регистации 14 лет", "ОK");
                        }
                    }
                    else
                    {
                        if (!await DisplayAlert("Ошибка", "Вы заполнили не все поля", "Заполнить", "Выйти")) { await Navigation.PopModalAsync(); }
                    }
                }
                else
                {
                    await DisplayAlert("Предупреждение", "Пароли не совпадают", "ОK");
                }
            }
            catch { }
        }

        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();

        private new void SizeChanged(object sender, EventArgs e) //Стилизация
        {
            if (size_form.GetHeightSize() > size_form.GetWidthSize())
            {
                Login_ColumnDefinition_Ziro.Width = 5;
                Login_ColumnDefinition_One.Width = new GridLength(1, GridUnitType.Star);
                Login_ColumnDefinition_Two.Width = 5;
            }
            else
            {
                Login_ColumnDefinition_Ziro.Width = new GridLength(1, GridUnitType.Star);
                Login_ColumnDefinition_One.Width = 600;
                Login_ColumnDefinition_Two.Width = new GridLength(1, GridUnitType.Star);
            }
        }
    }
}