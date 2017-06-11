using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ETLAvailability
{
    class Program
    {
        //Data host, port, dan APIkey diambil dari database tabel ServerUtilization.ServerVariables
        //Mengambil semua data ResourceId dari database tabel ServerUtilization.dimServerGroup
        //Melakukan perulangan sebanyak data ResourceId untuk mengambil data AVAILTODAYPERCENT dari http://{host}:{port}/AppManager/xml/ListMGDetails?apikey={APIkey}&groupId={ResourceId}
        //Mengambil data AVAILTODAYMENIT dari hasil AVAILTODAYPERCENT*(24*60)
        //Menginputkan data AVAILTODAYPERCENT, AVAILTODAYMENIT, ResourceId, FullDate ke tabel ServerUtilization.factAvailability

        public static string CleanInvalidXmlChars(string text)
        {
            // From xml spec valid chars: 
            // #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]     
            // any Unicode character, excluding the surrogate blocks, FFFE, and FFFF. 
            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(text, re, "");
        }

        private static void RefreshServerAttribute()
        {
            Console.WriteLine("Get Refresh Server Attribute Start .....");
            Console.WriteLine("-----------------------------------------------------------------");

            //connection string
            //var connection = System.Configuration.ConfigurationManager.ConnectionStrings["Conn"].ConnectionString;
            System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection();
            connection.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Conn"].ConnectionString;
            //connection.ConnectionString = @"Data Source=ALDE-PC\SQLEXPRESS;Initial Catalog=ETLManageEngine;Integrated Security=True";

            DataSet dsDataAttribute;
            try
            {
                using (SqlConnection conn = connection)
                {
                    //membuka koneksi database
                    conn.Open();
                    //mengambil data resource dari tabel dimResource
                    SqlDataAdapter command = new SqlDataAdapter("Select * From ServerUtilization.dimServerGroup", conn);
                    DataSet dimResource = new DataSet("Resource");
                    command.FillSchema(dimResource, SchemaType.Source, "ServerUtilization.dimServerGroup");
                    command.Fill(dimResource, "ServerUtilization.dimServerGroup");

                    DataTable tbldimResource;
                    tbldimResource = dimResource.Tables["ServerUtilization.dimServerGroup"];

                    //Mengambil data host, webapi, dan port
                    command = new SqlDataAdapter("Select * From dbo.ServerVariables", conn);
                    DataSet ServerVariables = new DataSet("ServerVariables");
                    command.FillSchema(ServerVariables, SchemaType.Source, "dbo.ServerVariables");
                    command.Fill(ServerVariables, "dbo.ServerVariables");

                    DataTable tblServerVariables;
                    tblServerVariables = ServerVariables.Tables["dbo.ServerVariables"];

                    //Mengisi data variable host, port, webapi
                    string host = tblServerVariables.Rows[0][2].ToString();
                    string port = tblServerVariables.Rows[0][3].ToString();
                    string webapi = tblServerVariables.Rows[0][4].ToString();

                    //looping untuk mengambil data availability setiap ResourceId yang telah didapat
                    int ServerGroupRowNo = 0;
                    for (int i = 0; i < tbldimResource.Rows.Count; i++)
                    {
                        ServerGroupRowNo++;
                        int groupid = Convert.ToInt32(tbldimResource.Rows[i][2].ToString());
                        using (WebClient webAttribute = new WebClient())
                        {
                            string responseAttribute = string.Empty;
                            string urlAttribute = string.Format("http://{0}:{1}/AppManager/xml/ListMGDetails?apikey={2}&groupId={3}", host, port, webapi, groupid);
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
                                if (dtAttribute.TableName == "Monitors")
                                {
                                    Console.WriteLine("# of ResourceId = {0}", groupid);
                                    Console.WriteLine("-----------------------------------------------------------------");
                                    DateTime Tanggal = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":00");

                                    int MonitorsRowNo = 0;
                                    foreach (DataRow drAttr in dtAttribute.Rows)
                                    {
                                        MonitorsRowNo++;

                                        //Ganti BN : begin Kenapa pakai kolom no, gunakan kolom name. Kemudian Parsing menggunakan cara dibawah. 
                                        //decimal AvailabilityToday = Convert.ToDecimal(drAttr[5]);

                                        NumberStyles style = NumberStyles.Number | NumberStyles.AllowDecimalPoint;
                                        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

                                        Decimal AvailabilityToday;
                                        Decimal.TryParse(drAttr["TODAYAVAILPERCENT"].ToString(), style, culture, out AvailabilityToday);

                                        //Ganti BN : end

                                        int resourceid = Convert.ToInt32(drAttr[11]);
                                        using (SqlCommand cmd = new SqlCommand("INSERT INTO ServerUtilization.factAvailability (todayAvailability, todayAvailabilityMenit, ResourceId, FullDate) VALUES (@todayAvailability, @todayAvailabilityMenit, @ResourceId, @FullDate)", conn))
                                        {
                                            //cmd.Connection = conn;
                                            cmd.Parameters.AddWithValue("@todayAvailability", AvailabilityToday);
                                            //Ganti BN : Availibility Today adalah nilai percent, kenapa langsung dikali harusnya AvailabilityToday/100 * 1440
                                            //cmd.Parameters.AddWithValue("@todayAvailabilityMenit", (AvailabilityToday * 1440));
                                            cmd.Parameters.AddWithValue("@todayAvailabilityMenit", ((AvailabilityToday/100) * 1440));
                                            cmd.Parameters.AddWithValue("@ResourceId", resourceid);
                                            //cmd.Parameters.AddWithValue("@FullDate", String.Format("{0:yyyy-MM-dd hh:mm}",Tanggal));
                                            cmd.Parameters.AddWithValue("@FullDate", Tanggal);
                                            cmd.ExecuteNonQuery();
                                        }
                                        Console.WriteLine("Inserted ServerGroup ke {0} dari {1}, Server ke {2} dari {3} ",ServerGroupRowNo,tbldimResource.Rows.Count,MonitorsRowNo,dtAttribute.Rows.Count);
                                    }
                                }
                            }
                            //dsDataAttribute.Dispose();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Console.Read();
            }
            Console.WriteLine("----------------------------Done----------------------------");

        }

        static void Main(string[] args)
        {
            RefreshServerAttribute();
        }
    }
}