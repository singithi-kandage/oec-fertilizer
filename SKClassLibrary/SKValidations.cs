using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace SKClassLibrary
{
    public class SKValidations : ValidationAttribute
    {
        public static string SKCapitalize(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString) == false)
            {
                inputString = inputString.Trim();
                inputString = inputString.ToLower();
                inputString = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(inputString);
            }

            return inputString;
        }

        public static bool SKPostalCodeValidation(ref string inputString) 
        {
            string postalCodeString = "";

            if (string.IsNullOrEmpty(inputString) == true)
            {
                return true;
            }
            else
            {
                foreach (char c in inputString)
                {
                    if (char.IsLetterOrDigit(c))
                    {
                        postalCodeString += c;
                    }
                }


                if (postalCodeString.Length != 6)
                {
                    return false;
                }
                else
                {
                    Regex pattern = new Regex(@"[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ] ?[0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]", RegexOptions.IgnoreCase);

                    if (pattern.IsMatch(postalCodeString.ToString()) == true)
                    {
                        if (postalCodeString.Length == 6)
                        {
                            inputString = postalCodeString.Insert(3, " "); 
                        }

                        inputString = inputString.ToUpper();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
        }

        public static bool SKZipCodeValidation(ref string inputString)
        {
            string postalCodeString = "";

            if (string.IsNullOrEmpty(inputString) == true)
            {
                return true;
            }
            else
            {
                foreach (char c in inputString)
                {
                    if (char.IsDigit(c))
                    {
                        postalCodeString += c;
                    }
                }

                if (postalCodeString.Length != 5 && postalCodeString.Length != 9)
                {
                    return false;
                }
                else
                {
                    Regex pattern = new Regex(@"^\d{5}(?:\d{4})??$");
                    if (pattern.IsMatch(postalCodeString.ToString()) == true)
                    {
                        if (postalCodeString.Length == 5)
                        {
                            inputString = String.Format("{0:#####}", int.Parse(postalCodeString));
                        }
                        if (postalCodeString.Length == 9)
                        {
                            inputString = String.Format("{0:#####-####}", int.Parse(postalCodeString));
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                       
                }
               
            }
           
        }
    }
}
