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
using System.Web;
using System.Windows.Forms;

namespace EmployeeManagementSyst
{
    public partial class AdminVerification : Form
    {

        public AdminVerification()
        {
            InitializeComponent();

        }
        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Event handler for the OK button click. Verifies the admin code input by the user.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Ok_Click(object sender, EventArgs e)
        {
            string userInput = textBox1.Text.Trim();

            AdminVerify(userInput);

        }

        /// <summary>
        /// Verifies if the provided admin code exists in the database.
        /// </summary>
        /// <param name="adminCode">The admin code to be verified.</param>
        public void AdminVerify(string adminCode)
        {
            try
            {
                using (SqlConnection serverConnect = ServerConnection.GetOpenConnection())
                {

                    // Check that the provided clock pin belongs to a user with role 'admin'
                    string querytoCheck = "SELECT UserRole FROM EmployeeDetails WHERE ClockPin = @clockPin;";
                    using (SqlCommand mySqlCommand = new SqlCommand(querytoCheck, serverConnect))
                    {
                        mySqlCommand.Parameters.AddWithValue("@clockPin", adminCode);
                        object roleObj = mySqlCommand.ExecuteScalar();
                        if (roleObj == null || roleObj == DBNull.Value)
                        {
                            this.Close();
                            MessageBox.Show("Code incorrect");
                        }
                        else
                        {
                            string role = roleObj.ToString();
                            if (string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase))
                            {
                                AdminForm page = new AdminForm();
                                page.Show();
                                this.Close();
                            }
                            else
                            {
                                this.Close();
                                MessageBox.Show("Access denied: user is not an admin");
                            }
                        }
                    }
                    serverConnect.Close();
                }

            }
            catch (Exception ex) { Console.WriteLine("Admin Verification Error: " + ex.Message); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
