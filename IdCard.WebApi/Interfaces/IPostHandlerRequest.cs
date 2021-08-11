using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdCard.WebApi.Entities;

namespace IdCard.WebApi.Interfaces
{
   public interface IPostHandlerRequest
   {
       public Task<JsonData> PostHandler(string requestUri, Dictionary<string, string> requestParameters);
   }
}
