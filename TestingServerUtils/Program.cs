using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestingServerUtils
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

        private static void RefreshMonitorAttribute(string HostName, string HostPort, string APIKey)
        {
            Console.WriteLine("Get Refresh Monitor Attribute Start .....");
            Console.WriteLine("-----------------------------------------------------------------");

            dsTestingServerUtilsTableAdapters.qryFactUtilization da = new dsTestingServerUtilsTableAdapters.qryFactUtilization();

            using (WebClient web = new WebClient())
            {
                string url = string.Format("http://{0}:{1}/AppManager/xml/ListMonitorGroups?apikey={2}&type=all", HostName, HostPort, APIKey);
                string response = web.DownloadString(url);

                using (DataSet dsData = new DataSet())
                {
                    response = CleanInvalidXmlChars(response);
                    using (StringReader stringReader = new StringReader(response))
                    {
                        dsData.Clear();
                        dsData.ReadXml(stringReader);
                    }

                    foreach (DataTable dt in dsData.Tables)
                    {
                        if (dt.TableName == "MonitorGroup" || dt.TableName == "SubMonitorGroup")
                        {
                            int sNoMonitor = 0;
                            foreach (DataRow dr in dt.Rows)
                            {
                                sNoMonitor++;
                                Console.WriteLine("Server Group Row # {0} of {1}", sNoMonitor, dt.Rows.Count);
                                Console.WriteLine("-----------------------------------------------------------------");
                                
                                da.InsertTempMonitoring(
                                        Convert.ToInt32(dr["RESOURCEID"]),
                                        dr["Name"].ToString(),
                                        dr["DISPLAYNAME"].ToString()
                                    );
                                using (WebClient oWebClient = new WebClient())
                                {
                                    string sResponseMonitor = string.Empty;
                                    string sUrlMonitor = string.Format("http://{0}:{1}/AppManager/xml/ListMGDetails?apikey={2}&groupId={3}", HostName, HostPort, APIKey, Convert.ToInt32(dr["RESOURCEID"]));
                                    sResponseMonitor = oWebClient.DownloadString(sUrlMonitor);


                                    using (DataSet dsDataMonitor = new DataSet())
                                    {
                                        sResponseMonitor = CleanInvalidXmlChars(sResponseMonitor);

                                        using (StringReader stringReader = new StringReader(sResponseMonitor))
                                        {
                                            dsDataMonitor.Clear();
                                            dsDataMonitor.ReadXml(stringReader);
                                        }

                                        foreach (DataTable dtMonitor in dsDataMonitor.Tables)
                                        {
                                            if (dtMonitor.TableName == "Monitors")
                                            {
                                                int sNoServer = 0;
                                                foreach (DataRow drMonitor in dtMonitor.Rows)
                                                {
                                                    sNoServer++;
                                                    Console.WriteLine("Server Row # {0} of {1}", sNoServer, dtMonitor.Rows.Count);
                                                    Console.WriteLine("-----------------------------------------------------------------");

                                                    da.InsertTempResources(
                                                            Convert.ToInt32(drMonitor["RESOURCEID"]),
                                                            drMonitor["Name"].ToString(),
                                                            drMonitor["DISPLAYNAME"].ToString(),
                                                            drMonitor["TYPE"].ToString(),
                                                            Convert.ToInt32(dr["RESOURCEID"])
                                                        );

                                                    using(WebClient oWCAttribute = new WebClient())
                                                    {
                                                        string sResponseAttribute = string.Empty;
                                                        string sUrlAttribute = string.Format("http://{0}:{1}/AppManager/xml/GetMonitorData?apikey={2}&resourceid={3}", HostName, HostPort, APIKey, drMonitor["RESOURCEID"].ToString());
                                                        sResponseAttribute = oWCAttribute.DownloadString(sUrlAttribute);

                                                        using (DataSet dsDataAttribute = new DataSet())
                                                        {
                                                            sResponseAttribute = CleanInvalidXmlChars(sResponseMonitor);

                                                            using (StringReader stringReader = new StringReader(sResponseAttribute))
                                                            {
                                                                dsDataAttribute.Clear();
                                                                dsDataAttribute.ReadXml(stringReader);
                                                            }

                                                            foreach (DataTable dtAttribute in dsDataAttribute.Tables)
                                                            {
                                                                if (dtAttribute.TableName == "Attribute")
                                                                {
                                                                    //Console.WriteLine("# of Attribute = {0}", dtAttribute.Rows.Count);
                                                                    //Console.WriteLine("-----------------------------------------------------------------");
                                                                    int sNoAttribute = 0;

                                                                    foreach (DataRow drAttr in dtAttribute.Rows)
                                                                    {
                                                                        sNoAttribute++;
                                                                        Console.WriteLine("Server Row # {0} of {1}", sNoAttribute, dtAttribute.Rows.Count);
                                                                        Console.WriteLine("-----------------------------------------------------------------");

                                                                        da.InsertTempAttribute(
                                                                                Convert.ToInt32(drAttr["AttributeID"]),
                                                                                Convert.ToInt32(dr["RESOURCEID"]),
                                                                                drAttr["DISPLAYNAME"].ToString(),
                                                                                drAttr["Units"].ToString()
                                                                            );
                                                                    }

                                                                }
                                                            }
                                                        } //end dsDataAttribute

                                                    }// oWCAttribute
                                                }
                                            }
                                        }

                                    } // end dsDataMonitor Server
                                } //oWebClient Server
                            }
                        }
                    } 
                }// end dsData Server Group

            }// end web Server Group



            //Console.WriteLine("List Of Monitor start");
            //Console.WriteLine("-----------------------------------------------------------------");


            //using (WebClient oWebClient = new WebClient())
            //{
            //    DataSet dsDataMonitor;

            //    string sResponseMonitor = string.Empty;
            //    string sUrlMonitor = string.Format("http://{0}:{1}/AppManager/xml/ListMonitor?apikey={2}&type=all", HostName, HostPort, APIKey);
            //    sResponseMonitor = oWebClient.DownloadString(sUrlMonitor);

            //    sResponseMonitor = CleanInvalidXmlChars(sResponseMonitor);

            //    using (StringReader stringReader = new StringReader(sResponseMonitor))
            //    {
            //        dsDataMonitor = new DataSet();
            //        dsDataMonitor.ReadXml(stringReader);
            //    }

            //    da.TruncateDimServerGroup();

            //    foreach (DataTable dtMonitor in dsDataMonitor.Tables)
            //    {
            //        if (dtMonitor.TableName == "Monitor")
            //        {
            //            Console.WriteLine("# of Monitor = {0}", dtMonitor.Rows.Count);
            //            Console.WriteLine("-----------------------------------------------------------------");
            //            foreach (DataRow drMonitor in dtMonitor.Rows)
            //            {
            //                da.InsertQueryDimServerGroup(
            //                        drMonitor["ASSOCIATEDGROUPS"].ToString(),
            //                        Convert.ToInt32(drMonitor["RESOURCEID"])
            //                    );
            //            }
            //        }
            //    }
            //    dsDataMonitor.Dispose();

            //}

            //int? sVal = da.ScalarQueryCountTempResources();

            //da.UpdateQueryDimResources();
            //da.InsertQueryDimResources();

            //da.UpdateQueryDimAttribute();
            //da.InsertQueryDimAttribute();

            //Console.WriteLine("# of Uploaded = {0}", sVal.ToString());
            //Console.WriteLine("-----------------------------------------------------------------");

        }

        static void Main(string[] args)
        {
            Console.WriteLine("Start .....");
            Console.WriteLine("=================================================================");

            Console.WriteLine("Initializing .....");
            Console.WriteLine("-----------------------------------------------------------------");

            dsTestingServerUtilsTableAdapters.ServerVariablesTableAdapter daServer = new dsTestingServerUtilsTableAdapters.ServerVariablesTableAdapter();
            dsTestingServerUtils.ServerVariablesDataTable dtServer = daServer.GetData();

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

            RefreshMonitorAttribute(HostName, HostPort, APIKey);


        }
    }
}
