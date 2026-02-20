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

    public partial class SchedulePaySlip : Form
    {

        public SchedulePaySlip()
        {
            InitializeComponent();

            InitializeCombo(comboBox1);
        }


        /// <summary>
        /// Handles the click event for the "OK" button, updating the last executed day
        /// based on the user's selection from the combo box.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event data.</param>
        private void Ok_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string selectedDay = comboBox1.SelectedItem.ToString();

                UpdtExec(selectedDay);
            }

            this.Close();
        }


        /// <summary>
        /// Updates the last executed day in the database for the specified day.
        /// </summary>
        /// <param name="day">The day of the week selected by the user.</param>
        private void UpdtExec(string day)
        {
            try
            {
                using (SqlConnection conn = ServerConnection.GetOpenConnection())
                {
                    // Update the LastExecution row for Payslip to store the selected day
                    string query = "UPDATE LastExecution SET DayOfWeek = @day WHERE KeyName = 'Payslip';";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@day", day);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                LastRunTime();
            }
            catch (Exception e) { MessageBox.Show($"Error Getting Last Executed Date: {e.Message}"); }
        }



        /// <summary>
        /// Initializes the combo box with the days of the week for user selection.
        /// </summary>
        /// <param name="comboBox">The combo box to populate with days of the week.</param>
        public static void InitializeCombo(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.Items.AddRange(new string[]
                {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"
                });
        }


        /// <summary>
        /// Checks if a payslip should be sent based on the last executed day and updates the last execution date.
        /// If the payslip needs to be sent today, it will trigger the sending process and update the execution date.
        /// </summary>
        public void LastRunTime()
        {
            try
            {
                using (SqlConnection conn = ServerConnection.GetOpenConnection())

                {

                    string query = "SELECT DayOfWeek, LastExecutedDate FROM LastExecution WHERE KeyName = 'Payslip';";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // DayOfWeek is stored as varchar, LastExecutedDate is stored as DATE (may be null)
                            var storedDayObj = reader["DayOfWeek"];
                            var lastExecObj = reader["LastExecutedDate"];

                            string storedDay = storedDayObj == DBNull.Value ? string.Empty : storedDayObj.ToString();
                            DateTime? lastExecDate = lastExecObj == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(lastExecObj);

                            if (Enum.TryParse(storedDay, true, out DayOfWeek targetDayOfWeek))
                            {
                                bool notRunToday = !lastExecDate.HasValue || lastExecDate.Value.Date != DateTime.Today;
                                bool shouldRunToday = DateTime.Today.DayOfWeek == targetDayOfWeek && notRunToday;

                                if (shouldRunToday)
                                {
                                    reader.Close();

                                    EmailConfiguration emailConfig = new EmailConfiguration();
                                    emailConfig.SendPaySlip();

                                    string updateQuery = "UPDATE LastExecution SET LastExecutedDate = @date WHERE KeyName = 'Payslip';";
                                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                                    updateCmd.Parameters.AddWithValue("@date", DateTime.Today);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
