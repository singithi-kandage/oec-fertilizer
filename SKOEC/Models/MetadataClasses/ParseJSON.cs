using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace JsonManipulation
{
    public class ParseJSON
    {      
        /// <summary>
        /// ParseJsonKey - given a source file-name (or a JSON string),
        /// - find the first occurrence of the given key and return its value (or null if key or file not found) 
        /// </summary>
        /// <param name="jsonSource">either the name of the JSON file or a string of JSON code</param>
        /// <param name="searchKey">the name of the key being sought</param>
        /// <returns></returns>
        public static string ParseJsonKey(string jsonSource, string searchKey)
        {
            string jsonStuff = "", result = "";
            if (jsonSource == null) return null;

            // get JSON contents ... from file or source string
            if (System.IO.File.Exists(jsonSource))
                jsonStuff = System.IO.File.OpenText(jsonSource).ReadToEnd();
            else
                jsonStuff = jsonSource;
            // look for the search key, followed by :
            // - pick up next thing fitting the pattern quote-something-quote
            // - or space-true-comma or space-false-comma
            // - or space-numbers-space
            Regex pattern = new Regex($"\"{searchKey}\".*:.*(\".*\"| true,| false,| \\d+ )");
            Match match = pattern.Match(jsonStuff);
            if (match.Success && match.Groups.Count > 1)
            {   // get value string, single-up \ for C# string
                result = match.Groups[1].Value.Replace(@"\\", @"\");
                if (result.Length > 2) // remove quotes or space-comma around true/false
                    result = result.Substring(1, result.Length - 2);
                return result;
            }
            else
                return null;
        }
    }    
}
