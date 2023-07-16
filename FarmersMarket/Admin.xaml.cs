using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace FarmersMarket
{
    /// <summary>
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        public Admin()
        {
            InitializeComponent();
            showTable();
        }
        string query;
        NpgsqlParameter[] parameters;
        private void showTable()
        {
            query = "SELECT * FROM \"Products\"";
            DataTable productsInfo = DataBaseManager.ExecuteQuery(query, null);
            DataGrid_db.ItemsSource = productsInfo.DefaultView;
        }

        private void clearTextField()
        {
            txtBox_id.Text = string.Empty;
            txtBox_prdName.Text = string.Empty;
            txtBox_amount.Text = string.Empty;
            txtBox_price.Text = string.Empty;
        }

        private void btn_SearchById_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtBox_searchID.Text, out int searchId))
            {
                Products prdct = DataBaseManager.RetrieveProductsByID(searchId);
                if (prdct != null)
                {
                    txtBox_id.Text = prdct.id.ToString();
                    txtBox_prdName.Text = prdct.name;
                    txtBox_amount.Text = prdct.amount.ToString();
                    txtBox_price.Text = prdct.price.ToString();
                }
                else
                {
                    MessageBox.Show("Product not found.");
                }
            }
            else
            {
                MessageBox.Show("Invalid ID. Please enter a valid integer value.");
            }
        }

        private void DataGrid_db_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid_db.SelectedItem != null)

             {
                DataRowView selectedRow = (DataRowView)DataGrid_db.SelectedItem;

                int id = (int)selectedRow["Product ID"];
                string name = (string)selectedRow["Product Name"];
                decimal amount = (decimal)selectedRow["Amount(kg)"];
                decimal price = (decimal)selectedRow["Price per kg"];

                txtBox_id.Text = id.ToString();
                txtBox_prdName.Text = name;
                txtBox_amount.Text = amount.ToString();
                txtBox_price.Text = price.ToString();
             }
         

        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txtBox_id.Text);
            string name = txtBox_prdName.Text;
            decimal amount = decimal.Parse(txtBox_amount.Text);
            decimal price = decimal.Parse(txtBox_price.Text);

            string query = "INSERT INTO \"Products\" (\"Product ID\", \"Product Name\", \"Amount(kg)\", \"Price per kg\") " +
                           "VALUES (@id, @name, @amount, @price)";

            
            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@name", name),
                new NpgsqlParameter("@amount", amount),
                new NpgsqlParameter("@price", price)
            };

            
            DataBaseManager.ExecuteNonQuery(query, parameters);

            
            MessageBox.Show("Data added successfully.");

            showTable();
            clearTextField();
            
            
        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txtBox_id.Text);
            string name = txtBox_prdName.Text;
            decimal amount = decimal.Parse(txtBox_amount.Text);
            decimal price = decimal.Parse(txtBox_price.Text);

            string query = "UPDATE \"Products\" SET \"Product Name\" = @name, \"Amount(kg)\" = @amount, \"Price per kg\" = @price WHERE \"Product ID\" = @id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@name", name),
                new NpgsqlParameter("@amount", amount),
                new NpgsqlParameter("@price", price)
            };

            DataBaseManager.ExecuteNonQuery(query, parameters);

            MessageBox.Show("Data updated successfully.");

            showTable();
            clearTextField();
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txtBox_id.Text);

            string query = "DELETE FROM \"Products\" WHERE \"Product ID\" = @id";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("@id", id)
            };

            DataBaseManager.ExecuteNonQuery(query, parameters);

            MessageBox.Show("Data deleted successfully.");

            showTable();
            clearTextField();
        }

        private void btn_Home_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();   
            mainWindow.Show();
            this.Close();
        }
    }
    
}
