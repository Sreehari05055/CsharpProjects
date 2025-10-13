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
    public partial class RotaVerification : Form
    {
        public RotaVerification()
        {
            InitializeComponent();

        } 
        
        // Event handler when 'Ok' is clicked
        private void Ok_Click(object sender, EventArgs e) 
        {
            SendRotaEmail sendRotaEmail = new SendRotaEmail();
            sendRotaEmail.CreateRota();
            this.Close();
        }     
    }
}
