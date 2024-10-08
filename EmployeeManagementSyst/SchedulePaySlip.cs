﻿using Microsoft.Data.SqlClient;
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
        private string serverConnection;
       
        public SchedulePaySlip()
        {
            InitializeComponent();
            InitiateServer();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            InitializeCombo();
        }
        public void InitiateServer()
        {
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("connectionString.json", optional: true, reloadOnChange: true);
                IConfiguration configuration = builder.Build();

                // Get connection string
                string connectionString = configuration.GetConnectionString("EmployeeDatabase");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Connection string 'EmployeeDatabase' not found in configuration file.");
                }

                serverConnection = connectionString;
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            string selectedDay = comboBox1.SelectedItem.ToString();

            UpdtExec(selectedDay);

            this.Close();
        }
        private void UpdtExec(string day) 
        {
            try
            {             
                using (SqlConnection conn = new SqlConnection(serverConnection))
                {
                    conn.Open();
                    string query = "UPDATE lastExecuted SET dayof_week = @day WHERE row_id = '2';";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@day", day);
                    cmd.ExecuteNonQuery();                    
                }
                LastRunTime();
            }
            catch (Exception e) { MessageBox.Show($"Error Getting Last Executed Date: {e.Message}"); }
        }
        private void InitializeCombo()
        {
            comboBox1.Items.AddRange(new string[]
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
        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void LastRunTime()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(serverConnection))

                {
                    conn.Open();
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
