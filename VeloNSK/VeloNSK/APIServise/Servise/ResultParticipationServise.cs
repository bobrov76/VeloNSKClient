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
    internal class ResultParticipationServise
    {
        private GetClientServise getClientServise = new GetClientServise();
        private static Lincs server_lincs = new Lincs();

        // получаем информацию
        public async Task<IEnumerable<ResultParticipant>> Get()
        {
            HttpClient client = getClientServise.GetClient();
            string result = await client.GetStringAsync("http://90.189.158.10/api/ResultParticipations/");
            return JsonConvert.DeserializeObject<IEnumerable<ResultParticipant>>(result);
        }

        // получаем информацию по id
        public async Task<ResultParticipant> Get_ID(int id)
        {
            HttpClient client = getClientServise.GetClient();
            string result = await client.GetStringAsync("http://90.189.158.10/api/ResultParticipations/" + id.ToString());
            return JsonConvert.DeserializeObject<ResultParticipant>(result);
        }

        // удалить информацию
        public async Task<ResultParticipant> Delete(int id)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.DeleteAsync("http://90.189.158.10/api/ResultParticipations/" + id);
            if (response.StatusCode != HttpStatusCode.OK)
                return null;
            return JsonConvert.DeserializeObject<ResultParticipant>(await response.Content.ReadAsStringAsync());
        }

        // добавляем информацию
        public async Task<ResultParticipant> Add(ResultParticipant resultParticipation)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.PostAsync("http://90.189.158.10/api/ResultParticipations/",
                new StringContent(JsonConvert.SerializeObject(resultParticipation), Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<ResultParticipant>(await response.Content.ReadAsStringAsync());
        }

        // обновляем информацию
        public async Task<ResultParticipant> Update(ResultParticipant resultParticipation)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.PutAsync("http://90.189.158.10/api/ResultParticipations/" + resultParticipation.IdResultParticipation,
                new StringContent(
                    JsonConvert.SerializeObject(resultParticipation),
                    Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<ResultParticipant>(await response.Content.ReadAsStringAsync());
        }
    }
}