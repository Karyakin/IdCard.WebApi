using Newtonsoft.Json.Linq;

namespace IdCard.WebApi.Interfaces
{
    public interface IDigitalSignature
    {
        public string GetDigitalSignature(JToken hreq, string dataToSign);
    }
}
