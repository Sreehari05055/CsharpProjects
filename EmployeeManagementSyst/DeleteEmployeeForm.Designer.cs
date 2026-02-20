namespace EmployeeManagementSyst
{
    partial class DeleteEmployeeForm
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
            label1 = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.BackColor = Color.LightGray;
            button1.Location = new Point(176, 235);
            button1.Name = "button1";
            button1.Size = new Size(118, 59);
            button1.TabIndex = 0;
            button1.Text = "Yes";
            button1.UseVisualStyleBackColor = false;
            button1.Click += Yes_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.LightGray;
            button2.Location = new Point(373, 235);
            button2.Name = "button2";
            button2.Size = new Size(107, 59);
            button2.TabIndex = 1;
            button2.Text = "No";
            button2.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(176, 95);
            label1.Name = "label1";
            label1.Size = new Size(304, 25);
            label1.TabIndex = 2;
            label1.Text = "Are You Sure You Want to Delete";
            // 
            // DeleteEmployeeForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Teal;
            ClientSize = new Size(703, 350);
            Controls.Add(label1);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "DeleteEmployeeForm";
            Text = "DeleteEmp";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button button2;
        private Label label1;
    }
}