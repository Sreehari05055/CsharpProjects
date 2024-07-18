using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Net.Mail;
using System.Net;
namespace ConsoleApp1
{
    internal class Program
    {
        private static string serverConnection = "server=localhost;uid=root;pwd=Bit4$ree@0505;database=employeedb";

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
        
         
        /*
         * Assigning properties for the created private fields
         */
        public DateTime ShiftStart 
        {
            get { return shiftStart;}
            set { shiftStart = value; }
        }
        public DateTime ShiftEnd 
        {
            get { return shiftEnd;} 
            set { shiftEnd = value;}
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
            try {
                using (MySqlConnection serverConn = new MySqlConnection(serverConnection))
                {
                    serverConn.Open();
                    string querydlt = "DELETE FROM employeedata " +
                                      "WHERE fullname = @fullname; ";
                    MySqlCommand exec = new MySqlCommand(querydlt, serverConn);
                    exec.Parameters.AddWithValue("@fullname", Name);
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
            } catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
        // Generate unique code for employee
        public String EmployeeCode()
        {
            try
            {
                using (MySqlConnection servr = new MySqlConnection(serverConnection))
                {
                    servr.Open();
                    string queryCode = "SELECT id FROM employeedata WHERE id = @id;";
                    bool uniqueCode = false;

                    Random num = new Random();
                    while (!uniqueCode)
                    {
                        Code = "";
                        for (int i = 0; i <= 6; i++)
                        {

                            int randNum = num.Next(0, 10);
                            string randomNum = randNum.ToString();
                            Code += randomNum;

                        }
                        MySqlCommand mySqlCommand = new MySqlCommand(queryCode, servr);
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
                using (MySqlConnection Connection = new MySqlConnection(serverConnection))
                {
                    Connection.Open();
                    string rotatableQuery = "SELECT id FROM rotatable;";
                    MySqlCommand rotaCmd = new MySqlCommand(rotatableQuery, Connection);

                    HashSet<string> obj = new HashSet<string>();
                    using (MySqlDataReader reader = rotaCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obj.Add(reader.GetString("id"));
                        }
                    }

                    string nameQuery = "SELECT fullname,email FROM employeedata WHERE id = @id;";
                    MySqlCommand commnd = new MySqlCommand(nameQuery, Connection);

                    string query = "SELECT start_work,finish_work,day_ofweek FROM rotatable WHERE id = @id2  ORDER BY start_work;";
                    MySqlCommand commndRota = new MySqlCommand(query, Connection);

                    foreach (string id in obj)
                    {
                        commnd.Parameters.Clear();
                        commnd.Parameters.AddWithValue("@id", id);
                        string queryName = "";
                        string emailAdd = "";

                        using (MySqlDataReader reader1 = commnd.ExecuteReader())
                        {
                            if (reader1.Read())
                            {
                                queryName = reader1.GetString("fullname");
                                emailAdd = reader1.GetString("email");
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
                        using (MySqlDataReader rotaFormat = commndRota.ExecuteReader())
                        {

                            while (rotaFormat.Read())
                            {
                                DateTime shiftStart = rotaFormat.GetDateTime("start_work");
                                DateTime shiftEnd = rotaFormat.GetDateTime("finish_work");
                                string day = rotaFormat.GetString("day_ofweek");

                                rota += $"{shiftStart:g} -- {shiftEnd:g} -- {day}\n";
                            }
                            
                        }
                        SendEmail(emailAdd, "Your Work Rota", rota);
                    }
                }

            } catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
        }
        private static void SendEmail(string emailAdd,string subject,string body)
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

                    using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com",587)) 
                    {
                        smtpClient.Credentials = new NetworkCredential("sreekuttankzm@gmail.com", "tqyi rthe cjgt znox");
                        smtpClient.EnableSsl = true;
                        smtpClient.Send(mailMessage);
                    }
                }
                Console.WriteLine("Email sent to: "+emailAdd);
            }catch(SmtpException ex) {Console.WriteLine(ex.Message); }
            catch (Exception e) { Console.WriteLine("Error: "+e.Message); }
        }
        //Method to update employee details by using SQL queries appropriately
        public void UpdateEmp(String cde)
        {
            try
            {
                using (MySqlConnection servrCon = new MySqlConnection(serverConnection))
                {
                    servrCon.Open();
                    string updtquery = "UPDATE employeedata SET ";
                    MySqlCommand execte = new MySqlCommand(null, servrCon);
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
            } catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
        // Method to start work using stopwatch.
        public void StartWatch()
        {
            DateTime startTime = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch();
            File.WriteAllText(Code + ".txt", startTime.ToString("o"));
        }
        // Method to stop work using stopwatch.
        public void StopWatch()
        {

            if (File.Exists(Code + ".txt"))
            {
                DateTime startTime = DateTime.Now;
                string startTimeStr = File.ReadAllText(Code + ".txt");
                if (DateTime.TryParse(startTimeStr, out startTime))
                {
                    TimeSpan elapsed = DateTime.Now - startTime;

                    Console.WriteLine("You have finished work");

                    string elapsedHrs = ((int)elapsed.TotalHours).ToString("00");
                    string elapsedMinutes = elapsed.Minutes.ToString("00");
                    string seconds = elapsed.Seconds.ToString("00");

                    string elapsedTime = $"{elapsedHrs}:{elapsedMinutes}:{seconds}";

                    double elapsedHours = elapsed.TotalHours;
                    HoursDone = elapsedHours;
                    WorkdueDate();
                    PayTotal();

                    Console.WriteLine("Elapsed Time: " + elapsedTime);


                    File.Delete(Code + ".txt");
                }
            }
        }
        // Method to calculate pay for each employee
        public void PayTotal()
        {
            try
            {
                using (MySqlConnection server = new MySqlConnection(serverConnection))
                {
                    server.Open();
                    string payQuery = "SELECT hourlyrate FROM employeedata WHERE id = @id;";
                    MySqlCommand payExec = new MySqlCommand(payQuery, server);
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
        public void NetPay()
        {
            try
            {
                DateTime startDate = DateTime.Now;
                using (MySqlConnection server = new MySqlConnection(serverConnection)) 
                {
                    server.Open();
                    string qry = "SELECT id, SUM(total_pay) AS total_pay FROM employeepay GROUP BY id;";
                    MySqlCommand sqlCommand = new MySqlCommand(qry, server);

                    string emailQuery = "SELECT email FROM employeedata WHERE id = @id;";
                    MySqlCommand command = new MySqlCommand(emailQuery, server);
                   
                    MySqlDataReader reader = sqlCommand.ExecuteReader();                   
                    while (reader.Read()) 
                    {
                        string id = reader.GetString("id");
                        decimal totalPay = reader.GetDecimal("total_pay");
                        
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("id", id);
                        
                      
                    }
                   

                }    
            
            } catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
           
        }
        public void AdminTable() 
        {
            try 
            {
                using (MySqlConnection adminConnect = new MySqlConnection(serverConnection))
                {
                    adminConnect.Open();
                    string adminQuery = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'employeedb' AND table_name = 'admintable';";
                    MySqlCommand adminExec = new MySqlCommand(adminQuery, adminConnect);
                    Object adminData = adminExec.ExecuteScalar();
                    if (adminData == null) 
                    {
                        string createRota = "CREATE TABLE admintable(id VARCHAR(7) REFERENCES employeedata(id), Admin_name VARCHAR(50) REFERENCES employeedata(fullname), Admin_contact VARCHAR(100) REFERENCES employeedata(email));";
                        MySqlCommand adminExc = new MySqlCommand(createRota, adminConnect);
                        adminExc.ExecuteNonQuery();
                        Console.WriteLine("Admin Table Created");
                    }
                }
                }catch(Exception e) { Console.WriteLine("Error:"+ e.Message); }
        }
        public void AdminInsert(string id, string name,string email) 
        {
            try 
            {
                using (MySqlConnection admin = new MySqlConnection(serverConnection)) 
                {
                    admin.Open();
                    string insertAdmin = """INSERT INTO admintable(id,Admin_name,Admin_contact)  VALUES(@id,@adminName,@adminEmail)""";
                    MySqlCommand adminExec = new MySqlCommand(insertAdmin, admin);

                    adminExec.Parameters.AddWithValue("@id", id);
                    adminExec.Parameters.AddWithValue("@adminName", name);
                    adminExec.Parameters.AddWithValue("@adminEmail",email);
                   
                    int affectedRow = adminExec.ExecuteNonQuery();
                }
            }catch (Exception e) { Console.WriteLine("Error: "+ e.Message); }

        }
        public void GetAdmininfo(string id) 
        {
            try 
            {
                using (MySqlConnection conn = new MySqlConnection(serverConnection)) 
                {
                conn.Open();
                    string admindetailQuery = "SELECT fullname,email FROM employeedata WHERE id = @id";
                    MySqlCommand detailQuery = new MySqlCommand(admindetailQuery,conn);

                    detailQuery.Parameters.Clear();
                    detailQuery.Parameters.AddWithValue("@id", id);

                    MySqlDataReader reader = detailQuery.ExecuteReader();
                    if (reader.Read()) 
                    {
                        string adminName = reader.GetString("fullname");
                        string adminEmail = reader.GetString("email");
                        AdminInsert(id, adminName, adminEmail);
                    }
                }

            }
            catch (Exception e) { Console.WriteLine("Error: "+e.Message); }
        }
        public void RotaTable() 
        {
            try
            {
                using(MySqlConnection toConnect = new MySqlConnection(serverConnection))
                {
                    toConnect.Open();
                    string rotaQuery = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'employeedb' AND table_name = 'rotatable';";
                    MySqlCommand rotaExec = new MySqlCommand(rotaQuery, toConnect);
                    Object data = rotaExec.ExecuteScalar();
                    if (data == null) 
                    {
                        string createRota = "CREATE TABLE rotatable(day_ofweek VARCHAR(9),start_work DATETIME, finish_work DATETIME, id VARCHAR(7) REFERENCES employeedata(id));";
                        MySqlCommand toExc = new MySqlCommand(createRota, toConnect);
                        toExc.ExecuteNonQuery();
                        Console.WriteLine("Rota table Created");
                    }
                } 
            
            }catch(Exception ex) { Console.WriteLine("Error: "+ex.Message); }
        }
        public void AddRota(DateTime startShift, DateTime endShift, string id,DateTime date) 
        {
            try
            {   
                string datetoUse = date.DayOfWeek.ToString();
                using (MySqlConnection Conectt = new MySqlConnection(serverConnection))
                {
                    Conectt.Open();
                    string insertquery = """INSERT INTO rotatable(day_ofweek ,start_work,finish_work,id)   VALUES (@dayofweek,@start,@finish,@id)""";

                    MySqlCommand execute = new MySqlCommand(insertquery, Conectt);

                    execute.Parameters.AddWithValue("@dayofweek", datetoUse);
                    execute.Parameters.AddWithValue("@start", startShift);
                    execute.Parameters.AddWithValue("@finish", endShift);
                    execute.Parameters.AddWithValue("@id", id);

                    int rowsAffected = execute.ExecuteNonQuery();
                    Console.WriteLine("rota added");
                }
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
        public void ViewRota() 
        {
            
            try 
            {
                using (MySqlConnection Connection = new MySqlConnection(serverConnection)) 
                {
                    Connection.Open();
                    string rotatableQuery = "SELECT id FROM rotatable;";
                    MySqlCommand rotaCmd = new MySqlCommand(rotatableQuery, Connection);

                    HashSet <string> obj = new HashSet<string>();
                    using (MySqlDataReader reader = rotaCmd.ExecuteReader()) 
                    {
                        while (reader.Read()) 
                        {
                            obj.Add (reader.GetString("id"));
                        }
                    }

                    string nameQuery = "SELECT fullname FROM employeedata WHERE id = @id;";
                    MySqlCommand commnd = new MySqlCommand(nameQuery, Connection);

                    string query = "SELECT start_work,finish_work,day_ofweek FROM rotatable WHERE id = @id2  ORDER BY start_work;";
                    MySqlCommand commndRota = new MySqlCommand(query, Connection);

                    foreach (string id in obj)
                    {
                        commnd.Parameters.Clear();
                        commnd.Parameters.AddWithValue("@id", id);
                        string queryName = "";

                        using (MySqlDataReader reader1 = commnd.ExecuteReader())
                        {
                            if (reader1.Read())
                            {
                                queryName = reader1.GetString("fullname");
                            }
                        }
                        if (String.IsNullOrEmpty(queryName))
                        {
                            Console.WriteLine("Employee not found");
                            continue;
                        }

                        commndRota.Parameters.Clear();
                        commndRota.Parameters.AddWithValue("@id2", id);
                        using (MySqlDataReader rotaFormat = commndRota.ExecuteReader())
                        {
                            Console.WriteLine("Rota for: " + queryName);
                            Console.WriteLine("------------------------------------------------------");

                            while (rotaFormat.Read())
                            {
                                DateTime shiftStart = rotaFormat.GetDateTime("start_work");
                                DateTime shiftEnd = rotaFormat.GetDateTime("finish_work");
                                string day = rotaFormat.GetString("day_ofweek");

                                 Console.WriteLine($"{shiftStart:g} -- {shiftEnd:g} -- {day}");
                            }
                            Console.WriteLine();
                        }
                    } 
                }
            } catch (Exception ex) { Console.WriteLine("Error: "+ex.Message); }
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

                using (MySqlConnection serverConection = new MySqlConnection(serverConnection))
                {
                    serverConection.Open();
                    string timeQuery = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'employeedb' AND table_name = 'employeepay';";
                    MySqlCommand payExec = new MySqlCommand(timeQuery, serverConection);
                    Object data = payExec.ExecuteScalar();
                    if (data == null)
                    {
                        string createPayroll = "CREATE TABLE employeepay(date_of_work DATE, total_pay DECIMAL(10,2), hours_done VARCHAR(100), id VARCHAR(7) REFERENCES employeedata(id));";
                        MySqlCommand toexc = new MySqlCommand(createPayroll, serverConection);
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
                using (MySqlConnection Conect = new MySqlConnection(serverConnection))
                {
                    Conect.Open();
                    string insertquery = """INSERT INTO employeepay(date_of_work,total_pay,hours_done,id)   VALUES (@date_of_work,@totalpay,@hours_done,@id)""";

                    MySqlCommand execute = new MySqlCommand(insertquery, Conect);

                    execute.Parameters.AddWithValue("@date_of_work", WorkDate);
                    execute.Parameters.AddWithValue("@totalpay", TotalPay);
                    execute.Parameters.AddWithValue("@hours_done", HoursDone);
                    execute.Parameters.AddWithValue("@id", Code);

                    int rowsAffected = execute.ExecuteNonQuery();
                    Console.WriteLine("Employee details added");
                }
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
        // Method to set up a connection to MySQL and setting up a connection and checking if the table exists or not
        public void FurtherProcess()
        {
            try 
            {
                
                using (MySqlConnection serverCon = new MySqlConnection(serverConnection))
                {
                    serverCon.Open();
                    string queryinfo = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'employeedb' AND table_name = 'employeedata'; ";
                    MySqlCommand toExecute = new MySqlCommand(queryinfo, serverCon);
                    Object data = toExecute.ExecuteScalar();
                    if (data.Equals(null))
                    {
                        string queryThree = "CREATE TABLE employeedata(id VARCHAR(7) PRIMARY KEY,fullname VARCHAR(100),age VARCHAR(50), phonenumber VARCHAR(50), email VARCHAR(100), hourlyrate VARCHAR(20));";
                        MySqlCommand toexecute = new MySqlCommand(queryThree, serverCon);
                        toexecute.ExecuteNonQuery();
                        Console.WriteLine("Database Created Successfuly");
                    }

                    string insertQuery = """INSERT INTO employeedata(id,fullname,age,phonenumber,email,hourlyrate)   VALUES (@id,@fullname,@age,@phonenumber,@email,@hourlyrate)""";

                    MySqlCommand execute = new MySqlCommand(insertQuery, serverCon);

                    execute.Parameters.AddWithValue("@id", EmployeeCode());
                    execute.Parameters.AddWithValue("@fullname", Name);
                    execute.Parameters.AddWithValue("@age", Age);
                    execute.Parameters.AddWithValue("@phonenumber", PhoneNumber);
                    execute.Parameters.AddWithValue("@email", Email);
                    execute.Parameters.AddWithValue("@hourlyrate", HourlyRate);

                    int rowsAffected = execute.ExecuteNonQuery();
                    Console.WriteLine("Employee added");
                }
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }


        }
        public void CheckAdmin(string adminCode) 
        {
            try
            {
                using (MySqlConnection serverConnect = new MySqlConnection(serverConnection))
                {
                    serverConnect.Open();
                    String querytoCheck = "SELECT id FROM admintable WHERE id = @id;";
                    MySqlCommand mySqlCommand = new MySqlCommand(querytoCheck, serverConnect);
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
        // Method to check if the employees unique code exists in the db
        public void Check(String codeToCheck)
        {
            try {
                using (MySqlConnection serverConnect = new MySqlConnection(serverConnection))
                {
                    serverConnect.Open();
                    String querytoCheck = "SELECT id FROM employeedata WHERE id = @id;";
                    MySqlCommand mySqlCommand = new MySqlCommand(querytoCheck, serverConnect);
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
                Program objEmp = new Program();
                Console.Write("Updated employee Name (leave blank to keep current): ");
                String employename = Console.ReadLine();
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
                Program obj = new Program();
                Console.Write("Enter employee Name: ");
                String empname = Console.ReadLine();
                obj.Name = empname;
                obj.DeleteEmp();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        // Method to take input from users to add employee to the employeedata db
        
        public static void Connect()
        {
            try
            {

                Program Obj = new Program();
                Console.Write("Enter employee Name: ");
                String name = Console.ReadLine();
                Obj.Name = name;

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
                Obj.FurtherProcess();


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
                program.TimeDone();
                program.EmployeeCode();
                program.RotaTable();
                program.AdminTable();
                 
                Console.WriteLine("1) Start/End work");
                Console.WriteLine("2) Management info");
                Console.Write("Enter your choice(1-2): ");
                string reading = Console.ReadLine().Trim();
                Console.WriteLine();
                switch (reading)
                {
                    case "1":
                        Console.Write("1) Start; 2) End; Enter '1' or '2': ");
                        string ans = Console.ReadLine().Trim();
                        if (ans.Equals("1"))
                        {
                            Console.Write("Enter Code: ");
                            String givenCode = Console.ReadLine();
                            program.Check(givenCode);

                            program.StartWatch();

                        }
                        else if (ans.Equals("2"))
                        {
                            Console.Write("Enter Code: ");
                            String givenCode = Console.ReadLine();
                            program.Check(givenCode);

                            program.StopWatch();
                            program.TableInsert();
                        }
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
                            Console.Write("Enter your choice(1-7): ");
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

                                    Console.Write("Enter Employee Code: ");
                                    string empuniqCode = Console.ReadLine().Trim();
                                    program.Check(empuniqCode); 
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
                                    program.AddRota(shiftStart,shiftEnd,empuniqCode,date);
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
                                Console.WriteLine("Employee was made admin");
                                break;
                                     default:
                                         Console.WriteLine("Invalid choice");
                                         break;
                                 
                            }
                        
                        
                        break;
                }

            }catch (Exception e) { Console.WriteLine("Error: "+e.Message); }
    }
    }
}

