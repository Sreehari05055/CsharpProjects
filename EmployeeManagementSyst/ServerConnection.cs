using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace EmployeeManagementSyst
{
    class ServerConnection
    {
        private static readonly Lazy<string> _connectionString = new Lazy<string>(LoadConnectionString);


        public static string LoadConnectionString()
        {

                try
                {
                // Use explicit assembly overload to avoid generic resolution issues
                var builder = new ConfigurationBuilder();
                UserSecretsConfigurationExtensions.AddUserSecrets(builder, typeof(Program).Assembly, optional: false);
                var config = builder.Build();
                string? cs = config["EmployeeDatabase"];

                if (string.IsNullOrEmpty(cs))
                        {
                            throw new Exception("Connection string 'EmployeeDatabase' not found or empty in ENV file.");
                        }
                return cs;
                }
                catch (Exception ex) {
                MessageBox.Show("Failed to load database connection string: " + ex.Message);
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public static string GetConnectionString() => _connectionString.Value;

        public static SqlConnection GetOpenConnection()
        {
            try
            {
                var serverCon = new SqlConnection(GetConnectionString());

                serverCon.Open();
                return serverCon;


            }
            catch (Exception e)
            {
                MessageBox.Show("Error Initiating Connection: " + e.Message);
                Debug.WriteLine(e.Message);
                throw;
            }

        }
        public static SqlConnection CloseConnection()
        {
            try
            {
                var serverCon = new SqlConnection(GetConnectionString());

                serverCon.Close();
                return serverCon;


            }
            catch (Exception e)
            {
                MessageBox.Show("Error Initiating Connection: " + e.Message);
                Debug.WriteLine(e.Message);
                throw;
            }

        }
    }
}
