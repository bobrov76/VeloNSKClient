using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VeloNSK.HelpClass.Style
{
    class Animations
    {
        public async void Animations_Entry(Entry entry)//Анимация кнопки
        {
            uint timeout = 50;
            await entry.TranslateTo(-15, 0, timeout);
            await entry.TranslateTo(15, 0, timeout);
            await entry.TranslateTo(-10, 0, timeout);
            await entry.TranslateTo(10, 0, timeout);
            await entry.TranslateTo(-5, 0, timeout);
            await entry.TranslateTo(5, 0, timeout);
            entry.TranslationX = 0;
        }
        public async void Animations_Button(Button button)
        {
            await Task.Delay(200);
            await button.FadeTo(0, 250);
            await Task.Delay(200);
            await button.FadeTo(1, 250);        
        }
    }
}
