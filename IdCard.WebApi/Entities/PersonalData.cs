using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IdCard.WebApi.Entities
{
    public class PersonalData
    {

        [JsonProperty("err")]
        public string Error { get; set; }

        [JsonProperty("idcard")]
        public bool IdCard { get; set; }



        [JsonProperty("personal_data")]
        public Person Person{ get; set; }
















/*
        [JsonProperty("BE:_Family_name")]
        public string FamilyName { get; set; }

        [JsonProperty("BE:_Given_name")]
        public string GivenName { get; set; }

        [JsonProperty("BE:_Middle_name")]
        public string MiddleName { get; set; }

        [JsonProperty("BE_Place_of_birth")]
        public string PlaceOfBirth { get; set; }

        [JsonProperty("Citizenship")]
        public string Citizenship { get; set; }

        [JsonProperty("Date_of_expiry")]
        public string DateOfExpiry { get; set; }// may be datatime

        [JsonProperty("Date_of_issuance")]
        public string DateOfIssuance { get; set; }// may be datatime

        [JsonProperty("ID")]
        public string Id { get; set; }

        [JsonProperty("Issuance_board")]
        public string IssuanceBoard { get; set; }

        [JsonProperty("Issuing_State")]
        public string IssuingState { get; set; }

        [JsonProperty("LA:_Family_name")]
        public string LaFamilyName { get; set; }

        [JsonProperty("LA:_Given_name")]
        public string LaGivenName { get; set; }

        [JsonProperty("RU:_Family_name")]
        public string RuFamilyName { get; set; }

        [JsonProperty("RU:_Given_name")]
        public string RuGivenName { get; set; }

        [JsonProperty("RU:_Middle_name")]
        public string RuMiddleName { get; set; }

        [JsonProperty("RU_Place_of_birth")]
        public string RuPlaceOfBirth { get; set; }

        [JsonProperty("Serial_Number")]
        public string SerialNumber { get; set; }

        [JsonProperty("Sex")]
        public string Sex { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("birth_date")]
        public string BirthDate { get; set; }*/
    }
}
