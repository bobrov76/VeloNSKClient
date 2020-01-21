using System.Text.RegularExpressions;

namespace VeloNSK.HelpClass.Validate
{
    class RegularValidate
    {
        public bool Vadidation(string password, string regex)
        {
            if (password != "" && Regex.IsMatch(password, regex)) { return true; }
            else { return false; }
        }
    }
}
