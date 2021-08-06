using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdCard.WebApi.Controllers
{
    public class IdCard : ControllerBase
    {
        



    }
}
/* [HttpPost]
        [ClientAjaxErrorHandler]
        public dynamic GetAllFromIDCard(string tempKey)
        {
            //pin1 533790
            //pin2 7896313
            //can 042465

            string cpAdress = "http://192.168.5.100:8084/";
            //string cpAdress = "http://localhost:8084/";
            string terminalAdress = "http://192.168.5.135:48777/";
            string version = "api/v1/";
            string json;
            //string json = "{\"param1\":\"val\",\"param2\":\"val\"}";


            //********** авторизация bauth **********
            //1)
            json = "{\"init\":\"true\"}";
            var res = postRequest(cpAdress + "data_groups", json);
            //string so_sert = "MIIDLDCCAuagAwIBAgIMQOVqrk7w/xsAAOe4MA0GCSpwAAIAImUtDAUAMIGrMTswOQYDVQQDHjIEIAQ1BEEEPwRDBDEEOwQ4BDoEMAQ9BEEEOgQ4BDkAIAQjBCYAIAAoBEIENQRBBEIAKTFdMFsGA1UECh5UBCAEIwQfACAAIgQdBDAERgQ4BD4EPQQwBDsETAQ9BEsEOQAgBEYENQQ9BEIEQAAgBE0EOwQ1BDoEQgRABD4EPQQ9BEsERQAgBEMEQQQ7BEMEMwAiMQ0wCwYDVQQGHgQAQgBZMB4XDTIwMDEzMTA3NDMyM1oXDTI1MDEzMDIwNTk1OVowYDFRME8GA1UECh5IBB0EJgQtBCMAIAQ+BD8ENQRABDAEQgQ+BEAAIAQ/BEAEPgQzBEAEMAQ8BDwEPQQ+BDMEPgAgBDoEPgQ8BD8EOwQ1BDoEQQQwMQswCQYDVQQGEwJCWTBdMBgGCipwAAIAImUtAgEGCipwAAIAImUtAwEDQQBHIdsG/m6/HzWM4tuLopZxC+w2ar5sr6bJl5v7fhAlbG131N64kUHibLTVu/qTQzOnwdCK/Go8h+OCPFfj5gHYo4IBMTCCAS0wHwYDVR0jBBgwFoAU1J/YfkmYoODPfkzHuOthdSTEz7YwCQYDVR0TBAIwADAtBgNVHR8EJjAkMCKgIKAehhxodHRwOi8vY3JsLW9jc3Avc3ViY2FSVUMuY3JsMGcGCCsGAQUFBwEBBFswWTArBggrBgEFBQcwAYYfaHR0cDovL2NybC1vY3NwOjgwODIvcmVzcG9uZGVyLzAqBggrBgEFBQcwAoYeaHR0cDovL2NybC1vY3NwL2Nlci9yb290Y2EuY2VyMAsGA1UdDwQEAwIF4DAdBgNVHQ4EFgQUeJOe8Vvhxqui/5WmKWzvzsL+gdwwIQYJKnABAgEBAQECBBQeEgAxADkAMQA3ADAAMAAxADYAMTAYBgNVHSAEETAPMA0GCypwAQIBAQEDAgEEMA0GCSpwAAIAImUtDAUAAzEAevHlcyHwZSKACUCdB/8gAsyTMT1cNxisiTHYAF4PVy5zfm0O5oQ5QPGUPGdivxcT";
            string so_sert = res["SO_CERT"];

            //2)
            json = "{" +
                String.Format("\"so_certificate\":\"{0}\"", so_sert)
                + "}";
            res = postRequest(terminalAdress + version + "bauth_init", json);
            string term_cert = res["terminal_certificate"];
            string cmd_to_card = res["cmd_to_card"];
            string hreq = res["hreq"];

            //3)
            json = "{" +
                String.Format("\"terminal_certificate\":\"{0}\",", term_cert) +
                String.Format("\"cmd_to_card\":\"{0}\"", cmd_to_card) +
                "}";
            res = postRequest(cpAdress + version + "terminal_proxy_bauth_init", json);
            string card_responce = res["card_response"];

            string header;
            while (true)
            {
                //4)
                json = "{" +
                    String.Format("\"hreq\":\"{0}\",", hreq) +
                    String.Format("\"card_response\":\"{0}\"", card_responce) +
                    "}";
                res = postRequest(terminalAdress + version + "bauth_process", json);
                header = res["header_cmd_to_card"];
                cmd_to_card = res["cmd_to_card"];

                var status = res["is_bauth_established"];
                //Console.WriteLine(status);
                if (status == "True")
                {
                    break;
                }

                //5)
                json = "{" +
                    String.Format("\"header_cmd_to_card\":\"{0}\",", header) +
                    String.Format("\"cmd_to_card\":\"{0}\"", cmd_to_card) +
                    "}";
                res = postRequest(cpAdress + version + "terminal_proxy_bauth", json);
                card_responce = res["card_response"];
            }


            //********** получение групп данных **********
            //1)
            json = "{" +
                    String.Format("\"hreq\":\"{0}\",", hreq) +
                    "\"data_groups_to_read\":[ \"dg1\", \"dg2\", \"dg3\", \"dg4\", \"dg5\" ]" +
                    "}";
            res = postRequest(terminalAdress + version + "read_dg_init", json);
            header = res["header_cmd_to_card"];
            cmd_to_card = res["cmd_to_card"];

            //2)
            json = "{" +
                    String.Format("\"header_cmd_to_card\":\"{0}\",", header) +
                    String.Format("\"cmd_to_card\":\"{0}\"", cmd_to_card) +
                    "}";
            res = postRequest(cpAdress + version + "terminal_proxy_command", json);
            card_responce = res["card_response"];

            while (true)
            {
                //3)
                json = "{" +
                        String.Format("\"card_response\":\"{0}\",", card_responce) +
                        String.Format("\"hreq\":\"{0}\"", hreq) +
                        "}";
                res = postRequest(terminalAdress + version + "read_dg", json);
                header = res["header_cmd_to_card"];
                cmd_to_card = res["cmd_to_card"];
                var status = res["is_last_dg_readed"];
                //Console.WriteLine(status);
                if (status == "True")
                {
                    break;
                }

                //4)
                json = "{" +
                        String.Format("\"header_cmd_to_card\":\"{0}\",", header) +
                        String.Format("\"cmd_to_card\":\"{0}\"", cmd_to_card) +
                        "}";
                res = postRequest(cpAdress + version + "terminal_proxy_command", json);
                card_responce = res["card_response"];
            }

            //5)
            json = "{" +
                    String.Format("\"hreq\":\"{0}\"", hreq) +
                    "}";
            res = postRequest(terminalAdress + version + "request_dg", json);
            var data = res["personal_data"];
            //Console.WriteLine(data);
            var dataForSign = tempKey;
                //data["ID"] + " " + 
                //data["birth_date"] + " " + 
                //data["RU:_Family_name"] + " " + 
               // data["RU:_Given_name"] + " " + 
                //data["RU:_Middle_name"];
            //Console.WriteLine(data);
            //return;


            //********** выработка эцп **********
            //1)
            json = "{" +
                    String.Format("\"hreq\":\"{0}\"", hreq) +
                    "}";
            res = postRequest(terminalAdress + version + "sign_init", json);
            header = res["header_cmd_to_card"];
            cmd_to_card = res["cmd_to_card"];

            //2)
            json = "{" +
                    String.Format("\"header_cmd_to_card\":\"{0}\",", header) +
                    String.Format("\"cmd_to_card\":\"{0}\"", cmd_to_card) +
                    "}";
            res = postRequest(cpAdress + version + "terminal_proxy_sign_init", json);
            card_responce = res["card_response"];

            //3)
            json = "{" +
                    String.Format("\"card_response\":\"{0}\",", card_responce) +
                    String.Format("\"hreq\":\"{0}\"", hreq) +
                    "}";
            res = postRequest(terminalAdress + version + "sign_select_app", json);
            header = res["header_cmd_to_card"];
            cmd_to_card = res["cmd_to_card"];

            //4)
            json = "{" +
                    String.Format("\"header_cmd_to_card\":\"{0}\",", header) +
                    String.Format("\"cmd_to_card\":\"{0}\"", cmd_to_card) +
                    "}";
            res = postRequest(cpAdress + version + "terminal_proxy_command", json);
            card_responce = res["card_response"];

            //5)
            json = "{" +
                    String.Format("\"card_response\":\"{0}\",", card_responce) +
                    String.Format("\"hreq\":\"{0}\",", hreq) +
                    String.Format("\"data_to_sign\":\"{0}\"", dataForSign) +
                    "}";
            res = postRequest(terminalAdress + version + "sign_data", json);
            header = res["header_cmd_to_card"];
            cmd_to_card = res["cmd_to_card"];

            //6)
            json = "{" +
                    String.Format("\"header_cmd_to_card\":\"{0}\",", header) +
                    String.Format("\"cmd_to_card\":\"{0}\"", cmd_to_card) +
                    "}";
            res = postRequest(cpAdress + version + "terminal_proxy_command", json);
            card_responce = res["card_response"];

            //7)
            json = "{" +
                    String.Format("\"card_response\":\"{0}\",", card_responce) +
                    String.Format("\"hreq\":\"{0}\"", hreq) +
                    "}";
            res = postRequest(terminalAdress + version + "sign_result", json);
            //var signature = res["signature"];
            //Console.WriteLine(signature);

            data.Merge(res, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });

            return res;
        }
        

        static dynamic postRequest(string method, string json, bool print = false)
        {
            var request = WebRequest.Create(method);
            request.ContentType = "application/json";
            request.Method = "POST";
            //request.Timeout = 10 * 1000;

            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string result = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                result = reader.ReadToEnd();
            }
            if (print)
            {
                Console.WriteLine(result);
            }
            return JsonConvert.DeserializeObject(result);
        }*/