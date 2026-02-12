using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.NativeInterop;
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
    public partial class DeleteEmployeeForm : Form
    {
        private string code;

        /// <summary>
        /// Initializes a new instance of the DeleteEmp form with the specified employee ID.
        /// </summary>
        /// <param name="id">The ID of the employee to be deleted.</param>
        public DeleteEmployeeForm(string id)
        {
            this.code = id;
            InitializeComponent();

        }
        /// <summary>
        /// Event handler for the delete button click. 
        /// It checks if the employee exists and removes their data from multiple tables.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Yes_Click(object sender, EventArgs e)
        {
            // Check if the employee is currently working (exists in hourstable)
            if (!WorkStatus(code))
            {

                if (!RemoveAdmin(code))
                {
                    MessageBox.Show("Failed to delete admin data. Operation halted.");
                    return;
                }

                if (!RemovePay(code))
                {
                    MessageBox.Show("Failed to delete pay details. Operation halted.");
                    return;
                }

                if (!RemoveCard(code))
                {
                    MessageBox.Show("Failed to delete card details. Operation halted.");
                    return;
                }
                if (!DeletEmp(code))
                {
                    MessageBox.Show("Failed to delete employee details. Operation halted.");
                    return;
                }
            }
            else 
            {
                Console.WriteLine("Process Halted: Employee Currently Working");
            }
            this.Close();

        }
        /// <summary>
        /// Removes the admin data associated with the specified employee ID.
        /// </summary>
        /// <param name="id">The employee's ID.</param>
        /// <returns>True if the admin data was successfully deleted, otherwise false.</returns>
        public static bool RemoveAdmin(string id)
        {
            try
            {
                using (SqlConnection conn = ServerConnection.GetOpenConnection())
                {
                 
                    string deleteAdmin = "DELETE FROM AdminInformation WHERE EmployeeId = @id; "; ;
                    SqlCommand detailQuery = new SqlCommand(deleteAdmin, conn);

                    detailQuery.Parameters.Clear();
                    detailQuery.Parameters.AddWithValue("@id", id);

                    int rowsAffected = detailQuery.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {                     
                        MessageBox.Show("Admin Deleted");
                    }

                    conn.Close();
                    return rowsAffected > 0;
                }

            }
            catch (Exception e)
            { 
                MessageBox.Show("Error Removing Admin: " + e.Message);
                return false;
            }
        }
        /// <summary>
        /// Removes the pay details associated with the specified employee ID.
        /// </summary>
        /// <param name="id">The employee's ID.</param>
        /// <returns>True if the pay details were successfully deleted, otherwise false.</returns>
        public static bool RemovePay(string id)
        {
            try
            {
                using (SqlConnection conn = ServerConnection.GetOpenConnection())
                {
                   
                    string deleteAdmin = "DELETE FROM EmployeePayInfo WHERE EmployeeId = @id; "; 
                    SqlCommand detailQuery = new SqlCommand(deleteAdmin, conn);

                    detailQuery.Parameters.Clear();
                    detailQuery.Parameters.AddWithValue("@id", id);

                    int rowsAffected = detailQuery.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Pay Deleted");
                    }

                    conn.Close();
                    return rowsAffected > 0;
                }

            }
            catch (Exception e) { MessageBox.Show("Error Removing Admin: " + e.Message);
                return false;
            }
        }
        /// <summary>
        /// Checks whether the employee is currently working by querying the hourstable.
        /// </summary>
        /// <param name="id">The employee's ID.</param>
        /// <returns>True if the employee is found in the hourstable (i.e., working), otherwise false.</returns>
        public static bool WorkStatus(string id) 
        {
            try
            {
                using (SqlConnection conn = ServerConnection.GetOpenConnection())
                {
                    string empStatus = "SELECT COUNT(*) FROM TimeLogs WHERE EmployeeId = @id AND EndTime IS NULL; ";
                    SqlCommand detailQuery = new SqlCommand(empStatus, conn);

                    detailQuery.Parameters.Clear();
                    detailQuery.Parameters.AddWithValue("@id", id);

                    int count = (int)detailQuery.ExecuteScalar();

                    conn.Close();
                    return count > 0;
                }
            }
            catch (Exception e) { Console.WriteLine("Error checking Status (Hours table): "+e.Message); }
                    return false;
        }
        /// <summary>
        /// Removes the card data associated with the specified employee ID.
        /// </summary>
        /// <param name="code">The employee's ID.</param>
        /// <returns>True if the card data was successfully deleted, otherwise false.</returns>
        private static bool RemoveCard(string code)
        {
            try
            {
                using (SqlConnection conn = ServerConnection.GetOpenConnection())
                {
                    
                    string deleteCard = "DELETE FROM CardInformation WHERE EmployeeId = @id; "; ;
                    SqlCommand detailQuery = new SqlCommand(deleteCard, conn);

                    detailQuery.Parameters.Clear();
                    detailQuery.Parameters.AddWithValue("@id", code);

                    int rowsAffected = detailQuery.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Card Deleted");
                    }
                    else { MessageBox.Show("Failed to delete Card or Card not found "); }
                    conn.Close();

                    return rowsAffected > 0;
                }

            }
            catch (Exception e) { MessageBox.Show("Error Deleting Card: " + e.Message);
            return false;
            }
        }
        /// <summary>
        /// Deletes the employee details from the database.
        /// </summary>
        /// <param name="empCode">The employee's ID.</param>
        /// <returns>True if the employee was successfully deleted, otherwise false.</returns>
        public static bool DeletEmp(string empCode)
        {
            try
            {
                using (SqlConnection serverConn = ServerConnection.GetOpenConnection())
                {
                    serverConn.Open();
                    string querydlt = "DELETE FROM EmployeeDetails WHERE Id = @id; ";
                    SqlCommand exec = new SqlCommand(querydlt, serverConn);
                    exec.Parameters.AddWithValue("@id", empCode);
                    int rowsAffected = exec.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Employee deleted successfully");
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete employee or employee not found");
                    }
                    serverConn.Close();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Deleting Employee: " + ex.Message);
                return false;
            }
        }
        }
}
