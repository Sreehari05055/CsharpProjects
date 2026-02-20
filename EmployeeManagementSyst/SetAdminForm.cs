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
    public partial class SetAdminForm : Form
    {
        private string AdmnId;


        /// <summary>
        /// Initializes the form with the provided employee ID and sets visual properties like form border style and background color.
        /// </summary>
        /// <param name="empid">The employee ID used to identify the employee.</param>
        public SetAdminForm(String empid)
        {
            this.AdmnId = empid;
            InitializeComponent();

        }

        /// <summary>
        /// Handles the click event for the OK button. It retrieves the administrator information
        /// based on the provided ID and closes the form.
        /// </summary>
        private void Ok_Click(object sender, EventArgs e)
        {

            SetAdmin(AdmnId);
            this.Close();

        }

        public void SetAdmin(string empid) 
        {
            try
            {
                using (SqlConnection serverConnect = ServerConnection.GetOpenConnection())
                {
                    // Check current role first
                    string checkQuery = "SELECT UserRole FROM EmployeeDetails WHERE Id = @Id";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, serverConnect))
                    {
                        checkCmd.Parameters.AddWithValue("@Id", empid);
                        object roleObj = checkCmd.ExecuteScalar();
                        if (roleObj == null || roleObj == DBNull.Value)
                        {
                            MessageBox.Show("No employee found with the provided ID.");
                            return;
                        }

                        string currentRole = roleObj.ToString();
                        if (string.Equals(currentRole, "admin", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("Employee is already an admin.");
                            return;
                        }
                    }

                    // Update role to admin
                    string qry = "UPDATE EmployeeDetails SET UserRole = 'admin' WHERE EmployeeId = @Id;";
                    using (SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect))
                    {
                        mySqlCommand.Parameters.AddWithValue("@Id", AdmnId);
                        int affected = mySqlCommand.ExecuteNonQuery();
                        if (affected > 0)
                        {
                            MessageBox.Show("Employee was made admin.");
                        }
                        else
                        {
                            MessageBox.Show("No matching employee found to update.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Set Admin Error: " + ex.Message);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
