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
        private readonly Config _config = new Config();
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
            if (!EmployeeHelper.ExistsByClockPin(codeToCheck))
            {
                this.Close();
                MessageBox.Show("Code incorrect");
            }
        }
        /// <summary>
        /// Event handler for the OK button click event. Verifies the employee code and calculates hours worked.
        /// </summary>
        private void Ok_Click(object sender, EventArgs e)
        {
            string userInput = textBox1.Text;
            Code = userInput;
            Verify(userInput);
            string employeeId = EmployeeHelper.GetIdByClockPin(userInput);
            GetStatus(employeeId);

        }
        /// <summary>
        /// Checks if the employee has completed their shift and calculates the hours worked.
        /// </summary>
        public void GetStatus(string employeeId)
        {
            try
            {
                var startTime = EmployeeHelper.GetOpenShiftStartTime(employeeId);
                if (startTime == null)
                {
                    MessageBox.Show("You haven't started a shift to end.");
                    this.Close();
                    return;
                }

                StopWatch(startTime.Value, employeeId);
            }
            catch (Exception e) {
                MessageBox.Show("Error Getting Completed Hours: " + e.Message);
            }
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
                    UpdateShiftEnd();
                    CalculatePay();

                    
                    InsertEmployeePay();
                }
            }
            catch (Exception e) { MessageBox.Show("Error during hour check: " + e.Message); }
        }

        /// <summary>
        /// Processes the time worked using the provided start <see cref="DateTime"/> to calculate the time difference.
        /// </summary>
        /// <param name="startTime">The start time of the shift.</param>
        public void StopWatch(DateTime startTime, string employeeId)
        {
            TimeSpan timeDifference = DateTime.Now - startTime;
            double workedHours = timeDifference.TotalHours;

            // Perform hours check immediately after calculation
                if (workedHours > _config.LegalWorkHours)
                {
                MessageBox.Show("Hours Done More Than Legal Working Hours.");
                UpdateShiftEnd();
                string employeeName = EmployeeHelper.GetNameById(employeeId);

                // notify all admins
                var adminEmails = EmployeeHelper.GetAdminEmails();
                    if (adminEmails != null && adminEmails.Length > 0)
                    {
                        var subject = "Overtime Alert: Employee " + employeeName;
                        var body = $"Overtime Alert: Employee {employeeName} (ClockCode: {Code}) has worked {workedHours:F2} hours, exceeding the legal limit of {_config.LegalWorkHours} hours.";
                        var emailer = new EmailConfiguration();
                        foreach (var admin in adminEmails)
                        {
                            try
                            {
                                emailer.SendEmail(admin, subject, body);
                            }
                            catch
                            {
                                // swallow per-admin email errors so all admins are attempted
                            }
                        }
                    }
                    this.Close();
                    return;  // Stops further code execution if over the limit
                }

            this.HoursDone = workedHours;
            this.StringHours = workedHours.ToString("F2");

            HoursCheck(workedHours);
        }

        /// <summary>
        /// Updates the end time record for the employee in the database after shift completion.
        /// </summary>
        public void UpdateShiftEnd()
        {
            try
            {
                // Resolve employee id from clock pin before updating TimeLogs
                var employeeId = EmployeeHelper.GetIdByClockPin(Code);
                if (string.IsNullOrWhiteSpace(employeeId))
                {
                    MessageBox.Show("Unable to resolve employee id from provided code.");
                    return;
                }

                using (SqlConnection connection = ServerConnection.GetOpenConnection())
                {
                    string chckQry = "UPDATE TimeLogs SET EndTime = @end WHERE EmployeeId = @empId AND EndTime IS NULL";
                    SqlCommand exec = new SqlCommand(chckQry, connection);
                    exec.Parameters.AddWithValue("@empId", employeeId);
                    exec.Parameters.AddWithValue("@end", DateTime.Now);
                    int rowsAffected = exec.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e) { MessageBox.Show("Error Clocking Out Employee: " + e.Message); }
        }

        /// <summary>
        /// Inserts the employee's pay record into the database after calculating the total pay.
        /// </summary>
        public void InsertEmployeePay()
        {
            try
            {
                // Resolve employee id from provided clock pin
                var employeeId = EmployeeHelper.GetIdByClockPin(Code);
                if (string.IsNullOrWhiteSpace(employeeId))
                {
                    MessageBox.Show("Unable to resolve employee id from provided code.");
                    return;
                }

                using (SqlConnection connection = ServerConnection.GetOpenConnection())
                {
                    string insertquery = "INSERT INTO EmployeePayInfo(DateOfWork,TotalPay,HoursDone,EmployeeId)   VALUES (@date_of_work,@totalpay,@hours_done,@id)";

                    SqlCommand execute = new SqlCommand(insertquery, connection);

                    execute.Parameters.AddWithValue("@date_of_work", DateTime.Today);
                    execute.Parameters.AddWithValue("@totalpay", TotalPay);
                    execute.Parameters.AddWithValue("@hours_done", StringHours);
                    execute.Parameters.AddWithValue("@id", employeeId);

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

                    string payQuery = "SELECT HourlyRate FROM EmployeeDetails WHERE ClockPin = @clockPin;";
                    SqlCommand payExec = new SqlCommand(payQuery, server);
                    payExec.Parameters.AddWithValue("@clockPin", Code);
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
