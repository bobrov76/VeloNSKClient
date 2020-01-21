using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace VeloNSK.HelpClass.Validate
{
    class Hash
    {        
        public string GetHash(string input)
        { 
            return Convert.ToBase64String(SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(input)));
        }
    }
}
