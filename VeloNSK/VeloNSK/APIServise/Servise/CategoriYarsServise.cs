using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VeloNSK.APIServise.Model;

namespace VeloNSK.APIServise.Servise
{
    class CategoriYarsServise
    {
        GetClientServise getClientServise = new GetClientServise();
        static Lincs server_lincs = new Lincs();

        // получаем информацию о пользователе
        public async Task<IEnumerable<CategoriYars>> Get()
        {
            HttpClient client = getClientServise.GetClient();
            string result = await client.GetStringAsync("http://90.189.158.10/api/CategoriYars/");
            return JsonConvert.DeserializeObject<IEnumerable<CategoriYars>>(result);
        }

        // получаем информацию о пользователе
        public async Task<CategoriYars> Get_ID(int id)
        {
            HttpClient client = getClientServise.GetClient();
            string result = await client.GetStringAsync("http://90.189.158.10/api/CategoriYars/" + id.ToString());
            return JsonConvert.DeserializeObject<CategoriYars>(result);
        }

        // удаляем 
        public async Task<CategoriYars> Delete(int id)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.DeleteAsync("http://90.189.158.10/api/CategoriYarss/" + id);
            if (response.StatusCode != HttpStatusCode.OK)
                return null;
            return JsonConvert.DeserializeObject<CategoriYars>(await response.Content.ReadAsStringAsync());
        }
        // добавляем одного друга
        public async Task<CategoriYars> Add(CategoriYars categoriYars)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.PostAsync("http://90.189.158.10/api/CategoriYarss/",
                new StringContent(JsonConvert.SerializeObject(categoriYars), Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<CategoriYars>(await response.Content.ReadAsStringAsync());
        }
        // обновляем друга
        public async Task<CategoriYars> Update(CategoriYars categoriYars)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.PutAsync("http://90.189.158.10/api/CategoriYarss/" + categoriYars.IdCategori,
                new StringContent(
                    JsonConvert.SerializeObject(categoriYars),
                    Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<CategoriYars>(await response.Content.ReadAsStringAsync());
        }

    }

}

