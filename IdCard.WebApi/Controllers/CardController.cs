using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdCard.WebApi.Entities;
using IdCard.WebApi.Interfaces;

namespace IdCard.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardController : ControllerBase
    {

        private readonly IIdCardAuthentication _idCardAuthentication;

        public CardController(IIdCardAuthentication idCardAuthentication)
        {
            _idCardAuthentication = idCardAuthentication;
        }

        //pin1 533790
        //pin2 7896313
        //can 042465

        /*  string cpAdress = "http://192.168.5.100:8084/";
          //string cpAdress = "http://localhost:8084/";
          string terminalAdress = "http://192.168.5.135:48777/";
          string version = "api/v1/";
          string json;
          //string json = "{\"param1\":\"val\",\"param2\":\"val\"}";
  */

        string cpAdress = "http://192.168.5.100:8084/";
        string terminalAdress = "http://192.168.5.135:48777/";
        string version = "api/v1/";


        [HttpGet]
        public IActionResult Get()
        {

            // var hreq = Bauth();
            var hreq = _idCardAuthentication.Bauth();

            var digitalSignature =  GetDigitalSignature(hreq, "Привет Мир!");

          //  var dataGroupe = GetDataGroupe(hreq).Result;

            return Ok(digitalSignature);
        }

        public JToken Bauth()
        {
            return _idCardAuthentication.Bauth();

            /* //TODO проверка на наличие коробки
 
             // (1) auth_sign
             var requestParameter = new Dictionary<string, string>
             {
                 { "init", "true" }
             };
             var response = PostHandler($"{cpAdress}auth_sign", requestParameter).Result;
 
             // (2) bauth_init
             var bauthInitRequest = new Dictionary<string, string>
             {
                 { "so_certificate", $"{response?.SoCert}" }
             };
             response = PostHandler($"{terminalAdress}{version}bauth_init", bauthInitRequest).Result;
             var hreq = response?.Hreq;
 
             // (3) terminal_proxy_bauth_init
             var terminalProxyBauthInitRequest = new Dictionary<string, string>
             {
                 { "terminal_certificate", $"{response?.TerminalCertificate}" },
                 { "cmd_to_card", $"{response?.CmdToCard}" }
 
             };
             response = PostHandler($"{cpAdress}{version}terminal_proxy_bauth_init", terminalProxyBauthInitRequest).Result;
 
             // (4-5) bauth_process, terminal_proxy_bauth
             do
             {
                 var bauthProcessRequest = new Dictionary<string, string>
                 {
                     { "hreq", $"{hreq}" },
                     { "card_response", $"{response?.CardResponse}" }
                 };
                 response = PostHandler($"{terminalAdress}{version}bauth_process", bauthProcessRequest).Result;
 
                 if (response?.IsBauthEstablished is true)
                 {
                     break;
                 }
 
                 var terminalProxyBauthRequest = new Dictionary<string, string>
                 {
                     { "header_cmd_to_card", $"{response?.HeaderCmdToCard}" },
                     { "cmd_to_card", $"{response?.CmdToCard}" }
                 };
                 response = PostHandler($"{cpAdress}{version}terminal_proxy_bauth", terminalProxyBauthRequest).Result;
 
             } while (true);
 
             //todo если придет null вернуть исключение "Получение данных не удалось"
             return hreq;*/
        }

        public async Task<PersonalData> GetDataGroupe(JToken hreq)
        {
            string dg1 = "dg1";
            string dg2 = "dg2";
            string dg3 = "dg3";
            string dg4 = "dg4";
            string dg5 = "dg5";

            using var client = new HttpClient();

            #region read_dg_init (1) gets: header_cmd_to_card, cmd_to_card

            var readDgInitRequesr =
                $"{{{String.Format("\"hreq\":\"{0}\",", hreq)}\"data_groups_to_read\":[ \"{dg1}\", \"{dg2}\", \"{dg3}\", \"{dg4}\", \"{dg5}\" ]}}";
            var readDgInitResponse = await client.PostAsync($"{terminalAdress}{version}read_dg_init", new StringContent(readDgInitRequesr, Encoding.UTF8, "application/json"));

            string readDgInitContent = await readDgInitResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<JsonData>(readDgInitContent);

            #endregion

            #region terminal_proxy_command (2) gets: card_responce

            var terminalProxyCommandRequest = new Dictionary<string, string>
            {
                { "header_cmd_to_card", $"{response?.HeaderCmdToCard}" },
                { "cmd_to_card", $"{response?.CmdToCard}" }

            };
            response = PostHandler($"{cpAdress}{version}terminal_proxy_command", terminalProxyCommandRequest).Result;

            #endregion

            #region read_dg, terminal_proxy_command (3-4) 

            while (true)
            {
                var readDgRequest = new Dictionary<string, string>
                {
                    { "card_response", $"{response.CardResponse}" },
                    { "hreq", $"{hreq}" }
                };

                response = PostHandler($"{terminalAdress}{version}read_dg", readDgRequest).Result;

                if (response.IsLastDgReaded is true)
                {
                    break;
                }

                terminalProxyCommandRequest = new Dictionary<string, string>
                {
                    { "header_cmd_to_card", $"{response?.HeaderCmdToCard}" },
                    { "cmd_to_card", $"{response?.CmdToCard}" }

                };
                response = PostHandler($"{cpAdress}{version}terminal_proxy_command", terminalProxyCommandRequest).Result;
            }
            #endregion

            #region personal_data(5), return personal_data
            var personalDataRequest = new Dictionary<string, string>
              {
                  { "hreq", $"{hreq}" }
              };

            var responseData = PostHandlerDataGroupe($"{terminalAdress}{version}request_dg", personalDataRequest).Result;

            #endregion


            return responseData;
        }

        public string GetDigitalSignature(JToken hreq, string dataToSign)
        {
            // (1)sign_init
            var requestParametrs = new Dictionary<string, string>
            {
                { "hreq", $"{hreq}" }
            };
            var response = PostHandler($"{terminalAdress}{version}sign_init", requestParametrs).Result;

            // (2)terminal_proxy_sign_init
            requestParametrs = new Dictionary<string, string>
            {
                { "header_cmd_to_card", $"{response.HeaderCmdToCard}" },
                { "cmd_to_card", $"{response.CmdToCard}" }
            };
            response = PostHandler($"{cpAdress}{version}terminal_proxy_sign_init", requestParametrs).Result;

            // (3)sign_select_app
            requestParametrs = new Dictionary<string, string>
            {
                { "card_response", $"{response.CardResponse}" },
                { "hreq", $"{hreq}" }
            };
            response = PostHandler($"{terminalAdress}{version}sign_select_app", requestParametrs).Result;

            // (4)terminal_proxy_command
            requestParametrs = new Dictionary<string, string>
            {
                { "header_cmd_to_card", $"{response.HeaderCmdToCard}" },
                { "cmd_to_card", $"{response.CmdToCard}" }
            };
            response = PostHandler($"{cpAdress}{version}terminal_proxy_command", requestParametrs).Result;


            // (5)sign_data
            byte[] dataForSignInDase64 = Encoding.ASCII.GetBytes(dataToSign);
            requestParametrs = new Dictionary<string, string>
            {
                { "card_response", $"{response.CardResponse}" },
                { "hreq", $"{hreq}" },
                { "data_to_sign", $"{dataForSignInDase64}" }
            };
            response = PostHandler($"{terminalAdress}{version}sign_data", requestParametrs).Result;


            // (6)terminal_proxy_command
            requestParametrs = new Dictionary<string, string>
            {
                { "header_cmd_to_card", $"{response.HeaderCmdToCard}" },
                { "cmd_to_card", $"{response.CmdToCard}" }
            };
            response = PostHandler($"{cpAdress}{version}terminal_proxy_command", requestParametrs).Result;

            // (7)sign_result
            requestParametrs = new Dictionary<string, string>
            {
                { "card_response", $"{response.CardResponse}" },
                { "hreq", $"{hreq}" }
            };
            response = PostHandler($"{terminalAdress}{version}sign_result", requestParametrs).Result;

            return response.Signature;
        }

       private async Task<JsonData> PostHandler(string requestUri, Dictionary<string, string> requestParameters)
        {
            using var client = new HttpClient();
            var parameters = JsonConvert.SerializeObject(requestParameters);

            var response = await client.PostAsync(requestUri, new StringContent(parameters, Encoding.UTF8, "application/json"));
            string responseContent = await response.Content.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<JsonData>(responseContent);

            return request;
        }

        static async Task<PersonalData> PostHandlerDataGroupe(string requestUri, Dictionary<string, string> requestParameters)
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

