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
        private void Ok_Click(object sender, EventArgs e)
        {
            InitiateServer();
            string userInput = dateTimePicker1.Text;
            string dayString = userInput.Substring(0, 2).Trim();

            string finishInp = dateTimePicker2.Text;
            string startInp = dateTimePicker3.Text;

            int day = int.Parse(dayString);
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            DateTime shiftStart = DateTime.Parse($"{year}-{month}-{day} {startInp}");
            DateTime shiftEnd = DateTime.Parse($"{year}-{month}-{day} {finishInp}");

            DateTime date = DateTime.Parse($"{year}-{month}-{day}");

            ScheduleRota(shiftStart, shiftEnd, date);

            this.Close();
           

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
    }
}
