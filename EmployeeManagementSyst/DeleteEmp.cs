using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.NativeInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeManagementSyst
{
    public partial class DeleteEmp : Form
    {
        private string code;
        private string serverConnection;
        public DeleteEmp(string id)
        {
            this.code = id;
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
        }
        // Method to initiate the server connection
        public void InitiateServer()
        {
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("connectionString.json", optional: true, reloadOnChange: true);
                IConfiguration configuration = builder.Build();

                // Get connection string
                string connectionString = configuration.GetConnectionString("EmployeeDatabase");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Connection string 'EmployeeDatabase' not found in configuration file.");
                }

                serverConnection = connectionString;
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
        // Event handler for the delete button click
        private void button1_Click(object sender, EventArgs e)
        {
            InitiateServer();
            
            RemoveAdmin(code);
            RemovePay(code);
            RemoveCard(code);
            DeletEmp(code);          
            this.Close();

        }
        // Method to remove the admin data associated with the employee
        public void RemoveAdmin(string id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(serverConnection))
                {
                    conn.Open();
                    string deleteAdmin = "DELETE FROM admintable WHERE id = @id; "; ;
                    SqlCommand detailQuery = new SqlCommand(deleteAdmin, conn);

                    detailQuery.Parameters.Clear();
                    detailQuery.Parameters.AddWithValue("@id", id);

                    int rowsAffected = detailQuery.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Admin Deleted");
                    }


                }

            }
            catch (Exception e) { MessageBox.Show("Error Removing Admin: " + e.Message); }
        }
        // Method to remove the pay data associated with the employee
        public void RemovePay(string id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(serverConnection))
                {
                    conn.Open();
                    string deleteAdmin = "DELETE FROM employeepay WHERE id = @id; "; ;
                    SqlCommand detailQuery = new SqlCommand(deleteAdmin, conn);

                    detailQuery.Parameters.Clear();
                    detailQuery.Parameters.AddWithValue("@id", id);

                    int rowsAffected = detailQuery.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Pay Deleted");
                    }


                }

            }
            catch (Exception e) { MessageBox.Show("Error Removing Admin: " + e.Message); }
        }
        // Method to remove the card data associated with the employee
        private void RemoveCard(string code)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(serverConnection))
                {
                    conn.Open();
                    string deleteCard = "DELETE FROM carddata WHERE id = @id; "; ;
                    SqlCommand detailQuery = new SqlCommand(deleteCard, conn);

                    detailQuery.Parameters.Clear();
                    detailQuery.Parameters.AddWithValue("@id", code);

                    int rowsAffected = detailQuery.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Card Deleted");
                    }
                    else { MessageBox.Show("Failed to delete Card or Card not found "); }

                }

            }
            catch (Exception e) { MessageBox.Show("Error Deleting Card: " + e.Message); }
        }
        // Method to delete the employee from the employee details
        public void DeletEmp(string empCode)
        {
            try
            {
                using (SqlConnection serverConn = new SqlConnection(serverConnection))
                {
                    serverConn.Open();
                    string querydlt = "DELETE FROM employeedetails WHERE id = @id; ";
                    SqlCommand exec = new SqlCommand(querydlt, serverConn);
                    exec.Parameters.AddWithValue("@id", empCode);
                    int rowsAffected = exec.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Employee deleted successfully");
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete employee or employee not found");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Deleting Employee: " + ex.Message); }
        }
        // Event handler for the cancel button click
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        }
}
