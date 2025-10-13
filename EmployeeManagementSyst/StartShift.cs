using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.NativeInterop;

namespace EmployeeManagementSyst
{
    public partial class StartShift : Form
    {
        private string serverConnection;

        public StartShift()
        {
            InitializeComponent();

            DateTime startTime = DateTime.Now;
            string start = startTime.ToString("HH:mm");           
            label1.Text = $"You have Clocked in at {start}";
           
        }
        

    }
}
