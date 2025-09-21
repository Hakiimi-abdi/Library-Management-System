namespace Library_System
{
    partial class Borrow
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
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            panel1 = new Panel();
            textBox1 = new TextBox();
            label5 = new Label();
            comboBox2 = new ComboBox();
            comboBox1 = new ComboBox();
            dataGridView1 = new DataGridView();
            label4 = new Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.BackColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.Location = new Point(55, 295);
            button1.Name = "button1";
            button1.Size = new Size(362, 48);
            button1.TabIndex = 0;
            button1.Text = "Issue Book";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.White;
            label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label1.Location = new Point(12, 96);
            label1.Name = "label1";
            label1.Size = new Size(97, 28);
            label1.TabIndex = 1;
            label1.Text = "Member:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.White;
            label2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label2.Location = new Point(12, 156);
            label2.Name = "label2";
            label2.Size = new Size(66, 28);
            label2.TabIndex = 3;
            label2.Text = "Book:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.White;
            label3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label3.Location = new Point(12, 215);
            label3.Name = "label3";
            label3.Size = new Size(97, 28);
            label3.TabIndex = 4;
            label3.Text = "Overdue:";
            // 
            // panel1
            // 
            panel1.Controls.Add(label3);
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(comboBox2);
            panel1.Controls.Add(comboBox1);
            panel1.Controls.Add(dataGridView1);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(label4);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1401, 598);
            panel1.TabIndex = 7;
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(1105, 27);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(275, 34);
            textBox1.TabIndex = 12;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.White;
            label5.BorderStyle = BorderStyle.Fixed3D;
            label5.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label5.Location = new Point(214, 215);
            label5.Name = "label5";
            label5.Size = new Size(2, 30);
            label5.TabIndex = 8;
            // 
            // comboBox2
            // 
            comboBox2.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(113, 156);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(333, 36);
            comboBox2.TabIndex = 11;
            // 
            // comboBox1
            // 
            comboBox1.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(113, 96);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(333, 36);
            comboBox1.TabIndex = 10;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(480, 67);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.Size = new Size(919, 453);
            dataGridView1.TabIndex = 9;
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.White;
            label4.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label4.Location = new Point(338, 9);
            label4.Name = "label4";
            label4.Size = new Size(135, 28);
            label4.TabIndex = 8;
            label4.Text = "Borrow Book";
            // 
            // Borrow
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1401, 598);
            Controls.Add(panel1);
            Name = "Borrow";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Borrow";
            Load += Borrow_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Panel panel1;
        private DataGridView dataGridView1;
        private Label label4;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private Label label5;
        private TextBox textBox1;
    }
}