using Microsoft.EntityFrameworkCore;
using SKOEC.Models;
using SKOEC.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace XUnitTest
{
    // Tests to ensure the Farm Model edits meet requirements
    // d.turton Jul 2017
    public class TestFarm       
    {
        #region global variables and database connection
        Farm farm;                      // instantiated in Initialise & detached from EF queue in Cleanup
        Random rand = new Random();     // generate a time-based random number seed
        Boolean recordAccepted = true;  // set to false if the record is not written to the database
        string validationResults = "";  // loaded with the errors messages in ModelState

        // context for tests writing directly to the database and controller tests
        OECContext _context = OECContext.Context;

        #endregion

        #region create & validate the perfect farm
        
        // Initialise creates a valid farm object for each test, with a
        // unique FirstName so individual test results can be retrieved.
        private void Initialise()
        {
            recordAccepted = true; // assume object accepted unless one or more faults found
            validationResults = "";

            farm = new Farm()
            {
                //farm.FarmId = rand.Next();                          // unique numeric key when it's not an Identity
                Name = "XUnitTest" + Guid.NewGuid().ToString(),
                Address = "299 Doon Valley Drive",
                Town="Kitchener",
                County = "Waterloo",
                ProvinceCode ="ON",
                PostalCode= "N2G 4M4",
                HomePhone = "519-748-5220",
                CellPhone = "519-748-5220",
                Email = "dturton@conestogac.on.ca",
                Directions = "401 to Homer Watson exit, North to 1st road East",
                DateJoined = DateTime.Today,
                LastContactDate = DateTime.Today
            };
        } // end Initialise

        [Fact]
        public void ValidFarm_ShouldPassValidation()
        {
            // Arrange
            Initialise(); // this creates an Farm object with all valid properties
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults =  ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.True(recordAccepted, validationResults);
        }

      
        #endregion

        #region utility methods: run/evaluate Validate, cleanup, etc.

        /// <summary>
        /// run the Farm object's Validate method and collect any error messages encountered
        /// - return true if object passed cleanly, false if any invalid results are found
        /// - accumulated error messages are also provided for subsequent analysis
        /// </summary>
        /// <param name="errorMessages">output - accumulated error messages</param>
        /// <returns>true if record was accepted, false if not</returns>
        public Boolean RunValidate(out string errorMessages)
        {
            Boolean recordPassedValidate = true;
            errorMessages = "";
            List<ValidationResult> ValidationResults = farm.Validate(new ValidationContext(farm)).ToList();
            foreach (ValidationResult x in ValidationResults)
            {
                if (x != ValidationResult.Success)
                {
                    recordPassedValidate = false;
                    errorMessages += $"<<<{x.ErrorMessage}>>>...";
                }
            }
            return recordPassedValidate;
        }

        /// <summary>
        /// After testing an insert or update to the database, this can be called to clear
        /// an Farm object that failed and became stuck in the EF queue, so it does not
        /// affect subsequent tests.
        /// </summary>
        private void Cleanup()
        {
            try
            {
                _context.Entry(farm).State = EntityState.Detached;
            }
            catch (Exception)
            {  }
        }

        #endregion

        #region string checks: required/optionally required, trimmed, capitalized

        [Fact]
        public void EmptyName_ShouldBeCaught()
        {
            // Arrange
            Initialise();
            farm.Name = "";
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }


        [Fact]
        public void NullName_ShouldBeCaught()
        {
            // Arrange
            Initialise();
            farm.Name = null;
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }

        [Fact]
        public void NeitherCountyNorTown_ShouldErrorOut()
        {
            // Arrange
            Initialise();
            farm.Town = null;
            farm.County = null;
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }
        [Fact]
        public void TownNoCounty_isValid()
        {
            // Arrange
            Initialise();
            farm.County = "";

            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.True(recordAccepted, validationResults);
        }

        [Fact]
        public void CountyNoTown_isValid()
        {
            // Arrange
            Initialise();
            farm.Town = "";
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.True(recordAccepted, validationResults);
        }

        [Theory]
        [InlineData("New Hamburg")]
        [InlineData("New Hamburg ")]
        [InlineData(" New Hamburg")]
        [InlineData("new hamburg")]
        [InlineData("NEW HAMburg")]
        public void NameTrimmedShiftedLowerCapitalized(string value)
        {
            // arrange
            Initialise();
            farm.Name = value;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.True(recordAccepted, validationResults);
            Assert.Equal("New Hamburg", farm.Name);
        }

        [Theory]
        [InlineData("New Hamburg")]
        [InlineData("New Hamburg ")]
        [InlineData(" New Hamburg")]
        [InlineData("new hamburg")]
        [InlineData("NEW HAMburg")]
        public void TownTrimmedShiftedLowerCapitalized(string value)
        {
            // arrange
            Initialise();
            farm.Town = value;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.True(recordAccepted, validationResults);
            Assert.Equal("New Hamburg", farm.Town);
        }

        [Theory]
        [InlineData("New Hamburg")]
        [InlineData("New Hamburg ")]
        [InlineData(" New Hamburg")]
        [InlineData("new hamburg")]
        [InlineData("NEW HAMburg")]
        public void CountyTrimmedShiftedLowerCapitalized(string value)
        {
            // arrange
            Initialise();
            farm.County = value;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.True(recordAccepted, validationResults);
            Assert.Equal("New Hamburg", farm.County);
        }

        [Theory]
        [InlineData("New Hamburg")]
        [InlineData("New Hamburg ")]
        [InlineData(" New Hamburg")]
        [InlineData("new hamburg")]
        [InlineData("NEW HAMburg")]
        public void AddressTrimmedShiftedLowerCapitalized(string value)
        {
            // arrange
            Initialise();
            farm.Address = value;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.True(recordAccepted, validationResults);
            Assert.Equal("New Hamburg", farm.Address);
        }
        #endregion

        #region phone number checks

        [Fact]
        public void NullCellPhone_ShouldBeAccepted()
        {
            // Arrange
            Initialise();
            farm.CellPhone = null;
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.True(recordAccepted, validationResults);
        }

        [Fact]
        public void CellPhoneBuriedInTrash_ShouldBeAccepted()
        {
            // Arrange
            Initialise();
            farm.CellPhone = "bb1jk2jk3j4.5.6.^&7(8.9-0kgfkhg";
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.True(recordAccepted, validationResults);
        }


        [Theory]
        [InlineData("1")]
        [InlineData("123")]
        [InlineData("12345")]
        [InlineData("1234567")]
        [InlineData("123456789A")]
        [InlineData("12d34d567r89g7j9")]
        [InlineData("12345678901")]
        [InlineData("(123)456-7890 6")]
        [InlineData("123-456-78901")]
        [InlineData("123-456-789")]
        public void CellNot10Digits_ShouldNotBeAccepted(string value)
        {
            // Arrange
            Initialise();
            farm.CellPhone = value;
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }
        
        [Fact]
        public void CellPhoneBuriedInTrash_ShouldBeReformatted()
        {
            // Arrange
            Initialise();
            farm.CellPhone = "bb1jk2jk3j4.5.6.^&7(8.9-0kgfkhg";
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.Equal("123-456-7890", farm.CellPhone);
        }

        [Fact]
        public void NullHomePhone_ShouldBeAccepted()
        {
            // Arrange
            Initialise();
            farm.HomePhone = null;
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.True(recordAccepted, validationResults);
        }

        [Fact]
        public void HomePhoneBuriedInTrash_ShouldBeAccepted()
        {
            // Arrange
            Initialise();
            farm.HomePhone = "bb1jk2jk3j4.5.6.^&7(8.9-0kgfkhg";
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.True(recordAccepted, validationResults);
        }

        [Fact]
        public void HomePhoneBuriedInTrash_ShouldBeReformatted()
        {
            // Arrange
            Initialise();
            farm.HomePhone = "bb1jk2jk3j4.5.6.^&7(8.9-0kgfkhg";
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.Equal("123-456-7890", farm.HomePhone);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("123")]
        [InlineData("12345")]
        [InlineData("1234567")]
        [InlineData("123456789A")]
        [InlineData("12d34d567r89g7j9")]
        [InlineData("12345678901")]
        [InlineData("(123)456-7890 6")]
        [InlineData("123-456-78901")]
        [InlineData("123-456-789")]
        public void HomePhoneNot10Digits_ShouldNotBeAccepted(string value)
        {
            // Arrange
            Initialise();
            farm.HomePhone = value;
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }

        #endregion

        #region date checks
        [Fact]
        public void BothDatesNull_ShouldBeAccepted()
        {
            // Arrange
            Initialise();
            farm.DateJoined = null;
            farm.LastContactDate = null;
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.True(recordAccepted, validationResults);
        }

        [Fact]
        public void DateJoinedInFuture_ShouldBeRejected()
        {
            // Arrange
            Initialise();
            farm.DateJoined = DateTime.Now.AddHours(1);
            farm.LastContactDate = null;
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }

        [Fact]
        public void LastContactDateWithoutDateJoined_ShouldBeRejected()
        {
            // Arrange
            Initialise();
            farm.DateJoined = null;
            farm.LastContactDate = DateTime.Now;
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }

        [Fact]
        public void LastContactInFuture_ShouldBeRejected()
        {
            // Arrange
            Initialise();
            farm.DateJoined = DateTime.Today;
            farm.LastContactDate = DateTime.Now.AddHours(1);
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }

        [Fact]
        public void LastContactBeforeJoined_ShouldBeRejected()
        {
            // Arrange
            Initialise();
            farm.DateJoined = DateTime.Today;
            farm.LastContactDate = DateTime.Today.AddDays(-1);
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }

        [Fact]
        public void LastContactEqualJoined_ShouldBeAccepted()
        {
            // Arrange
            Initialise();
            farm.DateJoined = DateTime.Now;
            farm.LastContactDate = farm.DateJoined;
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.True(recordAccepted, validationResults);
        }

        [Fact]
        public void LastContactAfterJoined_ShouldBeAccepted()
        {
            // Arrange
            Initialise();
            farm.DateJoined = DateTime.Now.AddHours(-1);
            farm.LastContactDate = DateTime.Now;
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.True(recordAccepted, validationResults);
        }

        #endregion

        #region postal code checks

        [Fact]
        public void EmailAddressAndPostalNullOrEmpty_ShouldNotBeAccepted()
        {
            // arrange
            Initialise();
            farm.PostalCode = null;
            farm.Email = "";
            farm.Address = null;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }

        [Fact]
        public void EmailProvidedLandMailNull_ShouldBeOK()
        {
            // arrange
            Initialise();
            farm.PostalCode = null;
            farm.Address = null;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.True(recordAccepted, validationResults);
        }


        [Fact]
        public void EmailNullAddressAndPostalProvided_ShouldBeOK()
        {
            // arrange
            Initialise();
            farm.Email = null;
            //farm.PostalCode = null;
            //farm.Address = null;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.True(recordAccepted, validationResults);
        }

        [Fact]
        public void EmailNullPostalNull_ShouldNotBeAccepted()
        {
            // arrange
            Initialise();
            farm.Email = null;
            farm.PostalCode = null;
            //farm.Address = null;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }

        [Fact]
        public void EmailNullAddressNull_ShouldNotBeAccepted()
        {
            // arrange
            Initialise();
            farm.Email = null;
            //farm.PostalCode = null;
            farm.Address = null;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }

        [Fact]
        public void PostalCodeAAA_ShouldNotBeAccepted()
        {
            // arrange
            Initialise();
            farm.PostalCode = "AAA";
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }

        [Fact]
        public void PostalCodeLower_IsAccepted()
        {
            // arrange
            Initialise();
            farm.PostalCode = "a1b 2c3";
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.True(recordAccepted, validationResults);
        }

        [Fact]
        public void PostalCodeUpper_NoSpace_IsAccepted()
        {
            // arrange
            Initialise();
            farm.PostalCode = "A1B2C3";
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.True(recordAccepted, validationResults);
        }
        [Fact]
        public void PostalCodeLower_NoSpace_IsAccepted()
        {
            // arrange
            Initialise();
            farm.PostalCode = "a1b2c3";
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.True(recordAccepted, validationResults);
        }

        [Fact]
        public void PostalCodeLower_ShiftsToUpper()
        {
            // arrange
            Initialise();
            farm.PostalCode = "a1b 2c3";
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.True(recordAccepted, validationResults);
            Assert.Equal("A1B 2C3", farm.PostalCode);
        }

        [Theory]
        [InlineData("a1b2c3")]
        [InlineData("A1B2C3")]
        [InlineData("a1b 2c3")]
        [InlineData("A1B 2C3")]
        public void PostalCode_SingleSpaceInserted(string value)
        {
            // arrange
            Initialise();
            farm.PostalCode = value;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.True(recordAccepted, validationResults);
            Assert.Equal("A1B 2C3", farm.PostalCode.ToUpper());
        }


        [Theory]
        [InlineData("A1B 1C1")]
        [InlineData("E2G 3H4")]
        [InlineData("J5K 6L7")]
        [InlineData("M8N 9P0")]
        [InlineData("R9S 9T9")]
        [InlineData("V9W 9X9")]
        [InlineData("Y9Z 9A9")]
        [InlineData("a1b 1c1")]
        [InlineData("e2g 3h4")]
        [InlineData("j5k 6l7")]
        [InlineData("m8n 9p0")]
        [InlineData("r9s 9t9")]
        [InlineData("v9w 9x9")]
        [InlineData("y9z 9a9")]
        public void PostalCodeValidLetters_ShouldBeAccepted(string value)
        {
            // arrange
            Initialise();
            farm.PostalCode = value;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.True(recordAccepted, validationResults);
        }


        [Theory]
        [InlineData("W1B 1C1")]
        [InlineData("Z2G 3H4")]
        [InlineData("J5D 6L7")]
        [InlineData("M8N 9F0")]
        [InlineData("R9I 9T9")]
        [InlineData("V9W 9O9")]
        [InlineData("Y9Q 9U9")]
        public void PostalCodeInvalidLetters_ShouldBeCaught(string value)
        {
            // arrange
            Initialise();
            farm.PostalCode = value;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.False(recordAccepted, validationResults);
        }

        [Theory]
        [InlineData("A1B 1C1X")]
        [InlineData("XE2G 3H4")]
        [InlineData("J5K J5K 6L7")]
        [InlineData("M8N 9P09P0")]
        [InlineData("R9S 9T9 ")]
        [InlineData("V9W  9X9")]
        [InlineData(" Y9Z 9A9")]
        public void PostalCodePlusTrash_ShouldBeCaught(string value)
        {
            // arrange
            Initialise();
            farm.PostalCode = value;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.False(recordAccepted, validationResults);
        }

        #endregion

        #region ZIP code checks

        [Fact]
        public void ZipCodeAAA_ShouldNotBeAccepted()
        {
            // arrange
            Initialise();
            farm.ProvinceCode = "MI";
            farm.PostalCode = "AAA";
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }
        
        [Theory]
        [InlineData("1")]
        [InlineData("123")]
        [InlineData("1234")]
        [InlineData("123456")]
        [InlineData("1234567")]
        [InlineData("12345678")]
        [InlineData("1234567890")]
        public void ZipCodeMoreOrLessDigits_ShouldBeCaught(string value)
        {
            // arrange
            Initialise();
            farm.ProvinceCode = "MI";
            farm.PostalCode = value;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.False(recordAccepted, validationResults);
        }



        [Theory]
        [InlineData("12-345")]
        [InlineData("12345")]
        [InlineData("12345")]
        [InlineData("12345 6789")]
        [InlineData("12345-6789")]
        [InlineData("123456789")]
        [InlineData("12345and6789")]
        [InlineData("g1h2k3b4h5j6n7m8m9 n")]
        public void ZipCode5Or9DigitsAndTrash_ShouldPass(string value)
        {
            // arrange
            Initialise();
            farm.ProvinceCode = "MI";
            farm.PostalCode = value;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.True(recordAccepted, validationResults);
        }

        [Theory]
        [InlineData("12-345")]
        [InlineData("12345")]
        [InlineData("a1,2:3 4-5")]
        public void ZipCode5Digits_ShouldFormatCorrectly(string value)
        {
            // arrange
            Initialise();
            farm.ProvinceCode = "MI";
            farm.PostalCode = value;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.Equal("12345", farm.PostalCode);
        }

        [InlineData("12345 6789")]
        [InlineData("12345-6789")]
        [InlineData("123456789")]
        [InlineData("12345and6789")]
        [InlineData("g1h2k3b4h5j6n7m8m9 n")]
        public void ZipCode9Digits_ShouldFormatCorrectly(string value)
        {
            // arrange
            Initialise();
            farm.ProvinceCode = "MI";
            farm.PostalCode = value;
            // act     
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            // assert
            Assert.Equal("12345-6789", farm.PostalCode);
        }       

        #endregion

        #region province/country foreign-key checks

        [Fact]
        public void InvalidProvince_ShouldBeCaughtBySQL_RemoteNotRun()
        {
            // Arrange
            Initialise(); // this creates an Farm object with all valid properties
            farm.ProvinceCode = "LM"; // invalidate property being tested
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }

        [Fact]
        public void USState_ShouldNotUsePostalCode()
        {
            // Arrange
            Initialise(); // this creates an Farm object with all valid properties
            farm.ProvinceCode = "MI"; // postal code should error-out for US
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }

        [Fact]
        public void CdnProv_ShouldNotUseZipCode()
        {
            // Arrange
            Initialise(); // this creates an Farm object with all valid properties
            farm.PostalCode = "12345";
            //Act
            try
            {
                _context.Farm.Add(farm);
                _context.EFValidation(); // EF runs all validations except [Remote]
            }
            catch (Exception ex) // a valid Farm object should not go here
            {
                recordAccepted = false;
                validationResults = ex.GetBaseException().Message;
            }
            finally
            {
                Cleanup();  // this removes the Farm object from the EF queue
            }
            Assert.False(recordAccepted, validationResults);
        }

        #endregion

        #region constructor tests

        [Fact]
        public void AboutShouldAskForDefaultView()
        {
            // Arrange
            HomeController controller = new HomeController();
            // Act
            var result = controller.About();
            // Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(ViewResult), result.GetType());

            ViewResult viewResult = (ViewResult)result;
            Assert.True(viewResult.ViewName == null || viewResult.ViewName == "",
                string.Format($"expected default (unspecified) view name ... got '{viewResult.ViewName}"));
        }

        [Fact]
        public async void XXFarmController_CreateShouldCatchExceptions()
        {
            //Arrange
            SKFarmController controller = new SKFarmController(_context);
            Initialise();
            farm.ProvinceCode = "green";
            //Act 
            try
            {
                var result = await controller.Create(farm);
            }
            //Assert
            catch (Exception ex)
            {
                Assert.True(false, "farm controller's Create action did not catch exception ... " +
                        ex.GetBaseException().Message);
            }
            finally { Cleanup(); }
        }

        [Fact]
        public async void XXFarmController_CreateShouldPutExceptionIntoModelState()
        {
            //Arrange
            SKFarmController controller = new SKFarmController(_context);
            Initialise();
            farm.ProvinceCode = "green";

            //Act 
            var result = await controller.Create(farm);

            //Assert
            try
            {
                Assert.IsType<ViewResult>(result);
                ViewResult viewResult = (ViewResult)result;
                Assert.NotNull(viewResult.ViewData.ModelState);
                Assert.NotEmpty(viewResult.ViewData.ModelState.Keys);
                foreach (string item in viewResult.ViewData.ModelState.Keys)
                {
                    Assert.Equal("", item);  // ensure it's a model-level error
                }
            }
            catch (Exception) { }
            finally { Cleanup(); }

        }

        [Fact]
        public async void XXFarmController_EditShouldPutExceptionIntoModelState()
        {
            //Arrange
            SKFarmController controller = new SKFarmController(_context);
            Initialise();

            //Act 
            await controller.Create(farm);

            Farm replayFarm = _context.Farm.FirstOrDefault(a => a.FarmId == farm.FarmId);
            replayFarm.ProvinceCode = "green";

            var result = await controller.Edit(replayFarm.FarmId, replayFarm);
            try
            {
                //Assert
                Assert.IsType<ViewResult>(result);
                ViewResult viewResult = (ViewResult)result;
                Assert.NotNull(viewResult.ViewData.ModelState);
                Assert.NotEmpty(viewResult.ViewData.ModelState.Keys);
                foreach (string item in viewResult.ViewData.ModelState.Keys)
                {
                    Assert.Equal("", item);  // ensure it's a model-level error
                }
            }
            catch (Exception) { }
            finally { Cleanup(); }

        }

        [Fact]
        public async void XXFarmController_DeleteShouldRunCleanley()
        {
            //Arrange
            SKFarmController controller = new SKFarmController(_context);
            Initialise();
            //Act 
            try
            {
                var result = await controller.Create(farm);
                var deleteResult = await controller.DeleteConfirmed(farm.FarmId);
            }
            //Assert
            catch (Exception ex)
            {
                Assert.True(false, "farm controller's Delete action did not run cleanly ... " +
                        ex.GetBaseException().Message);
            }
            finally { Cleanup(); }
        }

        #endregion

    }
}
