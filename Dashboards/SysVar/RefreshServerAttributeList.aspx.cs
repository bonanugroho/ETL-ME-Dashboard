using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dashboards.SysVar
{
    public partial class RefreshServerAttributeList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Models.dsETLManageEngineTableAdapters.qryTempTables da = new Models.dsETLManageEngineTableAdapters.qryTempTables();
                int? sVal = da.ScalarQueryCountDimResources();

                lbCurrent.Text = sVal.ToString();
            }

        }

        public static string CleanInvalidXmlChars(string text)
        {
            // From xml spec valid chars: 
            // #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]     
            // any Unicode character, excluding the surrogate blocks, FFFE, and FFFF. 
            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(text, re, "");
        }


        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            Models.dsETLManageEngineTableAdapters.ServerVariablesTableAdapter daServ = new Models.dsETLManageEngineTableAdapters.ServerVariablesTableAdapter();
            Models.dsETLManageEngine.ServerVariablesDataTable dtServ = daServ.GetData();

            string HostName = string.Empty;
            string HostPort = string.Empty;
            string APIKey = string.Empty;

            foreach (DataRow dr in dtServ.Rows)
            {
                HostName = dr["ServerHostName"].ToString();
                HostPort = dr["ServerPortNo"].ToString();
                APIKey = dr["ServerAPIKey"].ToString();
            }

            WebClient web = new WebClient();
            string url = string.Format("http://{0}:{1}/AppManager/xml/ListServer?apikey={2}&type=all",HostName,HostPort,APIKey);
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

            Models.dsETLManageEngineTableAdapters.qryTempTables da = new Models.dsETLManageEngineTableAdapters.qryTempTables();
            da.TruncateTempResources();
            da.TruncateTempAttribute();

            foreach (DataTable dt in dsData.Tables)
            {
                if (dt.TableName == "Server")
                {

                    foreach(DataRow dr in dt.Rows)
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
                            urlAttribute = string.Format("http://{0}:{1}/AppManager/xml/GetMonitorData?apikey={2}&resourceid={3}",HostName, HostPort, APIKey, dr["RESOURCEID"].ToString());
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

            lblUploaded.Text = sVal.ToString();


        }
    }
}