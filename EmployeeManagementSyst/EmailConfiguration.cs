using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace EmployeeManagementSyst
{
  public class EmailConfiguration
    {
        private readonly Config config;
        private string attachment;
        private string code;

        public EmailConfiguration()
        {
            config = new Config();
        }


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

        /// <summary>
        /// Generates and sends payslip emails for employees based on the total pay from the last seven days.
        /// </summary>
        public void SendPaySlip()
        {
            try
            {
                using SqlConnection server = ServerConnection.GetOpenConnection();

                string qry = "SELECT EmployeeId AS id, SUM(TotalPay) AS total_pay FROM EmployeePayInfo WHERE DateOfWork BETWEEN @finished_date AND @current_date GROUP BY EmployeeId;";
                SqlCommand sqlCommand = new SqlCommand(qry, server);
                DateTime payDate = DateTime.Now;
                DateTime sevenDaysBefore = payDate.AddDays(-7); //Fetches all data from the SQL database corresponding to the last seven days.



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
                    string emailQuery = "SELECT Email, FullName FROM EmployeeDetails WHERE Id = @id;";
                    SqlCommand command = new SqlCommand(emailQuery, server);
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader2 = command.ExecuteReader())
                    {

                        if (reader2.Read())
                        {
                            string name = reader2.GetString(reader2.GetOrdinal("FullName"));
                            string email = reader2.GetString(reader2.GetOrdinal("Email"));

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

                server.Close();


            }
            catch (Exception e) { MessageBox.Show("Error Creating PaySlip: " + e.Message); }
        }

        /// <summary>
        /// Sends an email with the specified details and an optional attachment.
        /// </summary>
        /// <param name="emailAdd">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body content of the email.</param>
        public void SendEmail(string emailAdd, string subject, string body)
        {
            try
            {
                using (MailMessage mailMessage = new MailMessage())

                {

                    mailMessage.From = new MailAddress(config.EmailSender);
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
                        // Use explicit credentials and STARTTLS on port 587
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.Timeout = 20000; // 20s timeout
                        smtpClient.Credentials = new NetworkCredential(config.EmailSender, config.EmailPassword);
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
