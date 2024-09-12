using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.NativeInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace EmployeeManagementSyst
{
    public partial class AddEmp : Form
    {
        private string serverConnection;
        private string code;
        private string surname;
        private string fullname;

        public String SurName
        {
            get { return surname; }
            set { surname = value; }
        }
        public String FullName
        {
            get { return fullname; }
            set { fullname = value; }
        }
        public String Code
        {
            get { return code; }
            set { code = value; }
        }
        public AddEmp()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void AddEmp_Load(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitiateServer();
            EmployeeCode();
            FullName = textBox1.Text.Trim().ToLower();
            string ageInp = textBox2.Text;
            string phoneInp = textBox8.Text;
            string emailInp = textBox3.Text;           
            string rateInp = textBox9.Text;
            string cardNumInp = textBox4.Text;
            string cardExpInp = textBox5.Text;
            string cvvInp = textBox6.Text;
            string cardNameInp = textBox7.Text;
            GetSurname();
            
            InsertEmployeeDetails(FullName,ageInp,phoneInp,emailInp,rateInp,SurName);
            InsertCardDetails(cardNumInp, cardExpInp, cvvInp, cardNameInp);
            this.Close();
        }
        private void InsertCardDetails(string cardNum, string expiryDate, string cvv, string holderName)
        {
            try
            {
                using (SqlConnection serverCon = new SqlConnection(serverConnection))
                {
                    serverCon.Open();
                    string insertQuery = """INSERT INTO carddata(id,cardNum,expiryDate,cvv,holderName)   VALUES (@id,@cardNum,@expiryDate,@cvv,@holderName)""";

                    SqlCommand execute = new SqlCommand(insertQuery, serverCon);

                    execute.Parameters.AddWithValue("@id", Code);
                    execute.Parameters.AddWithValue("@cardNum", cardNum);
                    execute.Parameters.AddWithValue("@expiryDate", expiryDate);
                    execute.Parameters.AddWithValue("@cvv", cvv);
                    execute.Parameters.AddWithValue("@holderName", holderName);
                    int rowsAffected = execute.ExecuteNonQuery();
                    MessageBox.Show("Employee Card Detail Added");
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Inserting Values (Card Data): " + ex.Message); }
        }
        public String EmployeeCode()
        {
            try
            {
                
                using (SqlConnection conn = new SqlConnection(serverConnection))
                {
                    conn.Open();
                    string queryCode = "SELECT id FROM employeedetails WHERE id = @id;";
                    bool uniqueCode = false;

                    Random num = new Random();
                    while (!uniqueCode)
                    {
                        Code = "";
                        for (int i = 0; i <= 3; i++)
                        {

                            int randNum = num.Next(0, 10);
                            string randomNum = randNum.ToString();
                            Code += randomNum;

                        }
                        SqlCommand mySqlCommand = new SqlCommand(queryCode, conn);
                        mySqlCommand.Parameters.Clear();
                        mySqlCommand.Parameters.AddWithValue("@id", Code);
                        object dataTocheck = mySqlCommand.ExecuteScalar();
                        if (dataTocheck == null)
                        {
                            uniqueCode = true; // exits the loop if the same code is not found 
                        }
                    }
                }
            }
            catch (Exception e) { MessageBox.Show("Error Generating Unique Code: " + e.Message); }
            return Code;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void GetSurname()
        {
            try
            {
                string[] aray = FullName.Split();
                string surname = aray[^1];
                SurName = surname;
            }
            catch (Exception ex) { MessageBox.Show("Error (Surname Comprehension): " + ex.Message); }
        }
        public void InsertEmployeeDetails(string Name,string Age,string PhoneNumber, string Email,string HourlyRate,string SurName)
        {
            try
            {
                
                using (SqlConnection serverCon = new SqlConnection(serverConnection))
                {
                    serverCon.Open();
                    string insertQuery = """INSERT INTO employeedetails(id,fullname,age,phonenumber,email,hourlyrate,surname)   VALUES (@id,@fullname,@age,@phonenumber,@email,@hourlyrate,@surname)""";

                    SqlCommand execute = new SqlCommand(insertQuery, serverCon);

                    execute.Parameters.AddWithValue("@id", Code);
                    execute.Parameters.AddWithValue("@fullname", Name);
                    execute.Parameters.AddWithValue("@age", Age);
                    execute.Parameters.AddWithValue("@phonenumber", PhoneNumber);
                    execute.Parameters.AddWithValue("@email", Email);
                    execute.Parameters.AddWithValue("@hourlyrate", HourlyRate);
                    execute.Parameters.AddWithValue("@surname", SurName);

                    int rowsAffected = execute.ExecuteNonQuery();
                    MessageBox.Show("Employee added");
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Inserting Values (Employee Details): " + ex.Message); }


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
