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
    public partial class LoadingForm : Form
    {
        private bool _isNewDatabase;
        public LoadingForm()
        {
            InitializeComponent();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            label1.Text = e.UserState?.ToString();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var config = new Config();

            try
            {
                // Initialize server connection from config app connection
                ServerConnection.Initialize(config);

                var initializer = new TableInitialization(config);

                backgroundWorker1.ReportProgress(10, "Checking database status...");
                _isNewDatabase = initializer.CreateDatabaseAndTables();
                backgroundWorker1.ReportProgress(70, "Final checks...");
                backgroundWorker1.ReportProgress(99, "Final checks...");
                backgroundWorker1.ReportProgress(100, "Database and tables ready!");

                e.Result = _isNewDatabase;
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Exception ex)
            {
                MessageBox.Show("Initialization error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }
            
            this.Hide();
       
            new LandingPage().Show();
            

            // ensure modal dialog ends so Program.Main can continue
            this.DialogResult = DialogResult.OK;
        }

        private void LoadingForm_Load(object sender, EventArgs e)
        {
            // Start initialization when the form is first displayed
            if (!backgroundWorker1.IsBusy)
            {
                label1.Text = "Starting initialization...";
                backgroundWorker1.RunWorkerAsync();
            }
        }
    }
}
