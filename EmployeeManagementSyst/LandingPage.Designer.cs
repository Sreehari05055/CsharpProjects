namespace EmployeeManagementSyst
{
    partial class LandingPage
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            button2 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ControlDark;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button1.Location = new Point(341, 363);
            button1.Margin = new Padding(4, 5, 4, 5);
            button1.Name = "button1";
            button1.Size = new Size(269, 81);
            button1.TabIndex = 0;
            button1.Text = "Start/End \r\n";
            button1.UseVisualStyleBackColor = false;
            button1.Click += StartEnd_Click;
            // 
            // button2
            // 
            button2.BackColor = SystemColors.ControlDark;
            button2.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button2.Location = new Point(618, 363);
            button2.Margin = new Padding(4, 5, 4, 5);
            button2.Name = "button2";
            button2.Size = new Size(290, 81);
            button2.TabIndex = 1;
            button2.Text = "Management Info";
            button2.UseVisualStyleBackColor = false;
            button2.Click += ManagementInfoClick;
            // 
            // LandingPage
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Teal;
            ClientSize = new Size(1290, 802);
            Controls.Add(button2);
            Controls.Add(button1);
            Margin = new Padding(4, 5, 4, 5);
            Name = "LandingPage";
            Text = "Home";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button2;
    }
}
