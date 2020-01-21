using Newtonsoft.Json;
using Plugin.Connectivity;
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
using VeloNSK.HelpClass.Validate;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationOneStadiousPage : ContentPage
    {
        RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        links picture_lincs = new links();
        Get_Password get_Password = new Get_Password();
        ConnectClass connectClass = new ConnectClass();
        RegularValidate validation = new RegularValidate();
        Hash hash_password = new Hash();
        IEnumerable<InfoUser> infoUsers;
        public RegistrationOneStadiousPage()
        {
            InitializeComponent();

            Get_User();

            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы            
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };


            Fon.BackgroundImageSource = ImageSource.FromResource(picture_lincs.GetFon());//Установка фона
            password_status_image.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "anvisible_password.png");//Инициализация статуса пароля

            Back_Button.Clicked += async (s, e) => await Navigation.PopModalAsync();//Переход назад 
            Registrations_Button.Clicked += async (s, e) =>//переход на 2 стадию регистрации
            {
                if (Login_Entry.Text != null && Email_Entry.Text != null && Password_Entry.Text != null)
                {
                    if (!validation.Vadidation(Email_Entry.Text, @"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)"))
                    {
                        Error_Email_RowDefinition.Height = 40;
                        Error_Email_Lable.Text = "E-mail не соответствует требованию";
                    }
                    else
                    {
                        Error_Email_RowDefinition.Height = 0;
                        Error_Email_Lable.Text = "";
                        bool animated = true;
                        await Navigation.PushModalAsync(new RegistrationTwoStadiousPage(Login_Entry.Text, Email_Entry.Text, Password_Entry.Text), animated);
                    }
                }
                else { if (!await DisplayAlert("Ошибка", "Вы заполнили не все поля", "Заполнить", "Выйти")) { await Navigation.PopModalAsync(); } }                             
            };
            Generate_Password_CheckBox.CheckedChanged += (s, e) => //событие автогенерации пароля
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

            Login_Entry.TextChanged += (s, e) =>
            {
                if (Login_Entry.Text.Length == 17)
                {
                   // var get = infoUsers.FirstOrDefault(x => x.Login == Login_Entry.Text);
                    //if (get != null)
                    //{
                    //    Error_Login_RowDefinition.Height = 40;
                    //    Error_Login_Lable.Text = "Пользоватьль с таким логином уже существует";
                    //}
                    //else
                    //{
                    //    Error_Login_RowDefinition.Height = 0;
                    //    Error_Login_Lable.Text = "";
                    //}
                }
            };
            //Email_Entry.TextChanged += (s, e) => 
            //{
            //    if (Login_Entry.Text != null)
            //    {
            //        if (Login_Entry.Text.Length > 5)
            //        {
            //            var get = infoUsers.FirstOrDefault(x => x.Email == Email_Entry.Text);
            //            if (get != null)
            //            {
            //                Error_Email_RowDefinition.Height = 40;
            //                Error_Login_Lable.Text = "Пользоватьль с таким E-mail уже существует";
            //            }
            //            else
            //            {
            //                Error_Email_RowDefinition.Height = 0;
            //                Error_Login_Lable.Text = "";
            //            }
            //        }
            //    }
            //};



        }
        public async Task Get_User() { infoUsers = await registrationUsersService.Get_user(); }
        public async Task Connect_ErrorAsync() { await Navigation.PushModalAsync(new ErrorConnectPage()); }
        bool status_password = false;
        private void Password_IsVisible(object sender, EventArgs e)
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
        
        
        
        
    }
}