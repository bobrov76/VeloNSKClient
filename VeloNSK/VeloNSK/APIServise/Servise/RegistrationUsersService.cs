using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise.Model;


namespace VeloNSK.APIServise.Servise
{
    class RegistrationUsersService
    {
        GetClientServise getClientServise = new GetClientServise();
        static Lincs server_lincs = new Lincs();
        // добавляем пользователя  
       

            // получить все статусы
            public async Task<IEnumerable<UserHelth>> get_hels_status()
            {
                HttpClient client = getClientServise.GetClient();
                string result = await client.GetStringAsync("http://90.189.158.10/api/HelthStatus");
                return JsonConvert.DeserializeObject<IEnumerable<UserHelth>>(result);
            }

        // получаем информацию о пользователе
        public async Task<IEnumerable<InfoUser>> Get_user()
        {
            HttpClient client = getClientServise.GetClient();
            string result = await client.GetStringAsync("http://90.189.158.10/api/UserInfoes/");
            return JsonConvert.DeserializeObject<IEnumerable<InfoUser>>(result);
        }

        // получаем информацию о пользователе
        public async Task<InfoUser> Get_user_id(int id)
        {
            HttpClient client = getClientServise.GetClient();
            string result = await client.GetStringAsync("http://90.189.158.10/api/UserInfoes/"+id.ToString());
            return JsonConvert.DeserializeObject<InfoUser>(result);
        }

        // удаляем 
        public async Task<InfoUser> Delete(int id)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.DeleteAsync("http://90.189.158.10/api/UserInfoes/" + id);
            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<InfoUser>(await response.Content.ReadAsStringAsync());
        }






    }
}
