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
    public partial class RemoveAdminList : Form
    {
        
        public RemoveAdminList()
        {
            InitializeComponent();
           
            AdminDetails();

        }

        /// <summary>
        /// Loads the details of the admins from the database and displays them in a DataGridView.
        /// </summary>
        public void AdminDetails()
        {
            try
            {
                DataTable dataTable = new DataTable();

                dataTable.Columns.Add("Admin Name", typeof(string));
                dataTable.Columns.Add("Id", typeof(string));


                using (SqlConnection serverConnect = ServerConnection.GetOpenConnection())
                {
                   
                    string qry = "SELECT EmployeeId,AdminName FROM AdminInformation;";
                    SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect);
                    using (SqlDataReader reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                DataRow row = dataTable.NewRow();
                                row["Admin Name"] = reader["AdminName"].ToString();
                                row["Id"] = reader["EmployeeId"].ToString();

                                dataTable.Rows.Add(row);
                            }
                        }
                    }
                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("Admin not found");
                    }
                    serverConnect.Close();
                }
                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex) { MessageBox.Show("Admin Details Error: " + ex.Message); }
        }

        /// <summary>
        /// Handles the DataGridView cell click event.
        /// Retrieves admin information and opens the RemoveAdmin form.
        /// </summary>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the current row
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string employeeName = row.Cells["Admin Name"].Value.ToString();
                string code = row.Cells["Id"].Value.ToString();

                using (SqlConnection serverConnect = ServerConnection.GetOpenConnection())
                {
                    
                    string qry = "SELECT EmployeeId FROM AdminInformation WHERE AdminName = @fname OR EmployeeId = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect);
                    mySqlCommand.Parameters.AddWithValue("@fname", employeeName);
                    mySqlCommand.Parameters.AddWithValue("@id", code);

                    object result = mySqlCommand.ExecuteScalar();

                    if (result != null)
                    {
                        string empId = result.ToString();
                       TerminateAdminForm removeAdmin = new TerminateAdminForm(empId);
                        removeAdmin.Show();
                        this.Close();
                    }
                    else { MessageBox.Show("Error Finding Admin ID"); }
                    serverConnect.Close();
                }
            }
        }
        /// <summary>
        /// Handles text changes in the input TextBox.
        /// Filters the admin list based on user input.
        /// </summary>
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
                dataTable.Columns.Add("Admin Name", typeof(string));
                dataTable.Columns.Add("Id", typeof(string));


                using (SqlConnection serverConnect = ServerConnection.GetOpenConnection())
                {
                   
                    string qry = "SELECT a.EmployeeId,a.AdminName FROM AdminInformation a JOIN EmployeeDetails e on a.EmployeeId = e.Id WHERE e.Surname = @surname OR a.EmployeeId = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect);
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@surname", userInput);
                    mySqlCommand.Parameters.AddWithValue("@id", userInput);
                    using (SqlDataReader reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                DataRow row = dataTable.NewRow();
                                row["id"] = reader["EmployeeId"].ToString();
                                row["Admin Name"] = reader["AdminName"].ToString();

                                dataTable.Rows.Add(row);
                            }
                            dataGridView1.DataSource = dataTable;
                        }
                    }
                    serverConnect.Close();
                }

            }

            catch (Exception ex) { MessageBox.Show("Admin Details Error: " + ex.Message); }
        }
        /// <summary>
        /// Loads all admin details from the database into the DataGridView.
        /// </summary>
        private void LoadAllData()
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("id", typeof(string));
                dataTable.Columns.Add("Admin Name", typeof(string));



                using (SqlConnection connection = ServerConnection.GetOpenConnection())
                {
                   
                    string query = "SELECT EmployeeId,AdminName FROM AdminInformation";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                DataRow row = dataTable.NewRow();
                                row["id"] = reader["EmployeeId"].ToString();
                                row["Admin Name"] = reader["AdminName"].ToString();

                                dataTable.Rows.Add(row);
                            }
                            dataGridView1.DataSource = dataTable;
                        }
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
        /// Event handler for the form's Load event.
        /// Loads all admin details when the form is loaded.
        /// </summary>
        private void EmployeeDetailGrid_Load(object sender, EventArgs e)
        {
            LoadAllData();
        }
    }
}
