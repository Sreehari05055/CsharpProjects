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
                ("LastExecution", "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'LastExecution') BEGIN CREATE TABLE dbo.LastExecution(Id INT IDENTITY(1,1) PRIMARY KEY, KeyName NVARCHAR(100) NOT NULL, DayOfWeek NVARCHAR(20) NULL, LastExecutedDate DATE NULL); END; " +
                    "IF NOT EXISTS (SELECT 1 FROM dbo.LastExecution WHERE KeyName = 'Payslip') INSERT INTO dbo.LastExecution(KeyName, DayOfWeek, LastExecutedDate) VALUES('Payslip', NULL, NULL); " +
                    "IF NOT EXISTS (SELECT 1 FROM dbo.LastExecution WHERE KeyName = 'WeeklySave') INSERT INTO dbo.LastExecution(KeyName, DayOfWeek, LastExecutedDate) VALUES('WeeklySave', NULL, NULL);"),
                ("EmployeeDetails", "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EmployeeDetails') BEGIN CREATE TABLE dbo.EmployeeDetails(Id VARCHAR(7) PRIMARY KEY, FullName NVARCHAR(200), Age VARCHAR(50), PhoneNumber VARCHAR(50) UNIQUE, Email NVARCHAR(200) UNIQUE, HourlyRate VARCHAR(20), Surname NVARCHAR(100)); END"),
                ("AdminInformation", "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AdminInformation') BEGIN CREATE TABLE dbo.AdminInformation(EmployeeId VARCHAR(7) FOREIGN KEY REFERENCES dbo.EmployeeDetails(Id), AdminName NVARCHAR(100) NOT NULL, AdminContact NVARCHAR(100) NOT NULL); END"),
                ("ScheduleInformation", "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ScheduleInformation') BEGIN CREATE TABLE dbo.ScheduleInformation(DayOfWeek NVARCHAR(20), StartWork DATETIME, FinishWork DATETIME, EmployeeId VARCHAR(7) FOREIGN KEY REFERENCES dbo.EmployeeDetails(Id)); END"),
                ("EmployeePayInfo", "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EmployeePayInfo') BEGIN CREATE TABLE dbo.EmployeePayInfo(DateOfWork DATE, TotalPay DECIMAL(10,2), HoursDone VARCHAR(100), EmployeeId VARCHAR(7) FOREIGN KEY REFERENCES dbo.EmployeeDetails(Id)); END"),
                ("CardInformation", "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CardInformation') BEGIN CREATE TABLE dbo.CardInformation(EmployeeId VARCHAR(7) PRIMARY KEY, CardNumber VARCHAR(18), ExpiryDate VARCHAR(6), Cvv VARCHAR(4), HolderName NVARCHAR(255)); END"),
                ("TimeLogs", "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TimeLogs') BEGIN CREATE TABLE dbo.TimeLogs(Id INT IDENTITY(1,1) PRIMARY KEY, EmployeeId VARCHAR(7) FOREIGN KEY REFERENCES dbo.EmployeeDetails(Id), EmployeeName NVARCHAR(200), StartTime DATETIME2, EndTime DATETIME2 NULL); CREATE INDEX IX_TimeLogs_EmployeeId_StartTime ON dbo.TimeLogs(EmployeeId, StartTime); END")
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
