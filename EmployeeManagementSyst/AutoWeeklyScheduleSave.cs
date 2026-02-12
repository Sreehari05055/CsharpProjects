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
    public partial class AutoWeeklyScheduleSave : Form
    {

        public AutoWeeklyScheduleSave()
        {
            InitializeComponent();


            SchedulePaySlip.InitializeCombo(comboBox1);
        }

        /// <summary>
        /// Updates the last executed day in the database.
        /// </summary>
        /// <param name="day">The day to update in the database.</param>
        private void UpdtExec(string day)
        {
            try
            {
                using (SqlConnection conn = ServerConnection.GetOpenConnection())
                {

                    string query = "UPDATE LastExecution SET DayOfWeek = @day WHERE KeyName = 'WeeklySave';";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@day", day);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                SetSaveDate();
            }
            catch (Exception e) { MessageBox.Show($"Error Getting Last Executed Date: {e.Message}"); }
        }
        /// <summary>
        /// Event handler for the OK button click. Updates the execution day in the database if a valid day is selected.
        /// </summary>
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
        /// Saves the weekly rota data to a text file.
        /// </summary>
        private void SaveWeeklyData()
        {
            try
            {
                DateTime dateTime = DateTime.Now;

                string format = dateTime.ToString("yyyy-MM-dd");
                string path = $@"C:\Users\sreek\OneDrive\المستندات\WeeklyRota_{format}.txt"; ;
                StringBuilder sb = new StringBuilder();
                using (SqlConnection con = ServerConnection.GetOpenConnection())
                {

                    string saveQuery = """SELECT r.EmployeeId AS id, e.FullName AS fullname, r.StartWork AS start_work, r.FinishWork AS finish_work, r.DayOfWeek AS day_ofweek FROM ScheduleInformation r INNER JOIN EmployeeDetails e ON r.EmployeeId = e.Id;""";
                    SqlCommand sqlCommand = new SqlCommand(saveQuery, con);

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        bool hasData = false;
                        while (reader.Read())
                        {
                            string fullName = reader.GetString(reader.GetOrdinal("fullname"));
                            string id = reader.GetString(reader.GetOrdinal("id"));
                            DateTime startWork = (DateTime)reader["start_work"];
                            DateTime finishWork = (DateTime)reader["finish_work"];
                            string dayOfWeek = reader["day_ofweek"].ToString();

                            sb.AppendLine($"{id}\t{fullName}\t{startWork:g}\t{finishWork:g}\t{dayOfWeek}");
                            hasData = true;
                        }
                        if (!hasData)
                        {
                            MessageBox.Show("No data found.");
                        }
                        reader.Close();
                    }

                    if (sb.Length > 0)
                    {
                        File.WriteAllText(path, sb.ToString()); // Write data to the specified file
                        ResetWeeklyData();   // Reset weekly data (method not shown in the code)
                    }
                    con.Close();
                }


            }
            catch (Exception e) { MessageBox.Show("Error Saving Rota Data: " + e.Message); }

        }
        /// <summary>
        /// Sets the date of the last save operation in the database.
        /// </summary>
        public void SetSaveDate()
        {
            try
            {
                using (SqlConnection conn = ServerConnection.GetOpenConnection())

                {

                    string query = "SELECT DayOfWeek, LastExecutedDate FROM LastExecution WHERE KeyName = 'WeeklySave';";
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

                                SaveWeeklyData();


                                string updateQuery = "UPDATE LastExecution SET LastExecutedDate = @date WHERE KeyName = 'WeeklySave';";
                                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                                updateCmd.Parameters.AddWithValue("@date", DateTime.Today);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                        reader.Close();
                    }
                    conn.Close();
                }
            }


            catch (Exception e) { MessageBox.Show("Error Setting Date to Save Data: " + e.Message); }
        }
        /// <summary>
        /// Resets the weekly rota data by deleting all records from the 'rotatable' table.
        /// </summary>
        private void ResetWeeklyData()
        {
            try
            {
                using (SqlConnection connection = ServerConnection.GetOpenConnection())
                {

                    DateTime now = DateTime.Now;
                    DateTime sevenDaysBefore = now.AddDays(-7);

                    string resetQuery = "DELETE FROM ScheduleInformation;";
                    SqlCommand resetCmd = new SqlCommand(resetQuery, connection);
                    int rowsAffected = resetCmd.ExecuteNonQuery();


                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Resetting Weekly Data: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
