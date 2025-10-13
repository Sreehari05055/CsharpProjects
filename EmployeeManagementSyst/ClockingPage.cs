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

        }

        /// <summary>
        /// Event handler for loading the form.
        /// Currently not implemented but reserved for future use if any initialization is needed on load.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments for form load.</param>
        private void Form2_Load(object sender, EventArgs e)
        {
             
        }
        /// <summary>
        /// Event handler for the Start button click.
        /// Opens the Verification form for clock-in verification and closes the current form.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments for button click.</param>
        private void Start_Click(object sender, EventArgs e)
        {    
            Verification verification = new Verification();
            verification.Show();
            this.Close();
        }
        /// <summary>
        /// Event handler for the End button click.
        /// Opens the EndVerification form for clock-out verification and closes the current form.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments for button click.</param>
        private void End_Click(object sender, EventArgs e)
        {
            EndVerification vfication = new EndVerification();
            vfication.Show();
            this.Close();
        }
    }
}
