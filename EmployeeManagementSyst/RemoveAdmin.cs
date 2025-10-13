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

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveAdmin"/> class.
        /// </summary>
        /// <param name="adminCode">The admin code (ID) of the admin to be removed.</param>
        public RemoveAdmin(string adminCode)
        {
            this.adminCode = adminCode;
            InitializeComponent();

        }
        /// <summary>
        /// Event handler for the 'OK' button click. Removes the admin and closes the form.
        /// </summary>
        private void Ok_Click(object sender, EventArgs e)        
        {
           
            RemveAdmin(adminCode);
            this.Close();
        }


        /// <summary>
        /// Removes an admin from the database based on the provided admin ID.
        /// </summary>
        /// <param name="id">The admin ID to be deleted from the database.</param>
        public void RemveAdmin(string id)
        {
            try
            {
                using (SqlConnection conn = MainPage.ConnectionString())
                {
                   
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
                    conn.Close();
                }

            }
            catch (Exception e) { Console.WriteLine("Error Removing Admin: " + e.Message); }
        }

    }
}
