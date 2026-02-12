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
    public partial class ShiftEndVerification : Form
    {
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
        public ShiftEndVerification()
        {
            InitializeComponent();

        }
        /// <summary>
        /// Verifies the employee code against the database to ensure it exists.
        /// </summary>
        /// <param name="codeToCheck">The employee code to verify.</param>
        public void Verify(String codeToCheck)
        {
            try
            {
                using (SqlConnection serverConnect = ServerConnection.GetOpenConnection())
                {

                    String querytoCheck = "SELECT Id FROM EmployeeDetails WHERE Id = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(querytoCheck, serverConnect);
                    mySqlCommand.Parameters.AddWithValue("@id", codeToCheck);
                    object dataTocheck = mySqlCommand.ExecuteScalar();
                    if (dataTocheck == null)
                    {
                        this.Close();
                        MessageBox.Show("Code incorrect");

                    }
                    serverConnect.Close();
                }

            }
            catch (Exception ex) { MessageBox.Show("Verification Error: " + ex.Message); }
        }
        /// <summary>
        /// Event handler for the OK button click event. Verifies the employee code and calculates hours worked.
        /// </summary>
        private void Ok_Click(object sender, EventArgs e)
        {
            string userInput = textBox1.Text;
            Code = userInput;
            Verify(userInput);
            CompletedHours();

        }
        /// <summary>
        /// Checks if the employee has completed their shift and calculates the hours worked.
        /// </summary>
        public void CompletedHours()
        {
            try
            {
                using (SqlConnection connection = ServerConnection.GetOpenConnection())
                {

                    string chckQry = "SELECT TOP 1 StartTime FROM TimeLogs WHERE EmployeeId = @Code AND EndTime IS NULL ORDER BY StartTime DESC";
                    SqlCommand exec = new SqlCommand(chckQry, connection);
                    exec.Parameters.AddWithValue("@Code", Code);
                    var result = exec.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("You Haven't Started Shift to End");
                        this.Close();
                        return;
                    }

                    var startTime = (DateTime)result;
                    StopWatch(startTime.ToString("O"));
                    connection.Close();
                }
            }
            catch (Exception e) { MessageBox.Show("Error Getting Completed Hours: " + e.Message); }
        }
        /// <summary>
        /// Processes the hours worked and records the pay for the employee.
        /// </summary>
        /// <param name="hours">The total hours worked by the employee.</param>
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
            }
            catch (Exception e) { MessageBox.Show("Error during hour check: " + e.Message); }
        }

        /// <summary>
        /// Processes the time worked using a stopwatch method to calculate the time difference from the start time.
        /// </summary>
        /// <param name="hourstring">The string representing the start time of the shift.</param>
        public void StopWatch(string hourstring)
        {
            if (DateTime.TryParse(hourstring, out DateTime dateTime))
            {
                TimeSpan timeDifference = DateTime.Now - dateTime;
                double workedHours = timeDifference.TotalHours;

                // Perform hours check immediately after calculation
                if (workedHours > 16)
                {
                    DeleteTime();
                    MessageBox.Show("Hours Done More Than Legal Working Hours.");
                    this.Close();
                    return;  // Stops further code execution if over the limit
                }
                this.HoursDone = workedHours;
                this.StringHours = workedHours.ToString("F2");

                HoursCheck(workedHours);

            }
            else
            {
                MessageBox.Show("Invalid hours input");
            }

        }

        /// <summary>
        /// Deletes the time record for the employee from the database after shift completion.
        /// </summary>
        public void DeleteTime()
        {
            try
            {
                using (SqlConnection connection = ServerConnection.GetOpenConnection())
                {
                    string chckQry = "UPDATE TimeLogs SET EndTime = @end WHERE EmployeeId = @Code AND EndTime IS NULL";
                    SqlCommand exec = new SqlCommand(chckQry, connection);
                    exec.Parameters.AddWithValue("@Code", Code);
                    exec.Parameters.AddWithValue("@end", DateTime.Now);
                    int rowsAffected = exec.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e) { MessageBox.Show("Error Clocking Out Employee: " + e.Message); }
        }

        /// <summary>
        /// Records the current date when the shift is completed.
        /// </summary>
        public void DateWorked()
        {
            DateTime today = DateTime.Today;
            String todayString = today.ToString("yyyy-MM-dd");
            WorkDate = todayString;
        }

        /// <summary>
        /// Inserts the employee's pay record into the database after calculating the total pay.
        /// </summary>
        public void InsertEmployeePay()
        {
            try
            {
                using (SqlConnection connection = ServerConnection.GetOpenConnection())
                {

                    string insertquery = """INSERT INTO EmployeePayInfo(DateOfWork,TotalPay,HoursDone,EmployeeId)   VALUES (@date_of_work,@totalpay,@hours_done,@id)""";

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
                        ShiftEndNotificationForm endShift = new ShiftEndNotificationForm();
                        endShift.Show();
                    }
                    else
                    {
                        MessageBox.Show("Insert operation failed.");
                    }
                    connection.Close();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Inserting Values (Employee Pay): " + ex.Message); }
        }

        /// <summary>
        /// Calculates the total pay for the employee based on the hourly rate and hours worked.
        /// </summary>
        public void CalculatePay()
        {
            try
            {
                using (SqlConnection server = ServerConnection.GetOpenConnection())
                {

                    string payQuery = "SELECT HourlyRate FROM EmployeeDetails WHERE Id = @id;";
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
                    server.Close();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Calculating Pay: " + ex.Message); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
