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
    public partial class VerifyAdmin : Form
    {

        public VerifyAdmin()
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
            string userInput = textBox1.Text;
            
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
                using (SqlConnection serverConnect = MainPage.ConnectionString())
                {
                   
                    String querytoCheck = "SELECT id FROM admintable WHERE id = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(querytoCheck, serverConnect);
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@id", adminCode);
                    object dataTocheck = mySqlCommand.ExecuteScalar();
                    if (dataTocheck == null)
                    {
                        this.Close();
                        MessageBox.Show("Code incorrect");             
                    }
                    else 
                    {
                        AdminPage page = new AdminPage();
                        page.Show();
                        this.Close();
                    }
                    serverConnect.Close();
                }

            }
            catch (Exception ex) { Console.WriteLine("Admin Verification Error: " + ex.Message); }
        }
    }
}
