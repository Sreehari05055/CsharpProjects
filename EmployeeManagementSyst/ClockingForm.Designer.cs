namespace EmployeeManagementSyst
{
    partial class ClockingForm 
    {
        
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            button2 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.BackColor = Color.LightGray;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button1.Location = new Point(341, 347);
            button1.Margin = new Padding(4, 4, 4, 4);
            button1.Name = "button1";
            button1.Size = new Size(290, 81);
            button1.TabIndex = 0;
            button1.Text = "Start";
            button1.UseVisualStyleBackColor = false;
            button1.Click += Start_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.LightGray;
            button2.Cursor = Cursors.SizeWE;
            button2.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button2.Location = new Point(639, 347);
            button2.Margin = new Padding(4, 4, 4, 4);
            button2.Name = "button2";
            button2.Size = new Size(290, 81);
            button2.TabIndex = 1;
            button2.Text = "End";
            button2.UseVisualStyleBackColor = false;
            button2.Click += End_Click;
            // 
            // ClockingForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Teal;
            ClientSize = new Size(1290, 803);
            Controls.Add(button2);
            Controls.Add(button1);
            ForeColor = SystemColors.ActiveCaptionText;
            Location = new Point(511, 286);
            Margin = new Padding(4, 4, 4, 4);
            Name = "ClockingForm";
            Text = "Clock";
            Load += Form2_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button2;
    }
}