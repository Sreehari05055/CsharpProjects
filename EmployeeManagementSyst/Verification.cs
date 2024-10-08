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
        private string serverConnection;
        public Verification()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
        }
        public void Verify(String codeToCheck)
        {
            try
            {
                using (SqlConnection serverConnect = new SqlConnection(serverConnection))
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
        public void CheckStatus(string id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
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
                    }
                }
            }
            catch (Exception e) { MessageBox.Show("Error Checking Employee Status: " + e.Message); }
        }
        private void Ok_Click(object sender, EventArgs e)
        {
            InitiateServer();
            string userInput = textBox1.Text;

            Verify(userInput);
            CheckStatus(userInput);
            
            



        }
        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
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
        public void InsertHoursTable(string time, string id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(serverConnection))
                {
                    conn.Open();
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
                    
                }
            }
            catch (Exception e) { MessageBox.Show("Error Inserting Values (Hours Table): " + e.Message); }
        }
        

        private void Verification_Load(object sender, EventArgs e)
        {

        }
    }
}
