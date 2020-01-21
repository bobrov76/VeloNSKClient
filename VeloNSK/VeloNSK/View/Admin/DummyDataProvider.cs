using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using VeloNSK.APIServise.Model;
using VeloNSK.APIServise.Servise;

namespace VeloNSK.View.Admin
{
    
    class DummyDataProvider
    {
        static GetClientServise gets = new GetClientServise();
        // получаем информацию о пользователе
        public static async System.Threading.Tasks.Task<List<InfoUser>> GetTeams()
        {
            HttpClient client = gets.GetClient(); 
            string result = await client.GetStringAsync("http://90.189.158.10/api/UserInfoes/");
            return JsonConvert.DeserializeObject<List<InfoUser>>(result);
        }
    }
}
