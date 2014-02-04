using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace BarnloppisSystem
{
    public class SalesRow
    {
        public int SellerId { get; set; }
        public Double Amount { get; set; }
        public string Timestamp { get; set; }

        public SalesRow()
        {
          
        }

        public SalesRow(int sellerId, Double amount)
        {
            SellerId = sellerId;
            Amount = amount;
           
        }

        public SalesRow(int sellerId, Double amount, string timestamp)
        {
            SellerId = sellerId;
            Amount = amount;
            Timestamp = timestamp;
        }

        public override string ToString()
        {
            return string.Format("{0}\t{1} kr", SellerId, Amount);
        }

        public static List<SalesRow> GetSaleRowsBySeller(int sellerId)
        {
            var dbConn = Functions.GetConnection();
            var saleRows = new List<SalesRow>();
            using (dbConn)
            {
                dbConn.Open();
                var cmd = dbConn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = string.Format("SELECT * FROM SaleRows WHERE SellerId = {0}", sellerId);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var sellerid = int.Parse(rdr["SellerId"].ToString());
                    
                    var thisRow = new SalesRow(sellerid, Convert.ToDouble(rdr["Price"]));
                    saleRows.Add(thisRow);
                }
            }
            return saleRows;
        }

        public static List<SalesRow> GetSaleRowsBySale(Guid salesId)
        {
            var dbConn = Functions.GetConnection();
            var saleRows = new List<SalesRow>();
            using (dbConn)
            {
                dbConn.Open();
                var cmd = dbConn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = string.Format("SELECT * FROM SaleRows WHERE SaleId = '{0}'", salesId);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    saleRows.Add(new SalesRow(int.Parse(rdr["SellerId"].ToString()), Convert.ToDouble(rdr["Price"])));
                }
            }
            return saleRows;
        }
    }
}