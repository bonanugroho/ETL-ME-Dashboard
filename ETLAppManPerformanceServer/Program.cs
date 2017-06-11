using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ETLAppManPerformanceServer
{
    class Program
    {
        public static string CleanInvalidXmlChars(string text)
        {
            // From xml spec valid chars: 
            // #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]     
            // any Unicode character, excluding the surrogate blocks, FFFE, and FFFF. 
            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(text, re, "");
        }

        static void Main(string[] args)
        {
            /*
             * Program/Aplikasi ini dijalankan setiap jam, menggunakan window Task Scheduler
             * ------------------------------------------------------------------------------
             * 
             * Baca server Variables untuk mengambil Nilai Server,Port dan API Key
             * 
             * Truncate table tempfactUtilization
             * 
             * Select distinct ResourceId from ServerUtilization.dimResources
             *      - foreach item loop
             *          - get API http://10.10.8.204:8080/AppManager/xml/GetMonitorData?apikey=e776bf59235b62d523a0e2aaf3a07b76&resourceid=CurrentResourceId
             *              - masuk ke table [attribute]
             *              - foreach item loop
             *                  - masukan ke table tempFactUtilization
             * 
             * Insert into factUtilization Where atribute in dimAttribute
             * 
             * 
             */


            Console.WriteLine("Start .....");
            Console.WriteLine("=================================================================");

            Console.WriteLine("Initializing .....");
            Console.WriteLine("-----------------------------------------------------------------");

            dsGetMonitorDataTableAdapters.ServerVariablesTableAdapter daServer = new dsGetMonitorDataTableAdapters.ServerVariablesTableAdapter();
            dsGetMonitorData.ServerVariablesDataTable dtServer = daServer.GetData();

            string HostName = string.Empty;
            string HostPort = string.Empty;
            string APIKey = string.Empty;

            Console.WriteLine("Get Server Variables .....");
            Console.WriteLine("-----------------------------------------------------------------");

            foreach (DataRow dr in dtServer.Rows)
            {
                HostName = dr["ServerHostName"].ToString();
                HostPort = dr["ServerPortNo"].ToString();
                APIKey = dr["ServerAPIKey"].ToString();
            }

            Console.WriteLine("Get Server Variables Done .....");
            Console.WriteLine("-----------------------------------------------------------------");


            // DOing
            int ServerRowNo = 0;
            DataSet dsAttribute;
            
            dsGetMonitorDataTableAdapters.qryFactUtilization da = new dsGetMonitorDataTableAdapters.qryFactUtilization();
            da.TruncateTempFactUtilization();

            dsGetMonitorDataTableAdapters.dimResourcesTableAdapter daResources = new dsGetMonitorDataTableAdapters.dimResourcesTableAdapter();
            dsGetMonitorData.dimResourcesDataTable dtResources = daResources.GetData();

            foreach(DataRow dr in dtResources.Rows)
            {
                ServerRowNo++;

                using (WebClient webAttribute = new WebClient())
                {
                    //Console.WriteLine("Get List Attribute Start .....");
                    //Console.WriteLine("-----------------------------------------------------------------");

                    string urlAttribute = string.Format("http://{0}:{1}/AppManager/xml/GetMonitorData?apikey={2}&resourceid={3}", HostName, HostPort, APIKey, dr["ResourceId"].ToString());
                    string responseAttribute = webAttribute.DownloadString(urlAttribute);

                    responseAttribute = CleanInvalidXmlChars(responseAttribute);
                    using (StringReader stringReader = new StringReader(responseAttribute))
                    {
                        dsAttribute = new DataSet();
                        dsAttribute.ReadXml(stringReader);
                    }

                    foreach (DataTable dtAttribute in dsAttribute.Tables)
                    {
                        if (dtAttribute.TableName == "Attribute")
                        {
                            Console.WriteLine("-----------------------------------------------------------------");
                            Console.WriteLine("# of Attribute = {0}", dtAttribute.Rows.Count);
                            Console.WriteLine("-----------------------------------------------------------------");

                            int AttributeRowNo = 0;
                            foreach (DataRow drAttr in dtAttribute.Rows)
                            {
                                AttributeRowNo++;
                                Console.WriteLine("Server ke {0} dari {1}, Attribute ke {2} dari {3}", ServerRowNo, dtResources.Rows.Count, AttributeRowNo, dtAttribute.Rows.Count);

                                NumberStyles style = NumberStyles.Number | NumberStyles.AllowDecimalPoint;
                                CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

                                Decimal sNum;
                                Decimal.TryParse(drAttr["Value"].ToString(), style, culture, out sNum);

                                DateTime resultDate = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":00");

                                da.InsertQueryTempFactUtilization(
                                        sNum,
                                        Convert.ToInt32(drAttr["AttributeId"]),
                                        Convert.ToInt32(dr["ResourceId"]),
                                        resultDate
                                    );

                                //da.InsertQueryTempAttribute(
                                //        Convert.ToInt32(drAttr["AttributeID"]),
                                //        drAttr["DISPLAYNAME"].ToString(),
                                //        drAttr["Units"].ToString(),
                                //        Convert.ToInt32(drMonitor["RESOURCEID"])
                                //    );
                            }

                        }
                    }

                    dsAttribute.Dispose();

                    //Console.WriteLine("Get List Attribute End .....");
                    //Console.WriteLine("-----------------------------------------------------------------");
                }

            }



            Console.WriteLine("Finalizing");
            Console.WriteLine("-----------------------------------------------------------------");

            //daUtil.DeleteQueryFactUtilization(); // tidak dipakai lagi
            da.InsertQueryFactUtilization();

            Console.WriteLine("DONE ...");
            Console.WriteLine("-----------------------------------------------------------------");
            //Console.Read();


        }
    }
}
