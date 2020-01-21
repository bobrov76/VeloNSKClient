using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VeloNSK.APIServise.Servise
{
    internal class DoubleAuthenticationService
    {
        private GetClientServise getClientServise = new GetClientServise();

        public async Task<string> Post(string Id, string masage)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(Id), "\"Id\"");
            content.Add(new StringContent(masage), "\"masage\"");
            var httpClient = getClientServise.GetClient();
            var servere_adres = "http://90.189.158.10/api/Mail/";
            var httpResponseMasage = await httpClient.PostAsync(servere_adres, content);
            var respond = await httpResponseMasage.Content.ReadAsStringAsync();
            return respond;
        }
    }
}