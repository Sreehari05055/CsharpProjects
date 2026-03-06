namespace EmployeeManagementSyst
{
    partial class ViewEditPaySlip
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
            dataGridView1 = new DataGridView();
            label1 = new Label();
            textBox1 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystroke;
            dataGridView1.Location = new Point(13, 124);
            dataGridView1.Margin = new Padding(4, 4, 4, 4);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.Size = new Size(1130, 605);
            dataGridView1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label1.Location = new Point(166, 76);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(655, 40);
            label1.TabIndex = 1;
            label1.Text = "Enter Employee Code or Surname to Filter:";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(829, 85);
            textBox1.Margin = new Padding(4, 4, 4, 4);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(314, 31);
            textBox1.TabIndex = 2;
            textBox1.TextChanged += Changing_Text;
            // 
            // ViewEditPaySlip
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Teal;
            ClientSize = new Size(1156, 742);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(dataGridView1);
            Margin = new Padding(4, 4, 4, 4);
            Name = "ViewEditPaySlip";
            Text = "ViewEditPaySlip";
            Load += PaySlipGrid_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Label label1;
        private TextBox textBox1;
    }
}