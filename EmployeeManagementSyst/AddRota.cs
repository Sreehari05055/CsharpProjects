﻿using Microsoft.Data.SqlClient;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EmployeeManagementSyst
{
    public partial class AddRota : Form
    {
        private string serverConnection;
        private string id;
        public AddRota(string id)
        {   
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.id = id;   
        }

        // Schedules a rota by inserting shift details into the database
        public void ScheduleRota(DateTime startShift, DateTime endShift, DateTime date)
        {
            try
            {
                string datetoUse = date.DayOfWeek.ToString();
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string insertquery = """INSERT INTO rotatable(day_ofweek ,start_work,finish_work,id)   VALUES (@dayofweek,@start,@finish,@id)""";

                    SqlCommand execute = new SqlCommand(insertquery, connection);

                    execute.Parameters.AddWithValue("@dayofweek", datetoUse);
                    execute.Parameters.AddWithValue("@start", startShift);
                    execute.Parameters.AddWithValue("@finish", endShift);
                    execute.Parameters.AddWithValue("@id", id);

                    int rowsAffected = execute.ExecuteNonQuery();
                    MessageBox.Show("Rota Added");
                }
            }
            catch (Exception ex) {MessageBox.Show("Error Scheduling Rota: " + ex.Message); }
        }

        // Event handler for clicking the "Ok" button to schedule the rota
        private void Ok_Click(object sender, EventArgs e)
        {
            serverConnection = MainPage.InitiateServer();
            string userInput = dateTimePicker1.Text;
            string dayString = userInput.Substring(0, 2).Trim();
            string finishInp = dateTimePicker2.Text;
            string startInp = dateTimePicker3.Text;

            int day = int.Parse(dayString);  // Parse the day from input
            int year = DateTime.Now.Year;  // Use the current year
            int month = DateTime.Now.Month;  // Use the current month


            // Combine the date and time for the shift start and end
            DateTime shiftStart = DateTime.Parse($"{year}-{month}-{day} {startInp}");
            DateTime shiftEnd = DateTime.Parse($"{year}-{month}-{day} {finishInp}");

            DateTime date = DateTime.Parse($"{year}-{month}-{day}");

            ScheduleRota(shiftStart, shiftEnd, date);

            this.Close();
           

        }
        // Event handler for clicking the "Cancel" button to close the form
        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
      
    }
}
