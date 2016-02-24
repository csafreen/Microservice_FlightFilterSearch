using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace RestServiceForFlightSearch
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RestServiceImp" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RestServiceImp.svc or RestServiceImp.svc.cs at the Solution Explorer and start debugging.
    public class RestServiceImp : IRestServiceImp
    {
        public string ConnectionString;
        public SqlConnection DbConnection;
        public string GetCityNames(string cityName)
        {
            ConnectionString = ConfigurationManager.AppSettings["DBConnection"].ToString();
            if (string.IsNullOrEmpty(ConnectionString))
                throw new Exception("No Connection String given for connecting to DB");
            DbConnection = new SqlConnection { ConnectionString = ConnectionString };
            List<string> listOfCities = new List<string>();
            
            IDbCommand command = new SqlCommand();
            command.Connection = DbConnection;
            DbConnection.Open();
            try
            {
                string query = "Select * from MicroServices_CityDetails Where %'REPLACE CONDITION'%";
                
                List<string> withQuotes = new List<string>();
                string condition = string.Empty;



                int quoteCount = cityName.Count(x => x == '\"');
                //Check if both quotes are present
                if (quoteCount%2 == 0)
                {
                    var reg = new Regex("\".*?\"");
                    var matches = reg.Matches(cityName);
                    foreach (var item in matches)
                    {
                        string tempString = item.ToString().Replace("\"", "");
                        withQuotes.Add(tempString.ToUpper(CultureInfo.InvariantCulture) + "%");
                    }
                }
                // if only one quote is there add it to the other array
                else if (quoteCount%2 == 1)
                    cityName = cityName.Replace("\"", "");
                int paramVal = 0;
                string[] inputParams = cityName.Split(' ');

                for (int i = 0; i < inputParams.Length; i++)
                {
                    inputParams[i] = inputParams[i].ToUpper(CultureInfo.InvariantCulture) + "%";
                }
                foreach (string w in withQuotes)
                {
                    condition += "(CityName like @param" + paramVal + ") OR";
                    paramVal++;
                }

                foreach (string s in inputParams)
                {
                    if (!s.Contains("\""))
                    {
                        condition += "(CityName like @param" + paramVal + ") OR ";
                        paramVal++;
                    }
                }
                
                condition = condition.Substring(0, condition.LastIndexOf(")", StringComparison.InvariantCulture) + 1);
                query = query.Replace("%'REPLACE CONDITION'%", condition);
                
                paramVal = 0;
                foreach (string w in withQuotes)
                {
                    command.Parameters.Add(new SqlParameter("param" + paramVal, w));
                    paramVal++;
                }

                foreach (string s in inputParams)
                {
                    if (!s.Contains("\""))
                    {
                        command.Parameters.Add(new SqlParameter("param" + paramVal, s));
                        paramVal++;
                    }
                }
                command.CommandText = query;
                
                IDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    listOfCities.Add(Convert.ToString(reader["CityName"]));
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DbConnection.Close();
            }
            return JsonConvert.SerializeObject(listOfCities);
        }
    }
}
