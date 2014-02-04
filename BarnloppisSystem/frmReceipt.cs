using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BarnloppisSystem
{
    public partial class frmReceipt : Form
    {
        public frmReceipt(Guid saleId)
        {
            InitializeComponent();
            ShowAllRows(saleId);
        }

        private void ShowAllRows(Guid saleId)
        {
            List<SalesRow> saleRows = SalesRow.GetSaleRowsBySale(saleId);
            Double sum = 0;
            foreach (SalesRow thisrow in saleRows)
            { 
                lstSale.Items.Add(thisrow.ToString());
                sum += thisrow.Amount;
            }
            txtSum.Text = sum.ToString();
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
