using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetDataProdToSimulator
{
    class Program
    {
        private static void InsertToDestination(NpgsqlConnection oDest, NpgsqlConnection oSource)
        {
            try
            {

                string sSQL = @"SELECT id, src_if, octets, packets, collection_time, min_octets, max_octets, avg_octets FROM srcif10min ;";
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sSQL, oSource);
                ds.Reset();

                da.Fill(ds);

                dt = ds.Tables[0];

                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = oDest;

                foreach (DataRow dr in dt.Rows)
                {
                    command.CommandText = "INSERT INTO srcif10min(id, src_if, octets, packets, collection_time, min_octets, max_octets, avg_octets) VALUES (" +
                        dr["id"].ToString() + ", " + dr["src_if"].ToString() + ", " + dr["octets"].ToString() + ", " + dr["packets"].ToString() + ", '" + dr["collection_time"].ToString() + "'" +
                        ", " + dr["min_octets"].ToString() + ", " + dr["max_octets"].ToString() + ", " + dr["avg_octets"].ToString() + ");";

                    command.ExecuteNonQuery();

                    
                }

                //NpgsqlCommand command = new NpgsqlCommand("INSERT INTO dstif10min(id, dst_if, octets, packets, collection_time, min_octets, max_octets, " +
                //                                "avg_octets) VALUES (5000, 5000, 5000, 5000, '2015-2-1 16:30', 5000, 5000, 5000);", oDest);

                //command.CommandText = "INSERT INTO dstif10min(id, dst_if, octets, packets, collection_time, min_octets, max_octets, " +
                //                                "avg_octets) VALUES (5000, 5000, 5000, 5000, '2015-2-1 16:30', 5000, 5000, 5000);";
                //Int32 rowsaffected;

                //rowsaffected = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                string sDestConnString = string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                                        "localhost", "5432", "postgres", "password1!", "Netflow");

                NpgsqlConnection oDestConn = new NpgsqlConnection(sDestConnString);
                oDestConn.Open();


                string sSourceConnString = string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                                        "10.226.16.63", "13310", "postgres", "", "netflow");

                NpgsqlConnection oSourceConn = new NpgsqlConnection(sSourceConnString);
                oSourceConn.Open();
                
                Console.WriteLine("Get Netflow Router -- Start");
                Console.WriteLine("-----------------------------------------------------------------");

                InsertToDestination(oDestConn, oSourceConn);

                Console.WriteLine("Get Netflow Router -- Done");
                Console.WriteLine("-----------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
