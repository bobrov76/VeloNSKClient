using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace VeloNSK.APIServise.Servise
{
    class GetClientServise
    {
        // создаем http-клиента с токеном 
        public HttpClient CreateClient(string accessToken = "")
        {
            var client = new HttpClient();
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }
            return client;
        }

        public HttpClient GetClient()// создаем http-клиента
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }
    }
}
