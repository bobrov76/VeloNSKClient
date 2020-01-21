using System;
using System.Collections.Generic;
using System.Text;

namespace VeloNSK.HelpClass.Validate
{
    class Get_Password
    {
        public string GetPassword()
        {
            string pass="";
            //генератор поролей
            var r = new Random();
            while (pass.Length < 9)
            {
                Char c = (char)r.Next(33, 125);
                if (Char.IsLetterOrDigit(c))
                    pass += c;
            }
            return pass.ToString();
        }
    }
}
