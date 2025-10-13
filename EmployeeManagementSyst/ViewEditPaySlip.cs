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
      
        public ViewEditPaySlip()
        {
            InitializeComponent();
           
            LoadAllData();

            dataGridView1.CellBeginEdit += dataGridView1_CellBeginEdit;
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
        }
        /// <summary>
        /// Event handler for the beginning of cell editing in the DataGridView.
        /// Cancels editing for all columns except the "Total Pay" column.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event arguments containing information about the editing cell.</param>
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
             if (dataGridView1.Columns[e.ColumnIndex].HeaderText != "Total Pay")
            {
                e.Cancel = true;  
            }
        }
        /// <summary>
        /// Event handler for the end of cell editing in the DataGridView.
        /// Validates and updates the "Total Pay" value in the database if valid.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event arguments containing information about the edited cell.</param>
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

        /// <summary>
        /// Updates the "Total Pay" value in the database for the specified employee ID.
        /// </summary>
        /// <param name="id">The ID of the employee whose total pay needs to be updated.</param>
        /// <param name="newTotalPay">The new total pay value to update in the database.</param>
        private void UpdateTotalPayInDatabase(int id, decimal newTotalPay)
        {
            string updateQuery = "UPDATE employeepay SET total_pay = @total_pay WHERE id = @id";

            using (SqlConnection cmd = MainPage.ConnectionString())
            {
               
                SqlCommand cmd2 = new SqlCommand(updateQuery, cmd);
                cmd2.Parameters.AddWithValue("@total_pay", newTotalPay);
                cmd2.Parameters.AddWithValue("@id", id);                
                cmd2.ExecuteNonQuery();
                cmd.Close();
            }
        }
        /// <summary>
        /// Handles text input in the search TextBox and loads filtered employee details based on the input.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="s">The event arguments.</param>
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

                using (SqlConnection serverConnect = MainPage.ConnectionString())
                {
                   
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
                    serverConnect.Close();
                }

            }

            catch (Exception ex) { MessageBox.Show("Employee Details Error: " + ex.Message); }
        }
        /// <summary>
        /// Loads all employee pay slip data from the database and displays it in the DataGridView.
        /// </summary>
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


                using (SqlConnection connection = MainPage.ConnectionString())
                {
                   
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

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        /// <summary>
        /// Event handler for the form load event; loads all data into the DataGridView when the form is loaded.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void PaySlipGrid_Load(object sender, EventArgs e)
        {
            LoadAllData();
        }
    }
}
