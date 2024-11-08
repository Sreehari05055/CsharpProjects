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


        /// <summary>
        /// Initializes the form with the provided administrator code and sets visual properties like form border style and background color.
        /// </summary>
        /// <param name="Code">The administrator code used to identify the employee.</param>
        public SetAdmin(String Code)
        {
            this.AdmnCode = Code;
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
        }

        /// <summary>
        /// Handles the click event for the OK button. It retrieves the administrator information
        /// based on the provided ID and closes the form.
        /// </summary>
        private void Ok_Click(object sender, EventArgs e)
        {
         
            GetAdmininfo(AdmnCode);
            this.Close();

        }

        /// <summary>
        /// Retrieves the administrator's information based on the provided employee ID.
        /// If the information exists, it inserts it into the admin table and notifies the user.
        /// </summary>
        /// <param name="id">The ID of the employee to be made an administrator.</param>
        public void GetAdmininfo(string id)
        {
            try
            {
                using (SqlConnection conn = MainPage.ConnectionString())
                {
                   
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
                    conn.Close();
                }

            }
            catch (Exception e) { MessageBox.Show("Error Getting Admin Information: " + e.Message); }
        }

        /// <summary>
        /// Inserts the administrator's information into the admin table in the database.
        /// </summary>
        /// <param name="id">The employee's ID who is being made an administrator.</param>
        /// <param name="name">The full name of the employee.</param>
        /// <param name="email">The email address of the employee.</param>
        public void InsertAdminInfo(string id, string name, string email)
        { 
            try
            {
                using (SqlConnection connection = MainPage.ConnectionString())
                {
    
                    string insertAdmin = """INSERT INTO admintable(id,Admin_name,Admin_contact)  VALUES(@id,@adminName,@adminEmail)""";
                    SqlCommand adminExec = new SqlCommand(insertAdmin, connection);

                    adminExec.Parameters.AddWithValue("@id", id);
                    adminExec.Parameters.AddWithValue("@adminName", name);
                    adminExec.Parameters.AddWithValue("@adminEmail", email);

                    int affectedRow = adminExec.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e) { MessageBox.Show("Error Inserting Values (Admin Table): " + e.Message); }

        }
    }
}
