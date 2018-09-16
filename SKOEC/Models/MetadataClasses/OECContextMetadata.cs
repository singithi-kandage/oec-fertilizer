using JsonManipulation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;



namespace SKOEC.Models
{
    public partial class OECContext
    {
        // Instantiate an object of this class
        // - passing it a DbContextOption containing the SQL Server connection string
        public static OECContext Context
        {
            get
            {
                var optionsBuilder = new DbContextOptionsBuilder<OECContext>();
                optionsBuilder.UseSqlServer(
                    ConnectionStringFromJson.GetConnectionString(
                        databaseConnectionName: "OECConnection", solutionName: "SKOEC", projectName: "SKOEC"));
                return new OECContext(optionsBuilder.Options);
            }
        }

        // override the SaveChangesAsync() method, allowing it to run validations
        public void EFValidation()
        {
            var serviceProvider = this.GetService<IServiceProvider>();
            var items = new Dictionary<object, object>();
            string errors = "";

            // go through each Add/Update entry in the EF queue and validate its object
            foreach (var entry in this.ChangeTracker.Entries()
                .Where(a=>a.State == EntityState.Added || a.State == EntityState.Modified))
            {
                var entity = entry.Entity; // extract the object from its action                
                var context = new ValidationContext(entity, serviceProvider, items);
                var results = new List<ValidationResult>();

                if (Validator.TryValidateObject(entity, context, results, true) == false)
                {
                    foreach (var result in results)  // accumulate all error messages
                    {
                        if (result != ValidationResult.Success) errors = errors + $"::: {result.ErrorMessage}";
                    }
                }             
            }
            // if any validation errors were found, throw them as an exception
            if (errors != "") throw new ValidationException(errors);
       }       
    }
}
