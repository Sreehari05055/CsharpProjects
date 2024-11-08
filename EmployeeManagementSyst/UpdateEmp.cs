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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace EmployeeManagementSyst
{
    public partial class UpdateEmp : Form
    {
       
        private string surname;
        public String SurName
        {
            get { return surname; }
            set { surname = value; }
        }
        public UpdateEmp()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
        }

        /// <summary>
        /// Handles the 'OK' button click event to update employee details.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void Ok_Click(object sender, EventArgs e) 
        {
            string updtName = textBox1.Text;
            string updtAge = textBox2.Text;
            string updtNum = textBox3.Text;
            string updtEmail = textBox4.Text;   
            string updtRate = textBox5.Text;
            string updtCode = textBox6.Text;
            if (!string.IsNullOrEmpty(updtName))
            { 
                GetSurname(updtName);
            }
           
            UpdateEmpSet(updtCode,updtName,updtAge,updtNum,updtEmail,updtRate);
            this.Close();
        }

        /// <summary>
        /// Extracts the surname from the full name.
        /// </summary>
        /// <param name="name">The full name of the employee.</param>
        public void GetSurname(string name)
        {
            try
            {
                string[] aray = name.Split();
                string surname = aray[^1];
                SurName = surname;
            }
            catch (Exception ex) { MessageBox.Show("Error (Surname Comprehension): " + ex.Message); }
        }

        /// <summary>
        /// Updates employee details in the database.
        /// </summary>
        /// <param name="code">The unique employee code (ID).</param>
        /// <param name="name">The employee's full name.</param>
        /// <param name="age">The employee's age.</param>
        /// <param name="phoneNum">The employee's phone number.</param>
        /// <param name="emailAdd">The employee's email address.</param>
        /// <param name="rate">The employee's hourly rate.</param>
        public void UpdateEmpSet(string code,string name,string age,string phoneNum,string emailAdd, string rate)
        {
            try
            {
                using (SqlConnection servrCon = MainPage.ConnectionString())
                {
                   

                    using (SqlTransaction transaction = servrCon.BeginTransaction())
                    {
                        string updtquery = "UPDATE employeedetails SET ";
                        SqlCommand execte = new SqlCommand(null, servrCon, transaction);
                        execte.Transaction = transaction;
                        bool status = true;
                        if (!string.IsNullOrEmpty(name))
                        {
                            if (!status) updtquery += ", ";
                            updtquery += "fullname = @fullname";
                            execte.Parameters.AddWithValue("@fullname", name);
                            status = false;
                        }
                        if (!string.IsNullOrEmpty(age))
                        {
                            if (!status) updtquery += ", ";
                            updtquery += "age = @age";
                            execte.Parameters.AddWithValue("@age", age);
                            status = false;

                        }
                        if (!string.IsNullOrEmpty(phoneNum))
                        {
                            if (!status) updtquery += ", ";
                            updtquery += "phonenumber = @phonenumber";
                            execte.Parameters.AddWithValue("@phonenumber", phoneNum);
                            status = false;
                        }
                        if (!string.IsNullOrEmpty(emailAdd))
                        {
                            if (!status) updtquery += ", ";
                            updtquery += "email = @email";
                            execte.Parameters.AddWithValue("@email", emailAdd);
                            status = false;
                        }
                        if (!string.IsNullOrEmpty(rate))
                        {
                            if (!status) updtquery += ", ";

                            updtquery += "hourlyrate = @hourlyrate";
                            execte.Parameters.AddWithValue("@hourlyrate", rate);
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
                            execte.Parameters.AddWithValue("@id", code);
                            execte.CommandText = updtquery;
                            execte.ExecuteNonQuery();

                            if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(emailAdd))
                            {
                                string updateAdminTableQuery = "UPDATE admintable SET ";
                                SqlCommand updateAdminTableCmd = new SqlCommand(updateAdminTableQuery, servrCon, transaction);
                                bool adminHasUpdates = true;
                                if (!string.IsNullOrEmpty(name))
                                {
                                    updateAdminTableQuery += "Admin_name = @newFullname";
                                    updateAdminTableCmd.Parameters.AddWithValue("@newFullname", name);
                                    adminHasUpdates = false;
                                }
                                if (!string.IsNullOrEmpty(emailAdd))
                                {
                                    if (!adminHasUpdates) updateAdminTableQuery += ", ";
                                    updateAdminTableQuery += "Admin_contact = @email";
                                    updateAdminTableCmd.Parameters.AddWithValue("@email", emailAdd);
                                    adminHasUpdates = false;
                                }
                                if (!adminHasUpdates)
                                {
                                    updateAdminTableQuery += " WHERE id = @id";
                                    updateAdminTableCmd.Parameters.AddWithValue("@id", code);
                                    updateAdminTableCmd.CommandText = updateAdminTableQuery;
                                    updateAdminTableCmd.ExecuteNonQuery();
                                }

                            }
                            if (!string.IsNullOrEmpty(name))
                            {
                                string updateHoursTableQry = "UPDATE hourstable SET ";
                                SqlCommand updatehoursTableCmd = new SqlCommand(updateHoursTableQry, servrCon, transaction);
                                bool hoursHasUpdates = true;
                                if (!string.IsNullOrEmpty(name))
                                {
                                    updateHoursTableQry += "empname = @newFullname";
                                    updatehoursTableCmd.Parameters.AddWithValue("@newFullname", name);
                                    hoursHasUpdates = false;
                                }
                                if (!hoursHasUpdates)
                                {
                                    updateHoursTableQry += " WHERE id = @id";
                                    updatehoursTableCmd.Parameters.AddWithValue("@id", code);
                                    updatehoursTableCmd.CommandText = updateHoursTableQry;
                                    updatehoursTableCmd.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                                MessageBox.Show("Information updated successfully.");
                        }
                        else
                        {
                            MessageBox.Show("No updates were made as no changes were provided.");
                            transaction.Rollback();
                        }
                       
                    }

                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error Updating Employee Details: " + ex.Message);

            }
        }
    }
}
