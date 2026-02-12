using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeManagementSyst
{
    class TableInitialization
    {
        private readonly string _masterConn;
        private readonly string _appConn;

        public TableInitialization(Config config) 
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _masterConn = config.MasterConn;
            _appConn = config.AppConn;
        }

        /// <summary>
        /// Performs full initialization: creates database if missing and ensures tables exist.
        /// </summary>
        public bool CreateDatabaseAndTables()
        {
            bool isFirstTimeSetup = false;

            // Step 1: Ensure database exists using master connection
            using (var masterConnection = new SqlConnection(_masterConn))
            {
                masterConnection.Open();
                string checkDbQuery = "SELECT COUNT(*) FROM sys.databases WHERE name = N'EmployeeManagement';";
                using (var checkCmd = new SqlCommand(checkDbQuery, masterConnection))
                {
                    int dbExists = (int)checkCmd.ExecuteScalar();

                    if (dbExists == 0)
                    {
                        string createDbQuery = "CREATE DATABASE EmployeeManagement;";
                        using (var createCmd = new SqlCommand(createDbQuery, masterConnection))
                        {
                            createCmd.ExecuteNonQuery();
                        }

                        isFirstTimeSetup = true;
                    }
                }

                masterConnection.Close();
            }

            // Step 2: Create or verify tables
            CreateTables();

            return isFirstTimeSetup;
        }

        public void CreateTables()
        {
            var steps = new (string Name, string Sql)[]
            {
                ("lastExecuted", "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'lastExecuted') BEGIN CREATE TABLE dbo.lastExecuted(row_id VARCHAR(7), dayof_week VARCHAR(100), last_exec_date VARCHAR(100)); END"),
                ("employeedetails", "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'employeedetails') BEGIN CREATE TABLE dbo.employeedetails(id VARCHAR(7) PRIMARY KEY, fullname VARCHAR(100), age VARCHAR(50), phonenumber VARCHAR(50) UNIQUE, email VARCHAR(100) UNIQUE, hourlyrate VARCHAR(20), surname VARCHAR(25)); END"),
                ("admintable", "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'admintable') BEGIN CREATE TABLE dbo.admintable(id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedetails(id), Admin_name VARCHAR(100) NOT NULL, Admin_contact VARCHAR(100) NOT NULL); END"),
                ("rotatable", "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'rotatable') BEGIN CREATE TABLE dbo.rotatable(day_ofweek VARCHAR(9), start_work DATETIME, finish_work DATETIME, id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedetails(id)); END"),
                ("employeepay", "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'employeepay') BEGIN CREATE TABLE dbo.employeepay(date_of_work DATE, total_pay DECIMAL(10,2), hours_done VARCHAR(100), id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedetails(id)); END"),
                ("carddata", "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'carddata') BEGIN CREATE TABLE dbo.carddata(id VARCHAR(7) PRIMARY KEY, cardNum VARCHAR(18), expiryDate VARCHAR(6), cvv VARCHAR(4), holderName VARCHAR(255)); END"),
                ("hourstable", "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'hourstable') BEGIN CREATE TABLE dbo.hourstable(id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedetails(id), empname VARCHAR(100), hours VARCHAR(100)); END")
            };

            using (var conn = new SqlConnection(_appConn))
            {
                conn.Open();
                int total = steps.Length;
                for (int i = 0; i < total; i++)
                {
                    var step = steps[i];
                    try
                    {
                        using (var cmd = new SqlCommand(step.Sql, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        // swallow per-table errors; higher-level logic may handle or log if desired
                    }
                    // no progress reporting in this simplified implementation
                }

                conn.Close();
            }
        }
    }
}
