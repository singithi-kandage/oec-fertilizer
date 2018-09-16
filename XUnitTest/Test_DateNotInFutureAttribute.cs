using System;
using System.ComponentModel.DataAnnotations;
using Xunit;
using SKClassLibrary;

namespace XUnitTest
{
    // Verify the DateNotInFutureAttribute works according to specifications
    public class Test_DateNotInFutureAttribute
    {
        // Create a subclass of the target ValidationAttribute ... with an IoC-inspired method
        // (call it and it'll create its own required ValidationContext parameter)
        // - a ValidationContext needs a DisplayName value at minimum
        public class Local_DateNotInFutureAttribute : SKDateNotInFutureAttribute
        {
            public ValidationResult Run_IsValid(object value)
            {
                return IsValid(value, new ValidationContext(new { DisplayName = "fred" }));
            }
        }

        // now, create an instance of the above subclass for the tests
        Local_DateNotInFutureAttribute validationInstance = new Local_DateNotInFutureAttribute();

        [Fact]
        public void Null_ShouldBeAcceptable()
        {
            Assert.Equal(ValidationResult.Success, validationInstance.Run_IsValid(null));
        }

        [Fact]        
        public void DateInFuture_IsNotAcceptable()
        {
            Assert.NotEqual(ValidationResult.Success, validationInstance.Run_IsValid(DateTime.Now.AddHours(1)));
        }

        [Fact]
        public void Now_ShouldBeAcceptable()
        {
            Assert.Equal(ValidationResult.Success, validationInstance.Run_IsValid(DateTime.Now));
        }

        [Fact]
        public void PastDate_ShouldBeAcceptable()
        {
            Assert.Equal(ValidationResult.Success, validationInstance.Run_IsValid(DateTime.Now.AddMonths(-1)));
        }
    }
}

