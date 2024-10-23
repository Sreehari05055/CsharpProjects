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
    public partial class RemoveAdmin : Form
    {
        private string adminCode;
        private string serverConnection;

        public RemoveAdmin(string adminCode)
        {
            this.adminCode = adminCode;
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
        }
        // Initialize server connection and remove admin when 'OK' is clicked
        private void Ok_Click(object sender, EventArgs e)        
        {
            serverConnection = MainPage.InitiateServer();
            RemveAdmin(adminCode);
            this.Close();
        }
        // Removes an admin from the database based on the provided admin ID.
        public void RemveAdmin(string id)
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
                    else { MessageBox.Show("Failed to delete admin  or admin not found "); }

                }

            }
            catch (Exception e) { Console.WriteLine("Error Removing Admin: " + e.Message); }
        }

    }
}
