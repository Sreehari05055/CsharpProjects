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
        private string serverConnection;

        public VerifyAdmin()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
        }
        private void Label1_Click(object sender, EventArgs e) 
        {
            
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
        // Event handler for OK button click
        private void Ok_Click(object sender, EventArgs e)
        {
            serverConnection = MainPage.InitiateServer();
            string userInput = textBox1.Text;
            
            AdminVerify(userInput);

            

        }
        // Method to check if the ID exists in the admin table
        public void AdminVerify(string adminCode)
        {
            try
            {
                using (SqlConnection serverConnect = new SqlConnection(serverConnection))
                {
                    serverConnect.Open();
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

                }

            }
            catch (Exception ex) { Console.WriteLine("Admin Verification Error: " + ex.Message); }
        }
    }
}
