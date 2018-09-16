/* FarmMetadata.cs
 *      Metadata class for Farm Model, Assignment 3                    
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
using System.Text.RegularExpressions;

namespace SKOEC.Models
{
    //Farm Partial Class
    [ModelMetadataType(typeof(FarmMetadata))]
    public partial class Farm : IValidatableObject
    {
        //Validate Method
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            OECContext _context = OECContext_Singleton.Context();


            //Trim all strings of leading and trailing spaces
            if (string.IsNullOrEmpty(Name) == false || string.IsNullOrWhiteSpace(Name))
            {
                Name = Name.Trim();
                //Use SKValidations.SKCapitalize to capitalize Name
                Name = SKValidations.SKCapitalize(Name);
            }
            if (Name == "")
            {
                yield return new ValidationResult(
                    "Name cannot be an empty string", new string[] { nameof(Name) });
            }


            if (string.IsNullOrEmpty(Address) == false)
            {
                Address = Address.Trim();
                //Use SKValidations.SKCapitalize to capitalize Address
                Address = SKValidations.SKCapitalize(Address);
            }
            if (string.IsNullOrEmpty(Town) == false) {

                Town = Town.Trim();
                //Use SKValidations.SKCapitalize to capitalize Town
                Town = SKValidations.SKCapitalize(Town);
            }
            if (string.IsNullOrEmpty(County) == false)
            {
                County = County.Trim();
                //Use SKValidations.SKCapitalize to capitalize County
                County = SKValidations.SKCapitalize(County);

            }
            if (string.IsNullOrEmpty(ProvinceCode) == false)
            {
                ProvinceCode = ProvinceCode.Trim();

                Regex pattern = new Regex(@"^[a-zA-Z]{2}$");

                //Force ProvinceCode to upper before writing to database
                ProvinceCode = ProvinceCode.ToUpper();
            }

            if (string.IsNullOrEmpty(PostalCode) == false)
            {
                PostalCode = PostalCode.Trim();
            }
            if (string.IsNullOrEmpty(HomePhone) == false)
            {
                HomePhone = HomePhone.Trim();
            }
            if (string.IsNullOrEmpty(CellPhone) == false)
            {
                CellPhone = CellPhone.Trim();
            }
            if (string.IsNullOrEmpty(Email) == false)
            {
                Email = Email.Trim();
            }
            if (string.IsNullOrEmpty(Directions) == false)
            {
                Directions = Directions.Trim();
            }

            //Either town or county must be provided, both are ok, but not necessary
            if (string.IsNullOrEmpty(Town) == true && string.IsNullOrEmpty(County) == true)
            {
                yield return new ValidationResult(
                   "At least one of Town or County must be provided.", new string[] { nameof(Town), nameof(County) });
            }

            //If email is not provided, address and postal code must be provided
            if (string.IsNullOrEmpty(Email) == true && 
                (string.IsNullOrEmpty(Address) == true || string.IsNullOrEmpty(PostalCode) == true))
            {
                yield return new ValidationResult(
                   "Either email, or both address and postal code must be provided.", new string[] { nameof(Email), nameof(Address), nameof(PostalCode) });
            }


            //Validate Postal Code
            var country = _context.Province.SingleOrDefault(p => p.ProvinceCode == ProvinceCode);
            string countryCode = country.CountryCode;
            bool isValid = true;
            string postalCode = PostalCode;

            //Validate Canadian Postal Code
            if (countryCode == "CA")
            {
                postalCode = postalCode.Trim();

                isValid = SKValidations.SKPostalCodeValidation(ref postalCode);

                if (isValid == false)
                {
                    yield return new ValidationResult(
                    "Postal (Zip) Code is not a valid Canadian pattern: A6A 6A6 or A6A6A6", new string[] { nameof(PostalCode) });
                }
                else
                {
                    PostalCode = postalCode;
                }
            }
            // Validate US Zip Code
            else if (countryCode == "US")
            {
                isValid = SKValidations.SKZipCodeValidation(ref postalCode);

                if (isValid == false)
                {
                    yield return new ValidationResult(
                    "Postal (Zip) Code is not a valid US pattern: 12345 or 12345-1234", new string[] { nameof(PostalCode) });
                }
                else
                {
                    PostalCode = postalCode;
                }
            }              


            //If both home and cell phone not provided
            if (string.IsNullOrEmpty(HomePhone) == true && string.IsNullOrEmpty(CellPhone) == true)
            {
                yield return new ValidationResult(
                      "Either one of Home Phone or Cell Phone is required", new string[] { nameof(HomePhone), nameof(CellPhone) });
            }

            //If home phone provided 
            if (string.IsNullOrWhiteSpace(HomePhone) == false)
            {

                //Extract phone number 
                string numString = "";

                foreach (char c in HomePhone)
                {
                    if (char.IsDigit(c))
                    {
                        numString += c;
                    }
                }

                //Check if phone number is 10 digits long
                if (numString.Length != 10)
                {
                    yield return new ValidationResult(
                      $"Not a valid phone number, must be 10 digits.", new string[] { nameof(HomePhone) });
                }
                //Format Cell Phone number before writing to Database
                else
                {
                    HomePhone = String.Format("{0:###-###-####}", double.Parse(numString));
                }
                
            }

            if (string.IsNullOrWhiteSpace(CellPhone) == false)
            {
                //Check if phone number is 10 digits long
                string numString = "";

                foreach (char c in CellPhone)
                {
                    if (char.IsDigit(c))
                    {
                        numString += c;
                    }
                }

                if (numString.Length != 10)
                {
                    yield return new ValidationResult(
                      $"Not a valid phone number, must be 10 digits.", new string[] { nameof(CellPhone) });
                }
                //Format Cell Phone number before writing to Database
                else
                {
                    CellPhone = String.Format("{0:###-###-####}", double.Parse(numString));
                }
            }

            //Last Contact Date cannot be provided unless DateJoined is available (but the reverse is allowed)
            if (LastContactDate != null && DateJoined == null)
            {
                yield return new ValidationResult(
                     "Last Contact date cannot be provided unless Date Joined is available", new string[] { nameof(LastContactDate) });
            }

            //A farmer cannot be contacted before they have joined the program.
            if (DateJoined != null && LastContactDate != null && (DateJoined > LastContactDate))
            {
                yield return new ValidationResult(
                     "A farmer cannot be contacted before they have joined the program.", new string[] { nameof(LastContactDate) });
            }

            yield return ValidationResult.Success;
        }
    }

    //Farm Metadata class
    public class FarmMetadata 
    {
        //Constructor for FarmMetadata
        //public FarmMetadata()
        //{
        //    Plot = new HashSet<Plot>();
        //}

        public int FarmId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Address { get; set; }

        public string Town { get; set; }

        public string County { get; set; }

        [Display(Name="Province")]
        [Remote("ValidateProvinceCode", "Remotes")]
        [RegularExpression(@"^[a-zA-Z]{2}$", ErrorMessage = "Province/State must be exactly 2 LETTERS")]
        [Required]
        public string ProvinceCode { get; set; }
        
        public string PostalCode { get; set; }

        [Display(Name = "Home Phone")]
        [DataType(DataType.PhoneNumber)]
        public string HomePhone { get; set; }

        [Display(Name = "Cell Phone")]
        [DataType(DataType.PhoneNumber)]
        public string CellPhone { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Directions { get; set; }

        [Display(Name = "Date Joined")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [SKDateNotInFuture]
        public DateTime? DateJoined { get; set; }

        [Display(Name = "Last Contact")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [SKDateNotInFuture]
        public DateTime? LastContactDate { get; set; }

        [Display(Name = "Province Code")]
        public Province ProvinceCodeNavigation { get; set; }
        public ICollection<Plot> Plot { get; set; }


    }
}
