using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
    public partial class SendRotaEmail : Form
    {
        private string serverConnection;
        public SendRotaEmail()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            InitiateServer();
        }
        public void CreateRota()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(serverConnection))
                {
                    conn.Open();
                    string rotatableQuery = "SELECT id FROM rotatable;";
                    SqlCommand rotaCmd = new SqlCommand(rotatableQuery, conn);

                    HashSet<string> obj = new HashSet<string>();
                    using (SqlDataReader reader = rotaCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obj.Add(reader.GetString(reader.GetOrdinal("id")));
                        }
                    }

                    string nameQuery = "SELECT fullname,email FROM employeedetails WHERE id = @id;";
                    SqlCommand commnd = new SqlCommand(nameQuery, conn);

                    string query = "SELECT start_work,finish_work,day_ofweek FROM rotatable WHERE id = @id2  ORDER BY start_work;";
                    SqlCommand commndRota = new SqlCommand(query, conn);

                    foreach (string id in obj)
                    {
                        commnd.Parameters.Clear();
                        commnd.Parameters.AddWithValue("@id", id);
                        string queryName = "";
                        string emailAdd = "";

                        using (SqlDataReader reader1 = commnd.ExecuteReader())
                        {
                            if (reader1.Read())
                            {
                                queryName = reader1.GetString(reader1.GetOrdinal("fullname"));
                                emailAdd = reader1.GetString(reader1.GetOrdinal("email"));
                            }
                        }
                        if (String.IsNullOrEmpty(queryName))
                        {
                            MessageBox.Show("Employee not found");
                            continue;
                        }
                        if (String.IsNullOrEmpty(emailAdd))
                        {
                            MessageBox.Show("Employee email address not found");
                            continue;
                        }
                        commndRota.Parameters.Clear();
                        commndRota.Parameters.AddWithValue("@id2", id);
                        string rota = $"Rota for: {queryName}\n-----------------------------------------------\n";
                        using (SqlDataReader rotaFormat = commndRota.ExecuteReader())
                        {

                            while (rotaFormat.Read())
                            {
                                DateTime shiftStart = rotaFormat.GetDateTime(rotaFormat.GetOrdinal("start_work"));
                                DateTime shiftEnd = rotaFormat.GetDateTime(rotaFormat.GetOrdinal("finish_work"));
                                string day = rotaFormat.GetString(rotaFormat.GetOrdinal("day_ofweek"));

                                rota += $"{shiftStart:g} - {shiftEnd:t} --- {day}\n\n";
                            }

                        }
                        PaySlip paySlip = new PaySlip();
                        paySlip.SendEmail(emailAdd, "Your Work Rota", rota);
                    }
                }

            }
            catch (Exception e) { MessageBox.Show("Error Creating Rota: " + e.Message); }
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
