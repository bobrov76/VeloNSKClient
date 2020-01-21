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
    internal class CompetentionsServise
    {
        private GetClientServise getClientServise = new GetClientServise();
        private static Lincs server_lincs = new Lincs();

        // получаем информацию
        public async Task<IEnumerable<Competentions>> Get()
        {
            HttpClient client = getClientServise.GetClient();
            string result = await client.GetStringAsync("http://90.189.158.10/api/Competentions/");
            return JsonConvert.DeserializeObject<IEnumerable<Competentions>>(result);
        }

        // получаем информацию по id
        public async Task<Competentions> Get_ID(int id)
        {
            HttpClient client = getClientServise.GetClient();
            string result = await client.GetStringAsync("http://90.189.158.10/api/Competentions/" + id.ToString());
            return JsonConvert.DeserializeObject<Competentions>(result);
        }

        // удалить информацию
        public async Task<Competentions> Delete(int id)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.DeleteAsync("http://90.189.158.10/api/Competentions/" + id);
            if (response.StatusCode != HttpStatusCode.OK)
                return null;
            return JsonConvert.DeserializeObject<Competentions>(await response.Content.ReadAsStringAsync());
        }

        // добавляем информацию
        public async Task<Competentions> Add(Competentions сompetentions)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.PostAsync("http://90.189.158.10/api/Competentions/",
                new StringContent(JsonConvert.SerializeObject(сompetentions), Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<Competentions>(await response.Content.ReadAsStringAsync());
        }

        // обновляем информацию
        public async Task<Competentions> Update(Competentions сompetentions)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.PutAsync("http://90.189.158.10/api/Competentions/" + сompetentions.IdCompetentions,
                new StringContent(
                    JsonConvert.SerializeObject(сompetentions),
                    Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<Competentions>(await response.Content.ReadAsStringAsync());
        }
    }
}