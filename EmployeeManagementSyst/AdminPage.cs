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

        // Constructor for AdminPage, initializes the form and sets properties like background and layout
        public AdminPage()
        {
            InitializeComponent();
        }

        // Event handler to open the Add Employee form when the AddEmp button is clicked
        private void AddEmp_Click(object sender, EventArgs e)
        {
            OpenForm(new AddEmp(), "Add Employee Page");

        }
        // Event handler to open the Delete Employee form when clicked
        private void DeleteEmp_Click(object sender, EventArgs e)
        {     
            OpenForm(new DeleteEmpGrid(), "Delete Employee Page");     

        }
        // Event handler to open the Update Employee form when clicked
        private void UpdtEmp_Click(object sender, EventArgs e)
        {
            OpenForm(new UpdateEmp(), "Update Employee Page");         

        }
        // Event handler to open the Add Rota form when clicked
        private void AddRota_Click(object sender, EventArgs e)
        {

            OpenForm(new AllEmployees(), "Add Rota Page");
           
        }

        // Event handler to open the View Rota form when the 'View Rota' button is clicked
        private void ViewRota_Click(object sender, EventArgs e)
        {
            OpenForm(new ViewRota(), "View Rota Page");

        }
        // Event handler to open the form for rota email verification when the 'Send Work Schedule' button is clicked
        private void RotaEmail_Click(object sender, EventArgs e)
        {
            OpenForm(new RotaVerification(), "Send Work Schedule");
           
        }

        // Event handler to open the form for setting admin permissions when the 'Set Admin' button is clicked
        private void SetAdmin_Click(object sender, EventArgs e)
        {
            OpenForm(new SetAdminGrid(), "Set Admin Page");        
        }

        // Event handler to open the form for removing admin permissions when the 'Remove Admin' button is clicked
        private void RemoveAdmin_Click(object sender, EventArgs e)
        {
            OpenForm(new RmoveAdminGrid(), "Remove Admin Page");

        }

        // Event handler to open the form for checking employee status when the 'Check Status' button is clicked
        private void CheckStatus_Click(object sender, EventArgs e)
        {
            OpenForm(new CheckStatus(), "Status Check Page");
          
        }
        // Event handler to open the form for viewing employee details when the 'Get Employee Details' button is clicked
        private void GetEmpDetails_Click(object sender, EventArgs e)
        {   
            OpenForm(new EmployeeDetailGrid(), "Employee Detail Page");

        }
        // Event handler to open the form for viewing or editing payslips when the 'View/Edit PaySlip' button is clicked
        private void ViewEditPaySlip(object sender, EventArgs e)
        {   
            OpenForm(new ViewEditPaySlip(), "View/Edit PaySlip Page");         

        }
        // Event handler to open the form for scheduling payslips when the 'Schedule PaySlip' button is clicked
        private void SchedulePaySlip(object sender, EventArgs e)
        {
            OpenForm(new SchedulePaySlip(), "Schedule PaySlip Page");          

        }
        // Event handler to open the form for weekly rota save confirmation when the 'Save Weekly Schedule(Rota) Date' button is clicked
        private void WeeklySave(object sender, EventArgs e)
        {
            OpenForm(new WeeklySaveConfirm(), "Save Weekly Schedule Page");
        }
        // Helper method to open a new form and display an error message if it fails
        private void OpenForm(Form form, string formName)
        {
            try
            {
                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Loading({formName}): " + ex.Message);
            }
        }

    }
}
