using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdCard.WebApi.Entities;

namespace IdCard.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardController : ControllerBase
    {
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
        public async Task<IActionResult> Get()
        {

            var hreq = Bauth();

            var dataGroupe = GetDataGroupe(hreq).Result;

            return Ok(dataGroupe);
        }

        public JToken Bauth()
        {
            //TODO проверка на наличие коробки

            // (1) auth_sign
            var requestParameter =  new Dictionary<string, string>
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
            var terminalProxyBauthInitRequest= new Dictionary<string, string>
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
            return hreq;
        }


       






        public async Task<string> GetDataGroupe(JToken hreq)
        {
            string dg1 = "dg1";
            string dg2 = "dg2";
            string dg3 = "dg3";
            string dg4 = "dg4";
            string dg5 = "dg5";


            using var client = new HttpClient();

            #region read_dg_init (1) gets: header_cmd_to_card, cmd_to_card

            var readDgInitRequesr = "{" +
                    String.Format("\"hreq\":\"{0}\",", hreq) +
                    $"\"data_groups_to_read\":[ \"{dg1}\", \"{dg2}\", \"{dg3}\", \"{dg4}\", \"{dg5}\" ]" +
                    "}";
            var readDgInitResponse = await client.PostAsync($"{terminalAdress}{version}" + "read_dg_init", new StringContent(readDgInitRequesr, Encoding.UTF8, "application/json"));

            string readDgInitContent = await readDgInitResponse.Content.ReadAsStringAsync();
           /* var readDgInitJObject = (JObject)JsonConvert.DeserializeObject(readDgInitContent);

            var header_cmd_to_card = readDgInitJObject?["header_cmd_to_card"];
            var cmd_to_card = readDgInitJObject?["cmd_to_card"];*/

            var response = JsonConvert.DeserializeObject<JsonData>(readDgInitContent);


            #endregion

            #region terminal_proxy_command (2) gets: card_responce

            var terminalProxyCommandRequest = JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                { "header_cmd_to_card", $"{response?.HeaderCmdToCard}" },
                { "cmd_to_card", $"{response?.CmdToCard}" }

            });

         /*   var terminalProxyCommandRequest = new Dictionary<string, string>
            {
                { "header_cmd_to_card", $"{response?.HeaderCmdToCard}" },
                { "cmd_to_card", $"{response?.CmdToCard}" }

            };
            response = PostHandler($"{cpAdress}{version}terminal_proxy_command", terminalProxyCommandRequest).Result;
*/


            var terminalProxyCommandResponse = await client.PostAsync($"{cpAdress}{version}terminal_proxy_command", new StringContent(terminalProxyCommandRequest, Encoding.UTF8, "application/json"));
            string terminalProxyCommandContent = await terminalProxyCommandResponse.Content.ReadAsStringAsync();
            var terminalProxyCommand = (JObject)JsonConvert.DeserializeObject(terminalProxyCommandContent);
            var card_response = terminalProxyCommand?["card_response"];








            #endregion


            #region read_dg, terminal_proxy_command (3-4) 

            // string status;

            while (true)
            {


                var readDgRequest = JsonConvert.SerializeObject(new Dictionary<string, string>
                 {
                     { "card_response", $"{response.CardResponse}" },
                     { "hreq", $"{hreq}" }
                 });

                var readDgRequestResponse = await client.PostAsync($"{terminalAdress}{version}" + "read_dg", new StringContent(readDgRequest, Encoding.UTF8, "application/json"));
                string readDgContent = await readDgRequestResponse.Content.ReadAsStringAsync();//err8
                var readDgCommand = (JObject)JsonConvert.DeserializeObject(readDgContent);

                /*
                                var readDgRequest = new Dictionary<string, string>
                                {
                                    { "card_response", $"{response.CardResponse}" },
                                    { "hreq", $"{hreq}" }
                                };
                                response = PostHandler($"{terminalAdress}{version}read_dg", readDgRequest).Result;*/
                /*
                                header_cmd_to_card = readDgCommand?["header_cmd_to_card"].ToString();
                                cmd_to_card = readDgCommand?["cmd_to_card"];
                                status = readDgCommand?["is_last_dg_readed"].ToString();*/

                if (response.IsBauthEstablished is true)
                {
                    break;
                }

                /* terminalProxyCommandRequest = new Dictionary<string, string>
                {
                    { "header_cmd_to_card", $"{response.HeaderCmdToCard}" },
                    { "cmd_to_card", $"{response.CmdToCard}" }
                };
                 response = PostHandler($"{cpAdress}{version}" + "terminal_proxy_command", terminalProxyCommandRequest).Result;*/
                terminalProxyCommandResponse = await client.PostAsync($"{cpAdress}{version}" + "terminal_proxy_command", new StringContent(terminalProxyCommandRequest, Encoding.UTF8, "application/json"));
                terminalProxyCommandContent = await terminalProxyCommandResponse.Content.ReadAsStringAsync();
                terminalProxyCommand = (JObject)JsonConvert.DeserializeObject(terminalProxyCommandContent);
                card_response = terminalProxyCommand?["card_response"];

            }

            #endregion

            #region personal_data(5), return personal_data
            var personalDataRequest = JsonConvert.SerializeObject(new Dictionary<string, string>
                {
                    { "hreq", $"{hreq}" }
                });
            var personalDataResponse = await client.PostAsync($"{terminalAdress}{version}" + "request_dg", new StringContent(personalDataRequest, Encoding.UTF8, "application/json"));
            var personalDataResponseContent = await personalDataResponse.Content.ReadAsStringAsync();
            var personalDataResponseContentCommand = (JObject)JsonConvert.DeserializeObject(personalDataResponseContent);
            var data = personalDataResponseContentCommand["personal_data"];
            #endregion




            return data.ToString();
        }
        static async Task<JsonData> PostHandler(string requestUri, Dictionary<string, string> requestParameters)
        {
            using var client = new HttpClient();

            var parameters = JsonConvert.SerializeObject(requestParameters);

            var response = await client.PostAsync(requestUri, new StringContent(parameters, Encoding.UTF8, "application/json"));
            string responseContent = await response.Content.ReadAsStringAsync();
            var bauthInitJson = JsonConvert.DeserializeObject<JsonData>(responseContent);

            return bauthInitJson;
        }
    }
}


