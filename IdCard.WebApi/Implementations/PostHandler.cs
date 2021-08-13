using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdCard.WebApi.Entities;
using IdCard.WebApi.Interfaces;
using Newtonsoft.Json;
using Serilog;

namespace IdCard.WebApi.Implementations
{
    public class PostHandler : IPostHandler
    {
        public async Task<JsonData> PostHandlerRequest(string requestUri, Dictionary<string, string> requestParameters)
        {
            using var client = new HttpClient();
            var parameters = JsonConvert.SerializeObject(requestParameters);

            HttpResponseMessage response;

            try
            {
                response = await client.PostAsync(requestUri, new StringContent(parameters, Encoding.UTF8, "application/json"));
            }
            catch (Exception e)
            {
                Log.Error($"\"PostHandlerRequest\" method. \nError code: {e.Message}");
                throw new InvalidOperationException($"Для работы с Id-card необходимо запустить программу \"NT Client Software\"");
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<JsonData>(responseContent);
            if (request?.Error != "0" && request?.Error != null)
            {
                switch (request.Error)
                {
                    case "3":
                        Log.Error($"\"PostHandlerRequest\" method. \nError code: {request.Error}");
                        throw new InvalidOperationException($"Проверьте подключение считывателя к персональному компьютеру, вставьте карточку в считыватель либо обратитесь к администратору. \nError code: {request.Error}");
                    case "8":
                        throw new InvalidOperationException();
                }
            }
            return request;
        }

        public async Task<PersonalData> PostHandlerDataGroupeRequest(string requestUri, Dictionary<string, string> requestParameters)
        {
            using var client = new HttpClient();
            var parameters = JsonConvert.SerializeObject(requestParameters);
            HttpResponseMessage response;

            try
            {
                response = await client.PostAsync(requestUri, new StringContent(parameters, Encoding.UTF8, "application/json"));
            }
            catch (Exception e)
            {
                Log.Error($"\"PostHandlerRequest\" method. \nError code: {e.Message}");
                throw new InvalidOperationException($"Для работы с Id-card необходимо запустить программу \"NT Client Software\"");
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<PersonalData>(responseContent);
            if (request?.Error != "0" && request?.Error != null)
            {
                switch (request.Error)
                {
                    case "3":
                        Log.Error($"\"PostHandlerRequest\" method. \nError code: {request.Error}");
                        throw new InvalidOperationException($"Проверьте подключение считывателя к персональному компьютеру, вставьте карточку в считыватель либо обратитесь к администратору. \nError code: {request.Error}");
                    case "8":
                        throw new InvalidOperationException();
                }
            }
            return request;
        }
    }
}
