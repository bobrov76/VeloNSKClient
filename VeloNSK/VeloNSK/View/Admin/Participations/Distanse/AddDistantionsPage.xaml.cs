using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise;
using VeloNSK.APIServise.Model;
using VeloNSK.APIServise.Servise;
using VeloNSK.HelpClass.Connected;
using VeloNSK.HelpClass.Style;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK.View.Admin.Participations.Distanse
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddDistantionsPage : ContentPage
    {
        private links picture_lincs = new links();
        private Lincs server_lincs = new Lincs();
        private Animations animations = new Animations();
        private ConnectClass connectClass = new ConnectClass();
        private DistantionsServise distantionsServise = new DistantionsServise();

        public AddDistantionsPage(int id)
        {
            InitializeComponent();
            get_infa(id);
            if (!connectClass.CheckConnection()) { Connect_ErrorAsync(); }//Проверка интернета при загрузке формы
            CrossConnectivity.Current.ConnectivityChanged += (s, e) => { if (!connectClass.CheckConnection()) Connect_ErrorAsync(); };

            image_fon.Source = ImageSource.FromResource(picture_lincs.GetFon());//Устанавливаем фон
            Users_Fon_Images.Source = ImageSource.FromResource(picture_lincs.LinksResourse() + "UserFon.png");

            Back_Button.Clicked += async (s, e) =>
            {
                animations.Animations_Button(Back_Button);
                await Task.Delay(1000);
                await Navigation.PopModalAsync();//Переход назад
            };

            Lengh_Entry.TextChanged += async (s, e) =>
            {
                try
                {
                    decimal num;
                    if (!decimal.TryParse(Lengh_Entry.Text, out num)) Lengh_Entry.Text = null;
                    if (Lengh_Entry.Text.Length > 0)
                    {
                        await test_length();
                    }
                }
                catch { }
            };

            Name_Entry.TextChanged += async (s, e) =>
            {
                try
                {
                    if (Name_Entry.Text.Length > 2)
                    {
                        await test_distans();
                    }
                }
                catch { }
            };

            Registrations_Button.Clicked += async (s, e) =>
            {
                if (id != 0) { await Update(id); }
                else { await Criate(); }
            };
        }

        private async Task test_distans()
        {
            IEnumerable<Distantion> info = await distantionsServise.Get();
            var get = info.FirstOrDefault(x => x.NameDistantion == Name_Entry.Text);
            if (get != null)
            {
                Error_Distantion.Height = 40;
                Error_Distand_Lable.Text = "Такая дистанция уже существует";
                Registrations_Button.IsEnabled = false;
            }
            else
            {
                Error_Distantion.Height = 0;
                Error_Distand_Lable.Text = "";
                Registrations_Button.IsEnabled = true;
            }
        }

        private async Task test_length()
        {
            IEnumerable<Distantion> info = await distantionsServise.Get();
            var get = info.FirstOrDefault(x => x.Lengs == Convert.ToDecimal(Lengh_Entry.Text));
            if (get != null)
            {
                Error_Length.Height = 40;
                Error_Length_Lable.Text = "Дистанция с такой дистанцией уже существует";
                Registrations_Button.IsEnabled = false;
            }
            else
            {
                Error_Length.Height = 0;
                Error_Length_Lable.Text = "";
                Registrations_Button.IsEnabled = true;
            }
        }

        public async Task Connect_ErrorAsync()
        {
            await Navigation.PushModalAsync(new ErrorConnectPage());
        }

        private async Task get_infa(int id)
        {
            Distantion distantion = await distantionsServise.Get_ID(id);

            if (id != 0)
            {
                Head_Lable.Text = "Редактировать дистанцию";
                Name_Entry.Text = distantion.NameDistantion;
                Lengh_Entry.Text = distantion.Lengs.ToString();
                Discription_Editor.Text = distantion.Discriptions;
            }
        }

        public async Task Update(int id)
        {
            if (Name_Entry.Text != null && Lengh_Entry.Text != null)
            {
                decimal lenght = Convert.ToDecimal(Lengh_Entry.Text);
                if (Discription_Editor.Text == null || Discription_Editor.Text == "")
                {
                    Discription_Editor.Text = "Описание отсутствует";
                }
                Distantion distantion = new Distantion
                {
                    IdDistantion = id,
                    NameDistantion = Name_Entry.Text,
                    Lengs = lenght,
                    Discriptions = Discription_Editor.Text,
                };
                await distantionsServise.Update(distantion);
                await Navigation.PopModalAsync();
            }
            else
            {
                if (!await DisplayAlert("Ошибка", "Вы заполнили не все поля", "Заполнить", "Выйти")) { await Navigation.PopModalAsync(); }
            }
        }

        public async Task Criate()
        {
            if (Name_Entry.Text != null && Lengh_Entry.Text != null)
            {
                decimal lenght = Convert.ToDecimal(Lengh_Entry.Text);
                if (Discription_Editor.Text == null || Discription_Editor.Text == "")
                {
                    Discription_Editor.Text = "Описание отсутствует";
                }
                Distantion distantion = new Distantion
                {
                    NameDistantion = Name_Entry.Text,
                    Lengs = lenght,
                    Discriptions = Discription_Editor.Text,
                };
                await distantionsServise.Add(distantion);
                if (!await DisplayAlert("", "Добавить еще одну дистанцию", "Да", "Нет")) { await Navigation.PopModalAsync(); }
                else
                {
                    Name_Entry.Text = null;
                    Discription_Editor.Text = null;
                    Lengh_Entry.Text = null;
                }
            }
            else
            {
                if (!await DisplayAlert("Ошибка", "Вы заполнили не все поля", "Заполнить", "Выйти")) { await Navigation.PopModalAsync(); }
            }
        }

        private HelpClass.Style.Size size_form = new HelpClass.Style.Size();

        private new void SizeChanged(object sender, EventArgs e) //Стилизация
        {
            if (size_form.GetHeightSize() > size_form.GetWidthSize())
            {
                Login_ColumnDefinition_Ziro.Width = 20;
                Login_ColumnDefinition_One.Width = new GridLength(1, GridUnitType.Star);
                Login_ColumnDefinition_Two.Width = 20;
            }
            else
            {
                Login_ColumnDefinition_Ziro.Width = new GridLength(1, GridUnitType.Star);
                Login_ColumnDefinition_One.Width = 480;
                Login_ColumnDefinition_Two.Width = new GridLength(1, GridUnitType.Star);
            }
        }
    }
}