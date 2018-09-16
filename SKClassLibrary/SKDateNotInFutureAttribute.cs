using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SKClassLibrary
{
    public class SKDateNotInFutureAttribute : ValidationAttribute
    {
        public SKDateNotInFutureAttribute()
        {
            ErrorMessage = "{0} is an invalid date. Cannot be in the the future. ";
        }

        protected override ValidationResult IsValid (object value, ValidationContext validationContext)
        {
            if (Convert.ToDateTime(value) != null)
            {
                if (Convert.ToDateTime(value) > DateTime.Now)
                {
                    return new ValidationResult(String.Format(ErrorMessage, validationContext.DisplayName));
                }

            }

            return ValidationResult.Success;
        }

    }
}
