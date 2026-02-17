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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EmployeeManagementSyst
{
    public partial class ShiftStartVerification : Form
    {

        public ShiftStartVerification()
        {
            InitializeComponent();

        }


        /// <summary>
        /// Verifies if the provided employee code exists in the database.
        /// </summary>
        /// <param name="codeToCheck">The employee code to be verified.</param>
        public void Verify(String codeToCheck)
        {
            if (!EmployeeHelper.ExistsByClockPin(codeToCheck))
            {
                this.Close();
                MessageBox.Show("Code incorrect");
            }
        }

        /// <summary>
        /// Checks the employee's clock-in status from the hourstable.
        /// </summary>
        /// <param name="id">The employee ID to check the clock-in status for.</param>
        public void CheckStatus(string id)
        {
            try
            {
                if (EmployeeHelper.HasOpenShift(id))
                {
                    this.Close();
                    MessageBox.Show("You haven't clocked out from previous work");
                    return;
                }

                DateTime startTime = DateTime.Now;
                InsertHoursTable(startTime, id);
            }
            catch (Exception e) { MessageBox.Show("Error Checking Employee Status: " + e.Message); }
        }

        /// <summary>
        /// Event handler for OK button click. Verifies the input and checks clock-in status.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Ok_Click(object sender, EventArgs e)
        {

            string userInput = textBox1.Text;

            Verify(userInput);
            // Retrieve employee id from helper and then check status
            var employeeId = EmployeeHelper.GetIdByClockPin(userInput);
            if (employeeId == null)
            {
                this.Close();
                MessageBox.Show("Employee ID not found for the provided code.");
                return;
            }

            CheckStatus(employeeId);

        }

        /// <summary>
        /// Inserts the hours of the employee into the hourstable.
        /// </summary>
        /// <param name="time">The time the employee started the shift.</param>
        /// <param name="id">The employee ID to insert the hours for.</param>
        public void InsertHoursTable(DateTime startTime, string id)
        {
            try
            {
                using (SqlConnection conn = ServerConnection.GetOpenConnection())
                {
                    string nameQry = "SELECT FullName FROM EmployeeDetails WHERE Id = @Code;";
                    SqlCommand fullnameExec = new SqlCommand(nameQry, conn);
                    fullnameExec.Parameters.AddWithValue("@Code", id);
                    
                    var employeeName = fullnameExec.ExecuteScalar()?.ToString();

                    string insertQuery = "INSERT INTO TimeLogs(EmployeeId,EmployeeName,StartTime) VALUES(@id,@employeeName,@start)";
                    SqlCommand nameExec = new SqlCommand(insertQuery, conn);

                    nameExec.Parameters.AddWithValue("@id", id);
                    nameExec.Parameters.AddWithValue("@employeeName", (object)employeeName ?? DBNull.Value);
                    nameExec.Parameters.AddWithValue("@start", startTime);

                    int affectedRow = nameExec.ExecuteNonQuery();
                    this.Close();
                    ShiftStartNotificationForm startShift = new ShiftStartNotificationForm();
                    startShift.Show();
                    conn.Close();
                }
            }
            catch (Exception e) { MessageBox.Show("Error Inserting Values (Hours Table): " + e.Message); }
        }

        /// <summary>
        /// Event handler for form load event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Verification_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
