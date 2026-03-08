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

namespace EmployeeManagementSyst
{
    public partial class EmployeeInformationForm : Form
    {
        
        public EmployeeInformationForm()
        {          
            InitializeComponent();

            
        }
        /// <summary>
        /// Event handler for text changes in the input textbox. Filters employee data based on the user's input.
        /// If the input is empty, it loads all employee data.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event data for the text change event.</param>
        private void Changing_Text(object sender, EventArgs e)
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
                dataTable.Columns.Add("Id", typeof(string));
                dataTable.Columns.Add("FullName", typeof(string));
                dataTable.Columns.Add("Age", typeof(string));
                dataTable.Columns.Add("PhoneNumber", typeof(string));
                dataTable.Columns.Add("Email", typeof(string));
                dataTable.Columns.Add("HourlyRate", typeof(string));

                using (SqlConnection conn = ServerConnection.GetOpenConnection())
                {
                    string qry = "SELECT Id, FullName, Age, PhoneNumber, Email, HourlyRate FROM EmployeeDetails WHERE Surname = @surname OR Id = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(qry, conn);


                    mySqlCommand.Parameters.AddWithValue("@surname", userInput);
                    mySqlCommand.Parameters.AddWithValue("@id", userInput);
                    SqlDataReader reader = mySqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var row = dataTable.NewRow();
                            row["Id"] = reader["Id"].ToString();
                            row["FullName"] = reader["FullName"].ToString();
                            row["Age"] = reader["Age"]?.ToString();
                            row["PhoneNumber"] = reader["PhoneNumber"]?.ToString();
                            row["Email"] = reader["Email"]?.ToString();
                            row["HourlyRate"] = reader["HourlyRate"]?.ToString();
                            dataTable.Rows.Add(row);
                        }
                        dataGridView1.DataSource = dataTable;
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching employees: " + ex.Message);
            }
        }
        /// <summary>
        /// Loads all employee data into the DataGridView when there are no filters applied.
        /// </summary>
        private void LoadAllData()
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Id", typeof(string));
                dataTable.Columns.Add("FullName", typeof(string));
                dataTable.Columns.Add("Age", typeof(string));
                dataTable.Columns.Add("PhoneNumber", typeof(string));
                dataTable.Columns.Add("Email", typeof(string));
                dataTable.Columns.Add("HourlyRate", typeof(string));

                using (SqlConnection connection = ServerConnection.GetOpenConnection())
                {
                   
                    string query = "SELECT Id,FullName,Age,PhoneNumber,Email,HourlyRate FROM EmployeeDetails";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                DataRow row = dataTable.NewRow();
                                row["Id"] = reader["Id"].ToString();
                                row["FullName"] = reader["FullName"].ToString();
                                row["Age"] = reader["Age"].ToString();
                                row["PhoneNumber"] = reader["PhoneNumber"].ToString();
                                row["Email"] = reader["Email"].ToString();
                                row["HourlyRate"] = reader["HourlyRate"].ToString();
                                dataTable.Rows.Add(row);
                            }
                            dataGridView1.DataSource = dataTable;
                        }
                    }
                }
                dataGridView1.DataSource = dataTable;
                if (dataGridView1.Columns["Id"] != null) dataGridView1.Columns["Id"].ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }
        /// <summary>
        /// Event handler for when the form loads. This is where all employee data is loaded initially.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event data for the form's load event.</param>
        private void EmployeeDetailGrid_Load(object sender, EventArgs e)
        {
            LoadAllData();
        }
    }
}
