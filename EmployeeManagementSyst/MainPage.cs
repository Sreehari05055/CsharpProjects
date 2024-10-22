using System;
using System.Configuration;
using System.Data;
using System.Drawing.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace EmployeeManagementSyst
{
    public partial class MainPage : Form
    {
        private static string serverConnection;
        public MainPage()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        public static string InitiateServer() {
            if (string.IsNullOrEmpty(serverConnection))
            {
                try
                {
                    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("connectionString.json", optional: true, reloadOnChange: true);
                    IConfiguration configuration = builder.Build();

                    // Get connection string
                    serverConnection = configuration.GetConnectionString("EmployeeDatabase");

                    if (string.IsNullOrEmpty(serverConnection))
                    {
                        throw new Exception("Connection string 'EmployeeDatabase' not found in configuration file.");
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); } 
            }
            return serverConnection;
        }
        // Method to check and create the lastExecuted table if it does not exist
        public void LastExecTable() 
        {
            try
            {

                using (SqlConnection serverCon = new SqlConnection(serverConnection))
                {
                    serverCon.Open();
                    string queryinfo = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'EmployeeManagement' AND table_schema = 'dbo' AND table_name = 'lastExecuted'; ";
                    SqlCommand toExecute = new SqlCommand(queryinfo, serverCon);
                    Object data = toExecute.ExecuteScalar();
                    if (data == null)
                    {
                        string queryThree = "CREATE TABLE dbo.lastExecuted(row_id VARCHAR(7), dayof_week VARCHAR(100), last_exec_date VARCHAR(100));";
                        SqlCommand toexecute = new SqlCommand(queryThree, serverCon);
                        toexecute.ExecuteNonQuery();
                        MessageBox.Show("Exe Database Created Successfuly");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Creating Table (Last Executed): " + ex.Message); }
        }
        // Method to check and create the employeedetails table if it does not exist
        public void EmployeeDetails()
        {
            try
            {

                using (SqlConnection serverCon = new SqlConnection(serverConnection))
                {
                    serverCon.Open();
                    string queryinfo = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'EmployeeManagement' AND table_schema = 'dbo' AND table_name = 'employeedetails'; ";
                    SqlCommand toExecute = new SqlCommand(queryinfo, serverCon);
                    Object data = toExecute.ExecuteScalar();
                    if (data == null)
                    {
                        string queryThree = "CREATE TABLE dbo.employeedetails(id VARCHAR(7) PRIMARY KEY,fullname VARCHAR(100) ,age VARCHAR(50), phonenumber VARCHAR(50) UNIQUE, email VARCHAR(100) UNIQUE, hourlyrate VARCHAR(20), surname VARCHAR(25) );";
                        SqlCommand toexecute = new SqlCommand(queryThree, serverCon);
                        toexecute.ExecuteNonQuery();
                        MessageBox.Show("Database Created Successfuly");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Creating Table (Employee Data): " + ex.Message); }

        }
        // Method to check and create the admintable if it does not exist
        public void AdminTable()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string adminQuery = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'EmployeeManagement' AND table_schema = 'dbo' AND table_name = 'admintable';";
                    SqlCommand adminExec = new SqlCommand(adminQuery, connection);
                    Object adminData = adminExec.ExecuteScalar();
                    if (adminData == null)
                    {
                        string createRota = "CREATE TABLE dbo.admintable(id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedetails(id), Admin_name VARCHAR(100) NOT NULL, Admin_contact VARCHAR(100) NOT NULL);";
                        SqlCommand adminExc = new SqlCommand(createRota, connection);
                        adminExc.ExecuteNonQuery();
                        MessageBox.Show("Admin Table Created");
                    }
                }
            }
            catch (Exception e) { MessageBox.Show("Error Creating Table (Admin Table):" + e.Message); }
        }
        // Method to check and create the rotatable table if it does not exist
        public void RotaTable()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string rotaQuery = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'EmployeeManagement' AND table_schema = 'dbo' AND table_name = 'rotatable';";
                    SqlCommand rotaExec = new SqlCommand(rotaQuery, connection);
                    Object data = rotaExec.ExecuteScalar();
                    if (data == null)
                    {
                        string createRota = "CREATE TABLE dbo.rotatable(day_ofweek VARCHAR(9),start_work DATETIME, finish_work DATETIME, id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedetails (id));";
                        SqlCommand toExc = new SqlCommand(createRota, connection);
                        toExc.ExecuteNonQuery();
                        MessageBox.Show("Rota table Created");
                    }
                }

            }
            catch (Exception ex) { MessageBox.Show("Error Creating Table (Rota Table): " + ex.Message); }
        }
        // Method to check and create the employeepay table if it does not exist
        public void EmployeePayment()
        {
            try
            {

                using (SqlConnection serverConection = new SqlConnection(serverConnection))
                {
                    serverConection.Open();
                    string timeQuery = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'EmployeeManagement' AND table_schema = 'dbo' AND table_name = 'employeepay';";
                    SqlCommand payExec = new SqlCommand(timeQuery, serverConection);
                    Object data = payExec.ExecuteScalar();
                    if (data == null)
                    {
                        string createPayroll = "CREATE TABLE dbo.employeepay(date_of_work DATE, total_pay DECIMAL(10,2), hours_done VARCHAR(100), id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedetails(id));";
                        SqlCommand toexc = new SqlCommand(createPayroll, serverConection);
                        toexc.ExecuteNonQuery();
                        MessageBox.Show("Payroll hours table Created");
                    }

                }

            }
            catch (Exception ex) { MessageBox.Show("Error Creating Table (Employee Payment): " + ex.Message); }

        }
        // Method to check and create the carddata table if it does not exist
        public void EmployeeCardDetails()
        {

            try
            {

                using (SqlConnection serverCon = new SqlConnection(serverConnection))
                {
                    serverCon.Open();
                    string queryinfo = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'EmployeeManagement' AND table_schema = 'dbo' AND table_name = 'carddata'; ";
                    SqlCommand toExecute = new SqlCommand(queryinfo, serverCon);
                    Object data = toExecute.ExecuteScalar();
                    if (data == null)
                    {
                        string queryThree = "CREATE TABLE dbo.carddata(id VARCHAR(7) PRIMARY KEY,cardNum VARCHAR(18) ,expiryDate VARCHAR(6), cvv VARCHAR(4), holderName VARCHAR(255));";
                        SqlCommand toexecute = new SqlCommand(queryThree, serverCon);
                        toexecute.ExecuteNonQuery();
                        MessageBox.Show("Database Created Successfuly");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Creating Table (Card Data): " + ex.Message); }

        }
        // Method to check and create the hourstable if it does not exist
        public void HoursTable()
        {
            try
            {
                using (SqlConnection server = new SqlConnection(serverConnection))
                {
                    server.Open();
                    string rotaQuery = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'EmployeeManagement' AND table_schema = 'dbo' AND table_name = 'hourstable';";
                    SqlCommand rotaExec = new SqlCommand(rotaQuery, server);
                    Object data = rotaExec.ExecuteScalar();
                    if (data == null)
                    {
                        string createRota = "CREATE TABLE dbo.hourstable(id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedetails(id), empname VARCHAR(100) ,hours VARCHAR(100));";
                        SqlCommand toExc = new SqlCommand(createRota, server);
                        toExc.ExecuteNonQuery();
                        MessageBox.Show("Hours table Created");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Creating Table (Hours Table): " + ex.Message); }
        }
        // Event handler for the StartEnd button click event
        private void StartEnd_Click(object sender, EventArgs e)
        {
            try
            {
                ClockingPage form2 = new ClockingPage();

                form2.Show();
                //this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Error: "+ex.Message); }
        }
        // Method to check if any administrators exist in the database
        private void AdminCheck()
        {
            try 
            {
                using (SqlConnection sqlConnection = new SqlConnection(serverConnection)) 
                {
                    sqlConnection.Open();
                    string admQuery = "SELECT COUNT(*) FROM admintable";
                    SqlCommand admExec = new SqlCommand(admQuery, sqlConnection);
                    int recordCount = (int)admExec.ExecuteScalar();
                    if (recordCount > 0)
                    {
                        VerifyAdmin form3 = new VerifyAdmin();

                        form3.Show();
                    }
                    else 
                    {
                    AdminPage adminPage = new AdminPage();  
                        adminPage.Show();
                    }
                }
            
            } catch (Exception e) { MessageBox.Show("Error Searching Admin: "+e.Message); } 
        }
        // Event handler for the ManagementInfo button click event
        private void ManagementInfoClick(object sender, EventArgs e)
        {
            try
            {
                InitiateServer();
                AdminCheck();
               
                
                
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
    
    }
}
