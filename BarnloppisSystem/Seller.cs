using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BarnloppisSystem
{
    public class Seller
    {
        public int SellerNo { get; set; }
        public String Name { get; set; }
        public String Telephone { get; set; }
        public String Comment { get; set; }
        public String Email { get; set; }
        public List<SalesRow> SaleRows { get; set; }
        public int Items { get; set; }
        public Double Sum { get; set; }

        public Seller()
        {
            SellerNo = -1;
            Name = string.Empty;
            Telephone = string.Empty;
            Comment = string.Empty;
            SaleRows = null;
        }

        public Seller(int sellerno, string name, string telephone, string comment, string email)
        {
            SellerNo = sellerno;
            Name = name;
            Telephone = telephone;
            Comment = comment;
            Email = email;
            SaleRows = SalesRow.GetSaleRowsBySeller(SellerNo);
        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}{1}{3}{1}{4}{1}{5}", SellerNo, "\t", Name, Telephone, Comment, Email);
        }

        public static List<Seller> GetSellers()
        {
            var dbConn = Functions.GetConnection();
            var sellers = new List<Seller>();

            using (dbConn)
            {
                dbConn.Open();
                var cmd = dbConn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM Seller";
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    sellers.Add(new Seller(int.Parse(rdr["Id"].ToString()), rdr["Name"].ToString(), rdr["Phone"].ToString(), rdr["Comment"].ToString(), rdr["Email"].ToString()));
                }
            }
            return sellers;
        }

        public static Seller GetSeller(int sellerId)
        {
            try
            {
                var dbConn = Functions.GetConnection();
                var seller = new Seller();
                using (dbConn)
                {
                    dbConn.Open();
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM Seller WHERE Id = " + sellerId;
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        seller = new Seller(int.Parse(rdr["Id"].ToString()), rdr["Name"].ToString(), rdr["Phone"].ToString(), rdr["Comment"].ToString(), rdr["Email"].ToString());
                    }
                }

                if (seller.SellerNo == -1)
                {
                    throw new Exception("Felaktigt försäljarenummer");
                }
                return seller;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Save()
        {
            string sql = string.Empty;

            try
            {
                GetSeller(SellerNo);
                sql =
                    String.Format(
                        "UPDATE Seller SET Name = '{1}', Phone = '{2}', Comment = '{3}', Email = '{4}' WHERE Id = {0}",
                        SellerNo, Name, Telephone, Comment, Email);


            }
            catch (Exception ex)
            {
                if (ex.Message == "Felaktigt försäljarenummer")
                {

                    sql =
                        String.Format(
                            "INSERT INTO Seller (Id, Name, Phone, Comment, Email) VALUES ({0},'{1}','{2}','{3}','{4}')",
                            SellerNo, Name, Telephone, Comment, Email);
                }
                else
                {
                    throw ex;
                }
            }

            if (sql != string.Empty)
            {
                try
                {
                    Functions.SaveToDatabase(sql);
                }
                catch (Exception ex)
                {     
                    throw ex;
                }
            }
        }

        public static int GetNextSellerNo()
        {
            var sellers = GetSellers();
            var query = from p in sellers select p.SellerNo;
            return query.Count()>0 ? query.Max() + 1 : 1;
        }
    }
}