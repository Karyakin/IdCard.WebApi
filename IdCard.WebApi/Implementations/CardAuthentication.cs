using System.Collections.Generic;
using IdCard.WebApi.Helpers;
using IdCard.WebApi.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace IdCard.WebApi.Implementations
{
    public class CardAuthentication : ICardAuthentication
    {
        private readonly IPostHandler _postHandlerRequest;
        private readonly IOptions<IdCardSettings> _idCardOptions;

        public CardAuthentication(IPostHandler postHandlerRequest, IOptions<IdCardSettings> idCardOptions)
        {
            _postHandlerRequest = postHandlerRequest;
            _idCardOptions = idCardOptions;
        }


        public JToken Bauth()
        {
            //TODO проверка на наличие коробки

            // (1) auth_sign
            var requestParameter = new Dictionary<string, string>
            {
                { "init", "true" }
            };
            var response = _postHandlerRequest.PostHandlerRequest($"{_idCardOptions.Value.CpAdress}auth_sign", requestParameter).Result;

            // (2) bauth_init
            var bauthInitRequest = new Dictionary<string, string>
            {
                 { "so_certificate", $"{response?.SoCert}" }
            };
            response = _postHandlerRequest.PostHandlerRequest($"{_idCardOptions.Value.TerminalAdress}{_idCardOptions.Value.Version}bauth_init", bauthInitRequest).Result;
            var hreq = response?.Hreq;

            // (3) terminal_proxy_bauth_init
            var terminalProxyBauthInitRequest = new Dictionary<string, string>
            {
                { "terminal_certificate", $"{response?.TerminalCertificate}" },
                { "cmd_to_card", $"{response?.CmdToCard}" }

            };
            response = _postHandlerRequest.PostHandlerRequest($"{_idCardOptions.Value.CpAdress}{_idCardOptions.Value.Version}terminal_proxy_bauth_init", terminalProxyBauthInitRequest).Result;

            // (4-5) bauth_process, terminal_proxy_bauth
            do
            {
                var bauthProcessRequest = new Dictionary<string, string>
                {
                    { "hreq", $"{hreq}" },
                    { "card_response", $"{response?.CardResponse}" }
                };
                response = _postHandlerRequest.PostHandlerRequest($"{_idCardOptions.Value.TerminalAdress}{_idCardOptions.Value.Version}bauth_process", bauthProcessRequest).Result;

                if (response?.IsBauthEstablished is true)
                {
                    break;
                }

                var terminalProxyBauthRequest = new Dictionary<string, string>
                {
                    { "header_cmd_to_card", $"{response?.HeaderCmdToCard}" },
                    { "cmd_to_card", $"{response?.CmdToCard}" }
                };
                response = _postHandlerRequest.PostHandlerRequest($"{_idCardOptions.Value.CpAdress}{_idCardOptions.Value.Version}terminal_proxy_bauth", terminalProxyBauthRequest).Result;

            } while (true);

            //todo если придет null вернуть исключение "Получение данных не удалось"
            return hreq;
        }
    }
}
