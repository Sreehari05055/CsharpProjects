namespace EmployeeManagementSyst
{
    partial class AdminForm
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
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            button7 = new Button();
            button8 = new Button();
            button9 = new Button();
            button10 = new Button();
            button11 = new Button();
            button12 = new Button();
            button13 = new Button();
            label1 = new Label();
            splitter1 = new Splitter();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button1.BackColor = Color.LightGray;
            button1.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            button1.Location = new Point(571, 115);
            button1.Margin = new Padding(4);
            button1.Name = "button1";
            button1.Size = new Size(277, 90);
            button1.TabIndex = 0;
            button1.Text = "Add Employee";
            button1.UseVisualStyleBackColor = false;
            button1.Click += AddEmp_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button2.BackColor = Color.LightGray;
            button2.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            button2.Location = new Point(571, 237);
            button2.Margin = new Padding(4);
            button2.Name = "button2";
            button2.Size = new Size(277, 90);
            button2.TabIndex = 1;
            button2.Text = "Terminate Employee";
            button2.UseVisualStyleBackColor = false;
            button2.Click += DeleteEmp_Click;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button3.BackColor = Color.LightGray;
            button3.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            button3.Location = new Point(571, 363);
            button3.Margin = new Padding(4);
            button3.Name = "button3";
            button3.Size = new Size(277, 90);
            button3.TabIndex = 2;
            button3.Text = " Update Employee Details";
            button3.UseVisualStyleBackColor = false;
            button3.Click += UpdtEmp_Click;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button4.BackColor = Color.LightGray;
            button4.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            button4.Location = new Point(571, 484);
            button4.Margin = new Padding(4);
            button4.Name = "button4";
            button4.Size = new Size(277, 90);
            button4.TabIndex = 3;
            button4.Text = "Add Work Schedule";
            button4.UseVisualStyleBackColor = false;
            button4.Click += AddRota_Click;
            // 
            // button5
            // 
            button5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button5.BackColor = Color.LightGray;
            button5.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            button5.Location = new Point(571, 603);
            button5.Margin = new Padding(4);
            button5.Name = "button5";
            button5.Size = new Size(277, 90);
            button5.TabIndex = 4;
            button5.Text = "View/Delete Work Schedule";
            button5.UseVisualStyleBackColor = false;
            button5.Click += ViewRota_Click;
            // 
            // button6
            // 
            button6.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button6.BackColor = Color.LightGray;
            button6.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            button6.Location = new Point(571, 726);
            button6.Margin = new Padding(4);
            button6.Name = "button6";
            button6.Size = new Size(277, 90);
            button6.TabIndex = 5;
            button6.Text = "Release Work Schedule";
            button6.UseVisualStyleBackColor = false;
            button6.Click += RotaEmail_Click;
            // 
            // button7
            // 
            button7.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button7.BackColor = Color.LightGray;
            button7.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            button7.Location = new Point(879, 115);
            button7.Margin = new Padding(4);
            button7.Name = "button7";
            button7.Size = new Size(277, 90);
            button7.TabIndex = 6;
            button7.Text = "Set Admin";
            button7.UseVisualStyleBackColor = false;
            button7.Click += SetAdmin_Click;
            // 
            // button8
            // 
            button8.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button8.BackColor = Color.LightGray;
            button8.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            button8.Location = new Point(879, 237);
            button8.Margin = new Padding(4);
            button8.Name = "button8";
            button8.Size = new Size(277, 90);
            button8.TabIndex = 7;
            button8.Text = "Revoke Admin Privilege";
            button8.UseVisualStyleBackColor = false;
            button8.Click += RemoveAdmin_Click;
            // 
            // button9
            // 
            button9.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button9.BackColor = Color.LightGray;
            button9.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            button9.Location = new Point(879, 363);
            button9.Margin = new Padding(4);
            button9.Name = "button9";
            button9.Size = new Size(277, 90);
            button9.TabIndex = 8;
            button9.Text = "Currently Working Employees";
            button9.UseVisualStyleBackColor = false;
            button9.Click += CheckStatus_Click;
            // 
            // button10
            // 
            button10.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button10.BackColor = Color.LightGray;
            button10.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            button10.Location = new Point(879, 484);
            button10.Margin = new Padding(4);
            button10.Name = "button10";
            button10.Size = new Size(277, 90);
            button10.TabIndex = 9;
            button10.Text = "Get Employee Details";
            button10.UseVisualStyleBackColor = false;
            button10.Click += GetEmpDetails_Click;
            // 
            // button11
            // 
            button11.BackColor = Color.LightGray;
            button11.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            button11.Location = new Point(879, 603);
            button11.Margin = new Padding(4);
            button11.Name = "button11";
            button11.Size = new Size(277, 90);
            button11.TabIndex = 10;
            button11.Text = "View/Edit PaySlip";
            button11.UseVisualStyleBackColor = false;
            button11.Click += ViewEditPaySlip;
            // 
            // button12
            // 
            button12.BackColor = Color.LightGray;
            button12.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            button12.Location = new Point(879, 726);
            button12.Margin = new Padding(4);
            button12.Name = "button12";
            button12.Size = new Size(277, 90);
            button12.TabIndex = 11;
            button12.Text = "Schedule PaySlip Release";
            button12.UseVisualStyleBackColor = false;
            button12.Click += SchedulePaySlip;
            // 
            // button13
            // 
            button13.BackColor = Color.LightGray;
            button13.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            button13.Location = new Point(571, 853);
            button13.Margin = new Padding(4);
            button13.Name = "button13";
            button13.Size = new Size(584, 91);
            button13.TabIndex = 12;
            button13.Text = "Weekly Schedule Save";
            button13.UseVisualStyleBackColor = false;
            button13.Click += WeeklySave;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.LightSeaGreen;
            label1.Font = new Font("Microsoft YaHei UI", 24F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label1.Location = new Point(47, 435);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(321, 64);
            label1.TabIndex = 13;
            label1.Text = "Admin Page";
            // 
            // splitter1
            // 
            splitter1.BackColor = Color.LightSeaGreen;
            splitter1.Location = new Point(0, 0);
            splitter1.Margin = new Padding(4);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(417, 1006);
            splitter1.TabIndex = 14;
            splitter1.TabStop = false;
            // 
            // AdminForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Teal;
            ClientSize = new Size(1227, 1006);
            Controls.Add(label1);
            Controls.Add(button13);
            Controls.Add(button12);
            Controls.Add(button11);
            Controls.Add(button10);
            Controls.Add(button9);
            Controls.Add(button8);
            Controls.Add(button7);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(splitter1);
            Margin = new Padding(4);
            Name = "AdminForm";
            Text = "AdminPage";
            Load += AdminForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Button button7;
        private Button button8;
        private Button button9;
        private Button button10;
        private Button button11;
        private Button button12;
        private Button button13;
        private Label label1;
        private Splitter splitter1;
    }
}