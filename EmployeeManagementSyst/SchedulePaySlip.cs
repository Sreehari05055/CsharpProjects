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
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
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
                using (SqlConnection conn = MainPage.ConnectionString())
                {
                    string query = "UPDATE lastExecuted SET dayof_week = @day WHERE row_id = '2';";
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
                using (SqlConnection conn = MainPage.ConnectionString())

                {
                   
                    string query = "SELECT dayof_week, last_exec_date FROM lastExecuted WHERE row_id = '2';";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                           
                            string storedDay = reader["dayof_week"].ToString();
                            string lastExecDateString = reader["last_exec_date"].ToString();
                            DateTime lastExecDate;

                           
                            Enum.TryParse(storedDay, true, out DayOfWeek targetDayOfWeek);

                           
                            DateTime.TryParse(lastExecDateString, out lastExecDate);

                         
                            bool shouldRunToday = DateTime.Today.DayOfWeek == targetDayOfWeek && lastExecDate.Date != DateTime.Today;

                            if (shouldRunToday)
                            {
                                reader.Close();
                            
                                PaySlip paySlip = new PaySlip();
                                paySlip.SendPaySlip();
                                

                                string updateQuery = "UPDATE lastExecuted SET last_exec_date = @date WHERE row_id = '2';";
                                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                                updateCmd.Parameters.AddWithValue("@date", DateTime.Today.ToString("yyyy-MM-dd"));
                                updateCmd.ExecuteNonQuery();
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
    }
}
