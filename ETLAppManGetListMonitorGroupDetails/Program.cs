using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ETLAppManGetListMonitorGroupDetails
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
             * Baca server Variables untuk mengambil Nilai Server,Port dan API Key
             * 
             * Truncate table tempServerGroup
             * Truncate table tempServerGroup
             * Truncate table tempServerGroup
             * 
             * Untuk table dimServerGroup http://10.10.8.204:8080/AppManager/xml/ListMGDetails?apikey=e776bf59235b62d523a0e2aaf3a07b76&type=all
             *      - Masuk ke table[MonitorGroup]
             *      - foreach item loop
             *          - Untuk table dimResources http://10.10.8.204:8080/AppManager/xml/ListMGDetails?apikey=e776bf59235b62d523a0e2aaf3a07b76&groupId=CurrentResourceId
             *              - masuk ke table [monitors]
             *              - foreach item loop
             *                  - Untuk table dimAttribute http://10.10.8.204:8080/AppManager/xml/GetMonitorData?apikey=e776bf59235b62d523a0e2aaf3a07b76&resourceid=CurrentResourceId
             *                      - masuk ke table [attribute]
             *                      - foreach item loop
             *                          - Masukan ke table tempAttribute    
             *                  - Masukan ke table tempResources
             *              - Masukan ke table tempServerGroup
             * 
             * Insert semua item dari tempServerGroup yang tidak ada di dimServerGroup
             * Insert semua item dari tempResources yang tidak ada di dimResources
             * Insert Semua item dari tempAttribute yang tidak ada di dimAttribute
             * 
             */

            Console.WriteLine("Start .....");
            Console.WriteLine("=================================================================");

            Console.WriteLine("Initializing .....");
            Console.WriteLine("-----------------------------------------------------------------");

            dsGetListMonitorGroupsDetailsTableAdapters.ServerVariablesTableAdapter daServer = new dsGetListMonitorGroupsDetailsTableAdapters.ServerVariablesTableAdapter();
            dsGetListMonitorGroupsDetails.ServerVariablesDataTable dtServer = daServer.GetData();

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



            Console.WriteLine("Get List Server Group Start .....");
            Console.WriteLine("-----------------------------------------------------------------");

            #region Server Group
            using (WebClient webServerGroup = new WebClient())
            {
                //webServerGroup.Proxy.Credentials


                string urlServerGroup = string.Format("http://{0}:{1}/AppManager/xml/ListMGDetails?apikey={2}&type=all", HostName, HostPort, APIKey);
                string responseServerGroup = webServerGroup.DownloadString(urlServerGroup);

                DataSet dsServerGroup, dsMonitor, dsAttribute;
                dsServerGroup = new DataSet();
                //dsMonitor = new DataSet();
                //dsAttribute = new DataSet();

                responseServerGroup = CleanInvalidXmlChars(responseServerGroup);

                using (StringReader stringReader = new StringReader(responseServerGroup))
                {
                    dsServerGroup.Clear();
                    dsServerGroup.ReadXml(stringReader);
                }

                dsGetListMonitorGroupsDetailsTableAdapters.qryFactUtilization da = new dsGetListMonitorGroupsDetailsTableAdapters.qryFactUtilization();

                Console.WriteLine("Truncate Temp Table .....");

                da.TruncateTempAttribute();
                da.TruncateTempResources();
                da.TruncateTempServerGroup();

                foreach (DataTable dtServerGroup in dsServerGroup.Tables)
                {
                    if (dtServerGroup.TableName == "MonitorGroup")
                    {
                        Console.WriteLine("-----------------------------------------------------------------");
                        Console.WriteLine("# of Server Group = {0}", dtServerGroup.Rows.Count);
                        Console.WriteLine("-----------------------------------------------------------------");

                        int ServerGroupRowNo = 0;

                        foreach (DataRow drServerGroup in dtServerGroup.Rows)
                        {
                            ServerGroupRowNo++;
                            #region Server/Monitor
                            using (WebClient webMonitor = new WebClient())
                            {
                                //Console.WriteLine("Get List Server Start .....");
                                //Console.WriteLine("-----------------------------------------------------------------");

                                string urlMonitor = string.Format("http://{0}:{1}/AppManager/xml/ListMGDetails?apikey={2}&groupId={3}", HostName, HostPort, APIKey, drServerGroup["RESOURCEID"].ToString());
                                string responseMonitor = webMonitor.DownloadString(urlMonitor);

                                responseMonitor = CleanInvalidXmlChars(responseMonitor);
                                using (StringReader stringReader = new StringReader(responseMonitor))
                                {
                                    dsMonitor = new DataSet();
                                    dsMonitor.ReadXml(stringReader);
                                }

                                foreach (DataTable dtMonitor in dsMonitor.Tables)
                                {
                                    if (dtMonitor.TableName == "Monitors")
                                    {
                                        Console.WriteLine("-----------------------------------------------------------------");
                                        Console.WriteLine("# of Server = {0}", dtServer.Rows.Count);
                                        Console.WriteLine("-----------------------------------------------------------------");

                                        int MonitorRowNo = 0;

                                        foreach (DataRow drMonitor in dtMonitor.Rows)
                                        {
                                            MonitorRowNo++;
                                            using (WebClient webAttribute = new WebClient())
                                            {
                                                //Console.WriteLine("Get List Attribute Start .....");
                                                //Console.WriteLine("-----------------------------------------------------------------");

                                                string urlAttribute = string.Format("http://{0}:{1}/AppManager/xml/GetMonitorData?apikey={2}&resourceid={3}", HostName, HostPort, APIKey, drMonitor["RESOURCEID"].ToString());
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
                                                            Console.WriteLine("Server Group ke {0} dari {1} Monitor ke {2} dari {3} Attribut ke {4} dari {5}", ServerGroupRowNo, dtServerGroup.Rows.Count, MonitorRowNo, dtServer.Rows.Count, AttributeRowNo, dtAttribute.Rows.Count);

                                                            da.InsertQueryTempAttribute(
                                                                    Convert.ToInt32(drAttr["AttributeID"]),
                                                                    drAttr["DISPLAYNAME"].ToString(),
                                                                    drAttr["Units"].ToString(),
                                                                    Convert.ToInt32(drMonitor["RESOURCEID"])
                                                                );
                                                        }

                                                    }
                                                }

                                                dsAttribute.Dispose();

                                                //Console.WriteLine("Get List Attribute End .....");
                                                //Console.WriteLine("-----------------------------------------------------------------");
                                            }

                                            Console.WriteLine("Server Group ke {0} dari {1} Monitor ke {2} dari {3}", ServerGroupRowNo, dtServerGroup.Rows.Count, MonitorRowNo, dtServer.Rows.Count);
                                            da.InsertQueryTempResources(
                                                Convert.ToInt32(drMonitor["RESOURCEID"])
                                                , drMonitor["Name"].ToString()
                                                , drMonitor["DISPLAYNAME"].ToString()
                                                , drMonitor["TYPE"].ToString()
                                                , Convert.ToInt32(drServerGroup["RESOURCEID"])
                                                );
                                        }
                                    }
                                }

                                dsMonitor.Dispose();

                                //Console.WriteLine("Get List Server End .....");
                                //Console.WriteLine("-----------------------------------------------------------------");

                            }

                            #endregion

                            Console.WriteLine("ServerGroup ke {0} dari {1}",ServerGroupRowNo, dtServerGroup.Rows.Count);
                            da.InsertQueryTempServerGroup(
                                drServerGroup["DISPLAYNAME"].ToString()
                                , Convert.ToInt32(drServerGroup["RESOURCEID"])
                                );

                        }
                    }
                }
                dsServerGroup.Dispose();

                Console.WriteLine("Insert to Master Table .....");
                da.InsertQueryDimAttribute();
                da.InsertQueryDimResources();
                da.InsertQueryDimServerGroup();
            }
            #endregion
            Console.WriteLine("Get List Server Group Done .....");
            Console.WriteLine("-----------------------------------------------------------------");


            Console.WriteLine("Finalizing");
            Console.WriteLine("-----------------------------------------------------------------");

            //daUtil.DeleteQueryFactUtilization(); // tidak dipakai lagi

            Console.WriteLine("DONE ...");
            Console.WriteLine("-----------------------------------------------------------------");
            //Console.Read();



        }
    }
}
