using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETLNetflowAnalyzer
{
    class Program
    {
        private static void GetNetflowRouter(NpgsqlConnection oConn, dsETLNetflowAnalyzerTableAdapters.qryNetworkUtilization daQry)
        {
            try
            {
                string sSQL = @"SELECT router_id, router_ip, router_displayname FROM netflow_router;";
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sSQL, oConn);
                ds.Reset();

                da.Fill(ds);

                dt = ds.Tables[0];

                foreach (DataRow dr in dt.Rows)
                {
                    daQry.InsertQuerydimCabang(
                        Convert.ToInt64(dr["router_id"]),
                        dr["router_ip"].ToString(),
                        dr["router_displayname"].ToString()
                    );
                }

            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        private static void GetNetflowInterface(NpgsqlConnection oConn, dsETLNetflowAnalyzerTableAdapters.qryNetworkUtilization daQry)
        {
            try
            {
                string sSQL = @"SELECT interface_id, router_id, snmp_description, speed, link_name FROM netflow_interface;";
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sSQL, oConn);
                ds.Reset();

                da.Fill(ds);

                dt = ds.Tables[0];

                foreach (DataRow dr in dt.Rows)
                {
                    daQry.InsertQuerydimInterface(
                        Convert.ToInt64(dr["interface_id"]),
                        dr["snmp_description"].ToString(),
                        dr["link_name"].ToString(),
                        Convert.ToInt64(dr["speed"]),
                        Convert.ToInt64(dr["router_id"])
                    );
                }

            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        //private static void GetInDstIf10Min(NpgsqlConnection oConn, dsETLNetflowAnalyzerTableAdapters.qryNetworkUtilization daQry)
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append(" SELECT");
        //        sb.Append(" MyTbl.calYear || '-' || calMonth || '-' || calDay || ' ' || calHour || ':00' as MyDate");
        //        sb.Append(" , MyTbl.InterfaceId");
        //        sb.Append(" , MyTbl.fMinOctets");
        //        sb.Append(" , MyTbl.fMaxOctets");
        //        sb.Append(" , MyTbl.fAvgOctets");
        //        sb.Append(" FROM");
        //        sb.Append(" (");
        //        sb.Append(" SELECT ");
        //        sb.Append(" dst_if as InterfaceId");
        //        sb.Append(" , EXTRACT(ISOYEAR FROM a.collection_time) as CalYear");
        //        sb.Append(" , EXTRACT(MONTH FROM a.collection_time) as CalMonth");
        //        sb.Append(" , EXTRACT(DAY FROM a.collection_time) as CalDay");
        //        sb.Append(" , EXTRACT(HOUR FROM a.collection_time) as CalHour");
        //        sb.Append(" , avg(min_octets)::integer as fMinOctets");
        //        sb.Append(" , avg(max_octets)::integer as fMaxOctets");
        //        sb.Append(" , avg(avg_octets)::integer as fAvgOctets");
        //        sb.Append(" FROM dstif10min a");
        //        //sb.Append(" where a.collection_time >= to_timestamp(to_char(current_date, 'YYYY-MM-DD') || to_char(extract(HOUR from current_time) , '00') || ':00', 'YYYY-MM-DD HH24:MI')::timestamp without time zone -  interval '4 hour'");
        //        //sb.Append(" and a.collection_time < to_timestamp(to_char(current_date, 'YYYY-MM-DD') || to_char(extract(HOUR from current_time) , '00') || ':00', 'YYYY-MM-DD HH24:MI')::timestamp without time zone -  interval '3 hour'");
        //        sb.Append(" GROUP BY");
        //        sb.Append(" a.dst_if");
        //        sb.Append(" , EXTRACT(ISOYEAR FROM a.collection_time)");
        //        sb.Append(" , EXTRACT(MONTH FROM a.collection_time)");
        //        sb.Append(" , EXTRACT(DAY FROM a.collection_time)");
        //        sb.Append(" , EXTRACT(HOUR FROM a.collection_time)");
        //        sb.Append(" ) as myTbl");


        //        string sSQL = sb.ToString();
        //        DataSet ds = new DataSet();
        //        DataTable dt = new DataTable();

        //        NpgsqlDataAdapter da = new NpgsqlDataAdapter(sSQL, oConn);
        //        ds.Reset();

        //        da.Fill(ds);

        //        dt = ds.Tables[0];
        //        Console.WriteLine(" # of Rows = {0} .....", dt.Rows.Count);

        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            daQry.InsertQueryDstUtilization(
        //                Convert.ToDateTime(dr["MyDate"].ToString()),
        //                Convert.ToInt32(dr["InterfaceId"]),
        //                Convert.ToInt64(dr["fMinOctets"]),
        //                Convert.ToInt64(dr["fMaxOctets"]),
        //                Convert.ToInt64(dr["fAvgOctets"])
        //            );
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }

        //}

        //private static void GetOutSrcIf10Min(NpgsqlConnection oConn, dsETLNetflowAnalyzerTableAdapters.qryNetworkUtilization daQry)
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append(" SELECT");
        //        sb.Append(" MyTbl.calYear || '-' || calMonth || '-' || calDay || ' ' || calHour || ':00' as MyDate");
        //        sb.Append(" , MyTbl.InterfaceId");
        //        sb.Append(" , MyTbl.fMinOctets");
        //        sb.Append(" , MyTbl.fMaxOctets");
        //        sb.Append(" , MyTbl.fAvgOctets");
        //        sb.Append(" FROM");
        //        sb.Append(" (");
        //        sb.Append(" SELECT ");
        //        sb.Append(" src_if as InterfaceId");
        //        sb.Append(" , EXTRACT(ISOYEAR FROM a.collection_time) as CalYear");
        //        sb.Append(" , EXTRACT(MONTH FROM a.collection_time) as CalMonth");
        //        sb.Append(" , EXTRACT(DAY FROM a.collection_time) as CalDay");
        //        sb.Append(" , EXTRACT(HOUR FROM a.collection_time) as CalHour");
        //        sb.Append(" , avg(min_octets)::integer as fMinOctets");
        //        sb.Append(" , avg(max_octets)::integer as fMaxOctets");
        //        sb.Append(" , avg(avg_octets)::integer as fAvgOctets");
        //        sb.Append(" FROM srcif10min a");
        //        //sb.Append(" where a.collection_time >= to_timestamp(to_char(current_date, 'YYYY-MM-DD') || to_char(extract(HOUR from current_time) , '00') || ':00', 'YYYY-MM-DD HH24:MI')::timestamp without time zone -  interval '4 hour'");
        //        //sb.Append(" and a.collection_time < to_timestamp(to_char(current_date, 'YYYY-MM-DD') || to_char(extract(HOUR from current_time) , '00') || ':00', 'YYYY-MM-DD HH24:MI')::timestamp without time zone -  interval '3 hour'");
        //        sb.Append(" GROUP BY");
        //        sb.Append(" a.src_if");
        //        sb.Append(" , EXTRACT(ISOYEAR FROM a.collection_time)");
        //        sb.Append(" , EXTRACT(MONTH FROM a.collection_time)");
        //        sb.Append(" , EXTRACT(DAY FROM a.collection_time)");
        //        sb.Append(" , EXTRACT(HOUR FROM a.collection_time)");
        //        sb.Append(" ) as myTbl");


        //        string sSQL = sb.ToString();
        //        DataSet ds = new DataSet();
        //        DataTable dt = new DataTable();

        //        NpgsqlDataAdapter da = new NpgsqlDataAdapter(sSQL, oConn);
        //        ds.Reset();

        //        da.Fill(ds);

        //        dt = ds.Tables[0];

        //        Console.WriteLine(" # of Rows = {0} .....", dt.Rows.Count);

        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            daQry.InsertQuerySrcUtilization(
        //                Convert.ToDateTime(dr["MyDate"].ToString()),
        //                Convert.ToInt32(dr["InterfaceId"]),
        //                Convert.ToInt64(dr["fMinOctets"]),
        //                Convert.ToInt64(dr["fMaxOctets"]),
        //                Convert.ToInt64(dr["fAvgOctets"])
        //            );
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }

        //}


        private static void GetInDstifhourly(NpgsqlConnection oConn, dsETLNetflowAnalyzerTableAdapters.qryNetworkUtilization daQry)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" SELECT dst_if, collection_time, min_octets, max_octets,");
                sb.Append(" avg_octets");
                sb.Append(" FROM dstifhourly ");
                sb.Append(" where collection_time >= to_timestamp(to_char(current_date, 'YYYY-MM-DD') || ' 00:00 ' , 'YYYY-MM-DD HH24:MI')::timestamp without time zone -  interval '1 day'");
                sb.Append(" and collection_time < to_timestamp(to_char(current_date, 'YYYY-MM-DD') || ' 00:00 ' , 'YYYY-MM-DD HH24:MI')::timestamp without time zone");
                //sb.Append(" WHERE collection_time >= '2015-03-18 20:00' ");
                //sb.Append(" AND collection_time < to_timestamp(to_char(current_date, 'YYYY-MM-DD') || ' 00:00 ' , 'YYYY-MM-DD HH24:MI')::timestamp without time zone");

                string sSQL = sb.ToString();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sSQL, oConn);
                ds.Reset();

                da.Fill(ds);

                dt = ds.Tables[0];

                Console.WriteLine(" # of Rows = {0} .....", dt.Rows.Count);

                foreach (DataRow dr in dt.Rows)
                {
                    daQry.InsertQueryDstUtilization(
                        Convert.ToDateTime(dr["collection_time"].ToString()),
                        Convert.ToInt32(dr["dst_if"]),
                        Convert.ToInt64(dr["min_octets"]),
                        Convert.ToInt64(dr["max_octets"]),
                        Convert.ToInt64(dr["avg_octets"])
                    );
                }
                
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        //private static void GetInDstifhourly_dstif_1h_20150320180822(NpgsqlConnection oConn, dsETLNetflowAnalyzerTableAdapters.qryNetworkUtilization daQry)
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append(" SELECT dst_if, collection_time, min_octets, max_octets,");
        //        sb.Append(" avg_octets");
        //        sb.Append(" FROM dstif_1h_20150320180822  ");
        //        //sb.Append(" where collection_time >= to_timestamp(to_char(current_date, 'YYYY-MM-DD') || ' 00:00 ' , 'YYYY-MM-DD HH24:MI')::timestamp without time zone -  interval '2 day'");
        //        //sb.Append(" and collection_time < to_timestamp(to_char(current_date, 'YYYY-MM-DD') || ' 00:00 ' , 'YYYY-MM-DD HH24:MI')::timestamp without time zone -  interval '1 day'");
        //        sb.Append(" WHERE collection_time >= '2015-03-18 20:00' ");

        //        string sSQL = sb.ToString();
        //        DataSet ds = new DataSet();
        //        DataTable dt = new DataTable();

        //        NpgsqlDataAdapter da = new NpgsqlDataAdapter(sSQL, oConn);
        //        ds.Reset();

        //        da.Fill(ds);

        //        dt = ds.Tables[0];

        //        Console.WriteLine(" # of Rows = {0} .....", dt.Rows.Count);

        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            daQry.InsertQueryDstUtilization(
        //                Convert.ToDateTime(dr["collection_time"].ToString()),
        //                Convert.ToInt32(dr["dst_if"]),
        //                Convert.ToInt64(dr["min_octets"]),
        //                Convert.ToInt64(dr["max_octets"]),
        //                Convert.ToInt64(dr["avg_octets"])
        //            );
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }
        //}

        //private static void GetInDstifhourly_dstif_1h_20150324185722(NpgsqlConnection oConn, dsETLNetflowAnalyzerTableAdapters.qryNetworkUtilization daQry)
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append(" SELECT dst_if, collection_time, min_octets, max_octets,");
        //        sb.Append(" avg_octets");
        //        sb.Append(" FROM dstif_1h_20150324185722 ");
        //        //sb.Append(" where collection_time >= to_timestamp(to_char(current_date, 'YYYY-MM-DD') || ' 00:00 ' , 'YYYY-MM-DD HH24:MI')::timestamp without time zone -  interval '2 day'");
        //        //sb.Append(" and collection_time < to_timestamp(to_char(current_date, 'YYYY-MM-DD') || ' 00:00 ' , 'YYYY-MM-DD HH24:MI')::timestamp without time zone -  interval '1 day'");
        //        sb.Append(" WHERE collection_time >= '2015-03-18 20:00' ");

        //        string sSQL = sb.ToString();
        //        DataSet ds = new DataSet();
        //        DataTable dt = new DataTable();

        //        NpgsqlDataAdapter da = new NpgsqlDataAdapter(sSQL, oConn);
        //        ds.Reset();

        //        da.Fill(ds);

        //        dt = ds.Tables[0];

        //        Console.WriteLine(" # of Rows = {0} .....", dt.Rows.Count);

        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            daQry.InsertQueryDstUtilization(
        //                Convert.ToDateTime(dr["collection_time"].ToString()),
        //                Convert.ToInt32(dr["dst_if"]),
        //                Convert.ToInt64(dr["min_octets"]),
        //                Convert.ToInt64(dr["max_octets"]),
        //                Convert.ToInt64(dr["avg_octets"])
        //            );
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }
        //}


        private static void GetInSrcifhourly(NpgsqlConnection oConn, dsETLNetflowAnalyzerTableAdapters.qryNetworkUtilization daQry)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" SELECT src_if, collection_time, min_octets, max_octets,");
                sb.Append(" avg_octets");
                sb.Append(" FROM srcifhourly ");
                sb.Append(" where collection_time >= to_timestamp(to_char(current_date, 'YYYY-MM-DD') || ' 00:00 ' , 'YYYY-MM-DD HH24:MI')::timestamp without time zone -  interval '1 day'");
                sb.Append(" and collection_time < to_timestamp(to_char(current_date, 'YYYY-MM-DD') || ' 00:00 ' , 'YYYY-MM-DD HH24:MI')::timestamp without time zone");
                //sb.Append(" WHERE collection_time >= '2015-03-18 20:00' ");
                //sb.Append(" AND collection_time < to_timestamp(to_char(current_date, 'YYYY-MM-DD') || ' 00:00 ' , 'YYYY-MM-DD HH24:MI')::timestamp without time zone");

                string sSQL = sb.ToString();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sSQL, oConn);
                ds.Reset();

                da.Fill(ds);

                dt = ds.Tables[0];

                Console.WriteLine(" # of Rows = {0} .....", dt.Rows.Count);

                foreach (DataRow dr in dt.Rows)
                {
                    daQry.InsertQuerySrcUtilization(
                        Convert.ToDateTime(dr["collection_time"].ToString()),
                        Convert.ToInt32(dr["src_if"]),
                        Convert.ToInt64(dr["min_octets"]),
                        Convert.ToInt64(dr["max_octets"]),
                        Convert.ToInt64(dr["avg_octets"])
                    );
                }

            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        //private static void GetInSrcifhourly_srcif_1h_20150320180822(NpgsqlConnection oConn, dsETLNetflowAnalyzerTableAdapters.qryNetworkUtilization daQry)
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append(" SELECT src_if, collection_time, min_octets, max_octets,");
        //        sb.Append(" avg_octets");
        //        sb.Append(" FROM srcif_1h_20150320180822  ");
        //        //sb.Append(" where collection_time >= to_timestamp(to_char(current_date, 'YYYY-MM-DD') || ' 00:00 ' , 'YYYY-MM-DD HH24:MI')::timestamp without time zone -  interval '2 day'");
        //        //sb.Append(" and collection_time < to_timestamp(to_char(current_date, 'YYYY-MM-DD') || ' 00:00 ' , 'YYYY-MM-DD HH24:MI')::timestamp without time zone -  interval '1 day'");
        //        sb.Append(" WHERE collection_time >= '2015-03-18 20:00' ");

        //        string sSQL = sb.ToString();
        //        DataSet ds = new DataSet();
        //        DataTable dt = new DataTable();

        //        NpgsqlDataAdapter da = new NpgsqlDataAdapter(sSQL, oConn);
        //        ds.Reset();

        //        da.Fill(ds);

        //        dt = ds.Tables[0];

        //        Console.WriteLine(" # of Rows = {0} .....", dt.Rows.Count);

        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            daQry.InsertQuerySrcUtilization(
        //                Convert.ToDateTime(dr["collection_time"].ToString()),
        //                Convert.ToInt32(dr["src_if"]),
        //                Convert.ToInt64(dr["min_octets"]),
        //                Convert.ToInt64(dr["max_octets"]),
        //                Convert.ToInt64(dr["avg_octets"])
        //            );
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }
        //}

        //private static void GetInSrcifhourly_srcif_1h_20150324185722(NpgsqlConnection oConn, dsETLNetflowAnalyzerTableAdapters.qryNetworkUtilization daQry)
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append(" SELECT src_if, collection_time, min_octets, max_octets,");
        //        sb.Append(" avg_octets");
        //        sb.Append(" FROM srcif_1h_20150324185722 ");
        //        //sb.Append(" where collection_time >= to_timestamp(to_char(current_date, 'YYYY-MM-DD') || ' 00:00 ' , 'YYYY-MM-DD HH24:MI')::timestamp without time zone -  interval '2 day'");
        //        //sb.Append(" and collection_time < to_timestamp(to_char(current_date, 'YYYY-MM-DD') || ' 00:00 ' , 'YYYY-MM-DD HH24:MI')::timestamp without time zone -  interval '1 day'");
        //        sb.Append(" WHERE collection_time >= '2015-03-18 20:00' ");

        //        string sSQL = sb.ToString();
        //        DataSet ds = new DataSet();
        //        DataTable dt = new DataTable();

        //        NpgsqlDataAdapter da = new NpgsqlDataAdapter(sSQL, oConn);
        //        ds.Reset();

        //        da.Fill(ds);

        //        dt = ds.Tables[0];

        //        Console.WriteLine(" # of Rows = {0} .....", dt.Rows.Count);

        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            daQry.InsertQuerySrcUtilization(
        //                Convert.ToDateTime(dr["collection_time"].ToString()),
        //                Convert.ToInt32(dr["src_if"]),
        //                Convert.ToInt64(dr["min_octets"]),
        //                Convert.ToInt64(dr["max_octets"]),
        //                Convert.ToInt64(dr["avg_octets"])
        //            );
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }
        //}


        static void Main(string[] args)
        {
            /*
             *
             * Setting diambil dari NetworkUtilization.ServerVariables
             * 1. Tarik data netflow_router, masukan ke table temp, yang tidak ada dimasukan ke NetworkUtilization.dimCabang
             * 2. Tarik data netflow_interface, masukan ke table temp, yang tidak ada dimasukan ke NetworkUtilization.dimInterface
             * 3. Tarik data dstif10min & srcif10min masukan ke table temp, inner join dgn dimCabang, dimInterface, dimRegional 
             *    masukan ke table NetworkUtilization.factUtils
             * 
             */


            try
            {

                Console.WriteLine("Start .....");
                Console.WriteLine("=================================================================");

                Console.WriteLine("Initializing .....");
                Console.WriteLine("-----------------------------------------------------------------");

                dsETLNetflowAnalyzerTableAdapters.ServerVariablesTableAdapter daServer = new dsETLNetflowAnalyzerTableAdapters.ServerVariablesTableAdapter();
                dsETLNetflowAnalyzer.ServerVariablesDataTable dtServer = daServer.GetData();

                dsETLNetflowAnalyzerTableAdapters.qryNetworkUtilization daUtils = new dsETLNetflowAnalyzerTableAdapters.qryNetworkUtilization();

                string HostName = string.Empty;
                string HostPort = string.Empty;
                string UserId = string.Empty;
                string Password = string.Empty;
                string DatabaseName = string.Empty;

                Console.WriteLine("Get Server Variables, # of Server = {0} .....", dtServer.Rows.Count);
                Console.WriteLine("-----------------------------------------------------------------");

                int ServerNo = 1;
                string connString = string.Empty;
                foreach (DataRow dr in dtServer.Rows)
                {
                    Console.WriteLine("Server # {0} of {1}", ServerNo,dtServer.Rows.Count);
                    Console.WriteLine("-----------------------------------------------------------------");

                    HostName = dr["ServerIPAddress"].ToString();
                    HostPort = dr["ServerPortNo"].ToString();
                    UserId = dr["UserId"].ToString();
                    Password = dr["Password"].ToString();
                    DatabaseName = dr["DatabaseName"].ToString();

                    connString = string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                        HostName, HostPort, UserId, Password, DatabaseName);

                    NpgsqlConnection oConn = new NpgsqlConnection(connString);
                    oConn.Open();


                    Console.WriteLine("Get Netflow Router -- Start");
                    Console.WriteLine("-----------------------------------------------------------------");

                    //GetNetflowRouter(oConn, daUtils);

                    Console.WriteLine("Get Netflow Router -- Done");
                    Console.WriteLine("-----------------------------------------------------------------");

                    Console.WriteLine("Get Netflow Interface -- Start");
                    Console.WriteLine("-----------------------------------------------------------------");

                    //GetNetflowInterface(oConn, daUtils);

                    Console.WriteLine("Get Netflow Interface -- Done");
                    Console.WriteLine("-----------------------------------------------------------------");

                    Console.WriteLine("Get IN Archieved Data -- Start");
                    Console.WriteLine("-----------------------------------------------------------------");

                    //GetInDstIf10Min(oConn, daUtils);


                    GetInDstifhourly(oConn, daUtils);

                    Console.WriteLine("Get IN Archieved Data -- Done");
                    Console.WriteLine("-----------------------------------------------------------------");

                    Console.WriteLine("Get OUT Archieved Data -- Start");
                    Console.WriteLine("-----------------------------------------------------------------");

                    //GetOutSrcIf10Min(oConn, daUtils);

                    GetInSrcifhourly(oConn, daUtils);

                    Console.WriteLine("Get OUT Archieved Data -- Done");
                    Console.WriteLine("-----------------------------------------------------------------");

                    oConn.Close();
                
                }

                daUtils.InsertFactUtilizationFromIn();
                daUtils.InsertFactUtilizationFromOut();

                daUtils.TruncateTempTable();

                Console.WriteLine("DONE ...");
                Console.WriteLine("-----------------------------------------------------------------");
                //Console.Read();


            }
            catch (Exception msg)
            {
                Console.WriteLine(msg.ToString());
                throw ;
            }
        }
    }
}
