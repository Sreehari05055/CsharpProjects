using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeManagementSyst
{
    class TableInitialization
    {
        public TableInitialization()
        {
            using (SqlConnection conn = ServerConnection.GetOpenConnection())
            {
                LastExecTable(conn);
                EmployeeDetails(conn);
                AdminTable(conn);
                RotaTable(conn);
                EmployeePayment(conn);
                EmployeeCardDetails(conn);
                HoursTable(conn);
                conn.Close();
            }
            ServerConnection.CloseConnection();
        }
        public static void LastExecTable(SqlConnection conn)
        {
            try
            {


                    string queryinfo = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'EmployeeManagement' AND table_schema = 'dbo' AND table_name = 'lastExecuted'; ";
                    SqlCommand toExecute = new SqlCommand(queryinfo, conn);
                    Object data = toExecute.ExecuteScalar();
                    if (data == null)
                    {
                        string queryThree = "CREATE TABLE dbo.lastExecuted(row_id VARCHAR(7), dayof_week VARCHAR(100), last_exec_date VARCHAR(100));";
                        SqlCommand toexecute = new SqlCommand(queryThree, conn);
                        toexecute.ExecuteNonQuery();
                        MessageBox.Show("Exe Database Created Successfuly");
                    }
                
            }
            catch (Exception ex) { MessageBox.Show("Error Creating Table (Last Executed): " + ex.Message); }
        }

        /// <summary>
        /// Checks if the 'employeedetails' table exists and creates it if it doesn't.
        /// </summary>
        public static void EmployeeDetails(SqlConnection conn)
        {
            try
            {

                    string queryinfo = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'EmployeeManagement' AND table_schema = 'dbo' AND table_name = 'employeedetails'; ";
                    SqlCommand toExecute = new SqlCommand(queryinfo, conn);
                    Object data = toExecute.ExecuteScalar();
                    if (data == null)
                    {
                        string queryThree = "CREATE TABLE dbo.employeedetails(id VARCHAR(7) PRIMARY KEY,fullname VARCHAR(100) ,age VARCHAR(50), phonenumber VARCHAR(50) UNIQUE, email VARCHAR(100) UNIQUE, hourlyrate VARCHAR(20), surname VARCHAR(25) );";
                        SqlCommand toexecute = new SqlCommand(queryThree, conn);
                        toexecute.ExecuteNonQuery();
                        MessageBox.Show("Database Created Successfuly");
                    }
                
            }
            catch (Exception ex) { MessageBox.Show("Error Creating Table (Employee Data): " + ex.Message); }

        }
        /// <summary>
        /// Checks if the 'admintable' table exists and creates it if it doesn't.
        /// </summary>
        public static void AdminTable(SqlConnection conn)
        {
            try
            {

                    string adminQuery = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'EmployeeManagement' AND table_schema = 'dbo' AND table_name = 'admintable';";
                    SqlCommand adminExec = new SqlCommand(adminQuery, conn);
                    Object adminData = adminExec.ExecuteScalar();
                    if (adminData == null)
                    {
                        string createRota = "CREATE TABLE dbo.admintable(id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedetails(id), Admin_name VARCHAR(100) NOT NULL, Admin_contact VARCHAR(100) NOT NULL);";
                        SqlCommand adminExc = new SqlCommand(createRota, conn);
                        adminExc.ExecuteNonQuery();
                        MessageBox.Show("Admin Table Created");
                    }
                
            }
            catch (Exception e) { MessageBox.Show("Error Creating Table (Admin Table):" + e.Message); }
        }
        /// <summary>
        /// Checks if the 'rotatable' table exists and creates it if it doesn't.
        /// </summary>
        public static void RotaTable(SqlConnection conn)
        {
            try
            {

                    string rotaQuery = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'EmployeeManagement' AND table_schema = 'dbo' AND table_name = 'rotatable';";
                    SqlCommand rotaExec = new SqlCommand(rotaQuery, conn);
                    Object data = rotaExec.ExecuteScalar();
                    if (data == null)
                    {
                        string createRota = "CREATE TABLE dbo.rotatable(day_ofweek VARCHAR(9),start_work DATETIME, finish_work DATETIME, id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedetails (id));";
                        SqlCommand toExc = new SqlCommand(createRota, conn);
                        toExc.ExecuteNonQuery();
                        MessageBox.Show("Rota table Created");
                    }

            }
            catch (Exception ex) { MessageBox.Show("Error Creating Table (Rota Table): " + ex.Message); }
        }

        /// <summary>
        /// Checks if the 'employeepay' table exists and creates it if it doesn't.
        /// </summary>
        public static void EmployeePayment(SqlConnection conn)
        {
            try
            {

                    string timeQuery = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'EmployeeManagement' AND table_schema = 'dbo' AND table_name = 'employeepay';";
                    SqlCommand payExec = new SqlCommand(timeQuery, conn);
                    Object data = payExec.ExecuteScalar();
                    if (data == null)
                    {
                        string createPayroll = "CREATE TABLE dbo.employeepay(date_of_work DATE, total_pay DECIMAL(10,2), hours_done VARCHAR(100), id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedetails(id));";
                        SqlCommand toexc = new SqlCommand(createPayroll, conn);
                        toexc.ExecuteNonQuery();
                        MessageBox.Show("Payroll hours table Created");
                    }

            }
            catch (Exception ex) { MessageBox.Show("Error Creating Table (Employee Payment): " + ex.Message); }

        }

        /// <summary>
        /// Checks if the 'carddata' table exists and creates it if it doesn't.
        /// </summary>
        public static void EmployeeCardDetails(SqlConnection conn)
        {

            try
            {
                     string queryinfo = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'EmployeeManagement' AND table_schema = 'dbo' AND table_name = 'carddata'; ";
                    SqlCommand toExecute = new SqlCommand(queryinfo, conn);
                    Object data = toExecute.ExecuteScalar();
                    if (data == null)
                    {
                        string queryThree = "CREATE TABLE dbo.carddata(id VARCHAR(7) PRIMARY KEY,cardNum VARCHAR(18) ,expiryDate VARCHAR(6), cvv VARCHAR(4), holderName VARCHAR(255));";
                        SqlCommand toexecute = new SqlCommand(queryThree, conn);
                        toexecute.ExecuteNonQuery();
                        MessageBox.Show("Database Created Successfuly");
                    }
                
            }
            catch (Exception ex) { MessageBox.Show("Error Creating Table (Card Data): " + ex.Message); }

        }

        /// <summary>
        /// Checks if the 'hourstable' table exists and creates it if it doesn't.
        /// </summary>
        public static void HoursTable(SqlConnection conn)
        {
            try
            {

                    string rotaQuery = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'EmployeeManagement' AND table_schema = 'dbo' AND table_name = 'hourstable';";
                    SqlCommand rotaExec = new SqlCommand(rotaQuery, conn);
                    Object data = rotaExec.ExecuteScalar();
                    if (data == null)
                    {
                        string createRota = "CREATE TABLE dbo.hourstable(id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedetails(id), empname VARCHAR(100) ,hours VARCHAR(100));";
                        SqlCommand toExc = new SqlCommand(createRota, conn);
                        toExc.ExecuteNonQuery();
                        MessageBox.Show("Hours table Created");
                    }
                
            }
            catch (Exception ex) { MessageBox.Show("Error Creating Table (Hours Table): " + ex.Message); }
        }
    }
}
