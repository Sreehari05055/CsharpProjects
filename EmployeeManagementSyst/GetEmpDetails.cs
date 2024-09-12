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

namespace EmployeeManagementSyst
{
    public partial class GetEmpDetails : Form
    {
        private string serverConnection;
        public GetEmpDetails()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
        }
        
        private void Ok_Click(object sender, EventArgs e) 
        {
            string userInput = textBox1.Text;
            string inpAns = userInput.Trim().ToLower();
           EmployeeDetailGrid employeeDetailGrid = new EmployeeDetailGrid(inpAns);
            employeeDetailGrid.Show();
             
           this.Close();

        }
        private void Cancel_Click(object sender, EventArgs e) 
        {
            this.Close();
        }
    }
}
