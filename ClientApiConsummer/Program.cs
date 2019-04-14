using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
namespace ClientApiConsummer
{
    class Program
    {
        // You will need to substitute your own host Url here:
        static string host = "http://localhost:55146/";

        static void Main(string[] args)
        {
            Console.WriteLine("Attempting to Log in with default admin user");

            // Get hold of a Dictionary representing the JSON in the response Body:
            var responseDictionary =
                GetResponseAsDictionary("admin@example.com", "Admin@123456", out string token);
            foreach (var kvp in responseDictionary)
            {
                Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
            }
            Console.Read();
        }

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        static Dictionary<string, string> GetResponseAsDictionary(
            string userName, string password, out string token)
        {
            HttpClient client = new HttpClient();
            var pairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>( "grant_type", "password" ),
                    new KeyValuePair<string, string>( "username", userName ),
                    new KeyValuePair<string, string> ( "Password", password )
                };
            var content = new FormUrlEncodedContent(pairs);

            // Attempt to get a token from the token endpoint of the Web Api host:
            HttpResponseMessage response = client.PostAsync(host + "Token", content).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            // De-Serialize into a dictionary and return:
            Dictionary<string, string> tokenDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            token = tokenDictionary["access_token"];
            return tokenDictionary;
        }

        /// <summary>
        /// Get user informations
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        static string GetUserInfo(string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync(host + "api/Account/UserInfo").Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
