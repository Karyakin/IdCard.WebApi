using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdCard.WebApi.Entities;

namespace IdCard.WebApi.Interfaces
{
   public interface IPostHandler
   {
       public Task<JsonData> PostHandlerRequest(string requestUri, Dictionary<string, string> requestParameters);
       public Task<PersonalData> PostHandlerDataGroupeRequest(string requestUri, Dictionary<string, string> requestParameters); 
    }
}
