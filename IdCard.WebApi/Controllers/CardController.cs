using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IdCard.WebApi.Entities;
using IdCard.WebApi.Interfaces;

namespace IdCard.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardController : ControllerBase
    {
        private readonly ICardAuthentication _cardAuthentication;
        private readonly IDataGroupe _dataGroupe;
        private readonly IDigitalSignature _digitalSignature;

        public CardController(ICardAuthentication cardAuthentication, IDataGroupe dataGroupe, IDigitalSignature digitalSignature)
        {
            _cardAuthentication = cardAuthentication;
            _dataGroupe = dataGroupe;
            _digitalSignature = digitalSignature;
        }


        [HttpGet]
        public async Task<IActionResult> DataGroupe()
        {
            var hreq = _cardAuthentication.Bauth();

            if (hreq is null)
            {
                return BadRequest("Could not identify card. Details in IdCardLog.txt");
            }

            var dataGroupe = await _dataGroupe.GetDataGroupe(hreq);
            if (dataGroupe is null) 
            {
                return BadRequest("Failed to get group data. Details in IdCardLog.txt");
            }

            return Ok(dataGroupe);
        }


        [HttpPost]
        public IActionResult SignData([FromBody] JsonData dataToSign)
        {

            if (dataToSign.DataToSign is null)
            {
                return BadRequest("Enter the data you want to sign");
            }
            var hreq = _cardAuthentication.Bauth();

            if (hreq is null)
            {
                return BadRequest("Could not identify card. Details in IdCardLog.txt");
            }

            var digitalSignature = _digitalSignature.GetDigitalSignature(hreq, dataToSign.DataToSign);
            if (digitalSignature is null)
            {
                return BadRequest("FSailed to sign data. Details in IdCardLog.txt");
            }

            return Ok(digitalSignature);
        }

    }
}

