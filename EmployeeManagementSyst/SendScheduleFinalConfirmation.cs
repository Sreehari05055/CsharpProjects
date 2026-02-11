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
    public partial class SendScheduleFinalConfirmation : Form
    {
        public SendScheduleFinalConfirmation()
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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
