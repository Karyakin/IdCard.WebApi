using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdCard.WebApi.Helpers;
using IdCard.WebApi.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace IdCard.WebApi.Implementations
{
    public class DigitalSignature : IDigitalSignature
    {

        private readonly IPostHandler _postHandlerRequest;
        private readonly IOptions<IdCardSettings> _idCardOptions;

        public DigitalSignature(IPostHandler postHandlerRequest, IOptions<IdCardSettings> idCardOptions)
        {
            _postHandlerRequest = postHandlerRequest;
            _idCardOptions = idCardOptions;
        }
        public string GetDigitalSignature(JToken hreq, string dataToSign)
        {
            // (1)sign_init
            var requestParameters = new Dictionary<string, string>
            {
                { "hreq", $"{hreq}" }
            };
            var response = _postHandlerRequest.PostHandlerRequest($"{_idCardOptions.Value.TerminalAdress}{_idCardOptions.Value.Version}sign_init", requestParameters).Result;

            // (2)terminal_proxy_sign_init
            requestParameters = new Dictionary<string, string>
            {
                { "header_cmd_to_card", $"{response.HeaderCmdToCard}" },
                { "cmd_to_card", $"{response.CmdToCard}" }
            };
            response = _postHandlerRequest.PostHandlerRequest($"{_idCardOptions.Value.CpAdress}{_idCardOptions.Value.Version}terminal_proxy_sign_init", requestParameters).Result;

            // (3)sign_select_app
            requestParameters = new Dictionary<string, string>
            {
                { "card_response", $"{response.CardResponse}" },
                { "hreq", $"{hreq}" }
            };
            response = _postHandlerRequest.PostHandlerRequest($"{_idCardOptions.Value.TerminalAdress}{_idCardOptions.Value.Version}sign_select_app", requestParameters).Result;

            // (4)terminal_proxy_command
            requestParameters = new Dictionary<string, string>
            {
                { "header_cmd_to_card", $"{response.HeaderCmdToCard}" },
                { "cmd_to_card", $"{response.CmdToCard}" }
            };
            response = _postHandlerRequest.PostHandlerRequest($"{_idCardOptions.Value.CpAdress}{_idCardOptions.Value.Version}terminal_proxy_command", requestParameters).Result;


            // (5)sign_data
            byte[] dataForSignInDase64 = Encoding.ASCII.GetBytes(dataToSign);
            requestParameters = new Dictionary<string, string>
            {
                { "card_response", $"{response.CardResponse}" },
                { "hreq", $"{hreq}" },
                { "data_to_sign", $"{dataForSignInDase64}" }
            };
            response = _postHandlerRequest.PostHandlerRequest($"{_idCardOptions.Value.TerminalAdress}{_idCardOptions.Value.Version}sign_data", requestParameters).Result;


            // (6)terminal_proxy_command
            requestParameters = new Dictionary<string, string>
            {
                { "header_cmd_to_card", $"{response.HeaderCmdToCard}" },
                { "cmd_to_card", $"{response.CmdToCard}" }
            };
            response = _postHandlerRequest.PostHandlerRequest($"{_idCardOptions.Value.CpAdress}{_idCardOptions.Value.Version}terminal_proxy_command", requestParameters).Result;

            // (7)sign_result
            requestParameters = new Dictionary<string, string>
            {
                { "card_response", $"{response.CardResponse}" },
                { "hreq", $"{hreq}" }
            };
            response = _postHandlerRequest.PostHandlerRequest($"{_idCardOptions.Value.TerminalAdress}{_idCardOptions.Value.Version}sign_result", requestParameters).Result;

            return response.Signature;
        }
    }
}
