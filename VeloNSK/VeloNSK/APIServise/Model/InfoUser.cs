using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VeloNSK.APIServise.Model
{
    class InfoUser
    {
        public int IdUsers { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
        public string Jtoken { get; set; }
        public string Name { get; set; }
        public string Fam { get; set; }
        public string Patronimic { get; set; }
        public short Years { get; set; }
        public string Logo { get; set; }
        public string Email { get; set; }
        public bool Isman { get; set; }
        public short IdHelth { get; set; }        
    }
}
