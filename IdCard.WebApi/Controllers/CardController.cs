using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

            var hreq = Bauth().Result;

            var dataGroupe = GetDataGroupe(hreq).Result;

            return Ok(dataGroupe);
        }

        public async Task<JToken> Bauth()
        {


            //TODO проверка на наличие коробки


            var soSertRequest = JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                { "init", "true" }
            });


            using var client = new HttpClient();



            var soSertResponse = await client.PostAsync(cpAdress + "auth_sign", new StringContent(soSertRequest, Encoding.UTF8, "application/json"));

            string soSertResultContent = await soSertResponse.Content.ReadAsStringAsync();

            var SO_CERT = ((JObject)JsonConvert.DeserializeObject(soSertResultContent))["SO_CERT"];


            var bauthInitRequest = JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                { "so_certificate", $"{SO_CERT}" }
            });

            var bauthInitResponse = await client.PostAsync($"{terminalAdress}{version}" + "bauth_init", new StringContent(bauthInitRequest, Encoding.UTF8, "application/json"));
            string bauthInitResultContent = await bauthInitResponse.Content.ReadAsStringAsync();
            var bauthInitJObject = (JObject)JsonConvert.DeserializeObject(bauthInitResultContent);

            var hreq = bauthInitJObject?["hreq"];
            var terminal_certificate = bauthInitJObject?["terminal_certificate"];
            var cmd_to_card = bauthInitJObject?["cmd_to_card"];

            var terminalProxyBauthInitRequest = JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                { "terminal_certificate", $"{terminal_certificate}" },
                { "cmd_to_card", $"{cmd_to_card}" }

            });

            var terminalProxyBauthInitResponse = await client.PostAsync($"{cpAdress}{version}" + "terminal_proxy_bauth_init", new StringContent(terminalProxyBauthInitRequest, Encoding.UTF8, "application/json"));
            string terminalProxyBauthInitContent = await terminalProxyBauthInitResponse.Content.ReadAsStringAsync();
            var terminalProxyBauthInitJObject = (JObject)JsonConvert.DeserializeObject(terminalProxyBauthInitContent);
            var card_responce = terminalProxyBauthInitJObject?["card_response"];

            string header;
            string status;

            do
            {
                var bauthProcessRequest = JsonConvert.SerializeObject(new Dictionary<string, string>
                {
                    { "hreq", $"{hreq}" },
                    { "card_response", $"{card_responce}" }
                });


                var bauthProcessResponse = await client.PostAsync($"{terminalAdress}{version}" + "bauth_process", new StringContent(bauthProcessRequest, Encoding.UTF8, "application/json"));
                string bauthProcessContent = await bauthProcessResponse.Content.ReadAsStringAsync();
                var bauthProcessJObject = (JObject)JsonConvert.DeserializeObject(bauthProcessContent);
                header = bauthProcessJObject?["header_cmd_to_card"].ToString();
                cmd_to_card = bauthProcessJObject?["cmd_to_card"];
                status = bauthProcessJObject?["is_bauth_established"].ToString();

                if (status is "True")
                {
                    break;
                }

                var terminalProxyBauthRequest = JsonConvert.SerializeObject(new Dictionary<string, string>
                {
                    { "header_cmd_to_card", $"{header}" },
                    { "cmd_to_card", $"{cmd_to_card}" }
                });

                var terminalProxyBauthResponse = await client.PostAsync($"{cpAdress}{version}" + "terminal_proxy_bauth", new StringContent(terminalProxyBauthRequest, Encoding.UTF8, "application/json"));
                string terminalProxyBauthContent = await terminalProxyBauthResponse.Content.ReadAsStringAsync();
                var terminalProxyBauthJObject = (JObject)JsonConvert.DeserializeObject(terminalProxyBauthContent);
                card_responce = terminalProxyBauthJObject?["card_response"];
            } while (true);

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
            var readDgInitJObject = (JObject)JsonConvert.DeserializeObject(readDgInitContent);

            var header_cmd_to_card = readDgInitJObject?["header_cmd_to_card"];
            var cmd_to_card = readDgInitJObject?["cmd_to_card"];

            #endregion

            #region terminal_proxy_command (2) gets: card_responce

            var terminalProxyCommandRequest = JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                { "header_cmd_to_card", $"{header_cmd_to_card}" },
                { "cmd_to_card", $"{cmd_to_card}" }

            });

            var terminalProxyCommandResponse = await client.PostAsync($"{cpAdress}{version}" + "terminal_proxy_command", new StringContent(terminalProxyCommandRequest, Encoding.UTF8, "application/json"));
            string terminalProxyCommandContent = await terminalProxyCommandResponse.Content.ReadAsStringAsync();
            var terminalProxyCommand = (JObject)JsonConvert.DeserializeObject(terminalProxyCommandContent);
            var card_response = terminalProxyCommand?["card_response"];

            #endregion


            #region read_dg, terminal_proxy_command (3-4) 

            string status;

            while (true)
            {


                var readDgRequest = JsonConvert.SerializeObject(new Dictionary<string, string>
            {
               { "card_response", $"{card_response}" },
               { "hreq", $"{hreq}" }
            });

                var readDgRequestResponse = await client.PostAsync($"{terminalAdress}{version}" + "read_dg", new StringContent(readDgRequest, Encoding.UTF8, "application/json"));
                string readDgContent = await readDgRequestResponse.Content.ReadAsStringAsync();//err8
                var readDgCommand = (JObject)JsonConvert.DeserializeObject(readDgContent);

                header_cmd_to_card = readDgCommand?["header_cmd_to_card"].ToString();
                cmd_to_card = readDgCommand?["cmd_to_card"];
                status = readDgCommand?["is_last_dg_readed"].ToString();

                if (status is "True")
                {
                    break;
                }

                terminalProxyCommandRequest = JsonConvert.SerializeObject(new Dictionary<string, string>
                {
                    { "header_cmd_to_card", $"{header_cmd_to_card}" },
                    { "cmd_to_card", $"{cmd_to_card}" }
                });

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

    }
}


