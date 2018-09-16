/* CountryMetadata.cs
 *      Metadata class for Country Model, Assignment 3                    
 * 
 * Revision History
 *      Singithi Kandage, 2017.12.10 : Started
 *      Singithi Kandage, 2017.12.18 : Completed
*/

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SKClassLibrary;

namespace SKOEC.Models
{
    //Country Partial Class
    [ModelMetadataType(typeof(CountryMetadata))]
    public partial class Country : IValidatableObject
    {
        OECContext _context = OECContext_Singleton.Context();

        //Validate Method
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }       
    }

    //Country Metadata class
    public class CountryMetadata
    {

        //Constructor for CountryMetadata
        public CountryMetadata()
        {
            Province = new HashSet<Province>();
        }

        [Required]
        [Display(Name="Country Code")]
        public string CountryCode { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Postal Pattern")]
        public string PostalPattern { get; set; }

        [Display(Name = "Phone Pattern")]
        public string PhonePattern { get; set; }

        public ICollection<Province> Province { get; set; }

    }
}
