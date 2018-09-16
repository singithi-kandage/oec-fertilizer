/* ProvinceMetadata.cs
 *      Metadata class for Province Model, Assignment 3                    
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
    //Province Partial Class
    [ModelMetadataType(typeof(ProvinceMetadata))]
    public partial class Province : IValidatableObject
    {
        OECContext _context = OECContext_Singleton.Context();

        //Validate Method
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }       
    }

    //Province Metadata class
    public class ProvinceMetadata
    {

        //Constructor for ProvinceMetadata
        public ProvinceMetadata()
        {
            Farm = new HashSet<Farm>();
        }

        [Required]
        [Display(Name = "Province Code")]
        public string ProvinceCode { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }

        [Display(Name = "Country Code")]
        public Country CountryCodeNavigation { get; set; }

        public ICollection<Farm> Farm { get; set; }
    }
}
