﻿namespace EmployeeManagementSyst
{
    partial class AdminPage
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
            SuspendLayout();
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button1.BackColor = SystemColors.ControlDark;
            button1.Location = new Point(173, 39);
            button1.Name = "button1";
            button1.Size = new Size(363, 42);
            button1.TabIndex = 0;
            button1.Text = "Add Employee";
            button1.UseVisualStyleBackColor = false;
            button1.Click += AddEmp_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button2.BackColor = SystemColors.ControlDark;
            button2.Location = new Point(173, 73);
            button2.Name = "button2";
            button2.Size = new Size(363, 42);
            button2.TabIndex = 1;
            button2.Text = " Delete Employee";
            button2.UseVisualStyleBackColor = false;
            button2.Click += DeleteEmp_Click;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button3.BackColor = SystemColors.ControlDark;
            button3.Location = new Point(173, 109);
            button3.Name = "button3";
            button3.Size = new Size(363, 42);
            button3.TabIndex = 2;
            button3.Text = " Update Employee Details";
            button3.UseVisualStyleBackColor = false;
            button3.Click += UpdtEmp_Click;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button4.BackColor = SystemColors.ControlDark;
            button4.Location = new Point(173, 145);
            button4.Name = "button4";
            button4.Size = new Size(363, 42);
            button4.TabIndex = 3;
            button4.Text = "Add Work Schedule";
            button4.UseVisualStyleBackColor = false;
            button4.Click += AddRota_Click;
            // 
            // button5
            // 
            button5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button5.BackColor = SystemColors.ControlDark;
            button5.Location = new Point(173, 181);
            button5.Name = "button5";
            button5.Size = new Size(363, 42);
            button5.TabIndex = 4;
            button5.Text = "View Work Schedule";
            button5.UseVisualStyleBackColor = false;
            button5.Click += ViewRota_Click;
            // 
            // button6
            // 
            button6.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button6.BackColor = SystemColors.ControlDark;
            button6.Location = new Point(173, 217);
            button6.Name = "button6";
            button6.Size = new Size(363, 42);
            button6.TabIndex = 5;
            button6.Text = "Send Work Schedule";
            button6.UseVisualStyleBackColor = false;
            button6.Click += RotaEmail_Click;
            // 
            // button7
            // 
            button7.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button7.BackColor = SystemColors.ControlDark;
            button7.Location = new Point(173, 253);
            button7.Name = "button7";
            button7.Size = new Size(363, 42);
            button7.TabIndex = 6;
            button7.Text = "Set Admin";
            button7.UseVisualStyleBackColor = false;
            button7.Click += SetAdmin_Click;
            // 
            // button8
            // 
            button8.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button8.BackColor = SystemColors.ControlDark;
            button8.Location = new Point(173, 289);
            button8.Name = "button8";
            button8.Size = new Size(363, 42);
            button8.TabIndex = 7;
            button8.Text = "Remove Admin";
            button8.UseVisualStyleBackColor = false;
            button8.Click += RemoveAdmin_Click;
            // 
            // button9
            // 
            button9.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button9.BackColor = SystemColors.ControlDark;
            button9.Location = new Point(173, 325);
            button9.Name = "button9";
            button9.Size = new Size(363, 42);
            button9.TabIndex = 8;
            button9.Text = "Check working employees";
            button9.UseVisualStyleBackColor = false;
            button9.Click += CheckStatus_Click;
            // 
            // button10
            // 
            button10.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button10.BackColor = SystemColors.ControlDark;
            button10.Location = new Point(173, 361);
            button10.Name = "button10";
            button10.Size = new Size(363, 42);
            button10.TabIndex = 9;
            button10.Text = "Get Employee Details";
            button10.UseVisualStyleBackColor = false;
            button10.Click += GetEmpDetails_Click;
            // 
            // AdminPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(734, 465);
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
            Name = "AdminPage";
            Text = "AdminPage";
            ResumeLayout(false);
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
    }
}