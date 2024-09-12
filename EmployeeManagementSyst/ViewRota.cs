using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.NativeInterop;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmployeeManagementSyst
{
    public partial class ViewRota : Form
    {
        
        private string serverConnection;
        private string filePath = "lastExecuted.txt";
       
        public ViewRota()
        {
            InitializeComponent();
            InitiateServer();
            PopulateDataGridView();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
        }
        private void PopulateDataGridView()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();

                    // Query to get all IDs from rotatable table
                    string rotatableQuery = "SELECT id FROM rotatable;";
                    SqlCommand rotaCmd = new SqlCommand(rotatableQuery, connection);

                    HashSet<string> obj = new HashSet<string>();
                    using (SqlDataReader reader = rotaCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obj.Add(reader.GetString(reader.GetOrdinal("id")));
                        }
                    }

                    // Prepare queries
                    string nameQuery = "SELECT fullname FROM employeedetails WHERE id = @id;";
                    SqlCommand nameCmd = new SqlCommand(nameQuery, connection);

                    string rotaQuery = "SELECT start_work, finish_work, day_ofweek FROM rotatable WHERE id = @id2 ORDER BY start_work;";
                    SqlCommand rotaCmdDetails = new SqlCommand(rotaQuery, connection);

                    Dictionary<string, Dictionary<string, string>> employeeRota = new Dictionary<string, Dictionary<string, string>>();

                    foreach (string id in obj)
                    {
                        // Get the employee name
                        nameCmd.Parameters.Clear();
                        nameCmd.Parameters.AddWithValue("@id", id);
                        string employeeName = "";

                        using (SqlDataReader nameReader = nameCmd.ExecuteReader())
                        {
                            if (nameReader.Read())
                            {
                                employeeName = nameReader.GetString(nameReader.GetOrdinal("fullname"));
                            }
                        }

                        if (string.IsNullOrEmpty(employeeName))
                        {
                            continue;
                        }
                        if (!employeeRota.ContainsKey(employeeName))
                        {
                            employeeRota[employeeName] = new Dictionary<string, string>();
                        }

                        // Get the rota details
                        rotaCmdDetails.Parameters.Clear();
                        rotaCmdDetails.Parameters.AddWithValue("@id2", id);
                        using (SqlDataReader rotaReader = rotaCmdDetails.ExecuteReader())
                        {
                            while (rotaReader.Read())
                            {
                                DateTime shiftStart = rotaReader.GetDateTime(rotaReader.GetOrdinal("start_work"));
                                DateTime shiftEnd = rotaReader.GetDateTime(rotaReader.GetOrdinal("finish_work"));
                                string dayOfWeek = rotaReader.GetString(rotaReader.GetOrdinal("day_ofweek"));
                              
                                 string date = $"{shiftStart:d}";
                                
                                string shift = $"{shiftStart:t} - {shiftEnd:t}";
                                string key = $"{dayOfWeek} {date}";

                                employeeRota[employeeName][key] = shift;

                            }
                        }
                    }
                    DataTable rotaTable = new DataTable();

                    // Add the Employee Name column
                    rotaTable.Columns.Add("Employee Name", typeof(string));

                    // Determine all unique day/date combinations
                    HashSet<string> allDaysDates = new HashSet<string>();
                    foreach (var shifts in employeeRota.Values)
                    {
                        foreach (var dayDate in shifts.Keys)
                        {
                            allDaysDates.Add(dayDate);
                        }
                    }

                    // Add columns for each day/date combination
                    foreach (var dayDate in allDaysDates.OrderBy(d => d))
                    {
                        rotaTable.Columns.Add(dayDate, typeof(string));
                    }

                    // Add rows for each employee
                    foreach (var employee in employeeRota)
                    {
                        DataRow row = rotaTable.NewRow();
                        row["Employee Name"] = employee.Key;

                        foreach (var dayDate in allDaysDates.OrderBy(d => d))
                        {
                            row[dayDate] = employee.Value.ContainsKey(dayDate) ? employee.Value[dayDate] : string.Empty;
                        }

                        rotaTable.Rows.Add(row);
                    }
                    // Bind the DataTable to the DataGridView
                    dataGridView1.DataSource = rotaTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Viewing Rota: " + ex.Message);
            }
        }
        private void SaveWeeklyData()
        {
            try 
            {
                DateTime dateTime = DateTime.Now;

                string format = dateTime.ToString("yyyy-MM-dd");
                string path = $@"C:\Users\sreek\OneDrive\المستندات\WeeklyRota_{format}.txt"; ;
                StringBuilder sb = new StringBuilder();
                using (SqlConnection con = new SqlConnection(serverConnection))
                {
                    con.Open();
                    string saveQuery = """SELECT r.id, e.fullname, r.start_work, r.finish_work, r.day_ofweek FROM rotatable r INNER JOIN employeedetails e ON r.id = e.id;""";
                    SqlCommand sqlCommand = new SqlCommand(saveQuery, con);
                 
                    using (SqlDataReader reader = sqlCommand.ExecuteReader()) 
                    {
                        bool hasData = false;
                        while (reader.Read()) 
                        {
                            string fullName = reader.GetString(reader.GetOrdinal("fullname"));
                            string id = reader.GetString(reader.GetOrdinal("id"));
                            DateTime startWork = (DateTime)reader["start_work"];
                            DateTime finishWork = (DateTime)reader["finish_work"];
                            string dayOfWeek = reader["day_ofweek"].ToString();

                            sb.AppendLine($"{id}\t{fullName}\t{startWork:g}\t{finishWork:g}\t{dayOfWeek}");
                            hasData = true;
                        }
                        if (!hasData)
                        {
                           MessageBox.Show("No data found.");
                        }
                    }

                    if (sb.Length > 0)
                    {
                        File.WriteAllText(path, sb.ToString());
                        ResetWeeklyData();
                    }
                }


            } catch (Exception e) { MessageBox.Show("Error Saving Rota Data: "+e.Message    ); }
        
        }
        private void ResetWeeklyData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    DateTime now = DateTime.Now;
                    DateTime sevenDaysBefore = now.AddDays(-7);

                    string resetQuery = "DELETE FROM rotatable;";
                    SqlCommand resetCmd = new SqlCommand(resetQuery, connection);
                    int rowsAffected = resetCmd.ExecuteNonQuery();

                   // MessageBox.Show($"Weekly data reset completed. Rows affected: {rowsAffected}");

                }
            }
            catch (Exception ex)
            { 
                MessageBox.Show("Error Resetting Weekly Data: " + ex.Message);
            }
        }
        public void LastRunTime()
        {
            DateTime lastRunDate;
            if (File.Exists(filePath))
            {
                string dateText = File.ReadAllText(filePath);
                DateTime.TryParse(dateText, out lastRunDate);
            }
            else
            {
                lastRunDate = DateTime.MinValue;
            }

            
            if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday  && lastRunDate.Date != DateTime.Today)
            {
                SaveWeeklyData();
                File.WriteAllText(filePath, DateTime.Today.ToString("yyyy-MM-dd"));
            }
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
