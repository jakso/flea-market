using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace BarnloppisSystem
{
    public class Functions
    {
        public static int SaveToDatabase(string sql)
        {
            try
            {
                var dbConn = GetConnection();
                using (dbConn)
                {
                    dbConn.Open();
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static OleDbConnection GetConnection()
        {
            var dbConn = new OleDbConnection { ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=data\klädbytaredag.dat" };
            return dbConn;
        }
    }
}