using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace EmployeeManagementSyst
{
    // Utility helper for simple employee existence checks
    internal static class EmployeeHelper
    {
        public static bool ExistsByClockPin(string clockPin)
        {
            try
            {
                using var conn = ServerConnection.GetOpenConnection();
                using var cmd = new SqlCommand("SELECT 1 FROM EmployeeDetails WHERE ClockPin = @val;", conn);
                cmd.Parameters.AddWithValue("@val", clockPin);
                var res = cmd.ExecuteScalar();
                return res != null;
            }
            catch
            {
                return false;
            }
        }
        public static bool isAdmin(string employeeId)
        {
            try
            {
                using var conn = ServerConnection.GetOpenConnection();
                using var cmd = new SqlCommand("SELECT UserRole FROM EmployeeDetails WHERE ClockPin = @val;", conn);
                cmd.Parameters.AddWithValue("@val", employeeId);
                var res = cmd.ExecuteScalar();
                return res != null && res.ToString() == "admin";
            }
            catch
            {
                return false;
            }
        }
        public static bool ExistsById(string id)
        {
            try
            {
                using var conn = ServerConnection.GetOpenConnection();
                using var cmd = new SqlCommand("SELECT 1 FROM EmployeeDetails WHERE Id = @val;", conn);
                cmd.Parameters.AddWithValue("@val", id);
                var res = cmd.ExecuteScalar();
                return res != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the employee Id for the given clock pin, or null if not found or on error.
        /// </summary>
        public static string? GetIdByClockPin(string clockPin)
        {
            try
            {
                using var conn = ServerConnection.GetOpenConnection();
                using var cmd = new SqlCommand("SELECT Id FROM EmployeeDetails WHERE ClockPin = @val;", conn);
                cmd.Parameters.AddWithValue("@val", clockPin);
                var res = cmd.ExecuteScalar();
                return res?.ToString();
            }
            catch
            {
                return null;
            }
        }

        public static string? GetNameById(string employeeId)
        {
            try 
            {
                using var conn = ServerConnection.GetOpenConnection();
                using var cmd = new SqlCommand("SELECT FullName FROM EmployeeDetails WHERE Id = @empId", conn);
                cmd.Parameters.AddWithValue("@empId", employeeId);
                var res = cmd.ExecuteScalar();
                return res?.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns true when there is an open time log for the given employee (EndTime IS NULL).
        /// </summary>
        public static bool HasOpenShift(string employeeId)
        {
            try
            {
                using var conn = ServerConnection.GetOpenConnection();
                using var cmd = new SqlCommand("SELECT COUNT(1) FROM TimeLogs WHERE EmployeeId = @id AND EndTime IS NULL;", conn);
                cmd.Parameters.AddWithValue("@id", employeeId);
                var res = cmd.ExecuteScalar();
                return Convert.ToInt32(res) > 0;
            }
            catch
            {
                return false;
            }
        }

        public static string GetAdminEmail()
        {
            try
            {
                using var conn = ServerConnection.GetOpenConnection();
                using var cmd = new SqlCommand("SELECT Email FROM AdminDetails WHERE IsActive = 1;", conn);
                var res = cmd.ExecuteScalar();
                return res?.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the start time of the open shift for the given employee, or null if none.
        /// </summary>
        public static DateTime? GetOpenShiftStartTime(string employeeId)
        {
            try
            {
                using var conn = ServerConnection.GetOpenConnection();
                using var cmd = new SqlCommand("SELECT TOP 1 StartTime FROM TimeLogs WHERE EmployeeId = @id AND EndTime IS NULL ORDER BY StartTime DESC;", conn);
                cmd.Parameters.AddWithValue("@id", employeeId);
                var res = cmd.ExecuteScalar();
                if (res == null || res == DBNull.Value) return null;
                return Convert.ToDateTime(res);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns an array of email addresses for employees with UserRole = 'admin'.
        /// </summary>
        public static string[] GetAdminEmails()
        {
            try
            {
                var emails = new List<string>();
                using var conn = ServerConnection.GetOpenConnection();
                using var cmd = new SqlCommand("SELECT Email FROM EmployeeDetails WHERE UserRole = 'admin' AND Email IS NOT NULL;", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var email = reader["Email"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(email)) emails.Add(email);
                }
                return emails.ToArray();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }
    }
}
