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
    internal class DistantionsServise
    {
        private GetClientServise getClientServise = new GetClientServise();
        private static Lincs server_lincs = new Lincs();

        // получаем информацию о пользователе
        public async Task<IEnumerable<Distantion>> Get()
        {
            HttpClient client = getClientServise.GetClient();
            string result = await client.GetStringAsync("http://90.189.158.10/api/Distantions/");
            return JsonConvert.DeserializeObject<IEnumerable<Distantion>>(result);
        }

        public async Task<string> GetExport()
        {
            HttpClient client = getClientServise.GetClient();
            string result = await client.GetStringAsync("http://90.189.158.10/api/Distans/ExportDistans");
            return JsonConvert.DeserializeObject<string>(result);
        }

        // получаем информацию о пользователе
        public async Task<Distantion> Get_ID(int id)
        {
            HttpClient client = getClientServise.GetClient();
            string result = await client.GetStringAsync("http://90.189.158.10/api/Distantions/" + id.ToString());
            return JsonConvert.DeserializeObject<Distantion>(result);
        }

        // удаляем
        public async Task<Distantion> Delete(int id)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.DeleteAsync("http://90.189.158.10/api/Distantions/" + id);
            if (response.StatusCode != HttpStatusCode.OK)
                return null;
            return JsonConvert.DeserializeObject<Distantion>(await response.Content.ReadAsStringAsync());
        }

        // добавляем одного друга
        public async Task<Distantion> Add(Distantion distantion)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.PostAsync("http://90.189.158.10/api/Distantions/",
                new StringContent(JsonConvert.SerializeObject(distantion), Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<Distantion>(await response.Content.ReadAsStringAsync());
        }

        // обновляем друга
        public async Task<Distantion> Update(Distantion distantion)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.PutAsync("http://90.189.158.10/api/Distantions/" + distantion.IdDistantion,
                new StringContent(
                    JsonConvert.SerializeObject(distantion),
                    Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<Distantion>(await response.Content.ReadAsStringAsync());
        }
    }
}