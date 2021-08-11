using IdCard.WebApi.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdCard.WebApi.Interfaces
{
    public interface IDataGroupe
    {
        public Task<PersonalData> GetDataGroupe(JToken hreq);
    }
}
