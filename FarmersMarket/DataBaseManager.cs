using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FarmersMarket
{
    internal class DataBaseManager
    {
        private static string GetConnectionString()
        {
            string host = "Host=localhost;";
            string port = "Port=5432;";
            string dbName = "Database=FarmersMarket;";
            string userName = "Username=postgres;";
            string password = "Password=12345;";
            string connectionString = string.Format("{0}{1}{2}{3}{4}", host, port, dbName, userName, password);
            return connectionString;
        }

        public static void ExecuteNonQuery(string query, NpgsqlParameter[] parameters)
        {
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(GetConnectionString()))
                {
                    con.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Query executed successfully");
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static DataTable ExecuteQuery(string query, NpgsqlParameter[] parameters)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(GetConnectionString()))
                {
                    con.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(cmd);
                        dataAdapter.Fill(dataTable);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return dataTable;
        }

        public static Products RetrieveProductsByID(int id)
        {
            string query = "SELECT * FROM \"Products\" WHERE \"Product ID\" = @id";

            NpgsqlParameter[] parameter = new NpgsqlParameter[]
            {
                new NpgsqlParameter("@id", id)
            };

            DataTable dataTable = ExecuteQuery(query, parameter);

            if(dataTable.Rows.Count > 0)
            {
                DataRow dr = dataTable.Rows[0];

                Products prdct = new Products()
                {
                    id = (int)dr["Product ID"],
                    name = (string)dr["Product Name"],
                    amount = (decimal)dr["Amount(kg)"],
                    price = (decimal)dr["Price per kg"]
                };
                return prdct;
            }

            return null;
        }

        public static Products RetrieveProductsByProductName(string productName)
        {
            string query = "SELECT * FROM \"Products\" WHERE LOWER(\"Product Name\") = LOWER(@productName)";

            NpgsqlParameter[] parameter = new NpgsqlParameter[]
            {
                new NpgsqlParameter("@productName", productName)
            };

            DataTable dataTable = ExecuteQuery(query, parameter);

            if (dataTable.Rows.Count > 0)
            {
                DataRow dr = dataTable.Rows[0];

                Products prdct = new Products()
                {
                    
                    name = (string)dr["Product Name"],
                    amount = (decimal)dr["Amount(kg)"],
                    price = (decimal)dr["Price per kg"]
                };
                return prdct;
            }

            return null;
        }

        public static void UpdateProduct(Products product)
        {
            string query = "UPDATE \"Products\" SET \"Amount(kg)\" = @amount WHERE \"Product Name\" = @name";
            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("@name", product.name),
                new NpgsqlParameter("@amount", product.amount)
            };

            ExecuteNonQuery(query, parameters);
        }
    }
}
