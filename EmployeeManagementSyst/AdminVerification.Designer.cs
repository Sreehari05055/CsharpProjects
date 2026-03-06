namespace EmployeeManagementSyst
{
    partial class AdminVerification 
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
            textBox1 = new TextBox();
            button1 = new Button();
            button2 = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(382, 131);
            textBox1.Margin = new Padding(4);
            textBox1.MaxLength = 4;
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Enter 4-Digit Pin";
            textBox1.Size = new Size(178, 31);
            textBox1.TabIndex = 0;
            textBox1.UseSystemPasswordChar = true;
            textBox1.TextChanged += textBox1_TextChanged;
            textBox1.KeyPress += textBox1_KeyPress_1;
            // 
            // button1
            // 
            button1.BackColor = Color.LightGray;
            button1.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            button1.Location = new Point(149, 210);
            button1.Margin = new Padding(4);
            button1.Name = "button1";
            button1.Size = new Size(167, 82);
            button1.TabIndex = 2;
            button1.Text = "Ok";
            button1.UseVisualStyleBackColor = false;
            button1.Click += Ok_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.LightGray;
            button2.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            button2.Location = new Point(346, 210);
            button2.Margin = new Padding(4);
            button2.Name = "button2";
            button2.Size = new Size(178, 82);
            button2.TabIndex = 5;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label1.Location = new Point(88, 120);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(274, 42);
            label1.TabIndex = 4;
            label1.Text = "Enter Admin ID:";
            label1.Click += Label1_Click;
            // 
            // AdminVerification
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Teal;
            ClientSize = new Size(690, 379);
            Controls.Add(label1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Margin = new Padding(4);
            Name = "AdminVerification";
            Text = "Verification";
            Load += Form3_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Button button1;
        private Button button2;
        private Label label1;
    }
}