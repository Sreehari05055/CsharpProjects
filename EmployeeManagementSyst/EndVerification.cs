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
        private string stringHours;
        
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
        public String StringHours
        {
            get { return stringHours; }
            set { stringHours = value; }
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
        // Method to verify the employee code
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
        // Event handler for the OK button click
        private void Ok_Click(object sender, EventArgs e)
        {
            serverConnection = MainPage.InitiateServer();
            string userInput = textBox1.Text;
            Code = userInput;
            Verify(userInput);
            CompletedHours();

        }
        // Method to check if the employee has completed hours
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
        // Method to check hours worked and process them
        public void HoursCheck(double hours)
        {        
            try
            {              
                {
                    DateWorked();
                    CalculatePay();

                    DeleteTime();
                    InsertEmployeePay();
                }
            }catch(Exception e) { MessageBox.Show("Error during hour check: "+e.Message); }
        }
        // Method to process hours using a stopwatch
        public void StopWatch(string hourstring)
        {
            if (DateTime.TryParse(hourstring, out DateTime dateTime))
            {
                // Get the current time
                DateTime currentTime = DateTime.Now;

                // Calculate the time difference
                TimeSpan timeDifference = currentTime - dateTime;

                // Convert the difference to hours and minutes
                double result = timeDifference.TotalHours; // This will give the difference in hours

                this.HoursDone = result;

                // Perform hours check immediately after calculation
                if (result > 16)
                    {
                        DeleteTime();
                        MessageBox.Show("Hours Done More Than Legal Working Hours.");
                        this.Close();
                        return;  // Stops further code execution if over the limit
                    }

                String hourString = result.ToString("F2");
                this.StringHours = hourString;
                HoursCheck(result);
               
            }
            else
            {
                MessageBox.Show("Invalid hours input");
            }

        }
        // Method to delete time record from the database
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
        // Method to record the date worked
        public void DateWorked()
        {
            DateTime today = DateTime.Today;
            String todayString = today.ToString("yyyy-MM-dd");
            WorkDate = todayString;
        }
        // Method to insert employee pay record into the database
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
                    execute.Parameters.AddWithValue("@hours_done", StringHours);
                    execute.Parameters.AddWithValue("@id", Code);

                    int rowsAffected = execute.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Successfully inserted
                        this.Close();
                        EndShift endShift = new EndShift();
                        endShift.Show();
                    }
                    else
                    {
                        MessageBox.Show("Insert operation failed.");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Inserting Values (Employee Pay): " + ex.Message); }
        }
        // Method to calculate the employee's pay based on hours worked
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
                        this.TotalPay = Math.Round(completePay, 2);

                    }
                    else { MessageBox.Show("Hourly rate not found for employee id"); }
                }
            }
            catch (Exception ex) {MessageBox.Show("Error Calculating Pay: " + ex.Message); }
        }
    }
}
