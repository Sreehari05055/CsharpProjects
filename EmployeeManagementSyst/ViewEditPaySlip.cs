using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EmployeeManagementSyst
{
    public partial class ViewEditPaySlip : Form
    {
        private string serverConnection;
        public ViewEditPaySlip()
        {
            InitializeComponent();
            InitiateServer();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            dataGridView1.CellBeginEdit += dataGridView1_CellBeginEdit;
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
        }
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
             if (dataGridView1.Columns[e.ColumnIndex].HeaderText != "Total Pay")
            {
                e.Cancel = true;  
            }
        }
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
           
            if (dataGridView1.Columns[e.ColumnIndex].HeaderText == "Total Pay")
            {
                int rowIndex = e.RowIndex;
                string newTotalPayValue = dataGridView1.Rows[rowIndex].Cells["Total Pay"].Value.ToString();
                int id = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["Id"].Value);  

                
                decimal newTotalPay;
                if (decimal.TryParse(newTotalPayValue, out newTotalPay))
                {
                    UpdateTotalPayInDatabase(id, newTotalPay);  
                }
                else
                {
                    MessageBox.Show("Invalid Total Pay value.");
                }
            }
        }

        private void UpdateTotalPayInDatabase(int id, decimal newTotalPay)
        {
            string updateQuery = "UPDATE employeepay SET total_pay = @total_pay WHERE id = @id";

            using (SqlConnection cmd = new SqlConnection(serverConnection))
            {
                cmd.Open();
                SqlCommand cmd2 = new SqlCommand(updateQuery, cmd);
                cmd2.Parameters.AddWithValue("@total_pay", newTotalPay);
                cmd2.Parameters.AddWithValue("@id", id);                
                cmd2.ExecuteNonQuery();
            }
        }
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
        private void Changing_Text(object sender, EventArgs s)
        {
            string userInput = textBox1.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(userInput))
            {

                LoadAllData();
                return;
            }
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Id",typeof(string));
                dataTable.Columns.Add("Full Name", typeof(string));
                dataTable.Columns.Add("Date Of Work", typeof(string));
                dataTable.Columns.Add("Hours Done", typeof(string));
                dataTable.Columns.Add("Total Pay", typeof(string));

                using (SqlConnection serverConnect = new SqlConnection(serverConnection))
                {
                    serverConnect.Open();
                    string qry = "SELECT r.id, r.date_of_work, e.fullname, r.total_pay, r.hours_done FROM employeepay r INNER JOIN employeedetails e ON r.id = e.id WHERE e.surname = @surname OR r.id = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect);
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@surname", userInput);
                    mySqlCommand.Parameters.AddWithValue("@id", userInput);
                    SqlDataReader reader = mySqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DataRow row = dataTable.NewRow();
                            row["Id"] = reader["id"].ToString();
                            row["Full Name"] = reader["fullname"].ToString();
                            row["Date Of Work"] = reader["date_of_work"].ToString();
                            row["Hours Done"] = reader["hours_done"].ToString();
                            row["Total Pay"] = reader["total_pay"].ToString();

                            dataTable.Rows.Add(row);
                        }
                        dataGridView1.DataSource = dataTable;
                    }

                }

            }

            catch (Exception ex) { MessageBox.Show("Employee Details Error: " + ex.Message); }
        }
        private void LoadAllData()
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Id", typeof(string));
                dataTable.Columns.Add("Full Name",typeof(string));
                dataTable.Columns.Add("Date Of Work", typeof(string));
                dataTable.Columns.Add("Hours Done", typeof(string));
                dataTable.Columns.Add("Total Pay", typeof(string));


                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string query = "SELECT r.id, r.date_of_work, e.fullname, r.total_pay, r.hours_done FROM employeepay r INNER JOIN employeedetails e ON r.id = e.id;";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DataRow row = dataTable.NewRow();
                            row["Id"] = reader["id"].ToString(); 
                            row["Full Name"] = reader["fullname"].ToString();
                            row["Date Of Work"] = reader["date_of_work"].ToString();
                            row["Hours Done"] = reader["hours_done"].ToString();
                            row["Total Pay"] = reader["total_pay"].ToString();
                           
                            dataTable.Rows.Add(row);
                        }
                        dataGridView1.DataSource = dataTable;

                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void PaySlipGrid_Load(object sender, EventArgs e)
        {
            LoadAllData();
        }
    }
}
