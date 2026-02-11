using System;
using System.Configuration;
using System.Data;
using System.Drawing.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace EmployeeManagementSyst
{
    public partial class LandingPage : Form
    {
        public LandingPage()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void StartEnd_Click(object sender, EventArgs e)
        {
            try
            {
                ClockingForm form2 = new ClockingForm();

                form2.Show();
                //this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
        private void ManagementInfoClick(object sender, EventArgs e)
        {
            try
            {
                AdminCheck();                  
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private static void AdminCheck()
        {
            try
            {
                using (SqlConnection sqlConnection = ServerConnection.GetOpenConnection())
                {
                    string admQuery = "SELECT COUNT(*) FROM admintable";

                    SqlCommand admExec = new SqlCommand(admQuery, sqlConnection);

                    int recordCount = (int)admExec.ExecuteScalar();
                    if (recordCount > 0)
                    {
                        AdminVerification form3 = new AdminVerification();

                        form3.Show();
                    }
                    else
                    {
                        AdminForm adminPage = new AdminForm();
                        adminPage.Show();
                    }
                }

            }
            catch (Exception e) { MessageBox.Show("Error Searching Admin: " + e.Message); }
        }
    }
}
