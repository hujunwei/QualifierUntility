using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QualifierUtility
{
    class QualifierUntilityHelper
    {
        /// <summary>
        /// Reads the CSV file to json.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static string ReadCsvFileToJson(string path)
        {
            var jsonStr = "";
            try
            {
                using (var csvReader = new StreamReader(File.OpenRead(path)))
                {

                    //Parse first line as attributes(key for JSON)
                    var keyLine = csvReader.ReadLine();
                    if (keyLine != null)
                    {
                        string[] attributes = keyLine.Split(',');
                        int numAttributes = attributes.Count();
                        //Map values after first line to attributes 
                        int count = 0;
                        jsonStr += "[";
                        while (!csvReader.EndOfStream)
                        {
                            var line = csvReader.ReadLine();
                            if (line == null) 
                                continue;

                            var valueListForOneLine = line.Split(',');
                            if (!valueListForOneLine.Any()) 
                                continue;

                            if (count > 0)
                                jsonStr += ","; 

                            jsonStr += "{";
                            for (int i = 0; i < numAttributes; ++i)
                            {
                                jsonStr += "\"" + attributes[i] + "\"";
                                jsonStr += ":";
                                jsonStr += "\"" + valueListForOneLine[i] + "\"";
                                if (i < numAttributes - 1)
                                {
                                    jsonStr += ",";
                                }
                            }
                            jsonStr += "}";
                            count++;
                        }
                        jsonStr.Remove(1);
                        jsonStr += "]";
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nRead File Error: {0}", ex.Message);
                Console.ReadLine();
            }

            return ValidateJson(jsonStr) ? jsonStr : "";

        }


        public static List<string> ReadCsvFileToStringList_zuber(string path)
        {
            var valueList = new List<string>();
            try
            {
                using (var csvReader = new StreamReader(File.OpenRead(path)))
                {
                    bool hasOnlyOneColumn = false;
                    //Parse first line as attributes(key for JSON)
                    var keyLine = csvReader.ReadLine();
                    if (keyLine != null)
                    {
                        string[] attributes = keyLine.Split(',');
                        int numAttributes = attributes.Count();
                        if (numAttributes == 1)
                            hasOnlyOneColumn = true;
                        //Map values after first line to attributes 
                        
                        while (!csvReader.EndOfStream)
                        {
                            var line = csvReader.ReadLine();
                            var sb = new StringBuilder();
                            if (line == null) continue;
                            var valueListForOneLine = line.Split(',');
                            if (!valueListForOneLine.Any()) continue;

                            if (hasOnlyOneColumn)
                            {
                                valueList.Add(line);
                                continue;
                            }

                            // In case we have multiple columns per row
                            sb.Append("{");
                            for (int i = 0; i < numAttributes; ++i)
                            {

                                sb.Append(attributes[i]);
                                sb.Append(":");
                                sb.Append(valueListForOneLine[i]);
                                if (i < numAttributes - 1)
                                {
                                    sb.Append(",");
                                }
                            }
                            sb.Append("}");
                            valueList.Add(sb.ToString());
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nRead File Error: {0}", ex.Message);
                Console.ReadLine();
            }
            return valueList;
        }


        /// <summary>
        /// Adds the qualifier.
        /// </summary>
        /// <param name="name">The name for Qualifier.</param>
        /// <param name="url">The URL for Qualifier.</param>
        /// <param name="qualifierValList">The Qualifier in json.</param>
        /// <param name="apiUrl">The API URL.</param>
        /// <returns></returns>
        public static HttpWebResponse AddQualifier(string name, string url, List<string> qualifierValList, string apiUrl = "http://planningconfigtest.cloudapp.net/api/qualifier/add")
        {
            HttpWebResponse response = null;
            try
            {
                var createdQualifier = CreateQualifier(name, url, qualifierValList);
                
                var request = (HttpWebRequest) WebRequest.Create(apiUrl);
                var postData = JsonConvert.SerializeObject(createdQualifier);
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;


                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                response = (HttpWebResponse) request.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nAdd qualifier failed: {0}", ex.Message);
                Console.ReadLine();
            }
            

            return response;
        }


        /// <summary>
        ///     Create a Qualifier object
        /// </summary>
        /// <param name="name">Invariant Name to be indentified in Database for debug use</param>
        /// <param name="url">Url field for a Qualifier</param>
        /// <param name="values"></param>
        /// <returns>Qualifier object</returns>
        private static Qualifier CreateQualifier(string name, string url, List<string> values)
        {

            var qualifier = new Qualifier
            {
                //Value of Id does not matter because actual Id saved in the database has no relationship with this id
                Id = 1,
                Custom = true,
                Name = name,
                Url = url,
                Values = values
            };
            return qualifier;
        }



        /// <summary>
        /// Validates the json.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <returns></returns>
        private static bool ValidateJson(string jsonString)
        {
            try
            {
                var obj = JToken.Parse(jsonString);
                return true;
            }
            catch (JsonReaderException jex)
            {
                //Exception in parsing json
                Console.WriteLine("Invalid Json string: " + jex.Message);
                Console.ReadLine();
                return false;
            }
        }


    }
}
