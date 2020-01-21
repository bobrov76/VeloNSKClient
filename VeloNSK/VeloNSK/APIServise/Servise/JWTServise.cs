using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace VeloNSK.APIServise.Servise
{
    class JWTServise
    {
        static Lincs server_lincs = new Lincs();
        
         public Dictionary<string, string> GetTokenDictionary(string userName, string password)// получение токена
        {
            try
            {

                var pairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>( "grant_type", "password"),
                    new KeyValuePair<string, string>( "username", userName ),
                    new KeyValuePair<string, string> ( "password", password )
                };
                var content = new FormUrlEncodedContent(pairs);

                using (var client = new HttpClient())
                {
                    var response = client.PostAsync(server_lincs.GetServer() + "token", content).Result;
                    var result = response.Content.ReadAsStringAsync().Result;
                    // Десериализация полученного JSON-объекта
                    Dictionary<string, string> tokenDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                    return tokenDictionary;
                }
            }
            catch
            {
                Dictionary<string, string> countries = new Dictionary<string, string>(1);
                countries.Add("access_token", "");
                return countries;
            }

        }
    }
}
