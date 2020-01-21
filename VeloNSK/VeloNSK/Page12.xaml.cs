using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise.Model;
using VeloNSK.APIServise.Servise;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page12 : ContentPage
    {
        private Label header;
        private Picker picker;
        private RegistrationUsersService registrationUsersService = new RegistrationUsersService();
        private IEnumerable<InfoUser> infoUsers;

        public Page12()
        {
            InitializeComponent();
            SelectedwsProrerty();
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            header.Text = "Вы выбрали: " + picker.Items[picker.SelectedIndex];
        }

        private async Task SelectedwsProrerty()
        {
            infoUsers = await registrationUsersService.Get_user();
            header = new Label
            {
                Text = "Выберите язык",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
            };

            picker = new Picker
            {
                Title = "Язык"
            };
            foreach (var item in infoUsers)
            {
                picker.Items.Add(item.IdUsers.ToString());
            }

            picker.SelectedIndexChanged += picker_SelectedIndexChanged;

            this.Content = new StackLayout { Children = { header, picker } };
        }
    }
}