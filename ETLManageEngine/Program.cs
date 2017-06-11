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

namespace ETLManageEngine
{
    class Program
    {
        private static DateTime Number2Date(long lNum)
        {
            string strNum = lNum.ToString();

            string str = strNum.Substring(0, 10);
            double timestamp = Convert.ToDouble(str);

            // First make a System.DateTime equivalent to the UNIX Epoch.
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

            // Add the number of seconds in UNIX timestamp to be converted.
            dateTime = dateTime.AddSeconds(timestamp);
            dateTime = dateTime.AddHours(+7);

            return dateTime;
        }

        public static string CleanInvalidXmlChars(string text)
        {
            // From xml spec valid chars: 
            // #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]     
            // any Unicode character, excluding the surrogate blocks, FFFE, and FFFF. 
            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(text, re, "");
        }

        private static void RefreshServerAttribute(string HostName, string HostPort, string APIKey)
        {
            Console.WriteLine("Get Refresh Server Attribute Start .....");
            Console.WriteLine("-----------------------------------------------------------------");

            WebClient web = new WebClient();
            string url = string.Format("http://{0}:{1}/AppManager/xml/ListServer?apikey={2}&type=all", HostName, HostPort, APIKey);
            string response = web.DownloadString(url);

            DataSet dsData;
            dsData = new DataSet();

            response = CleanInvalidXmlChars(response);

            using (StringReader stringReader = new StringReader(response))
            {
                dsData.Clear();
                dsData.ReadXml(stringReader);
            }


            //WebClient webAttribute = new WebClient();
            string urlAttribute = string.Empty;
            string responseAttribute = string.Empty;

            DataSet dsDataAttribute;
            //dsDataAttribute = new DataSet();

            dsETLManageEngineTableAdapters.qryFactUtilization da = new dsETLManageEngineTableAdapters.qryFactUtilization();
            da.TruncateTempResources();
            da.TruncateTempAttribute();

            foreach (DataTable dt in dsData.Tables)
            {
                if (dt.TableName == "Server")
                {

                    Console.WriteLine("# of Server = {0}", dt.Rows.Count);
                    Console.WriteLine("-----------------------------------------------------------------");
                    foreach (DataRow dr in dt.Rows)
                    {
                        da.InsertQueryTempResources(
                                Convert.ToInt32(dr["RESOURCEID"]),
                                dr["Name"].ToString(),
                                dr["DISPLAYNAME"].ToString(),
                                dr["TYPE"].ToString()
                            );

                        using (WebClient webAttribute = new WebClient())
                        {
                            responseAttribute = string.Empty;
                            urlAttribute = string.Format("http://{0}:{1}/AppManager/xml/GetMonitorData?apikey={2}&resourceid={3}", HostName, HostPort, APIKey, dr["RESOURCEID"].ToString());
                            responseAttribute = webAttribute.DownloadString(urlAttribute);

                            //dsDataAttribute.Clear();
                            responseAttribute = CleanInvalidXmlChars(responseAttribute);

                            using (StringReader stringReader = new StringReader(responseAttribute))
                            {
                                dsDataAttribute = new DataSet();
                                dsDataAttribute.ReadXml(stringReader);
                            }

                            foreach (DataTable dtAttribute in dsDataAttribute.Tables)
                            {
                                if (dtAttribute.TableName == "Attribute")
                                {
                                    Console.WriteLine("# of Attribute = {0}", dtAttribute.Rows.Count);
                                    Console.WriteLine("-----------------------------------------------------------------");

                                    foreach (DataRow drAttr in dtAttribute.Rows)
                                    {
                                        da.InsertQueryTempAttribute(
                                                Convert.ToInt32(drAttr["AttributeID"]),
                                                drAttr["DISPLAYNAME"].ToString(),
                                                drAttr["Units"].ToString(),
                                                Convert.ToInt32(dr["RESOURCEID"])
                                            );
                                    }
                                }
                            }
                            dsDataAttribute.Dispose();
                        }

                    }
                }
            }

            Console.WriteLine("List Of Monitor start");
            Console.WriteLine("-----------------------------------------------------------------");

            using (WebClient oWebClient = new WebClient())
            {
                DataSet dsDataMonitor;

                string sResponseMonitor = string.Empty;
                string sUrlMonitor = string.Format("http://{0}:{1}/AppManager/xml/ListMonitor?apikey={2}&type=all", HostName, HostPort, APIKey);
                sResponseMonitor = oWebClient.DownloadString(sUrlMonitor);

                sResponseMonitor = CleanInvalidXmlChars(sResponseMonitor);

                using (StringReader stringReader = new StringReader(sResponseMonitor))
                {
                    dsDataMonitor = new DataSet();
                    dsDataMonitor.ReadXml(stringReader);
                }

                da.TruncateDimServerGroup();

                foreach (DataTable dtMonitor in dsDataMonitor.Tables)
                {
                    if (dtMonitor.TableName == "Monitor")
                    {
                        Console.WriteLine("# of Monitor = {0}", dtMonitor.Rows.Count);
                        Console.WriteLine("-----------------------------------------------------------------");
                        foreach (DataRow drMonitor in dtMonitor.Rows)
                        {
                            da.InsertQueryDimServerGroup(
                                    drMonitor["ASSOCIATEDGROUPS"].ToString(),
                                    Convert.ToInt32(drMonitor["RESOURCEID"])
                                );
                        }
                    }
                }
                dsDataMonitor.Dispose();

            }

            int? sVal = da.ScalarQueryCountTempResources();

            da.UpdateQueryDimResources();
            da.InsertQueryDimResources();

            da.UpdateQueryDimAttribute();
            da.InsertQueryDimAttribute();

            Console.WriteLine("# of Uploaded = {0}", sVal.ToString());
            Console.WriteLine("-----------------------------------------------------------------");

        }

        private static void RefreshMonitorAttribute(string HostName, string HostPort, string APIKey)
        {
            Console.WriteLine("Get Refresh Monitor Attribute Start .....");
            Console.WriteLine("-----------------------------------------------------------------");

            WebClient web = new WebClient();
            string url = string.Format("http://{0}:{1}/AppManager/xml/ListServer?apikey={2}&type=all", HostName, HostPort, APIKey);
            string response = web.DownloadString(url);

            DataSet dsData;
            dsData = new DataSet();

            response = CleanInvalidXmlChars(response);

            using (StringReader stringReader = new StringReader(response))
            {
                dsData.Clear();
                dsData.ReadXml(stringReader);
            }


            //WebClient webAttribute = new WebClient();
            string urlAttribute = string.Empty;
            string responseAttribute = string.Empty;

            DataSet dsDataAttribute;
            //dsDataAttribute = new DataSet();

            dsETLManageEngineTableAdapters.qryFactUtilization da = new dsETLManageEngineTableAdapters.qryFactUtilization();
            da.TruncateTempResources();
            da.TruncateTempAttribute();

            foreach (DataTable dt in dsData.Tables)
            {
                if (dt.TableName == "Server")
                {

                    Console.WriteLine("# of Server = {0}", dt.Rows.Count);
                    Console.WriteLine("-----------------------------------------------------------------");
                    foreach (DataRow dr in dt.Rows)
                    {
                        da.InsertQueryTempResources(
                                Convert.ToInt32(dr["RESOURCEID"]),
                                dr["Name"].ToString(),
                                dr["DISPLAYNAME"].ToString(),
                                dr["TYPE"].ToString()
                            );

                        using (WebClient webAttribute = new WebClient())
                        {
                            responseAttribute = string.Empty;
                            urlAttribute = string.Format("http://{0}:{1}/AppManager/xml/GetMonitorData?apikey={2}&resourceid={3}", HostName, HostPort, APIKey, dr["RESOURCEID"].ToString());
                            responseAttribute = webAttribute.DownloadString(urlAttribute);

                            //dsDataAttribute.Clear();
                            responseAttribute = CleanInvalidXmlChars(responseAttribute);

                            using (StringReader stringReader = new StringReader(responseAttribute))
                            {
                                dsDataAttribute = new DataSet();
                                dsDataAttribute.ReadXml(stringReader);
                            }

                            foreach (DataTable dtAttribute in dsDataAttribute.Tables)
                            {
                                if (dtAttribute.TableName == "Attribute")
                                {
                                    Console.WriteLine("# of Attribute = {0}", dtAttribute.Rows.Count);
                                    Console.WriteLine("-----------------------------------------------------------------");

                                    foreach (DataRow drAttr in dtAttribute.Rows)
                                    {
                                        da.InsertQueryTempAttribute(
                                                Convert.ToInt32(drAttr["AttributeID"]),
                                                drAttr["DISPLAYNAME"].ToString(),
                                                drAttr["Units"].ToString(),
                                                Convert.ToInt32(dr["RESOURCEID"])
                                            );
                                    }
                                }
                            }
                            dsDataAttribute.Dispose();
                        }

                    }
                }
            }

            Console.WriteLine("List Of Monitor start");
            Console.WriteLine("-----------------------------------------------------------------");

            using (WebClient oWebClient = new WebClient())
            {
                DataSet dsDataMonitor;

                string sResponseMonitor = string.Empty;
                string sUrlMonitor = string.Format("http://{0}:{1}/AppManager/xml/ListMonitor?apikey={2}&type=all", HostName, HostPort, APIKey);
                sResponseMonitor = oWebClient.DownloadString(sUrlMonitor);

                sResponseMonitor = CleanInvalidXmlChars(sResponseMonitor);

                using (StringReader stringReader = new StringReader(sResponseMonitor))
                {
                    dsDataMonitor = new DataSet();
                    dsDataMonitor.ReadXml(stringReader);
                }

                da.TruncateDimServerGroup();

                foreach (DataTable dtMonitor in dsDataMonitor.Tables)
                {
                    if (dtMonitor.TableName == "Monitor")
                    {
                        Console.WriteLine("# of Monitor = {0}", dtMonitor.Rows.Count);
                        Console.WriteLine("-----------------------------------------------------------------");
                        foreach (DataRow drMonitor in dtMonitor.Rows)
                        {
                            da.InsertQueryDimServerGroup(
                                    drMonitor["ASSOCIATEDGROUPS"].ToString(),
                                    Convert.ToInt32(drMonitor["RESOURCEID"])
                                );
                        }
                    }
                }
                dsDataMonitor.Dispose();

            }

            int? sVal = da.ScalarQueryCountTempResources();

            da.UpdateQueryDimResources();
            da.InsertQueryDimResources();

            da.UpdateQueryDimAttribute();
            da.InsertQueryDimAttribute();

            Console.WriteLine("# of Uploaded = {0}", sVal.ToString());
            Console.WriteLine("-----------------------------------------------------------------");

        }

        static void Main(string[] args)
        {

            /*
             * 
             * Setting ditable dimResource diambil dari http://10.10.8.204:8080/AppManager/xml/ListMonitor?apikey=e776bf59235b62d523a0e2aaf3a07b76&type=all
             * Setting ditable dimAttribute diambil dataset attribute di http://10.10.8.204:8080/AppManager/xml/GetMonitorData?apikey=e776bf59235b62d523a0e2aaf3a07b76&resourceid=10055131 
             * 
             * Loop akan dilakukan untuk menarik data dari inner join dimResource dan dimAttribute eksekusi API 
             * http://10.10.8.204:8080/AppManager/xml/ShowPolledData?apikey=e776bf59235b62d523a0e2aaf3a07b76&resourceid=10055131&period=-7&attributeID=9641
             * Data disimpan di RawData kemudian diambil max(result) group by day
             * 
             */


            Console.WriteLine("Start .....");
            Console.WriteLine("=================================================================");

            Console.WriteLine("Initializing .....");
            Console.WriteLine("-----------------------------------------------------------------");

            dsETLManageEngineTableAdapters.ServerVariablesTableAdapter daServer = new dsETLManageEngineTableAdapters.ServerVariablesTableAdapter();
            dsETLManageEngine.ServerVariablesDataTable dtServer = daServer.GetData();

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

            //RefreshServerAttribute(HostName, HostPort, APIKey); // memakan waktu 5 - 10 Menit

            Console.WriteLine("Get Resource & Attribute .....");
            Console.WriteLine("-----------------------------------------------------------------");

            dsETLManageEngineTableAdapters.dimAttributeTableAdapter da = new dsETLManageEngineTableAdapters.dimAttributeTableAdapter();
            dsETLManageEngine.dimAttributeDataTable dt = da.GetData();

            dsETLManageEngineTableAdapters.qryFactUtilization daUtil = new dsETLManageEngineTableAdapters.qryFactUtilization();
            daUtil.TruncateTempFactUtilization();

            string url = string.Empty;
            string response = string.Empty;
            DataSet dsData;

            Console.WriteLine("# of Record = {0} .....",dt.Rows.Count);
            Console.WriteLine("-----------------------------------------------------------------");

            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                Console.WriteLine("Row {0} ", i);
                Console.WriteLine("-----------------------------------------------------------------");

                using (WebClient web = new WebClient())
                {
                    response = string.Empty;
                    //url = string.Format("http://{0}:{1}/AppManager/xml/ShowPolledData?apikey={2}&resourceid={3}&period=-7&attributeID={4}", HostName, HostPort, APIKey, dr["ResourceId"].ToString(), dr["AttributeId"].ToString());
                    url = string.Format("http://{0}:{1}/AppManager/xml/ShowPolledData?apikey={2}&resourceid={3}&period=-30&attributeID={4}", HostName, HostPort, APIKey, dr["ResourceId"].ToString(), dr["AttributeId"].ToString());
                    response = web.DownloadString(url);

                    response = CleanInvalidXmlChars(response);

                    using (StringReader stringReader = new StringReader(response))
                    {
                        dsData = new DataSet();
                        dsData.ReadXml(stringReader);
                    }

                    foreach (DataTable dtData in dsData.Tables)
                    {
                        if (dtData.TableName == "ArchiveData")
                        {
                            Console.WriteLine("Get ArchiveData");
                            Console.WriteLine("-----------------------------------------------------------------");

                            int DataNo = 0;

                            foreach (DataRow drAttr in dtData.Rows)
                            {
                                DataNo++;
                                Console.WriteLine("Atribute ke {0} dari {1}, baris ke {2} dari {3} ", i, dt.Rows.Count, DataNo, dtData.Rows.Count);
                                long number;
                                long.TryParse(drAttr["ArchivedTime"].ToString(), out number);
                                DateTime resultDate = Number2Date(number);
                                
                                NumberStyles style = NumberStyles.Number | NumberStyles.AllowDecimalPoint;
                                CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

                                Decimal sNum;
                                Decimal.TryParse(drAttr["AvgValue"].ToString(),style,culture, out sNum);

                                daUtil.InsertQueryTempFactUtilization(
                                        Convert.ToInt64(drAttr["ArchivedTime"]),
                                        sNum,
                                        Convert.ToInt32(dr["AttributeId"]),
                                        Convert.ToInt32(dr["ResourceId"]),
                                        resultDate
                                    );

                                //da.InsertQueryTempAttribute(
                                //        Convert.ToInt32(drAttr["AttributeID"]),
                                //        drAttr["DISPLAYNAME"].ToString(),
                                //        drAttr["Units"].ToString(),
                                //        Convert.ToInt32(dr["RESOURCEID"])
                                //    );
                            }
                            Console.WriteLine("Get ArchiveData Done");
                            Console.WriteLine("-----------------------------------------------------------------");

                        }
                                
                    }
                }
                Console.WriteLine("Row # {0} of {1} Done", i, dt.Rows.Count);
                Console.WriteLine("-----------------------------------------------------------------");

            }
            Console.WriteLine("Looping Resource & Attribute Done");
            Console.WriteLine("-----------------------------------------------------------------");

            Console.WriteLine("Finalizing");
            Console.WriteLine("-----------------------------------------------------------------");
            
            //daUtil.DeleteQueryFactUtilization(); // tidak dipakai lagi
            daUtil.InsertQueryFactUtilization();

            Console.WriteLine("DONE ...");
            Console.WriteLine("-----------------------------------------------------------------");
            //Console.Read();

        }
    }
}
