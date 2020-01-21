using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise.Model;

namespace VeloNSK.APIServise.Servise
{
    class LoginUsersService
    {
        GetClientServise getClientServise = new GetClientServise();
        Lincs server_lincs = new Lincs();

        // получаем информацию о клиенте 
        public async Task<InfoUser> Get(string token)
        {
            HttpClient client = getClientServise.CreateClient(token);
            string result = await client.GetStringAsync("http://90.189.158.10/api/UserInfoes/info");
            var itog = JsonConvert.DeserializeObject<InfoUser>(result);
            return itog;
        }

    }
}
