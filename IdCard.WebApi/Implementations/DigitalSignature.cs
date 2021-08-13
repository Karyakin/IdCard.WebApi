using System;
using System.Collections.Generic;
using System.Text;
using IdCard.WebApi.Helpers;
using IdCard.WebApi.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Serilog;

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
            if (hreq is null)
            {
                Log.Error($"Parameter \"hreq\" can't be null. Method \"{nameof(GetDigitalSignature)}\", class \"{nameof(DigitalSignature)}\"");
                throw new NullReferenceException(nameof(hreq));
            }
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
            //var dataForSignInDase64 = Convert.ToBase64String(Encoding.GetEncoding(1251).GetBytes(dataToSign));

           var strModified = Convert.ToBase64String(Encoding.UTF8.GetBytes(dataToSign));

            requestParameters = new Dictionary<string, string>
            {
                { "card_response", $"{response.CardResponse}" },
                { "hreq", $"{hreq}" },
                //{ "data_to_sign", $"{dataForSignInDase64}" }
                //{ "data_to_sign", "8169C9F8-46F4-41A5-82A6-D85CEDB8023F" }
                { "data_to_sign", strModified }
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
