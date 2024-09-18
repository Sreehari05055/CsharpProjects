﻿using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.NativeInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeManagementSyst
{
    public partial class PaySlip : Form
    {
        private string serverConnection;
        private string attachment;
        private string code;
       

        public string AttachMent
        {
            get { return attachment; }
            set { attachment = value; }
        }
        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        public PaySlip()
        {
            InitializeComponent();
            InitiateServer();
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
        public void SendPaySlip()
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
                        string emailQuery = "SELECT email, fullname FROM employeedetails WHERE id = @id;";
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
            catch (Exception e) { MessageBox.Show("Error Creating PaySlip: " + e.Message); }
        }
       
        public void SendEmail(string emailAdd, string subject, string body)
            {
                try
                {
                    using (MailMessage mailMessage = new MailMessage())

                    {

                        mailMessage.From = new MailAddress("From_email_add");
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
                            smtpClient.Credentials = new NetworkCredential("from_email_add", "app_specific_password");
                            smtpClient.EnableSsl = true;
                            smtpClient.Send(mailMessage);

                        }

                    }
                    MessageBox.Show("Email sent to: " + emailAdd);
                    if (!string.IsNullOrWhiteSpace(AttachMent))
                    {
                        File.Delete(AttachMent);
                    }

                }
                catch (SmtpException ex) { MessageBox.Show("SMTP Error: " + ex.Message); }
                catch (Exception e) { MessageBox.Show("Error Sending Email: " + e.Message); }
            }






        }
    }

