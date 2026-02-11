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
    public partial class ShiftEndNotificationForm : Form
    {
        // Constructor for the EndShift form
        public ShiftEndNotificationForm()
        {
            InitializeComponent();

            DateTime endTime = DateTime.Now;
            string end = endTime.ToString("HH:mm");
            label1.Text = $"You have Clocked out at {end}";
        }
    }
}
