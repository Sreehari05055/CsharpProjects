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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EmployeeManagementSyst
{
    public partial class Verification : Form
    {
     
        public Verification()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
        }
        // Method to verify if the provided employee code exists in the database
        public void Verify(String codeToCheck)
        {
            try
            {
                using (SqlConnection serverConnect = MainPage.ConnectionString())
                {
                    serverConnect.Open();
                    String querytoCheck = "SELECT id FROM employeedetails WHERE id = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(querytoCheck, serverConnect);
                    mySqlCommand.Parameters.AddWithValue("@id", codeToCheck);
                    object dataTocheck = mySqlCommand.ExecuteScalar();
                    if (dataTocheck == null)
                    {
                        this.Close();
                        MessageBox.Show("Code incorrect");

                    }
                    

                }

            }
            catch (Exception ex) { MessageBox.Show("Verification Error: " + ex.Message); }
        }
        // Method to check the employee's clock-in status
        public void CheckStatus(string id)
        {
            try
            {
                using (SqlConnection connection = MainPage.ConnectionString())
                {
                   
                    string chckQry = "SELECT * FROM hourstable WHERE id = @Code;";
                    SqlCommand exec = new SqlCommand(chckQry, connection);
                    exec.Parameters.AddWithValue("@Code", id);
                    using (SqlDataReader reader = exec.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            this.Close();
                            MessageBox.Show("You haven't clocked out from previous work");
                            
                        }
                        else
                        {
                            DateTime startTime = DateTime.Now;
                            string start = startTime.ToString("O");
                            InsertHoursTable(start, id);
                        }
                        reader.Close();
                    }
                    connection.Close();
                }
            }
            catch (Exception e) { MessageBox.Show("Error Checking Employee Status: " + e.Message); }
        }
        // Event handler for OK button click
        private void Ok_Click(object sender, EventArgs e)
        {
           
            string userInput = textBox1.Text;

            Verify(userInput);
            CheckStatus(userInput);
            
            



        }

        // Method to insert hours into the hours table
        public void InsertHoursTable(string time, string id)
        {
            try
            {
                using (SqlConnection conn = MainPage.ConnectionString())
                {
                   
                    string nameQry = "SELECT fullname FROM employeedetails WHERE id = @Code;";
                    SqlCommand fullnameExec = new SqlCommand(nameQry, conn);
                    fullnameExec.Parameters.AddWithValue("@Code", id);
                    using (SqlDataReader reader2 = fullnameExec.ExecuteReader())
                    {
                        if (reader2.Read())
                        {
                            string name = reader2.GetString(reader2.GetOrdinal("fullname"));
                            Name = name;
                        }
                        reader2.Close();
                    }
                    string insertAdmin = """INSERT INTO hourstable(id,empname,hours)  VALUES(@id,@fullname,@hours)""";
                    SqlCommand nameExec = new SqlCommand(insertAdmin, conn);

                    nameExec.Parameters.AddWithValue("@id", id);
                    nameExec.Parameters.AddWithValue("@fullname", Name);
                    nameExec.Parameters.AddWithValue("@hours", time);

                    int affectedRow = nameExec.ExecuteNonQuery();
                    this.Close();
                    StartShift startShift = new StartShift();
                    startShift.Show();
                    conn.Close();
                }
            }
            catch (Exception e) { MessageBox.Show("Error Inserting Values (Hours Table): " + e.Message); }
        }

        // Event handler for form load event
        private void Verification_Load(object sender, EventArgs e)
        {

        }
    }
}
