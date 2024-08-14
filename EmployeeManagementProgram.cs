using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using Microsoft.Data.SqlClient;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp1
{
    internal class Program
    {
        private static string serverConnection;
        
        private DateTime shiftStart;
        private DateTime shiftEnd;
        private string name;
        private string age;
        private string hourlyRate;
        private string phoneNumber;
        private string email;
        private string code;
        private double hoursDone;
        private string dateofWork;
        private decimal totalPay;
        private string attachment;
        private string surname;
        private string filePath = "lastRunDate.txt";
        /*
         * Assigning properties for the created private fields
         */
        public String SurName
        {
            get { return surname; }
            set { surname = value; }
        }
        public String AttachMent
        {
            get { return attachment; }
            set { attachment = value; }
        }
        public DateTime ShiftStart
        {
            get { return shiftStart; }
            set { shiftStart = value; }
        }
        public DateTime ShiftEnd
        {
            get { return shiftEnd; }
            set { shiftEnd = value; }
        }
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        public String Code
        {
            get { return code; }
            set { code = value; }
        }
        public String Age
        {
            get { return age; }
            set { age = value; }
        }
        public String HourlyRate
        {
            get { return hourlyRate; }
            set { hourlyRate = value; }
        }
        public String PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
        public String Email
        {
            get { return email; }
            set { email = value; }
        }
        public double HoursDone
        {
            get { return hoursDone; }
            set { hoursDone = value; }
        }
        public String WorkDate
        {
            get { return dateofWork; }
            set { dateofWork = value; }
        }
        public decimal TotalPay
        {
            get { return totalPay; }
            set { totalPay = value; }
        }



        /*
         * Method to delete employee from SQL db by user just giving the name of the employee 
         */
        public void DeleteEmp(string empCode)
        {
            try
            {
                using (SqlConnection serverConn = new SqlConnection(serverConnection))
                {
                    serverConn.Open();
                    string querydlt = "DELETE FROM employeedata WHERE id = @id; ";
                    SqlCommand exec = new SqlCommand(querydlt, serverConn);
                    exec.Parameters.AddWithValue("@id", Code);
                    int rowsAffected = exec.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Employee deleted successfully");
                    }
                    else
                    {
                        Console.WriteLine("Failed to delete employee or employee not found");
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Deleting Employee: " + ex.Message); }
        }
        // Generates a 4-digit unique code for employee
        public String EmployeeCode()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(serverConnection))
                {
                    conn.Open();
                    string queryCode = "SELECT id FROM employeedata WHERE id = @id;";
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
            catch (Exception e) { Console.WriteLine("Error Generating Unique Code: " + e.Message); }
            return Code;
        }
        // Select id from the rota table and finds the email and fullname from employeedata using that id.  
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

                    string nameQuery = "SELECT fullname,email FROM employeedata WHERE id = @id;";
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
                            Console.WriteLine("Employee not found");
                            continue;
                        }
                        if (String.IsNullOrEmpty(emailAdd))
                        {
                            Console.WriteLine("Employee email address not found");
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
                        SendEmail(emailAdd, "Your Work Rota", rota);
                    }
                }

            }
            catch (Exception e) { Console.WriteLine("Error Creating Rota: " + e.Message); }
        }
        // Sends an email to the email address given as parameter along with the body and subject
        public void SendEmail(string emailAdd, string subject, string body)
        {
            try
            {
                using (MailMessage mailMessage = new MailMessage())

                {

                    mailMessage.From = new MailAddress("sreekuttankzm@gmail.com");
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.To.Add(emailAdd);
                    mailMessage.IsBodyHtml = false;

                    if (!string.IsNullOrWhiteSpace(AttachMent))
                    {
                        Attachment attach = new Attachment(AttachMent);
                        mailMessage.Attachments.Add(attach);
                    }


                    using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtpClient.Credentials = new NetworkCredential("sreekuttankzm@gmail.com", "tqyi rthe cjgt znox");
                        smtpClient.EnableSsl = true;
                        smtpClient.Send(mailMessage);

                    }

                }
                Console.WriteLine("Email sent to: " + emailAdd);
                if (!string.IsNullOrWhiteSpace(AttachMent))
                {
                    File.Delete(AttachMent);
                }

            }
            catch (SmtpException ex) { Console.WriteLine("SMTP Error: " + ex.Message); }
            catch (Exception e) { Console.WriteLine("Error Sending Email: " + e.Message); }
        }


        //Method to update employee details by using SQL queries appropriately
        public void UpdateEmp(String cde)
        {
            try
            {
                using (SqlConnection servrCon = new SqlConnection(serverConnection))
                {
                    servrCon.Open();

                    using (SqlTransaction transaction = servrCon.BeginTransaction())
                    {
                        string updtquery = "UPDATE employeedata SET ";
                        SqlCommand execte = new SqlCommand(null, servrCon, transaction);
                        execte.Transaction = transaction;
                        bool status = true;
                        if (!string.IsNullOrEmpty(Name))
                        {
                            if (!status) updtquery += ", ";
                            updtquery += "fullname = @fullname";
                            execte.Parameters.AddWithValue("@fullname", Name);
                            status = false;
                        }
                        if (!string.IsNullOrEmpty(Age))
                        {
                            if (!status) updtquery += ", ";
                            updtquery += "age = @age";
                            execte.Parameters.AddWithValue("@age", Age);
                            status = false;

                        }
                        if (!string.IsNullOrEmpty(PhoneNumber))
                        {
                            if (!status) updtquery += ", ";
                            updtquery += "phonenumber = @phonenumber";
                            execte.Parameters.AddWithValue("@phonenumber", PhoneNumber);
                            status = false;
                        }
                        if (!string.IsNullOrEmpty(Email))
                        {
                            if (!status) updtquery += ", ";
                            updtquery += "email = @email";
                            execte.Parameters.AddWithValue("@email", Email);
                            status = false;
                        }
                        if (!string.IsNullOrEmpty(HourlyRate))
                        {
                            if (!status) updtquery += ", ";

                            updtquery += "hourlyrate = @hourlyrate";
                            execte.Parameters.AddWithValue("@hourlyrate", HourlyRate);
                            status = false;
                        }
                        if (!string.IsNullOrEmpty(SurName))
                        {
                            if (!status) updtquery += ", ";

                            updtquery += "surname = @surname";
                            execte.Parameters.AddWithValue("@surname", SurName);
                            status = false;
                        }
                        if (!status)
                        {
                            updtquery += " WHERE id = @id;";
                            execte.Parameters.AddWithValue("@id", cde);
                            execte.CommandText = updtquery;
                            execte.ExecuteNonQuery();

                            if (!string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(Email))
                            {
                                string updateAdminTableQuery = "UPDATE admintable SET ";
                                SqlCommand updateAdminTableCmd = new SqlCommand(updateAdminTableQuery, servrCon, transaction);
                                bool adminHasUpdates = true;
                                if (!string.IsNullOrEmpty(Name))
                                {
                                    updateAdminTableQuery += "Admin_name = @newFullname";
                                    updateAdminTableCmd.Parameters.AddWithValue("@newFullname", Name);
                                    adminHasUpdates = false;
                                }
                                if (!string.IsNullOrEmpty(Email))
                                {
                                    if (!adminHasUpdates) updateAdminTableQuery += ", ";
                                    updateAdminTableQuery += "Admin_contact = @email";
                                    updateAdminTableCmd.Parameters.AddWithValue("@email", Email);
                                    adminHasUpdates = false;
                                }
                                if (!adminHasUpdates)
                                {
                                    updateAdminTableQuery += " WHERE id = @id";
                                    updateAdminTableCmd.Parameters.AddWithValue("@id", cde);
                                    updateAdminTableCmd.CommandText = updateAdminTableQuery;
                                    updateAdminTableCmd.ExecuteNonQuery();
                                }
                               
                            }
                            if (!string.IsNullOrEmpty(Name))
                            {
                                string updateHoursTableQry = "UPDATE hourstable SET ";
                                SqlCommand updatehoursTableCmd = new SqlCommand(updateHoursTableQry, servrCon, transaction);
                                bool hoursHasUpdates = true;
                                if (!string.IsNullOrEmpty(Name))
                                {
                                    updateHoursTableQry += "empname = @newFullname";
                                    updatehoursTableCmd.Parameters.AddWithValue("@newFullname", Name);
                                    hoursHasUpdates = false;
                                }
                                if (!hoursHasUpdates)
                                {
                                    updateHoursTableQry += " WHERE id = @id";
                                    updatehoursTableCmd.Parameters.AddWithValue("@id", cde);
                                    updatehoursTableCmd.CommandText = updateHoursTableQry;
                                    updatehoursTableCmd.ExecuteNonQuery();
                                }
                            }
                                transaction.Commit();
                            Console.WriteLine("Information updated successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No updates were made as no changes were provided.");
                            transaction.Rollback();
                        }
                    }

                }
            }

            catch (Exception ex) { Console.WriteLine("Error Updating Employee Details: " + ex.Message);
               
            }
        }
        // Checks if the employee is currently working
        public void EmployeeStatus()
        {
            try
            {
                using (SqlConnection server = new SqlConnection(serverConnection))
                {   
                    bool flag = false;
                    server.Open();
                    string queryCheck = "SELECT empname,id FROM hourstable;";
                    SqlCommand payExec = new SqlCommand(queryCheck, server);

                    using (SqlDataReader reader = payExec.ExecuteReader())
                    {
                    
                        while (reader.Read() && flag == false)
                        {
                            string id = reader.GetString(reader.GetOrdinal("id"));
                            string name = reader.GetString(reader.GetOrdinal("empname"));
                            flag = true;
                            Console.WriteLine($"{name} is currently working || Code: {id}");

                        }
                        if(flag == false) { Console.WriteLine("No one is working at the moment"); }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Checking Employee Status: " + ex.Message); }
        }
        // Checks if the hourstable exists, if not, it creates the hourstable
        public void HoursTable()
        {
            try
            {
                using (SqlConnection server = new SqlConnection(serverConnection))
                {
                    server.Open();
                    string rotaQuery = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'employeedb' AND table_schema = 'dbo' AND table_name = 'hourstable';";
                    SqlCommand rotaExec = new SqlCommand(rotaQuery, server);
                    Object data = rotaExec.ExecuteScalar();
                    if (data == null)
                    {
                        string createRota = "CREATE TABLE dbo.hourstable(id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedata(id), empname VARCHAR(100) ,hours VARCHAR(100));";
                        SqlCommand toExc = new SqlCommand(createRota, server);
                        toExc.ExecuteNonQuery();
                        Console.WriteLine("Hours table Created");
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Creating Table (Hours Table): " + ex.Message); }
        }
        // Inserts values provided by the user to the hourstable 
        public void InsertHoursTable(string time)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(serverConnection))
                {
                    conn.Open();
                    string nameQry = "SELECT fullname FROM employeedata WHERE id = @Code;";
                    SqlCommand fullnameExec = new SqlCommand(nameQry, conn);
                    fullnameExec.Parameters.AddWithValue("@Code", Code);
                    using (SqlDataReader reader2 = fullnameExec.ExecuteReader())
                    {
                        if (reader2.Read())
                        {
                            string name = reader2.GetString(reader2.GetOrdinal("fullname"));
                            Name = name;
                        }
                    }
                    string insertAdmin = """INSERT INTO hourstable(id,empname,hours)  VALUES(@id,@fullname,@hours)""";
                    SqlCommand nameExec = new SqlCommand(insertAdmin, conn);

                    nameExec.Parameters.AddWithValue("@id", Code);
                    nameExec.Parameters.AddWithValue("@fullname", Name);
                    nameExec.Parameters.AddWithValue("@hours", time);

                    int affectedRow = nameExec.ExecuteNonQuery();
                }
            }
            catch (Exception e) { Console.WriteLine("Error Inserting Values (Hours Table): " + e.Message); }
        }
        // Checks if the emplpoyee exists in the hourstable which means that they are working
        public void CheckStatus() 
        {
            try 
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string chckQry = "SELECT * FROM hourstable WHERE id = @Code;";
                    SqlCommand exec = new SqlCommand(chckQry, connection);
                    exec.Parameters.AddWithValue("@Code", Code);
                    using (SqlDataReader reader = exec.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            Console.WriteLine("You haven't clocked out from previous work");
                            System.Environment.Exit(1);
                        }
                    }
                }
            }catch (Exception e) { Console.WriteLine("Error Checking Employee Status: "+e.Message); }
        }
        // Gets the hours the employee has completed
        public void CompletedHours()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string chckQry = "SELECT hours FROM hourstable WHERE id = @Code ";
                    SqlCommand exec = new SqlCommand(chckQry, connection);
                    exec.Parameters.AddWithValue("@Code", Code);
                    using (SqlDataReader reader2 = exec.ExecuteReader())
                    {

                        if (reader2.Read())
                        {
                            string hours = reader2.GetString(reader2.GetOrdinal("hours"));
                            StopWatch(hours);
                        }
                    }
                }
            }
            catch (Exception e) { Console.WriteLine("Error Getting Completed Hours: " + e.Message); }
        }
        // On Clocking out the user will be deleted from the hourstable
        public void DeleteTime()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string chckQry = "DELETE FROM hourstable WHERE id = @Code ";
                    SqlCommand exec = new SqlCommand(chckQry, connection);
                    exec.Parameters.AddWithValue("@Code", Code);
                    int rowsAffected = exec.ExecuteNonQuery();
                }
            }
            catch (Exception e) { Console.WriteLine("Error Clocking Out Employee: " + e.Message); }
        }
        // Method to start work using stopwatch.
        public void StartWatch()
        {
            DateTime startTime = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch();
            CheckStatus();
            InsertHoursTable(startTime.ToString("o"));
            Console.WriteLine("You have clocked in");
            
        }
        // Method to stop work using stopwatch.
        public void StopWatch(string hours)
        {

           
            
                DateTime startTime = DateTime.Now;
                
                if (DateTime.TryParse(hours, out startTime))
                {
                    TimeSpan elapsed = DateTime.Now - startTime;

                    Console.WriteLine("You have clocked out");

                    string elapsedHrs = ((int)elapsed.TotalHours).ToString("00");
                    string elapsedMinutes = elapsed.Minutes.ToString("00");
                    string seconds = elapsed.Seconds.ToString("00");

                    string elapsedTime = $"{elapsedHrs}:{elapsedMinutes}:{seconds}";

                    double elapsedHours = elapsed.TotalHours;
                    HoursDone = elapsedHours;
                    DateWorked();
                    CalculatePay();

                    Console.WriteLine("Elapsed Time: " + elapsedTime);


                    DeleteTime();
                    InsertEmployeePay();
                
            }
            
        }
        // A custom rule based ChatBot that requires more data to work efficiently
        static async Task ChatBot()
        {          

            var customData = new List<(string Prompt, string Response)>
                {
            ("hello", "Hi there! How can I help you today?"),
            ("what is your name", "I am a custom chatbot ."),
            ("tell me a joke", "you")
        };

            while (true)
            {
                Console.Write("You: ");
                string userIn = Console.ReadLine();
                if (string.IsNullOrEmpty(userIn))
                    break;
                var customResponse = customData.FirstOrDefault(d => d.Prompt.Equals(userIn.ToLower(), StringComparison.OrdinalIgnoreCase)).Response;

                if (!string.IsNullOrEmpty(customResponse))
                {
                    Console.WriteLine("Bot: " + customResponse);
                }
                else
                {

                    Console.WriteLine("Bot: I'm not sure how to respond to that.");                    
                }
            }
        }
        // Method to calculate pay for each employee
        public void CalculatePay()
        {
            try
            {
                using (SqlConnection server = new SqlConnection(serverConnection))
                {
                    server.Open();
                    string payQuery = "SELECT hourlyrate FROM employeedata WHERE id = @id;";
                    SqlCommand payExec = new SqlCommand(payQuery, server);
                    payExec.Parameters.AddWithValue("@id", Code);
                    object result = payExec.ExecuteScalar();
                    if (result != null)
                    {
                        double hourlyRate = Convert.ToDouble(result);

                        double cmpltePay = HoursDone * hourlyRate;
                        decimal completePay = (decimal)cmpltePay;
                        TotalPay = completePay;

                    }
                    else { Console.WriteLine("Hourly rate not found for employee id"); }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Calculating Pay: "+ex.Message); }
        }
        // Method to get total pay for the week for each employee
        public void PaySlip()
        {        
           try
                    {
                        using (SqlConnection server = new SqlConnection(serverConnection))
                        {
                            server.Open();
                            string qry = "SELECT id, SUM(total_pay) AS total_pay FROM employeepay WHERE date_of_work BETWEEN @finished_date AND @current_date GROUP BY id;";
                            SqlCommand sqlCommand = new SqlCommand(qry, server);
                            DateTime payDate = DateTime.Now;
                            DateTime sevenDaysBefore = payDate.AddDays(-7);



                            sqlCommand.Parameters.AddWithValue("@finished_date", sevenDaysBefore);
                            sqlCommand.Parameters.AddWithValue("@current_date", payDate);

                            var payDetails = new List<(string id, decimal pay)>();
                            using (SqlDataReader reader = sqlCommand.ExecuteReader())
                            {

                                while (reader.Read())
                                {

                                    string id = reader.GetString(reader.GetOrdinal("id"));
                                    decimal totalPay = reader.GetDecimal(reader.GetOrdinal("total_pay"));

                                    payDetails.Add((id, totalPay));
                                }
                            }
                            foreach (var (id, pay) in payDetails)
                            {
                                string emailQuery = "SELECT email, fullname FROM employeedata WHERE id = @id;";
                                SqlCommand command = new SqlCommand(emailQuery, server);
                                command.Parameters.AddWithValue("@id", id);
                                using (SqlDataReader reader2 = command.ExecuteReader())
                                {

                                    if (reader2.Read())
                                    {
                                        string name = reader2.GetString(reader2.GetOrdinal("fullname"));
                                        string email = reader2.GetString(reader2.GetOrdinal("email"));

                                        string payment = $"Name: {name}\nID: {id}\n" +
                                                         $"Total Weekly Payment: £{pay}";
                                        string path = $"{id}.txt";
                                        File.WriteAllText(path, payment);

                                        Code = id;
                                        AttachMent = path;

                                        SendEmail(email, "Your Weekly Payment Details", "View Your Payslip in the attached file");
                                    }
                                }
                            }
                            

                        }


                    }
                    catch (Exception e) { Console.WriteLine("Error Creating PaySlip: " + e.Message); }
                
               
               

                
            
        }
        // Creates an admin table on checking whether it exists or not
        public void CreateAdminTable()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string adminQuery = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'employeedb' AND table_schema = 'dbo' AND table_name = 'admintable';";
                    SqlCommand adminExec = new SqlCommand(adminQuery, connection);
                    Object adminData = adminExec.ExecuteScalar();
                    if (adminData == null)
                    {
                        string createRota = "CREATE TABLE dbo.admintable(id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedata(id), Admin_name VARCHAR(100) NOT NULL, Admin_contact VARCHAR(100) NOT NULL);";
                        SqlCommand adminExc = new SqlCommand(createRota, connection);
                        adminExc.ExecuteNonQuery();
                        Console.WriteLine("Admin Table Created");
                    }
                }
            } catch (Exception e) { Console.WriteLine("Error Creating Table (Admin Table):" + e.Message); }
        }
        // Inserts values provided by user to the admintable
        public void InsertAdminInfo(string id, string name, string email)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string insertAdmin = """INSERT INTO admintable(id,Admin_name,Admin_contact)  VALUES(@id,@adminName,@adminEmail)""";
                    SqlCommand adminExec = new SqlCommand(insertAdmin, connection);

                    adminExec.Parameters.AddWithValue("@id", id);
                    adminExec.Parameters.AddWithValue("@adminName", name);
                    adminExec.Parameters.AddWithValue("@adminEmail", email);

                    int affectedRow = adminExec.ExecuteNonQuery();
                }
            } catch (Exception e) { Console.WriteLine("Error Inserting Values (Admin Table): " + e.Message); }

        }
        // Method to remove admins from the admintable
        public void RemoveAdmin(string id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(serverConnection))
                {
                    conn.Open();
                    string deleteAdmin = "DELETE FROM admintable WHERE id = @id; "; ;
                    SqlCommand detailQuery = new SqlCommand(deleteAdmin, conn);

                    detailQuery.Parameters.Clear();
                    detailQuery.Parameters.AddWithValue("@id", id);

                    int rowsAffected = detailQuery.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Admin Deleted");
                    }
                    else { Console.WriteLine("Failed to delete admin  or admin not found "); }

                }

            }
            catch (Exception e) { Console.WriteLine("Error Removing Admin: " + e.Message); }
        }
        
        // Method to get admins info
        public void GetAdmininfo(string id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(serverConnection))
                {
                    conn.Open();
                    string admindetailQuery = "SELECT fullname,email FROM employeedata WHERE id = @id";
                    SqlCommand detailQuery = new SqlCommand(admindetailQuery, conn);

                    detailQuery.Parameters.Clear();
                    detailQuery.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = detailQuery.ExecuteReader();
                    if (reader.HasRows) {
                        if (reader.Read())
                        {
                            // Add a method that checks if the person exists
                            string adminName = reader.GetString(reader.GetOrdinal("fullname"));
                            string adminEmail = reader.GetString(reader.GetOrdinal("email"));
                            InsertAdminInfo(id, adminName, adminEmail);
                            Console.WriteLine("Employee was made admin");
                        }
                    }
                    else { Console.WriteLine("No data found"); }
                }

            }
            catch (Exception e) { Console.WriteLine("Error Getting Admin Information: " + e.Message); }
        }
       // Method to create a rotatable on checking whether it exists or not
        public void RotaTable()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string rotaQuery = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'employeedb' AND table_schema = 'dbo' AND table_name = 'rotatable';";
                    SqlCommand rotaExec = new SqlCommand(rotaQuery, connection);
                    Object data = rotaExec.ExecuteScalar();
                    if (data == null)
                    {
                        string createRota = "CREATE TABLE dbo.rotatable(day_ofweek VARCHAR(9),start_work DATETIME, finish_work DATETIME, id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedata(id));";
                        SqlCommand toExc = new SqlCommand(createRota, connection);
                        toExc.ExecuteNonQuery();
                        Console.WriteLine("Rota table Created");
                    }
                }

            } catch (Exception ex) { Console.WriteLine("Error Creating Table (Rota Table): " + ex.Message); }
        }
        // Method to insert employee rota details to the rotatable
        public void ScheduleRota(DateTime startShift, DateTime endShift, string id, DateTime date)
        {
            try
            {
                string datetoUse = date.DayOfWeek.ToString();
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string insertquery = """INSERT INTO rotatable(day_ofweek ,start_work,finish_work,id)   VALUES (@dayofweek,@start,@finish,@id)""";

                    SqlCommand execute = new SqlCommand(insertquery, connection);

                    execute.Parameters.AddWithValue("@dayofweek", datetoUse);
                    execute.Parameters.AddWithValue("@start", startShift);
                    execute.Parameters.AddWithValue("@finish", endShift);
                    execute.Parameters.AddWithValue("@id", id);

                    int rowsAffected = execute.ExecuteNonQuery();
                    Console.WriteLine("Rota Added");
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Scheduling Rota: " + ex.Message); }
        }
        // Method to view the rota of employees
        public void ViewRota()
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
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

                    string nameQuery = "SELECT fullname FROM employeedata WHERE id = @id;";
                    SqlCommand commnd = new SqlCommand(nameQuery, connection);

                    string query = "SELECT start_work,finish_work,day_ofweek FROM rotatable WHERE id = @id2  ORDER BY start_work;";
                    SqlCommand commndRota = new SqlCommand(query, connection);

                    foreach (string id in obj)
                    {
                        commnd.Parameters.Clear();
                        commnd.Parameters.AddWithValue("@id", id);
                        string queryName = "";

                        using (SqlDataReader reader1 = commnd.ExecuteReader())
                        {
                            if (reader1.Read())
                            {
                                queryName = reader1.GetString(reader1.GetOrdinal("fullname"));
                            }
                        }
                        if (String.IsNullOrEmpty(queryName))
                        {
                            Console.WriteLine("Employee not found");
                            continue;
                        }

                        commndRota.Parameters.Clear();
                        commndRota.Parameters.AddWithValue("@id2", id);
                        using (SqlDataReader rotaFormat = commndRota.ExecuteReader())
                        {
                            Console.WriteLine("Rota for: " + queryName);
                            Console.WriteLine("------------------------------------------------------");

                            while (rotaFormat.Read())
                            {
                                DateTime shiftStart = rotaFormat.GetDateTime(rotaFormat.GetOrdinal("start_work"));
                                DateTime shiftEnd = rotaFormat.GetDateTime(rotaFormat.GetOrdinal("finish_work"));
                                string day = rotaFormat.GetString(rotaFormat.GetOrdinal("day_ofweek"));

                                Console.WriteLine($"{shiftStart:g} -- {shiftEnd:t} -- {day}");
                            }
                            Console.WriteLine();
                        }
                    }
                }
            } catch (Exception ex) { Console.WriteLine("Error Viewing Rota: " + ex.Message); }
        }
        // Method to get the surname by splitting their fullname
        public void GetSurname() 
        {
            try
            {
                string[] aray = Name.Split();
                string surname = aray[^1];
                SurName = surname;
            }
            catch (Exception ex) { Console.WriteLine("Error (Surname Comprehension): "+ex.Message); }
        }
        // Method to get the date on whichever days employee worked
        public void DateWorked()
        {
            DateTime today = DateTime.Today;
            String todayString = today.ToString("yyyy-MM-dd");
            WorkDate = todayString;
        }
        // Method to create a second table to record employee pay and their day of work
        public void EmployeePay()
        {
            try
            {

                using (SqlConnection serverConection = new SqlConnection(serverConnection))
                {
                    serverConection.Open();
                    string timeQuery = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'employeedb' AND table_schema = 'dbo' AND table_name = 'employeepay';";
                    SqlCommand payExec = new SqlCommand(timeQuery, serverConection);
                    Object data = payExec.ExecuteScalar();
                    if (data == null)
                    {
                        string createPayroll = "CREATE TABLE dbo.employeepay(date_of_work DATE, total_pay DECIMAL(10,2), hours_done VARCHAR(100), id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedata(id));";
                        SqlCommand toexc = new SqlCommand(createPayroll, serverConection);
                        toexc.ExecuteNonQuery();
                        Console.WriteLine("Payroll hours table Created");
                    }

                }

            }
            catch (Exception ex) { Console.WriteLine("Error Creating Table (Employee Pay): " + ex.Message); }

        }
        // Method to insert values to the second table created 
        public void InsertEmployeePay()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string insertquery = """INSERT INTO employeepay(date_of_work,total_pay,hours_done,id)   VALUES (@date_of_work,@totalpay,@hours_done,@id)""";

                    SqlCommand execute = new SqlCommand(insertquery, connection);

                    execute.Parameters.AddWithValue("@date_of_work", WorkDate);
                    execute.Parameters.AddWithValue("@totalpay", TotalPay);
                    execute.Parameters.AddWithValue("@hours_done", HoursDone);
                    execute.Parameters.AddWithValue("@id", Code);

                    int rowsAffected = execute.ExecuteNonQuery();
                    //Console.WriteLine("Employee details added");
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Inserting Values (Employee Pay): " + ex.Message); }
        }
        // Method to set up a connection to MySQL and setting up a connection and checking if the table exists or not
        public void EmployeeData()
        {
            try
            {

                using (SqlConnection serverCon = new SqlConnection(serverConnection))
                {
                    serverCon.Open();
                    string queryinfo = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'employeedb' AND table_schema = 'dbo' AND table_name = 'employeedata'; ";
                    SqlCommand toExecute = new SqlCommand(queryinfo, serverCon);
                    Object data = toExecute.ExecuteScalar();
                    if (data == null)
                    {
                        string queryThree = "CREATE TABLE dbo.employeedata(id VARCHAR(7) PRIMARY KEY,fullname VARCHAR(100) ,age VARCHAR(50), phonenumber VARCHAR(50) UNIQUE, email VARCHAR(100) UNIQUE, hourlyrate VARCHAR(20), surname VARCHAR(25) );";
                        SqlCommand toexecute = new SqlCommand(queryThree, serverCon);
                        toexecute.ExecuteNonQuery();
                        Console.WriteLine("Database Created Successfuly");
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Creating Table (Employee Data): " + ex.Message); }

        }
        // Method to insert values to the employeedata table 
        public void InsertEmployeeDetails()
        {
            try
            {
                using (SqlConnection serverCon = new SqlConnection(serverConnection))
                {
                    serverCon.Open();
                    string insertQuery = """INSERT INTO employeedata(id,fullname,age,phonenumber,email,hourlyrate,surname)   VALUES (@id,@fullname,@age,@phonenumber,@email,@hourlyrate,@surname)""";

                    SqlCommand execute = new SqlCommand(insertQuery, serverCon);

                    execute.Parameters.AddWithValue("@id", Code);
                    execute.Parameters.AddWithValue("@fullname", Name);
                    execute.Parameters.AddWithValue("@age", Age);
                    execute.Parameters.AddWithValue("@phonenumber", PhoneNumber);
                    execute.Parameters.AddWithValue("@email", Email);
                    execute.Parameters.AddWithValue("@hourlyrate", HourlyRate);
                    execute.Parameters.AddWithValue("@surname", SurName);

                    int rowsAffected = execute.ExecuteNonQuery();
                    Console.WriteLine("Employee added");
                }
            }catch (Exception ex) { Console.WriteLine("Error Inserting Values (Employee Data): "+ex.Message); }


        }
        public void InitiateServer() 
        {
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfiguration configuration = builder.Build();

                // Get connection string
                string connectionString = configuration.GetConnectionString("EmployeeDatabase");

                serverConnection = connectionString;
            }catch (Exception ex) { Console.WriteLine("Error Initiating Server: "+ex.Message); }
        }
        // Method to check if the admin code exists in the admin table
        public void VerifyAdmin(string adminCode) 
        {
            try
            {
                using (SqlConnection serverConnect = new SqlConnection(serverConnection))
                {
                    serverConnect.Open();
                    String querytoCheck = "SELECT id FROM admintable WHERE id = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(querytoCheck, serverConnect);
                    mySqlCommand.Parameters.Clear();    
                    mySqlCommand.Parameters.AddWithValue("@id", adminCode);
                    object dataTocheck = mySqlCommand.ExecuteScalar();
                    if (dataTocheck == null)
                    {
                        Console.WriteLine("Code incorrect");
                        System.Environment.Exit(1);
                    }
                    
                }

            }
            catch (Exception ex) { Console.WriteLine("Admin Verification Error: " + ex.Message); }
        }
        private void EmployeeCard() 
        {

            try
            {

                using (SqlConnection serverCon = new SqlConnection(serverConnection))
                {
                    serverCon.Open();
                    string queryinfo = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'employeedb' AND table_schema = 'dbo' AND table_name = 'carddata'; ";
                    SqlCommand toExecute = new SqlCommand(queryinfo, serverCon);
                    Object data = toExecute.ExecuteScalar();
                    if (data == null)
                    {
                        string queryThree = "CREATE TABLE dbo.carddata(id VARCHAR(7) PRIMARY KEY,cardNum VARCHAR(18) ,expiryDate VARCHAR(6), cvv VARCHAR(4), holderName VARCHAR(255));";
                        SqlCommand toexecute = new SqlCommand(queryThree, serverCon);
                        toexecute.ExecuteNonQuery();
                        Console.WriteLine("Database Created Successfuly");
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Creating Table (Card Data): " + ex.Message); }

        }
        private void RemoveCard(string code) 
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(serverConnection))
                {
                    conn.Open();
                    string deleteCard = "DELETE FROM carddata WHERE id = @id; "; ;
                    SqlCommand detailQuery = new SqlCommand(deleteCard, conn);

                    detailQuery.Parameters.Clear();
                    detailQuery.Parameters.AddWithValue("@id", code);

                    int rowsAffected = detailQuery.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Card Deleted");
                    }
                    else { Console.WriteLine("Failed to delete Card or Card not found "); }

                }

            }
            catch (Exception e) { Console.WriteLine("Error Deleting Card: " + e.Message); }
        }
        private void InsertCardDetails(string cardNum, string expiryDate, string cvv,string holderName) 
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
                    Console.WriteLine("Employee Card Detail Added");
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Inserting Values (Card Data): " + ex.Message); }
        }
        // Method to get employee details by passing the surname or employee code as input
        public void EmployeeDetails() 
        {
            Console.Write("Enter Employee Code or surname: ");
            string ans = Console.ReadLine().Trim().ToLower();
            try
            {
                using (SqlConnection serverConnect = new SqlConnection(serverConnection))
                {
                    serverConnect.Open();
                    string qry = "SELECT id,fullname,age,phonenumber,email,hourlyrate FROM employeedata WHERE surname = @surname OR id = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect);
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@surname",ans);
                    mySqlCommand.Parameters.AddWithValue("@id", ans);
                    SqlDataReader reader = mySqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read()) 
                        {
                            string id = reader.GetString(reader.GetOrdinal("id"));
                            string name = reader.GetString(reader.GetOrdinal("fullname"));
                            string age = reader.GetString(reader.GetOrdinal("age"));
                            string phoneNumber = reader.GetString(reader.GetOrdinal("phonenumber"));
                            string email = reader.GetString(reader.GetOrdinal("email"));
                            string hourlyPay = reader.GetString(reader.GetOrdinal("hourlyrate"));
                            Console.WriteLine($"Code: {id} || Full Name: {name} || Age: {age} || Phone Number: {phoneNumber} || Email Address: {email} || Hourly Pay: {hourlyPay}");
                        }

                    }
                    else { Console.WriteLine("Employee not found"); }
                }                 
            }catch(Exception ex ) { Console.WriteLine("Employee Details Error: "+ex.Message); }
        }
        // Checks the last time the NetPay method was executed and executes it accordingly by saving the dates onto a text file
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

            // Check if today is Tuesday and it hasn't run today
            if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday && lastRunDate.Date != DateTime.Today)
            {
                PaySlip();
                File.WriteAllText(filePath, DateTime.Today.ToString("yyyy-MM-dd"));
            }
        }
        // Method to check if the employees unique code exists in the db
        public void Verification(String codeToCheck)
        {
            try {
                using (SqlConnection serverConnect = new SqlConnection(serverConnection))
                {
                    serverConnect.Open();
                    String querytoCheck = "SELECT id FROM employeedata WHERE id = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(querytoCheck, serverConnect);
                    mySqlCommand.Parameters.AddWithValue("@id", codeToCheck);
                    object dataTocheck = mySqlCommand.ExecuteScalar();
                    if (dataTocheck != null)
                    {
                        Code = codeToCheck;
                        
                    }
                    else
                    {
                        Console.WriteLine("Code incorrect");
                        System.Environment.Exit(1);
                    }
                }

            } catch (Exception ex) { Console.WriteLine("Verification Error: " + ex.Message); }
        }


        // Method to take input from users to update the employeedata db
        public static void UpdateEmployee()
        {
            try
            {
                Console.WriteLine();
                Program objEmp = new Program();
                Console.Write("Updated employee Name (leave blank to keep current): ");
                String employename = Console.ReadLine().Trim().ToLower();
                objEmp.Name = employename;
                objEmp.GetSurname();

                Console.Write("Updated employee age (leave blank to keep current): ");
                String empAge = Console.ReadLine();
                objEmp.Age = empAge;

                Console.Write("Updated employee phone number (leave blank to keep current): ");
                String empNum = Console.ReadLine();
                objEmp.PhoneNumber = empNum;

                Console.Write("Updated employee email address (leave blank to keep current): ");
                String empEmail = Console.ReadLine();
                objEmp.Email = empEmail;

                Console.Write("Updated employee hourly rate (leave blank to keep current): ");
                String empRate = Console.ReadLine();
                objEmp.HourlyRate = empRate;

                Console.Write("Enter employee code to update details: ");
                String empcde = Console.ReadLine();
                
                objEmp.UpdateEmp(empcde);
                

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Following Given Details: " + ex.Message);
            }
        }
        // Method to take input from users to delete employee from employeedata db
        public static void DeleteEmployee()
        {
            try
            {
                Console.WriteLine();
                Program obj = new Program();
                Console.Write("Enter employee Code: ");
                String empCd = Console.ReadLine();
                obj.Code = empCd; // Not necessary 
                obj.DeleteEmp(empCd);
                obj.RemoveAdmin(empCd);
                obj.RemoveCard(empCd);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Reading Employee Code: " + ex.Message);
            }
        }
        public void RotaDetails() 
        {
            Console.Write("Enter Employee Code: ");
            string empuniqCode = Console.ReadLine().Trim();
            Verification(empuniqCode);
            Console.Write("Enter Shift Day (1-31): ");
            string shiftDay = Console.ReadLine().Trim();
            Console.Write("Enter Shift Start Time (HH:mm): ");
            string shiftStartTime = Console.ReadLine().Trim();
            Console.Write("Enter Shift End Time (HH:mm): ");
            string shiftEndTime = Console.ReadLine().Trim();

            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            DateTime date = DateTime.Parse($"{year}-{month}-{shiftDay}");
            DateTime shiftStart = DateTime.Parse($"{year}-{month}-{shiftDay} {shiftStartTime}");
            DateTime shiftEnd = DateTime.Parse($"{year}-{month}-{shiftDay} {shiftEndTime}");
            ScheduleRota(shiftStart, shiftEnd, empuniqCode, date);
        }

        // Method to take input from users to add employee to the employeedata db
        
        public static void EmployeDetails()
        {
            try
            {
                Console.WriteLine();
                Program Obj = new Program();
                Console.Write("Enter employee Name: ");
                 String name = Console.ReadLine().Trim().ToLower();
                Obj.Name = name;
                Obj.GetSurname();

                Console.Write("Enter employee Age: ");
                String age = Console.ReadLine();
                Obj.Age = age;

                Console.Write("Enter employee phone number: ");
                String number = Console.ReadLine();
                Obj.PhoneNumber = number;

                Console.Write("Enter employee email address: ");
                String email = Console.ReadLine();
                Obj.Email = email;

                Console.Write("Enter employee hourly rate: ");
                String rate = Console.ReadLine();
                Obj.HourlyRate = rate;
                Console.WriteLine();
                Console.WriteLine("=====================================");
                Console.WriteLine("ENTER THE FOLLOWING DETAILS CAREFULLY");
                Console.WriteLine("=====================================");
                Console.WriteLine();
                Console.Write("Employee Card Number: ");
                String cardNum = Console.ReadLine();

                Console.Write("Card Expiration Date: ");
                String cardExp = Console.ReadLine();

                Console.Write("Card CVV: ");
                String cardCvv = Console.ReadLine();

                Console.Write("Card Holder Name: ");
                String cardName = Console.ReadLine();

                string code = Obj.EmployeeCode();
                code = Obj.Code;
                // Calling the furtherProcess method  from the Program class 
                Obj.InsertEmployeeDetails();
                Obj.InsertCardDetails(cardNum,cardExp,cardCvv,cardName);


            }
            catch (MySql.Data.MySqlClient.MySqlException e)
            {
                if (e.Number == 1049)
                {
                    Console.WriteLine("Database does not exist");
                }
            }
            catch (Exception e) { Console.WriteLine("Error Following Given Details: " + e); }
        }

        /*
         * Main method to initiate the process
         */
        public static void Main(String[] args)
        {  
            try {
               
                Program program = new Program();
                program.InitiateServer();
                program.EmployeeData();
                program.EmployeePay();
                program.EmployeeCode();
                program.RotaTable();
                program.CreateAdminTable();
                program.HoursTable();
                program.EmployeeCard();
                if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)
                {
                    program.LastRunTime();
                    
                }
                Console.WriteLine("W  E  L  C  O  M  E");
                Console.WriteLine("===================");
                Console.WriteLine();
                Console.WriteLine("1) Start/End shift");
                Console.WriteLine("2) Management info");
                Console.WriteLine();
                Console.Write("Enter your choice(1-2): ");
                string reading = Console.ReadLine().Trim();
                Console.WriteLine();
                switch (reading)
                {
                    case "1":
                        Console.WriteLine("1) Start");
                        Console.WriteLine("2) End");
                        Console.WriteLine();
                        Console.Write("Enter '1' or '2': ");  
                        string ans = Console.ReadLine().Trim();
                        switch(ans)
                        {
                            case "1":
                            Console.Write("Enter Code: ");
                            String givenCode = Console.ReadLine().Trim();
                            program.Verification(givenCode);

                            program.StartWatch();
                                break;
                            case "2":
                        
                        
                            Console.Write("Enter Code: ");
                            String givnCode = Console.ReadLine().Trim();
                            program.Verification(givnCode);

                            program.CompletedHours();
                                break;
                        }
                        break;
                    case "2":
                        // From here:  comment this part of the code until you set an admin 
                        Console.Write("Enter Admin Id: ");
                        string read = Console.ReadLine().Trim();
                        Console.WriteLine();
                        program.VerifyAdmin(read);
                        // Until here
               
                            Console.WriteLine("1) Add Employee");
                            Console.WriteLine("2) Delete Employee");
                            Console.WriteLine("3) Update Employee Details");
                            Console.WriteLine("4) Add Rota");
                            Console.WriteLine("5) View Rota");
                            Console.WriteLine("6) Send Rota Email");                           
                            Console.WriteLine("7) Set Admin");
                            Console.WriteLine("8) Remove Admin");
                            Console.WriteLine("9) Check working employees");
                            Console.WriteLine("10) Get Employee Details");
                            Console.WriteLine("11) Need Help? Talk to our ChatBot!");
                            Console.WriteLine();
                            Console.Write("Enter your choice(1-11): ");
                            string answer = Console.ReadLine().Trim();
                            Console.WriteLine();
                            switch (answer)
                            {
                                     case "1":
                                         EmployeDetails();
                                         break;
                                     case "2":
                                         DeleteEmployee();
                                         break;
                                     case "3":
                                         UpdateEmployee();
                                         break;
                                    case "4":
                                        program.RotaDetails();                              
                                    break;

                                    case "5":
                                    program.ViewRota();
                                    break;
                                    case "6":
                                    Console.Write("Are you sure you want to proceed [y/n]");
                                   string anstoProceed = Console.ReadLine().Trim();
                                    switch (anstoProceed) 
                                {
                                    case "y":
                                        program.CreateRota();
                                        break;
                                    case "n":
                                        System.Environment.Exit(1);
                                        break;
                                    default:
                                        Console.WriteLine("Invalid Choice");
                                        break;
                                }
                                break;
                                    case "7":
                                Console.Write("Enter Employee Code(Admin ID): ");
                                string adminId = Console.ReadLine().Trim();
                                program.GetAdmininfo(adminId);
                                
                                break;
                            case "8":
                                Console.Write("Enter Admin ID (Employee Code): ");
                                string adminDlt = Console.ReadLine().Trim();
                                program.RemoveAdmin(adminDlt);
                                break;
                            case "9":
                                program.EmployeeStatus();
                                break;
                            case "10":
                                program.EmployeeDetails();
                                break;
                            case "11":
                                ChatBot().GetAwaiter().GetResult();
                                break;
                            
                            default:
                                Console.WriteLine("Invalid choice");
                                break;
                            }
                        break;
                    default:
                        Console.WriteLine("Invalid Choice");
                        break;
                } 

            }catch (Exception e) { Console.WriteLine("Error Validating Response: "+e.Message); }
    }
    }
}

