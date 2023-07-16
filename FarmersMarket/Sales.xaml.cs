using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

namespace FarmersMarket
{
    /// <summary>
    /// Interaction logic for Sales.xaml
    /// </summary>
    public partial class Sales : Window
    {

        private List<Products> cartProducts;
        private decimal totalAmount;
        public Sales()
        {
            InitializeComponent();

            showTable();

            List<string> products = GetAvailableProductsFromDatabase();

            
            cmbProducts.ItemsSource = products;

            cartProducts = new List<Products>();

        }

        private void showTable()
        {
            string query = "SELECT * FROM \"Products\"";
            DataTable productsInfo = DataBaseManager.ExecuteQuery(query, null);
            DataGrid_db.ItemsSource = productsInfo.DefaultView;
        }

        private List<string> GetAvailableProductsFromDatabase()
        {
            string query = "SELECT \"Product Name\" FROM \"Products\" WHERE \"Amount(kg)\" > 0";

            DataTable dataTable = DataBaseManager.ExecuteQuery(query,null);
            List<string> products = new List<string>();

          
            foreach (DataRow row in dataTable.Rows)
            {
                string productName = row["Product Name"].ToString();
                products.Add(productName);
            }

            return products;
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

               
                txtBox_prdName.Text = name;
                txtBox_amount.Text = amount.ToString();
                txtBox_price.Text = price.ToString();
            }
        }




        private void btn_Home_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void btn_SearchById_Click(object sender, RoutedEventArgs e)
        {
            string searchName = txtBox_searchName.Text.ToLower();

            if (!string.IsNullOrEmpty(searchName))
            {
                Products prdct = DataBaseManager.RetrieveProductsByProductName(searchName);

                if (prdct != null)
                {
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
                MessageBox.Show("Please enter a valid product name.");
            }
        }

        private void btn_shop_Click_1(object sender, RoutedEventArgs e)
        {
            string selectedProduct = cmbProducts.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedProduct))
            {
                var quantityStr = txtBox_prdctkg.Text;
                decimal quantity;

                if (decimal.TryParse(quantityStr, out quantity))
                {
                    var product = DataBaseManager.RetrieveProductsByProductName(selectedProduct);
                    if (product != null)
                    {
                        if (product.amount >= quantity)
                        {
                            product.amount -= quantity;
                            DataBaseManager.UpdateProduct(product);
                            product.amount = quantity;
                            cartProducts.Add(product);
                            dataGrid_Cart.ItemsSource = null;
                            dataGrid_Cart.ItemsSource = cartProducts;
                        }
                        else
                        {
                            MessageBox.Show("Not enough stock for this product.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid quantity.");
                }
            }
            else
            {
                MessageBox.Show("Please select a product.");
            }
            
            cmbProducts.SelectedIndex = -1;
            txtBox_prdctkg.Text = string.Empty;

            List<string> products = GetAvailableProductsFromDatabase();
            cmbProducts.ItemsSource = products;


        }

        private void btn_chekout_Click_1(object sender, RoutedEventArgs e)
        {
            totalAmount = 0;

            foreach (var product in cartProducts)
            {
                var dbProduct = DataBaseManager.RetrieveProductsByProductName(product.name);
                if (dbProduct != null)
                {
                    totalAmount += product.amount * product.price;
                }
            }

            txtBox_TotalAmount.Text = totalAmount.ToString(); 
            cartProducts.Clear(); 
            dataGrid_Cart.ItemsSource = null;
            showTable();
        }
    }
}
