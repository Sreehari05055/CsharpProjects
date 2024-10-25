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
        // Handles the click event for the "OK" button, updating the last executed day based on user selection.
        private void Ok_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string selectedDay = comboBox1.SelectedItem.ToString();

                UpdtExec(selectedDay);
            }

            this.Close();
        }
        // Updates the last executed day in the database for the specified day.
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
        // Initializes the combo box with the days of the week for user selection.
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
        // Checks if a payslip should be sent based on the last executed day and updates the last execution date.
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
