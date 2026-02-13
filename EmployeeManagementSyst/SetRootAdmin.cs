using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeManagementSyst
{
    public partial class SetRootAdmin : Form
    {
        public SetRootAdmin()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string fullName = textBox1.Text.Trim();
                string age = textBox2.Text.Trim();
                string phone = textBox8.Text.Trim();
                string email = textBox3.Text.Trim();
                string hourlyRate = textBox9.Text.Trim();
                string cardNumber = textBox4.Text.Trim();
                string cardExpiry = textBox5.Text.Trim();
                string cardCvv = textBox6.Text.Trim();
                string cardHolder = textBox7.Text.Trim();

                if (string.IsNullOrWhiteSpace(fullName))
                {
                    MessageBox.Show("Name is required");
                    return;
                }

                string surname = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Last();

                var manager = new EmployeeManager();
                var (newId, newPin) = manager.CreateEmployee(
                    fullName: fullName,
                    age: age,
                    phoneNumber: phone,
                    email: email,
                    hourlyRate: hourlyRate,
                    surname: surname,
                    userRole: "admin",
                    hireDate: DateTime.Today,
                    cardNumber: string.IsNullOrWhiteSpace(cardNumber) ? null : cardNumber,
                    expiryDate: string.IsNullOrWhiteSpace(cardExpiry) ? null : cardExpiry,
                    cvv: string.IsNullOrWhiteSpace(cardCvv) ? null : cardCvv,
                    holderName: string.IsNullOrWhiteSpace(cardHolder) ? null : cardHolder
                );

                if (!string.IsNullOrEmpty(newId))
                {
                    MessageBox.Show($"Root admin created. Id: {newId}");
                    if (!string.IsNullOrEmpty(newPin) && !string.IsNullOrWhiteSpace(email))
                    {
                        new EmailConfiguration().SendEmail(email, "Admin account created", $"Your admin account has been created. Clock PIN: {newPin}");
                    }
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating root admin: " + ex.Message);
            }
        }
    }
}
