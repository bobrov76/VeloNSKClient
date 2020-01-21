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
    class ParticipationService
    {
        GetClientServise getClientServise = new GetClientServise();
        static Lincs server_lincs = new Lincs();

        // получаем информацию о пользователе
        public async Task<IEnumerable<Participation>> Get()
        {
            HttpClient client = getClientServise.GetClient();
            string result = await client.GetStringAsync("http://90.189.158.10/api/Participations/");
            return JsonConvert.DeserializeObject<IEnumerable<Participation>>(result);
        }

        // получаем информацию о пользователе
        public async Task<Participation> Get_ID(int id)
        {
            HttpClient client = getClientServise.GetClient();
            string result = await client.GetStringAsync("http://90.189.158.10/api/Participations/" + id.ToString());
            return JsonConvert.DeserializeObject<Participation>(result);
        }

        // удаляем 
        public async Task<Participation> Delete(int id)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.DeleteAsync("http://90.189.158.10/api/Participations/" + id);
            if (response.StatusCode != HttpStatusCode.OK)
                return null;
            return JsonConvert.DeserializeObject<Participation>(await response.Content.ReadAsStringAsync());
        }
        // добавляем одного друга
        public async Task<Participation> Add(Participation participation)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.PostAsync("http://90.189.158.10/api/Participations/",
                new StringContent(JsonConvert.SerializeObject(participation), Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<Participation>(await response.Content.ReadAsStringAsync());
        }
        // обновляем друга
        public async Task<Participation> Update(Participation participation)
        {
            HttpClient client = getClientServise.GetClient();
            var response = await client.PutAsync("http://90.189.158.10/api/Participations/" + participation.IdParticipation,
                new StringContent(
                    JsonConvert.SerializeObject(participation),
                    Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<Participation>(await response.Content.ReadAsStringAsync());
        }

    }

}
