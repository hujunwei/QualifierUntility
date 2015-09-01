using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace QualifierUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please input csv file name with file extention:");
            string fileName = Console.ReadLine();
            var directoryPath = Directory.GetCurrentDirectory();
            Regex csvFileRegex = new Regex(@"(.*?)\.(csv)$");
            var csvFilePath = "";
            if (!string.IsNullOrEmpty(fileName))
            {
                Match match = csvFileRegex.Match(fileName);
                if (match.Success)
                {
                    csvFilePath = directoryPath + @"\" + fileName;
                }
                else
                {
                    throw new Exception("Filename is invalid");
                }
            }
            else
            {
                throw new Exception("Filename is invalid");
            }
            var valueListForQualifier = QualifierUntilityHelper.ReadCsvFileToStringList_zuber(csvFilePath);
            Console.WriteLine("Please input qualifer name:");
            string qualifierName = Console.ReadLine();
            if (string.IsNullOrEmpty(qualifierName))
            {
                throw new Exception("qualifierName could not be empty"); 
            }

            Console.WriteLine("Please input qualifer url:");
            string qualifierUrl = Console.ReadLine();
            Regex qualifierUrlRegex = new Regex(@"/^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$/");

            if (!string.IsNullOrEmpty(qualifierUrl))
            {
                if (!Uri.IsWellFormedUriString(qualifierUrl, UriKind.RelativeOrAbsolute))
                {
                    throw new Exception("Qualifier Url is invalid");
                }

            }
            else
            {
                throw new Exception("Qualifier Url is invalid");
            }

            Console.WriteLine("Sending Request...");

            HttpWebResponse response = QualifierUntilityHelper.AddQualifier(qualifierName, qualifierUrl, valueListForQualifier, "http://localhost:55658/api/qualifier/add");

            Console.WriteLine("ResponseHttpStatusCode: " + response.StatusCode);
            Console.ReadKey();

        }
    }
}
