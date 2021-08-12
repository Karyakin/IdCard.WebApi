using IdCard.WebApi.Entities;
using IdCard.WebApi.Helpers;
using IdCard.WebApi.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace IdCard.WebApi.Implementations
{
    public class DataGroupe : IDataGroupe
    {
        private readonly IPostHandler _postHandlerRequest;
        private readonly IOptions<IdCardSettings> _idCardOptions;

        public DataGroupe(IPostHandler postHandlerRequest, IOptions<IdCardSettings> idCardOptions)
        {
            _postHandlerRequest = postHandlerRequest;
            _idCardOptions = idCardOptions;
        }
        public async Task<PersonalData> GetDataGroupe(JToken hreq)
        {
            if (hreq is null)
            {
                Log.Error($"Parameter \"hreq\" can't be null. Method \"{nameof(GetDataGroupe)}\", class \"{nameof(DataGroupe)}\"");
                throw new NullReferenceException(nameof(hreq));
            }
            string dg1 = "dg1";
            string dg2 = "dg2";
            string dg3 = "dg3";
            string dg4 = "dg4";
            string dg5 = "dg5";
/*
            try
            {*/
                using var client = new HttpClient();

                //(1) gets: header_cmd_to_card, cmd_to_card
                var readDgInitRequesr = $"{{\"hreq\":\"{hreq}\",\"data_groups_to_read\":[ \"{dg1}\", \"{dg2}\", \"{dg3}\", \"{dg4}\", \"{dg5}\" ]}}";
                var readDgInitResponse = await client.PostAsync($"{_idCardOptions.Value.TerminalAdress}{_idCardOptions.Value.Version}read_dg_init", new StringContent(readDgInitRequesr, Encoding.UTF8, "application/json"));

                string readDgInitContent = await readDgInitResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<JsonData>(readDgInitContent);

                //(2) terminal_proxy_command
                var terminalProxyCommandRequest = new Dictionary<string, string>
                {
                    { "header_cmd_to_card", $"{response?.HeaderCmdToCard}" },
                    { "cmd_to_card", $"{response?.CmdToCard}" }

                };
                response = _postHandlerRequest.PostHandlerRequest($"{_idCardOptions.Value.CpAdress}{_idCardOptions.Value.Version}terminal_proxy_command", terminalProxyCommandRequest).Result;

                //(3-4) read_dg, terminal_proxy_command
                while (true)
                {
                    var readDgRequest = new Dictionary<string, string>
                    {
                        { "card_response", $"{response?.CardResponse}" },
                        { "hreq", $"{hreq}" }
                    };

                    response = _postHandlerRequest.PostHandlerRequest($"{_idCardOptions.Value.TerminalAdress}{_idCardOptions.Value.Version}read_dg", readDgRequest).Result;

                    if (response.IsLastDgReaded is true)
                    {
                        break;
                    }

                    terminalProxyCommandRequest = new Dictionary<string, string>
                    {
                        { "header_cmd_to_card", $"{response?.HeaderCmdToCard}" },
                        { "cmd_to_card", $"{response?.CmdToCard}" }

                    };
                    response = _postHandlerRequest.PostHandlerRequest($"{_idCardOptions.Value.CpAdress}{_idCardOptions.Value.Version}terminal_proxy_command", terminalProxyCommandRequest).Result;
                }

                //(5) request_dg
                var personalDataRequest = new Dictionary<string, string>
                {
                    { "hreq", $"{hreq}" }
                };

                var dataGroupe = _postHandlerRequest.PostHandlerDataGroupeRequest($"{_idCardOptions.Value.TerminalAdress}{_idCardOptions.Value.Version}request_dg", personalDataRequest).Result;
                return dataGroupe;
            }
           /* catch (Exception e)
            {
                Log.Error($"Failed to get data group. Details:{e.Message}");
                throw new Exception($"Failed to get data group. Details:{e.Message}");
            }
        }*/
    }
}
