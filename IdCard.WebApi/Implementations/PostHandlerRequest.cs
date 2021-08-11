using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdCard.WebApi.Entities;
using IdCard.WebApi.Interfaces;
using Newtonsoft.Json;

namespace IdCard.WebApi.Implementations
{
    public class PostHandlerRequest : IPostHandlerRequest
    {
       public async Task<JsonData> PostHandler(string requestUri, Dictionary<string, string> requestParameters)
        {
            using var client = new HttpClient();
            var parameters = JsonConvert.SerializeObject(requestParameters);

            var response = await client.PostAsync(requestUri, new StringContent(parameters, Encoding.UTF8, "application/json"));
            string responseContent = await response.Content.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<JsonData>(responseContent);

            return request;
        }
    }
}
