using Newtonsoft.Json.Linq;

namespace IdCard.WebApi.Interfaces
{
   public interface IIdCardAuthentication
   {
       public JToken Bauth();
   }
}
