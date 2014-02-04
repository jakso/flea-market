using System;
using System.Collections.Generic;

namespace BarnloppisSystem
{
    public class Sale
    {
        public Guid SaleNo { get; set; }
        public List<SalesRow> SalesRows { get; set; }
        public Double Sum { get; set; }

        public Sale()
        {
            SaleNo = Guid.NewGuid();
            SalesRows = new List<SalesRow>();
        }

        public void RemoveRow(SalesRow thisRow)
        {
            SalesRows.Remove(thisRow);
        }

        public void Save()
        { 
            try
            {
                string sql = String.Format("INSERT INTO Sale ([Id], [Sum]) VALUES ('{0}','{1}')", SaleNo, Sum);
                Functions.SaveToDatabase(sql);
                
                foreach (var thisRow in SalesRows)
                {
                    sql = String.Format("INSERT INTO SaleRows (SaleId, Price, SellerId) VALUES ('{0}','{1}',{2})", SaleNo, thisRow.Amount, thisRow.SellerId);
                    Functions.SaveToDatabase(sql);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }     
        }

       
    }
}