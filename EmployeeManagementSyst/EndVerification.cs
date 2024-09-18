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
    public partial class EndVerification : Form
    {
        private string serverConnection;
        private double hoursDone;
        private string dateofWork;
        private decimal totalPay;
        private string code;
        
        public String Code
        {
            get { return code; }
            set { code = value; }
        }
        public double HoursDone
        {
            get { return hoursDone; }
            set { hoursDone = value; }
        }
        public String WorkDate
        {
            get { return dateofWork; }
            set { dateofWork = value; }
        }
        public decimal TotalPay
        {
            get { return totalPay; }
            set { totalPay = value; }
        }
        public EndVerification()
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
        private void Ok_Click(object sender, EventArgs e)
        {
            InitiateServer();
            string userInput = textBox1.Text;
            Code = userInput;
            Verify(userInput);
            CompletedHours();

        }
        public void CompletedHours()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string qry = "SELECT id from hourstable WHERE id = @cde";
                    SqlCommand execute = new SqlCommand(qry, connection);
                    execute.Parameters.Clear();
                    execute.Parameters.AddWithValue("@cde", Code);
                    using (SqlDataReader reader = execute.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            this.Close();
                            MessageBox.Show("You Haven't Started Shift to End");

                        }
                    }
                    string chckQry = "SELECT hours FROM hourstable WHERE id = @Code ";
                    SqlCommand exec = new SqlCommand(chckQry, connection);
                    exec.Parameters.AddWithValue("@Code", Code);
                    using (SqlDataReader reader2 = exec.ExecuteReader())
                    {

                        if (reader2.Read())
                        {
                            string hours = reader2.GetString(reader2.GetOrdinal("hours"));
                            StopWatch(hours);
                        }
                    }
                }
            }
            catch (Exception e) { MessageBox.Show("Error Getting Completed Hours: " + e.Message); }
        }
        public void HoursCheck(double hours)
        {
           
            try
            {
                if (hours > 16) 
                {
                    DeleteTime();
                    MessageBox.Show("Hours Done More Than Legal Working Hours."); 
                    this.Close();
                }
                else
                {
                    DateWorked();
                    CalculatePay();

                    DeleteTime();
                    InsertEmployeePay();
                }
            }catch(Exception e) { MessageBox.Show("Error during hour check: "+e.Message); }
        }
        public void StopWatch(string hourstring)
        {
            if (DateTime.TryParse(hourstring, out DateTime dateTime))
            {
                double minutes = dateTime.Minute;

                double hours = dateTime.Hour;
                double result = (hours) + (minutes / 60);



                    HoursCheck(hours);
                  HoursDone = hours;             
            }
            else
            {

                MessageBox.Show("Invalid hours input");
            }

            }
        public void DeleteTime()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                { 
                    connection.Open();
                    string chckQry = "DELETE FROM hourstable WHERE id = @Code ";
                    SqlCommand exec = new SqlCommand(chckQry, connection);
                    exec.Parameters.AddWithValue("@Code", Code);
                    int rowsAffected = exec.ExecuteNonQuery();
                }
            }
            catch (Exception e) { MessageBox.Show("Error Clocking Out Employee: " + e.Message); }
        }
        public void DateWorked()
        {
            DateTime today = DateTime.Today;
            String todayString = today.ToString("yyyy-MM-dd");
            WorkDate = todayString;
        }
        public void InsertEmployeePay()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string insertquery = """INSERT INTO employeepay(date_of_work,total_pay,hours_done,id)   VALUES (@date_of_work,@totalpay,@hours_done,@id)""";

                    SqlCommand execute = new SqlCommand(insertquery, connection);

                    execute.Parameters.AddWithValue("@date_of_work", WorkDate);
                    execute.Parameters.AddWithValue("@totalpay", TotalPay);
                    execute.Parameters.AddWithValue("@hours_done", HoursDone);
                    execute.Parameters.AddWithValue("@id", Code);

                    int rowsAffected = execute.ExecuteNonQuery();
                    this.Close();
                    EndShift endShift = new EndShift(); 
                    endShift.Show();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Inserting Values (Employee Pay): " + ex.Message); }
        }
        public void CalculatePay()
        {
            try
            {
                using (SqlConnection server = new SqlConnection(serverConnection))
                {
                    server.Open();
                    string payQuery = "SELECT hourlyrate FROM employeedetails WHERE id = @id;";
                    SqlCommand payExec = new SqlCommand(payQuery, server);
                    payExec.Parameters.AddWithValue("@id", Code);
                    object result = payExec.ExecuteScalar();
                    if (result != null)
                    {
                        double hourlyRate = Convert.ToDouble(result);

                        double cmpltePay = HoursDone * hourlyRate;
                        decimal completePay = (decimal)cmpltePay;
                        TotalPay = completePay;

                    }
                    else { MessageBox.Show("Hourly rate not found for employee id"); }
                }
            }
            catch (Exception ex) {MessageBox.Show("Error Calculating Pay: " + ex.Message); }
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
