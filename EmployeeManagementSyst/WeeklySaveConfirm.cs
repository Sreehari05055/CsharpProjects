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
    public partial class WeeklySaveConfirm : Form
    {

        public WeeklySaveConfirm()
        {
            InitializeComponent();
           
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            SchedulePaySlip.InitializeCombo(comboBox1);
        }
        // Method to update the last executed day in the database
        private void UpdtExec(string day)
        {
            try
            {
                using (SqlConnection conn = MainPage.ConnectionString())
                {
                   
                    string query = "UPDATE lastExecuted SET dayof_week = @day WHERE row_id = '1';";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@day", day);
                    cmd.ExecuteNonQuery();
                conn.Close();
                }
                SetSaveDate();
            }
            catch (Exception e) { MessageBox.Show($"Error Getting Last Executed Date: {e.Message}"); }
        }
        // Event handler for the OK button click
        private void Ok_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string selectedDay = comboBox1.SelectedItem.ToString();
                UpdtExec(selectedDay);
            }
            this.Close();
        }
        // Method to save weekly data to a text file
        private void SaveWeeklyData()
        {
            try
            {
                DateTime dateTime = DateTime.Now;

                string format = dateTime.ToString("yyyy-MM-dd");
                string path = $@"C:\Users\sreek\OneDrive\المستندات\WeeklyRota_{format}.txt"; ;
                StringBuilder sb = new StringBuilder();
                using (SqlConnection con = MainPage.ConnectionString())
                {
                   
                    string saveQuery = """SELECT r.id, e.fullname, r.start_work, r.finish_work, r.day_ofweek FROM rotatable r INNER JOIN employeedetails e ON r.id = e.id;""";
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
        // Method to set the last save date in the database
        public void SetSaveDate()
        {
            try
            {
                using (SqlConnection conn = MainPage.ConnectionString())

                {
                    
                    string query = "SELECT dayof_week, last_exec_date FROM lastExecuted WHERE row_id = '1';";
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


                                string updateQuery = "UPDATE lastExecuted SET last_exec_date = @date WHERE row_id = '1';";
                                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                                updateCmd.Parameters.AddWithValue("@date", DateTime.Today.ToString("yyyy-MM-dd"));
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
        // Deletes data on weekly basis from the 'rotatable' table to reset the weekly rota data.
        private void ResetWeeklyData()
        {
            try
            {
                using (SqlConnection connection = MainPage.ConnectionString())
                {
                  
                    DateTime now = DateTime.Now;
                    DateTime sevenDaysBefore = now.AddDays(-7);

                    string resetQuery = "DELETE FROM rotatable;";
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


    }
}
