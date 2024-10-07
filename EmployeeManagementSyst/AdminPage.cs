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
            try
            {
                AddEmp addEmp = new AddEmp();
                addEmp.Show();
            }
            catch (Exception ex) { MessageBox.Show("Error Loading(Add Page): "+ex.Message); }
            
        }
        private void DeleteEmp_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteEmpGrid deleteEmpGrid = new DeleteEmpGrid();
            deleteEmpGrid.Show();
            }
            catch (Exception ex) { MessageBox.Show("Error Loading(Delete Page): " + ex.Message); }

        }
        private void UpdtEmp_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateEmp updateEmp = new UpdateEmp();
            updateEmp.Show();
            }
            catch (Exception ex) { MessageBox.Show("Error Loading(Update Page): " + ex.Message); }

        }
        private void AddRota_Click(object sender, EventArgs e)
        {
            try
            {
                AllEmployees allEmployees = new AllEmployees();
            allEmployees.Show();
            }
            catch (Exception ex) { MessageBox.Show("Error Loading(Add Rota Page): " + ex.Message); }
        }
        private void ViewRota_Click(object sender, EventArgs e)
        {
            try
            {
                ViewRota viewRota = new ViewRota();
            viewRota.Show();
            }
            catch (Exception ex) { MessageBox.Show("Error Loading(View Rota Page): " + ex.Message); }

        }
        private void RotaEmail_Click(object sender, EventArgs e)
        {
            try { 
            RotaVerification rotaVerification = new RotaVerification();
            rotaVerification.Show();
            }
            catch (Exception ex) { MessageBox.Show("Error Loading(Rota Email Page): " + ex.Message); }

        }
        private void SetAdmin_Click(object sender, EventArgs e)
        {
            try { 
            SetAdmin setAdmin = new SetAdmin();
            setAdmin.Show();
            }
            catch (Exception ex) { MessageBox.Show("Error Loading(Set Admin Page): " + ex.Message); }
        }
        private void RemoveAdmin_Click(object sender, EventArgs e)
        {
            try { 
            RmoveAdminGrid remAdmin = new RmoveAdminGrid();
            remAdmin.Show();
            }
            catch (Exception ex) { MessageBox.Show("Error Loading(Remove Admin Page): " + ex.Message); }

        }
        private void CheckStatus_Click(object sender, EventArgs e)
        {
            try { 
            CheckStatus checkStatus = new CheckStatus();
            checkStatus.Show();
            }
            catch (Exception ex) { MessageBox.Show("Error Loading(Status Check Page): " + ex.Message); }

        }
        private void GetEmpDetails_Click(object sender, EventArgs e)
        {
            try { 
            EmployeeDetailGrid employeeDetailGrid = new EmployeeDetailGrid();
            employeeDetailGrid.Show();
            }
            catch (Exception ex) { MessageBox.Show("Error Loading(Employee Detail Page): " + ex.Message); }

        }
        private void ViewEditPaySlip(object sender, EventArgs e)
        {
            try { 
            ViewEditPaySlip viewEditPaySlip = new ViewEditPaySlip();
            viewEditPaySlip.Show();
            }
            catch (Exception ex) { MessageBox.Show("Error Loading(View Edit Pay Page): " + ex.Message); }

        }
        private void SchedulePaySlip(object sender, EventArgs e)
        {
            try { 
            SchedulePaySlip schedulePaySlip = new SchedulePaySlip();
            schedulePaySlip.Show();
            }
            catch (Exception ex) { MessageBox.Show("Error Loading(Schedule PaySlip Page): " + ex.Message); }

        }

        private void WeeklySave(object sender, EventArgs e)
        {
            try { 
            WeeklySaveConfirm weeklySaveConfirm = new WeeklySaveConfirm();
            weeklySaveConfirm.Show();
            }
            catch (Exception ex) { MessageBox.Show("Error Loading(Rota Save Page): " + ex.Message); }
        }
   
    }
}
