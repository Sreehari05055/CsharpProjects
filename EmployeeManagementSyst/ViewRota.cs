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
       
        public ViewRota()
        {
            InitializeComponent();
            InitiateServer();
            PopulateDataGridView();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
        }
        // Populates the DataGridView with employee rota data from the database.
        // Retrieves employee names and their respective shift details, then binds this information to the DataGridView.
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
        // Method to initiate server connection
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
