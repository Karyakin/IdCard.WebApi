using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IdCard.WebApi.Entities
{
    public class JsonData
    {
     
        [JsonProperty("personal_data")]
        public string PersonalData { get; set; }

        [JsonProperty("SO_CERT")]
        public string SoCert { get; set; }
        
        private string _hreq;
        [JsonProperty("hreq")]
        public string Hreq
        {
            get => _hreq;
            set
            {
                if (value != null)
                    _hreq = value;
            }
        }
        
        [JsonProperty("terminal_certificate")]
        public string TerminalCertificate { get; set; }

        [JsonProperty("cmd_to_card")]
        public string CmdToCard { get; set; }

        [JsonProperty("card_response")]
        public string CardResponse { get; set; }

        [JsonProperty("header_cmd_to_card")]
        public string HeaderCmdToCard { get; set; }

        [JsonProperty("is_bauth_established")]
        public bool IsBauthEstablished { get; set; }

        [JsonProperty("is_last_dg_readed")]
        public bool IsLastDgReaded { get; set; }

        [JsonProperty("err")]
        public string Error { get; set; }


      





    }
}
