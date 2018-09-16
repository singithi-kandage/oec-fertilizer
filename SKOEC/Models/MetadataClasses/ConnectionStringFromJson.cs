using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace JsonManipulation
{
    /// <summary>
    /// Obtain a connection string by parsing a JSON file
    /// </summary>
    public class ConnectionStringFromJson
    {
        /// <summary>
        /// Provide the connection string for the specified database, from appsettings.json
        /// ... the same place dependency injection gets it for the Controllers
        /// </summary>
        public static string GetConnectionString(string databaseConnectionName, string solutionName, string projectName)
        {
            // find physical path to site root, where appsettings.json is
            string sitePath = "";
            string currentPath = System.IO.Directory.GetCurrentDirectory();
            Regex pattern = new Regex($"^.*{solutionName}"); // look for a bunch of stuff ending in the solution's name
            Match match = pattern.Match(currentPath);

            // if the path to the solution was found, save that ... else throw an exception
            if (match.Success)
                sitePath = match.Groups[0].ToString();
            else
                throw new Exception($"solution name '{solutionName}' was not not found (in directory path)");
            // other projects in solution need to add site's project folder to solution's path
            if (!sitePath.EndsWith($"\\{solutionName}\\{projectName}"))
                sitePath = sitePath + $"\\{projectName}";
            //read the JSON file for the given key & return its value
            return ParseJSON.ParseJsonKey($"{sitePath}\\appsettings.json", databaseConnectionName);
        }
    }
}
