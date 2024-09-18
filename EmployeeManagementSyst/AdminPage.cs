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
    public partial class AdminPage : Form
    {
        public AdminPage()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackgroundImage = Image.FromFile("""C:\Users\sreek\Downloads\EmpMan.jpg""");
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void AddEmp_Click(object sender, EventArgs e)
        {
            AddEmp addEmp = new AddEmp();
            addEmp.Show();
            // this.Close();
        }
        private void DeleteEmp_Click(object sender, EventArgs e)
        {
            DeleteEmp delEmp = new DeleteEmp();
            delEmp.Show();
            // this.Close();
        }
        private void UpdtEmp_Click(object sender, EventArgs e)
        {
            UpdateEmp updateEmp = new UpdateEmp();
            updateEmp.Show();

        }
        private void AddRota_Click(object sender, EventArgs e)
        {
            AllEmployees allEmployees = new AllEmployees();
            allEmployees.Show();
            //AddRota addRota = new AddRota();
            //  addRota.Show();

        }
        private void ViewRota_Click(object sender, EventArgs e)
        {
            ViewRota viewRota = new ViewRota();
            viewRota.Show();

        }
        private void RotaEmail_Click(object sender, EventArgs e)
        {
            RotaVerification rotaVerification = new RotaVerification();
            rotaVerification.Show();

        }
        private void SetAdmin_Click(object sender, EventArgs e)
        {
            SetAdmin setAdmin = new SetAdmin();
            setAdmin.Show();

        }
        private void RemoveAdmin_Click(object sender, EventArgs e)
        {
            RemoveAdmin setAdmin = new RemoveAdmin();
            setAdmin.Show();

        }
        private void CheckStatus_Click(object sender, EventArgs e)
        {
            CheckStatus checkStatus = new CheckStatus();
            checkStatus.Show();

        }
        private void GetEmpDetails_Click(object sender, EventArgs e)
        {
            EmployeeDetailGrid employeeDetailGrid = new EmployeeDetailGrid();
            employeeDetailGrid.Show();

        }
        private void ViewEditPaySlip(object sender, EventArgs e)
        {
            ViewEditPaySlip viewEditPaySlip = new ViewEditPaySlip();
            viewEditPaySlip.Show();

        }
        private void SchedulePaySlip(object sender, EventArgs e)
        {
            SchedulePaySlip schedulePaySlip = new SchedulePaySlip();
            schedulePaySlip.Show();

        }

        private void button13_Click(object sender, EventArgs e)
        {
            WeeklySaveConfirm weeklySaveConfirm = new WeeklySaveConfirm();
            weeklySaveConfirm.Show();
        }
    }
}
