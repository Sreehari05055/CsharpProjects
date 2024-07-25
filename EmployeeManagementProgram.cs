using System;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Completions;
using System.Diagnostics;
using OpenAI;
using System.Net.Mail;
using System.Net;
using Microsoft.Data.SqlClient;
using Org.BouncyCastle.Asn1;
namespace ConsoleApp1
{
    internal class Program
    {
        private static string serverConnection = "Server=SREEPC\\SQLEXPRESS;Database=employeedb;Integrated Security=True;TrustServerCertificate=True";

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
        public void DeleteEmp()
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
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
        // Generate unique code for employee
        public String EmployeeCode()
        {
            try
            {
                using (SqlConnection servr = new SqlConnection(serverConnection))
                {
                    servr.Open();
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
                        SqlCommand mySqlCommand = new SqlCommand(queryCode, servr);
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
            catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
            return Code;
        }
        public void SendRotaEmail()
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(serverConnection))
                {
                    Connection.Open();
                    string rotatableQuery = "SELECT id FROM rotatable;";
                    SqlCommand rotaCmd = new SqlCommand(rotatableQuery, Connection);

                    HashSet<string> obj = new HashSet<string>();
                    using (SqlDataReader reader = rotaCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obj.Add(reader.GetString(reader.GetOrdinal("id")));
                        }
                    }

                    string nameQuery = "SELECT fullname,email FROM employeedata WHERE id = @id;";
                    SqlCommand commnd = new SqlCommand(nameQuery, Connection);

                    string query = "SELECT start_work,finish_work,day_ofweek FROM rotatable WHERE id = @id2  ORDER BY start_work;";
                    SqlCommand commndRota = new SqlCommand(query, Connection);

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
            catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
        }
        // There are problems to fix here parameter 'null'
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


                    Attachment attach = new Attachment(AttachMent);
                    mailMessage.Attachments.Add(attach);


                    using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtpClient.Credentials = new NetworkCredential("sreekuttankzm@gmail.com", "tqyi rthe cjgt znox");
                        smtpClient.EnableSsl = true;
                        smtpClient.Send(mailMessage);

                    }

                }
                Console.WriteLine("Email sent to: " + emailAdd);
                File.Delete(AttachMent);

            }
            catch (SmtpException ex) { Console.WriteLine("Error: " + ex.Message); }
            catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
        }
        //Method to update employee details by using SQL queries appropriately
        public void UpdateEmp(String cde)
        {
            try
            {
                using (SqlConnection servrCon = new SqlConnection(serverConnection))
                {
                    servrCon.Open();
                    string updtquery = "UPDATE employeedata SET ";
                    SqlCommand execte = new SqlCommand(null, servrCon);
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
                    updtquery += " WHERE id = @id;";
                    execte.Parameters.AddWithValue("@id", cde);
                    execte.CommandText = updtquery;
                    try
                    {
                        int rowsAffected = execte.ExecuteNonQuery();
                        Console.WriteLine("Employee updated");
                    }
                    catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
        public void EmployeeCheck()
        {
            try
            {
                using (SqlConnection server = new SqlConnection(serverConnection))
                {
                    server.Open();
                    string queryCheck = "SELECT empname,id FROM hourstable;";
                    SqlCommand payExec = new SqlCommand(queryCheck, server);

                    using (SqlDataReader reader = payExec.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string id = reader.GetString(reader.GetOrdinal("id"));
                            string name = reader.GetString(reader.GetOrdinal("empname"));
                            Console.WriteLine($"{name} is currently working || Code: {id}");
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
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
                        string createRota = "CREATE TABLE dbo.hourstable(id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedata(id), empname VARCHAR(100) FOREIGN KEY REFERENCES dbo.employeedata(fullname),hours VARCHAR(100));";
                        SqlCommand toExc = new SqlCommand(createRota, server);
                        toExc.ExecuteNonQuery();
                        Console.WriteLine("Hours table Created");
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
        public void InsertHoursTable(string time)
        {
            try
            {
                using (SqlConnection admin = new SqlConnection(serverConnection))
                {
                    admin.Open();
                    string nameQry = "SELECT fullname FROM employeedata WHERE id = @Code;";
                    SqlCommand fullnameExec = new SqlCommand(nameQry, admin);
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
                    SqlCommand nameExec = new SqlCommand(insertAdmin, admin);

                    nameExec.Parameters.AddWithValue("@id", Code);
                    nameExec.Parameters.AddWithValue("@fullname", Name);
                    nameExec.Parameters.AddWithValue("@hours", time);

                    int affectedRow = nameExec.ExecuteNonQuery();
                }
            }
            catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
        }
        public void CheckHours() 
        {
            try 
            {
                using (SqlConnection admin = new SqlConnection(serverConnection))
                {
                    admin.Open();
                    string chckQry = "SELECT * FROM hourstable WHERE id = @Code;";
                    SqlCommand exec = new SqlCommand(chckQry, admin);
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
            }catch (Exception e) { Console.WriteLine("Error: "+e.Message); }
        }
        public void CheckTime()
        {
            try
            {
                using (SqlConnection time = new SqlConnection(serverConnection))
                {
                    time.Open();
                    string chckQry = "SELECT hours FROM hourstable WHERE id = @Code ";
                    SqlCommand exec = new SqlCommand(chckQry, time);
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
            catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
        }
        public void DeleteTime()
        {
            try
            {
                using (SqlConnection dlttime = new SqlConnection(serverConnection))
                {
                    dlttime.Open();
                    string chckQry = "DELETE FROM hourstable WHERE id = @Code ";
                    SqlCommand exec = new SqlCommand(chckQry, dlttime);
                    exec.Parameters.AddWithValue("@Code", Code);
                    int rowsAffected = exec.ExecuteNonQuery();
                }
            }
            catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
        }
        // Method to start work using stopwatch.
        public void StartWatch()
        {
            DateTime startTime = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch();
            CheckHours();
            InsertHoursTable(startTime.ToString("o"));
            Console.WriteLine("You have clocked in");
            
        }
        // Method to stop work using stopwatch.
        public void StopWatch(string hours)
        {

            CheckHours();
            
                DateTime startTime = DateTime.Now;
                //string startTimeStr = File.ReadAllText(Code + ".txt");
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
                    WorkdueDate();
                    PayTotal();

                    Console.WriteLine("Elapsed Time: " + elapsedTime);


                    DeleteTime();
                    TableInsert();
                
            }
            
        }
        static async Task ChatBot()
        {
            //string apiKey = "sk-proj-tPHJYMu8YT5YZ72UX2H2T3BlbkFJ9xUymDRiqA001vUJX7IV";
            //var openAIClient = new OpenAIAPI(apiKey);


            var customData = new List<(string Prompt, string Response)>
                {
            ("hello", "Hi there! How can I help you today?"),
            ("what is your name?", "I am a custom chatbot created by you."),
            ("tell me a joke.", "Why don't scientists trust atoms? Because they make up everything!")
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

                    /** try
                     {
                         // If no match in custom data, use OpenAI API
                         var completionRequest = new CompletionRequest
                         {
                             Prompt = userIn,
                             MaxTokens = 150
                         };

                         var result = await openAIClient.Completions.CreateCompletionAsync(completionRequest);
                         Console.WriteLine("Bot: " + result.Completions[0].Text.Trim());
                     }
                     catch (HttpRequestException ex) when (ex.Message.Contains("429"))
                     {
                         Console.WriteLine("Bot: Sorry, I've reached my usage limits. Please try again later.");
                     }
                     catch (Exception ex)
                     {
                         Console.WriteLine("Bot: An error occurred: " + ex.Message);
                     }**/
                }
            }




        }
        // Method to calculate pay for each employee
        public void PayTotal()
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
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        // Method to get total pay for the week for each employee
        public void  NetPay()
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

            } catch (Exception e) { Console.WriteLine("Error: " + e.Message); }

        }
        public void AdminTable()
        {
            try
            {
                using (SqlConnection adminConnect = new SqlConnection(serverConnection))
                {
                    adminConnect.Open();
                    string adminQuery = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'employeedb' AND table_schema = 'dbo' AND table_name = 'admintable';";
                    SqlCommand adminExec = new SqlCommand(adminQuery, adminConnect);
                    Object adminData = adminExec.ExecuteScalar();
                    if (adminData == null)
                    {
                        string createRota = "CREATE TABLE dbo.admintable(id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedata(id), Admin_name VARCHAR(100) FOREIGN KEY REFERENCES dbo.employeedata(fullname), Admin_contact VARCHAR(100) FOREIGN KEY REFERENCES dbo.employeedata(email));";
                        SqlCommand adminExc = new SqlCommand(createRota, adminConnect);
                        adminExc.ExecuteNonQuery();
                        Console.WriteLine("Admin Table Created");
                    }
                }
            } catch (Exception e) { Console.WriteLine("Error:" + e.Message); }
        }
        public void AdminInsert(string id, string name, string email)
        {
            try
            {
                using (SqlConnection admin = new SqlConnection(serverConnection))
                {
                    admin.Open();
                    string insertAdmin = """INSERT INTO admintable(id,Admin_name,Admin_contact)  VALUES(@id,@adminName,@adminEmail)""";
                    SqlCommand adminExec = new SqlCommand(insertAdmin, admin);

                    adminExec.Parameters.AddWithValue("@id", id);
                    adminExec.Parameters.AddWithValue("@adminName", name);
                    adminExec.Parameters.AddWithValue("@adminEmail", email);

                    int affectedRow = adminExec.ExecuteNonQuery();
                }
            } catch (Exception e) { Console.WriteLine("Error: " + e.Message); }

        }
        public void RemoveAdmin(string id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(serverConnection))
                {
                    conn.Open();
                    string deleteAdmin = "DELETE FROM employeedata WHERE id = @id; "; ;
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
            catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
        }
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
                            // add a method that checks if the person exists
                            string adminName = reader.GetString(reader.GetOrdinal("fullname"));
                            string adminEmail = reader.GetString(reader.GetOrdinal("email"));
                            AdminInsert(id, adminName, adminEmail);
                            Console.WriteLine("Employee was made admin");
                        }
                    }
                    else { Console.WriteLine("No data found"); }
                }

            }
            catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
        }
        public void RotaTable()
        {
            try
            {
                using (SqlConnection toConnect = new SqlConnection(serverConnection))
                {
                    toConnect.Open();
                    string rotaQuery = "SELECT table_name FROM information_schema.tables WHERE table_catalog = 'employeedb' AND table_schema = 'dbo' AND table_name = 'rotatable';";
                    SqlCommand rotaExec = new SqlCommand(rotaQuery, toConnect);
                    Object data = rotaExec.ExecuteScalar();
                    if (data == null)
                    {
                        string createRota = "CREATE TABLE dbo.rotatable(day_ofweek VARCHAR(9),start_work DATETIME, finish_work DATETIME, id VARCHAR(7) FOREIGN KEY REFERENCES dbo.employeedata(id));";
                        SqlCommand toExc = new SqlCommand(createRota, toConnect);
                        toExc.ExecuteNonQuery();
                        Console.WriteLine("Rota table Created");
                    }
                }

            } catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
        public void AddRota(DateTime startShift, DateTime endShift, string id, DateTime date)
        {
            try
            {
                string datetoUse = date.DayOfWeek.ToString();
                using (SqlConnection Conectt = new SqlConnection(serverConnection))
                {
                    Conectt.Open();
                    string insertquery = """INSERT INTO rotatable(day_ofweek ,start_work,finish_work,id)   VALUES (@dayofweek,@start,@finish,@id)""";

                    SqlCommand execute = new SqlCommand(insertquery, Conectt);

                    execute.Parameters.AddWithValue("@dayofweek", datetoUse);
                    execute.Parameters.AddWithValue("@start", startShift);
                    execute.Parameters.AddWithValue("@finish", endShift);
                    execute.Parameters.AddWithValue("@id", id);

                    int rowsAffected = execute.ExecuteNonQuery();
                    Console.WriteLine("Rota Added");
                }
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
        public void ViewRota()
        {

            try
            {
                using (SqlConnection Connection = new SqlConnection(serverConnection))
                {
                    Connection.Open();
                    string rotatableQuery = "SELECT id FROM rotatable;";
                    SqlCommand rotaCmd = new SqlCommand(rotatableQuery, Connection);

                    HashSet<string> obj = new HashSet<string>();
                    using (SqlDataReader reader = rotaCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obj.Add(reader.GetString(reader.GetOrdinal("id")));
                        }
                    }

                    string nameQuery = "SELECT fullname FROM employeedata WHERE id = @id;";
                    SqlCommand commnd = new SqlCommand(nameQuery, Connection);

                    string query = "SELECT start_work,finish_work,day_ofweek FROM rotatable WHERE id = @id2  ORDER BY start_work;";
                    SqlCommand commndRota = new SqlCommand(query, Connection);

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

                                Console.WriteLine($"{shiftStart:g} -- {shiftEnd:g} -- {day}");
                            }
                            Console.WriteLine();
                        }
                    }
                }
            } catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
        public void GetSurname() 
        {
            try
            {
                string[] aray = Name.Split();
                string surname = aray[^1];
                SurName = surname;
            }
            catch (Exception ex) { Console.WriteLine("Error: "+ex.Message); }
        }
        // Method to get the date on whichever days employee worked
        public void WorkdueDate()
        {
            DateTime today = DateTime.Today;
            String todayString = today.ToString("yyyy-MM-dd");
            WorkDate = todayString;
        }
        // Method to create a second table to record employee pay and their day of work
        public void TimeDone()
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
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }

        }
        // Method to insert values to the second table created 
        public void TableInsert()
        {
            try
            {
                using (SqlConnection Conect = new SqlConnection(serverConnection))
                {
                    Conect.Open();
                    string insertquery = """INSERT INTO employeepay(date_of_work,total_pay,hours_done,id)   VALUES (@date_of_work,@totalpay,@hours_done,@id)""";

                    SqlCommand execute = new SqlCommand(insertquery, Conect);

                    execute.Parameters.AddWithValue("@date_of_work", WorkDate);
                    execute.Parameters.AddWithValue("@totalpay", TotalPay);
                    execute.Parameters.AddWithValue("@hours_done", HoursDone);
                    execute.Parameters.AddWithValue("@id", Code);

                    int rowsAffected = execute.ExecuteNonQuery();
                    //Console.WriteLine("Employee details added");
                }
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
        // Method to set up a connection to MySQL and setting up a connection and checking if the table exists or not
        public void FurtherProcess()
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
                        string queryThree = "CREATE TABLE dbo.employeedata(id VARCHAR(7) PRIMARY KEY,fullname VARCHAR(100) UNIQUE,age VARCHAR(50), phonenumber VARCHAR(50) UNIQUE, email VARCHAR(100) UNIQUE, hourlyrate VARCHAR(20), surname VARCHAR(25) );";
                        SqlCommand toexecute = new SqlCommand(queryThree, serverCon);
                        toexecute.ExecuteNonQuery();
                        Console.WriteLine("Database Created Successfuly");
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }

        }
        public void InsertintoFurtherPr()
        {
            try
            {
                using (SqlConnection serverCon = new SqlConnection(serverConnection))
                {
                    serverCon.Open();
                    string insertQuery = """INSERT INTO employeedata(id,fullname,age,phonenumber,email,hourlyrate,surname)   VALUES (@id,@fullname,@age,@phonenumber,@email,@hourlyrate,@surname)""";

                    SqlCommand execute = new SqlCommand(insertQuery, serverCon);

                    execute.Parameters.AddWithValue("@id", EmployeeCode());
                    execute.Parameters.AddWithValue("@fullname", Name);
                    execute.Parameters.AddWithValue("@age", Age);
                    execute.Parameters.AddWithValue("@phonenumber", PhoneNumber);
                    execute.Parameters.AddWithValue("@email", Email);
                    execute.Parameters.AddWithValue("@hourlyrate", HourlyRate);
                    execute.Parameters.AddWithValue("@surname", SurName);

                    int rowsAffected = execute.ExecuteNonQuery();
                    Console.WriteLine("Employee added");
                }
            }catch (Exception ex) { Console.WriteLine("Error: "+ex.Message); }


        }
        public void CheckAdmin(string adminCode) 
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
                    if (dataTocheck != null)
                    {
                        
                    }
                    else
                    {
                        Console.WriteLine("Code incorrect");
                        System.Environment.Exit(1);
                    }
                }

            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
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
            }catch(Exception ex ) { Console.WriteLine("Error: "+ex.Message); }
        }
        // Method to check if the employees unique code exists in the db
        public void Check(String codeToCheck)
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

            } catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
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
                Console.WriteLine("Error: " + ex.Message);
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
                obj.Code = empCd;
                obj.DeleteEmp();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        public void RotaDetails() 
        {
            Console.Write("Enter Employee Code: ");
            string empuniqCode = Console.ReadLine().Trim();
            Check(empuniqCode);
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
            AddRota(shiftStart, shiftEnd, empuniqCode, date);
        }

        // Method to take input from users to add employee to the employeedata db
        
        public static void Connect()
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


                // Calling the furtherProcess method  from the Program class 
                Obj.InsertintoFurtherPr();


            }
            catch (MySql.Data.MySqlClient.MySqlException e)
            {
                if (e.Number == 1049)
                {
                    Console.WriteLine("Database does not exist");
                }
            }
            catch (Exception e) { Console.WriteLine("Error " + e); }
        }

        /*
         * Main method to initiate the process
         */
        public static void Main(String[] args)
        {  
            try {
                Program program = new Program();
                program.FurtherProcess();
                program.TimeDone();
                program.EmployeeCode();
                program.RotaTable();
                program.AdminTable();
                program.HoursTable();
                
                Console.WriteLine("1) Start/End work");
                Console.WriteLine("2) Management info");
                Console.Write("Enter your choice(1-2): ");
                string reading = Console.ReadLine().Trim();
                Console.WriteLine();
                switch (reading)
                {
                    case "1":
                        Console.WriteLine("1) Start");
                        Console.WriteLine("2) End");
                        Console.Write("Enter '1' or '2': ");  
                        string ans = Console.ReadLine().Trim();
                        if (ans.Equals("1"))
                        {
                            Console.Write("Enter Code: ");
                            String givenCode = Console.ReadLine().Trim();
                            program.Check(givenCode);

                            program.StartWatch();

                        }
                        else if (ans.Equals("2"))
                        {
                            Console.Write("Enter Code: ");
                            String givenCode = Console.ReadLine().Trim();
                            program.Check(givenCode);

                            program.CheckTime();
                        }
                        else { Console.WriteLine("Invalid choice"); }
                        break;
                    case "2":

                        Console.Write("Enter Admin Id: ");
                        string read = Console.ReadLine().Trim();
                        Console.WriteLine();
                        program.CheckAdmin(read);
               
                            Console.WriteLine("1) Add Employee");
                            Console.WriteLine("2) Delete Employee");
                            Console.WriteLine("3) Update Employee Details");
                            Console.WriteLine("4) Add Rota");
                            Console.WriteLine("5) View Rota");
                            Console.WriteLine("6) Send Rota Email");                           
                            Console.WriteLine("7) Set Admin");
                            Console.WriteLine("8) Remove Admin");
                            Console.WriteLine("9) Send Payment Details");
                            Console.WriteLine("10) Check working employees");
                            Console.WriteLine("11) Get Employee Details");
                            Console.WriteLine("12) Need Help? Talk to our ChatBot!");
                            
                            Console.Write("Enter your choice(1-12): ");
                            string answer = Console.ReadLine().Trim();
                            switch (answer)
                            {
                                     case "1":
                                         Connect();
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
                                        program.SendRotaEmail();
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
                                Console.Write("Are you sure you want to proceed [y/n]");
                                string ansPayment = Console.ReadLine().Trim();
                                switch (ansPayment) 
                                {
                                    case "y":
                                        program.NetPay();
                                        break;
                                    case "n":
                                        System.Environment.Exit(1);
                                        break;
                                    default:
                                        Console.WriteLine("Invalid Choice");
                                        break;
                                }
                                break;
                            case "10":
                                program.EmployeeCheck();
                                break;
                            case "11":
                                program.EmployeeDetails();
                                break;
                            case "12":
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

            }catch (Exception e) { Console.WriteLine("Error: "+e.Message); }
    }
    }
}

