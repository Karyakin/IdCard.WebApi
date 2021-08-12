using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdCard.WebApi.Entities;
using IdCard.WebApi.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IdCard.WebApi.Implementations
{
    public class PostHandler : IPostHandler
    {
       public async Task<JsonData> PostHandlerRequest(string requestUri, Dictionary<string, string> requestParameters)
        {
            using var client = new HttpClient();
            var parameters = JsonConvert.SerializeObject(requestParameters);

            try
            {
                var response = await client.PostAsync(requestUri, new StringContent(parameters, Encoding.UTF8, "application/json"));
                string responseContent = await response.Content.ReadAsStringAsync();
                var request = JsonConvert.DeserializeObject<JsonData>(responseContent);
                if (request?.Error != "0" && request?.Error != null)
                {
                    throw new InvalidOperationException($"{request.Error}");// здесь будет код самой ошибки
                }
                return request;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            
        }
        public async Task<PersonalData> PostHandlerDataGroupeRequest(string requestUri, Dictionary<string, string> requestParameters)
        {
            using var client = new HttpClient();
            var parameters = JsonConvert.SerializeObject(requestParameters);

            var response = await client.PostAsync(requestUri, new StringContent(parameters, Encoding.UTF8, "application/json"));
            string responseContent = await response.Content.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<PersonalData>(responseContent);

            return request;
        }
    }
}
