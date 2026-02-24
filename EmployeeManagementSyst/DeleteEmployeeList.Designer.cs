namespace EmployeeManagementSyst
{
    partial class DeleteEmployeeList
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
            textBox1 = new TextBox();
            label1 = new Label();
            DeletEmployeeList = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView1.Location = new Point(122, 227);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(641, 336);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellClick += dataGridView1_CellClick;
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold);
            textBox1.Location = new Point(566, 171);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(197, 33);
            textBox1.TabIndex = 2;
            textBox1.TextChanged += Changing_Text;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold);
            label1.Location = new Point(122, 171);
            label1.Name = "label1";
            label1.Size = new Size(438, 27);
            label1.TabIndex = 3;
            label1.Text = "Enter Employee Code or Surname to Filter:";
            // 
            // DeletEmployeeList
            // 
            DeletEmployeeList.AutoSize = true;
            DeletEmployeeList.Font = new Font("Microsoft YaHei UI", 24F, FontStyle.Bold, GraphicsUnit.Point, 134);
            DeletEmployeeList.Location = new Point(245, 74);
            DeletEmployeeList.Name = "DeletEmployeeList";
            DeletEmployeeList.Size = new Size(315, 42);
            DeletEmployeeList.TabIndex = 4;
            DeletEmployeeList.Text = "DeletEmployeeList";
            // 
            // DeleteEmployeeList
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Teal;
            ClientSize = new Size(860, 597);
            Controls.Add(DeletEmployeeList);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(dataGridView1);
            Name = "DeleteEmployeeList";
            Text = "DeleteEmpGrid";
            Load += EmployeeDetailGrid_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private TextBox textBox1;
        private Label label1;
        private Label DeletEmployeeList;
    }
}