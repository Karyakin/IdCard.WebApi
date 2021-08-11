using Newtonsoft.Json.Linq;

namespace IdCard.WebApi.Interfaces
{
   public interface ICardAuthentication
   {
       public JToken Bauth();
   }
}
