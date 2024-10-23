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
        private string serverConnection; // Connection string for the database
        private string code; //  Unique employee code
        private string surname; //  Employee's surname
        private string fullname; // Employee's full name

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

        // Constructor to initialize the form and set its propertiesb 
        public AddEmp()
        {
            InitializeComponent(); // Initializes the components of the form
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Sets the form border style
            this.BackColor = System.Drawing.Color.BlanchedAlmond; // Sets the background color of the form
            

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void AddEmp_Load(object sender, EventArgs e)
        {
            // Event handler for form load (can be implemented if needed)
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            serverConnection = MainPage.InitiateServer(); // Initializes the database connection
            EmployeeCode(); // Generates a unique employee code
            FullName = textBox1.Text.Trim().ToLower(); // Gets and formats the full name from input
            string ageInp = textBox2.Text; // Gets the age input
            string phoneInp = textBox8.Text; // Gets the phone number input
            string emailInp = textBox3.Text; // Gets the email input          
            string rateInp = textBox9.Text; // Gets the hourly rate input
            string cardNumInp = textBox4.Text; // Gets the card number input
            string cardExpInp = textBox5.Text; // Gets the card expiration date input
            string cvvInp = textBox6.Text; // Gets the CVV input
            string cardNameInp = textBox7.Text; // Gets the cardholder's name input
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
        // Method to generate a unique employee code by checking against existing records in the database
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
                            uniqueCode = true; // Exits the loop if code is unique
                        }
                    }
                }
            }
            catch (Exception e) { MessageBox.Show("Error Generating Unique Code: " + e.Message); }
            return Code;
        }

        // Method to extract the surname from the full name
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

        // Method to insert employee details into the database
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
    }
}
