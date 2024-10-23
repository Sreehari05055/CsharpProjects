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
    public partial class SetAdmin : Form
    {
        private string AdmnCode;
        private string serverConnection;
        public SetAdmin(String Code)
        {
            this.AdmnCode = Code;
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
        }
        // Handles the click event for the OK button
        private void Ok_Click(object sender, EventArgs e)
        {
            serverConnection = MainPage.InitiateServer();
            GetAdmininfo(AdmnCode);
            this.Close();

        }
     
        // Retrieves administrator information based on the provided ID and inserts it into the admin table.
        public void GetAdmininfo(string id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(serverConnection))
                {
                    conn.Open();
                    string admindetailQuery = "SELECT fullname,email FROM employeedetails WHERE id = @id";
                    SqlCommand detailQuery = new SqlCommand(admindetailQuery, conn);

                    detailQuery.Parameters.Clear();
                    detailQuery.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = detailQuery.ExecuteReader();
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            // Add a method that checks if the person exists
                            string adminName = reader.GetString(reader.GetOrdinal("fullname"));
                            string adminEmail = reader.GetString(reader.GetOrdinal("email"));
                            InsertAdminInfo(id, adminName, adminEmail);
                            MessageBox.Show("Employee was made admin");
                        }
                    }
                    else {
                        this.Close();
                        MessageBox.Show("No data found");
                                
                    }
                }

            }
            catch (Exception e) { MessageBox.Show("Error Getting Admin Information: " + e.Message); }
        }
        // Inserts the administrator information into the admin table.
        public void InsertAdminInfo(string id, string name, string email)
        { 
            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string insertAdmin = """INSERT INTO admintable(id,Admin_name,Admin_contact)  VALUES(@id,@adminName,@adminEmail)""";
                    SqlCommand adminExec = new SqlCommand(insertAdmin, connection);

                    adminExec.Parameters.AddWithValue("@id", id);
                    adminExec.Parameters.AddWithValue("@adminName", name);
                    adminExec.Parameters.AddWithValue("@adminEmail", email);

                    int affectedRow = adminExec.ExecuteNonQuery();
                }
            }
            catch (Exception e) { MessageBox.Show("Error Inserting Values (Admin Table): " + e.Message); }

        }
    }
}
