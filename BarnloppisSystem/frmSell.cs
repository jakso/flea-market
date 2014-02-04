using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BarnloppisSystem
{
    public partial class MainForm : Form
    {
        private Sale currSale = null;
        private bool SellerLoaded = false;

        public MainForm()
        {
            InitializeComponent();
            CreateNewSale();
        }

        private void cmdSell_Click(object sender, EventArgs e)
        {
            if ((txtSellerNo.Text != string.Empty) && (txtPrice.Text != string.Empty))
            {
                SellItem();
            }
        }

        private void SellItem()
        {
            try
            {
                var sellerNo = int.Parse(txtSellerNo.Text);
                var price = double.Parse(txtPrice.Text);
                var timestamp = DateTime.Now.ToLongTimeString();
                
                if (!CheckSeller(sellerNo))
                {
                    throw new Exception("Felaktigt säljarnummer");
                }

                if(chkRabatt.Checked)
                {
                    price = price/2;
                }

                currSale.SalesRows.Add(new SalesRow(sellerNo, price, timestamp));
                currSale.Sum += price;
                currSale.SalesRows.Sort(new SalesRowComparer());
                UpdateSellList();

                txtPrice.Text = string.Empty;
                txtSellerNo.Text = string.Empty;
                txtSellerNo.Focus();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Du måste skapa en ny försäljning!", "Fel!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fel!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CheckSeller(int sellerNo)
        {
            var seller = Seller.GetSeller(sellerNo);
            return (seller != null) ? true : false;
        }

        private void UpdateSellList()
        {
            var bindingSource = new BindingSource();
            bindingSource.DataSource = currSale.SalesRows;

            dataGridView1.DefaultCellStyle.Font = new Font("Verdana", 12);
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = bindingSource;
            
            dataGridView1.Columns.Add("SellerId", "Nr");
            if (dataGridView1 != null)
            {
                dataGridView1.Columns["SellerId"].DataPropertyName = "SellerId";
                dataGridView1.Columns["SellerId"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns.Add("Amount", "Pris");
                dataGridView1.Columns["Amount"].DataPropertyName = "Amount";
                dataGridView1.Columns["Amount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            
            lblSumma.Text = currSale.Sum.ToString();
            lblAntal.Text = currSale.SalesRows.Count.ToString();
        }

        private void txtSellerNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                txtPrice.Focus();
            }
        }

        private void txtPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                cmdSell.Focus();
            }
        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+")) && (e.KeyChar.ToString() != "\b"))
            {
                e.Handled = true;
            }
        }

        private void txtSellerNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+")) && (e.KeyChar.ToString() != "\b"))
            {
                e.Handled = true;
            }
        }

        private void cmdEndSale_Click(object sender, EventArgs e)
        {
            try
            {
                currSale.Save();
                ShowReceipt();
                CreateNewSale();

            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void ShowReceipt()
        {
            var receipt = new frmReceipt(currSale.SaleNo);
            receipt.Show();
        }

        private void CreateNewSale()
        {
            currSale = new Sale();


            UpdateSellList();
            lblSumma.Text = string.Empty;
            lblAntal.Text = string.Empty;
            toolStripStatusLabel1.Text = string.Empty;
        }

        private void GetSellerStatistics()
        {
            SellerLoaded = false;
            try
            {
                var sellers = Seller.GetSellers();
                Double totalSum = 0;

                foreach (var seller in sellers)
                {
                    seller.Items = 0;
                    seller.Sum = 0;
                    foreach (var thisRow in seller.SaleRows)
                    {
                        seller.Items += 1;
                        seller.Sum += thisRow.Amount;
                    }
                    totalSum += seller.Sum;
                }

                FörsäljningStatistik.Columns.Clear();
                FörsäljningStatistik.AutoGenerateColumns = false;
                FörsäljningStatistik.DataSource = sellers;

                FörsäljningStatistik.Columns.Add("SellerNo", "Nr");
                FörsäljningStatistik.Columns["SellerNo"].DataPropertyName = "SellerNo";
                FörsäljningStatistik.Columns["SellerNo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                FörsäljningStatistik.Columns.Add("Name", "Namn");
                FörsäljningStatistik.Columns["Name"].DataPropertyName = "Name";
                FörsäljningStatistik.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                FörsäljningStatistik.Columns.Add("Items", "Antal");
                FörsäljningStatistik.Columns["Items"].DataPropertyName = "Items";
                FörsäljningStatistik.Columns["Items"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                FörsäljningStatistik.Columns.Add("Sum", "Summa");
                FörsäljningStatistik.Columns["Sum"].DataPropertyName = "Sum";
                FörsäljningStatistik.Columns["Sum"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                lblTotalSum.Text = string.Format("{0} kr", totalSum.ToString());
                SellerLoaded = true;
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }

        }

        private void cmdRemove_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow thisRow in dataGridView1.SelectedRows)
            {
                var thisSalesRow = (SalesRow)thisRow.DataBoundItem;
                currSale.RemoveRow(thisSalesRow);
                currSale.Sum -= thisSalesRow.Amount;
            }
            UpdateSellList();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetSellerStatistics();

        }

        private void ShowSellerDetails(Seller seller)
        {
            txtNummer.Text = seller.SellerNo.ToString();
            txtNamn.Text = seller.Name;
            txtEmail.Text = seller.Email;
            txtTelefon.Text = seller.Telephone;
            txtKommentar.Text = seller.Comment;
        }

        private void FörsäljningStatistik_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (SellerLoaded)
            {
                var seller = Seller.GetSeller((int)FörsäljningStatistik.Rows[e.RowIndex].Cells[0].Value);
                ShowSellerDetails(seller);
            }
        }

        private void FörsäljningStatistik_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var seller = Seller.GetSeller((int) FörsäljningStatistik.Rows[e.RowIndex].Cells[0].Value);
                ShowSellerDetails(seller);
            }
        }
    

        private void cmdSaveSeller_Click(object sender, EventArgs e)
        {
            SaveSeller();
        }

        private void SaveSeller()
        {
            try
            {
                var seller = new Seller();
                seller.Name = txtNamn.Text;
                seller.Comment = txtKommentar.Text;
                seller.Email = txtEmail.Text;
                seller.SellerNo = int.Parse(txtNummer.Text);
                seller.Telephone = txtTelefon.Text;
                seller.Save();
                GetSellerStatistics();
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void CmdNewSeller_Click(object sender, EventArgs e)
        {
            SetupNewSeller();
        }

        private void SetupNewSeller()
        {
            var seller = new Seller();
            int nextSellerNo = Seller.GetNextSellerNo();
            seller.SellerNo = nextSellerNo;
            txtNummer.Text = seller.SellerNo.ToString();
            txtNamn.Text = seller.Name;
            txtNamn.Select();
            txtEmail.Text = seller.Email;
            txtKommentar.Text = seller.Comment;
            txtTelefon.Text = seller.Telephone;

        }
    }
}
