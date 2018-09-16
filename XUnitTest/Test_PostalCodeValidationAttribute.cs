using Microsoft.EntityFrameworkCore;
using SKOEC.Models;
using SKOEC.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using SKClassLibrary;

namespace XUnitTest
{
    // Verify the PostalCodeValidationAttribute works according to specifications
    public class Test_PostalCodeValidationAttribute
    {
        // Create a subclass of the target ValidationAttribute ... with an IoC-inspired method
        // (call it and it'll create its own required ValidationContext parameter)
        // - a ValidationContext needs a DisplayName value at minimum
        public class Local_PostalCodeValidationAttribute : SKPostalCodeValidationAttribute
        {
            public ValidationResult Run_IsValid(object value)
            {
                return IsValid(value, new ValidationContext(new { DisplayName = "fred" }));
            }
        }

        // now, create an instance of the above subclass for the tests
        Local_PostalCodeValidationAttribute validationInstance = new Local_PostalCodeValidationAttribute();

        [Fact]
        public void Empty_ShouldBeAcceptable()
        {
            Assert.Equal(ValidationResult.Success, validationInstance.Run_IsValid(""));
        }

        [Fact]
        public void Null_ShouldBeAcceptable()
        {
            Assert.Equal(ValidationResult.Success, validationInstance.Run_IsValid(null));
        }

        [Theory]
        [InlineData("A1A1A1")]
        [InlineData("A1A 1A1")]
        [InlineData("a1a1a1")]
        [InlineData("a1a 1a1")]
        [InlineData("M9A9A9")]
        [InlineData("m9A 9A9")]
        public void AcceptableInputs(string value)
        {
            Assert.Equal(ValidationResult.Success, validationInstance.Run_IsValid(value));
        }

        [Theory]
        [InlineData("D1A 1A1")]
        [InlineData("W1A 1A1")]
        [InlineData("Z1A 1A1")]
        [InlineData("A1F 1A1")]
        [InlineData("A1I 1A1")]
        [InlineData("A1O 1A1")]
        [InlineData("A1A 1Q1")]
        [InlineData("A1A 1U1")]
        public void UnacceptableLetters(string value)
        {
            Assert.NotEqual(ValidationResult.Success, validationInstance.Run_IsValid(value));
        }
    }
}
