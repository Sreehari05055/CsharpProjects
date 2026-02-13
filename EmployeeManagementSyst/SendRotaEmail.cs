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
      
        public SendRotaEmail()
        {
            InitializeComponent();


        }
        /// <summary>
        /// Creates the work rota for all employees and sends it to their email addresses.
        /// Retrieves the employee information from the database and formats the rota details to send via email.
        /// </summary>
        public void CreateRota()
        {
            try
            {
                using (SqlConnection conn = ServerConnection.GetOpenConnection())
                {
                   
                    string rotatableQuery = "SELECT EmployeeId FROM ScheduleInformation;";
                    SqlCommand rotaCmd = new SqlCommand(rotatableQuery, conn);

                    HashSet<string> obj = new HashSet<string>();
                    using (SqlDataReader reader = rotaCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obj.Add(reader.GetString(reader.GetOrdinal("EmployeeId")));
                        }
                    }

                    string nameQuery = "SELECT FullName,Email FROM EmployeeDetails WHERE Id = @id;";
                    SqlCommand commnd = new SqlCommand(nameQuery, conn);

                    string query = "SELECT StartWork,FinishWork,DayOfWeek FROM ScheduleInformation WHERE EmployeeId = @id2  ORDER BY StartWork;";
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
                                queryName = reader1.GetString(reader1.GetOrdinal("FullName"));
                                emailAdd = reader1.GetString(reader1.GetOrdinal("Email"));
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
                                DateTime shiftStart = rotaFormat.GetDateTime(rotaFormat.GetOrdinal("StartWork"));
                                DateTime shiftEnd = rotaFormat.GetDateTime(rotaFormat.GetOrdinal("FinishWork"));
                                string day = rotaFormat.GetString(rotaFormat.GetOrdinal("DayOfWeek"));

                                rota += $"{shiftStart:g} - {shiftEnd:t} --- {day}\n\n";
                            }

                        }
                        EmailConfiguration emailConfig = new EmailConfiguration();
                        emailConfig.SendEmail(emailAdd, "Your Work Rota", rota);
                    }
                    conn.Close();
                }

            }
            catch (Exception e) { MessageBox.Show("Error Creating Rota: " + e.Message); }
        }
    }
}
