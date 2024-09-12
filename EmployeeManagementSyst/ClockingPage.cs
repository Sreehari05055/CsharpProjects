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
    public partial class ClockingPage : Form
    {
        public ClockingPage()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;


        }

        private void Form2_Load(object sender, EventArgs e)
        {
             
        }
        private void Start_Click(object sender, EventArgs e)
        {    
            Verification verification = new Verification();
            verification.Show();
            this.Close();
        }
        private void End_Click(object sender, EventArgs e)
        {
            EndVerification vfication = new EndVerification();
            vfication.Show();
            this.Close();
        }
    }
}
