﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VeloNSK.View.Admin.ResultParticipation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddResultParticipationPagePage : ContentPage
    {
        public AddResultParticipationPagePage(int ID)
        {
            InitializeComponent();
        }
    }
}