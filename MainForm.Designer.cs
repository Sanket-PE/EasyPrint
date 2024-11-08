namespace EasyPrint
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboboxPlotstyle = new System.Windows.Forms.ComboBox();
            this.comboboxPaper = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboboxPrinter = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxBlockName = new System.Windows.Forms.Label();
            this.All = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxOrientation = new System.Windows.Forms.ComboBox();
            this.textBoxCopies = new System.Windows.Forms.TextBox();
            this.Copies = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonPickBlock = new System.Windows.Forms.Button();
            this.radioButtonBlock = new System.Windows.Forms.RadioButton();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.buttonPreview = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonHow = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboboxPlotstyle);
            this.groupBox1.Controls.Add(this.comboboxPaper);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboboxPrinter);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(336, 108);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Setting";
            // 
            // comboboxPlotstyle
            // 
            this.comboboxPlotstyle.FormattingEnabled = true;
            this.comboboxPlotstyle.Location = new System.Drawing.Point(79, 73);
            this.comboboxPlotstyle.Name = "comboboxPlotstyle";
            this.comboboxPlotstyle.Size = new System.Drawing.Size(251, 21);
            this.comboboxPlotstyle.TabIndex = 5;
            // 
            // comboboxPaper
            // 
            this.comboboxPaper.FormattingEnabled = true;
            this.comboboxPaper.Location = new System.Drawing.Point(79, 47);
            this.comboboxPaper.Name = "comboboxPaper";
            this.comboboxPaper.Size = new System.Drawing.Size(251, 21);
            this.comboboxPaper.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Plot Style";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Paper Size";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Printer";
            // 
            // comboboxPrinter
            // 
            this.comboboxPrinter.FormattingEnabled = true;
            this.comboboxPrinter.Location = new System.Drawing.Point(79, 19);
            this.comboboxPrinter.Name = "comboboxPrinter";
            this.comboboxPrinter.Size = new System.Drawing.Size(251, 21);
            this.comboboxPrinter.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxBlockName);
            this.groupBox2.Controls.Add(this.All);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.comboBoxOrientation);
            this.groupBox2.Controls.Add(this.textBoxCopies);
            this.groupBox2.Controls.Add(this.Copies);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.buttonPickBlock);
            this.groupBox2.Controls.Add(this.radioButtonBlock);
            this.groupBox2.Location = new System.Drawing.Point(12, 121);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(336, 91);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Method";
            // 
            // textBoxBlockName
            // 
            this.textBoxBlockName.AutoSize = true;
            this.textBoxBlockName.Location = new System.Drawing.Point(149, 22);
            this.textBoxBlockName.Name = "textBoxBlockName";
            this.textBoxBlockName.Size = new System.Drawing.Size(16, 13);
            this.textBoxBlockName.TabIndex = 15;
            this.textBoxBlockName.Text = "...";
            // 
            // All
            // 
            this.All.AutoSize = true;
            this.All.Checked = true;
            this.All.CheckState = System.Windows.Forms.CheckState.Checked;
            this.All.Location = new System.Drawing.Point(271, 65);
            this.All.Name = "All";
            this.All.Size = new System.Drawing.Size(37, 17);
            this.All.TabIndex = 11;
            this.All.Text = "All";
            this.All.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(97, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Orientation";
            // 
            // comboBoxOrientation
            // 
            this.comboBoxOrientation.FormattingEnabled = true;
            this.comboBoxOrientation.Items.AddRange(new object[] {
            "Auto",
            "Portrait",
            "Landscape"});
            this.comboBoxOrientation.Location = new System.Drawing.Point(160, 63);
            this.comboBoxOrientation.Name = "comboBoxOrientation";
            this.comboBoxOrientation.Size = new System.Drawing.Size(97, 21);
            this.comboBoxOrientation.TabIndex = 7;
            // 
            // textBoxCopies
            // 
            this.textBoxCopies.Location = new System.Drawing.Point(52, 61);
            this.textBoxCopies.Name = "textBoxCopies";
            this.textBoxCopies.Size = new System.Drawing.Size(39, 20);
            this.textBoxCopies.TabIndex = 9;
            this.textBoxCopies.Text = "1";
            // 
            // Copies
            // 
            this.Copies.AutoSize = true;
            this.Copies.Location = new System.Drawing.Point(7, 62);
            this.Copies.Name = "Copies";
            this.Copies.Size = new System.Drawing.Size(39, 13);
            this.Copies.TabIndex = 8;
            this.Copies.Text = "Copies";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(108, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Name";
            // 
            // buttonPickBlock
            // 
            this.buttonPickBlock.Location = new System.Drawing.Point(271, 18);
            this.buttonPickBlock.Name = "buttonPickBlock";
            this.buttonPickBlock.Size = new System.Drawing.Size(59, 20);
            this.buttonPickBlock.TabIndex = 7;
            this.buttonPickBlock.Text = "Pick";
            this.buttonPickBlock.UseVisualStyleBackColor = true;
            // 
            // radioButtonBlock
            // 
            this.radioButtonBlock.AutoSize = true;
            this.radioButtonBlock.Checked = true;
            this.radioButtonBlock.Location = new System.Drawing.Point(7, 18);
            this.radioButtonBlock.Name = "radioButtonBlock";
            this.radioButtonBlock.Size = new System.Drawing.Size(52, 17);
            this.radioButtonBlock.TabIndex = 0;
            this.radioButtonBlock.Text = "Block";
            this.radioButtonBlock.UseVisualStyleBackColor = true;
            // 
            // buttonPrint
            // 
            this.buttonPrint.Location = new System.Drawing.Point(11, 218);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(66, 31);
            this.buttonPrint.TabIndex = 7;
            this.buttonPrint.Text = "Print";
            this.buttonPrint.UseVisualStyleBackColor = true;
            // 
            // buttonPreview
            // 
            this.buttonPreview.Location = new System.Drawing.Point(83, 218);
            this.buttonPreview.Name = "buttonPreview";
            this.buttonPreview.Size = new System.Drawing.Size(105, 31);
            this.buttonPreview.TabIndex = 8;
            this.buttonPreview.Text = "Preview";
            this.buttonPreview.UseVisualStyleBackColor = true;
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(194, 218);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(105, 31);
            this.buttonClose.TabIndex = 9;
            this.buttonClose.Text = "Cancel";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // buttonHow
            // 
            this.buttonHow.Location = new System.Drawing.Point(305, 218);
            this.buttonHow.Name = "buttonHow";
            this.buttonHow.Size = new System.Drawing.Size(36, 31);
            this.buttonHow.TabIndex = 10;
            this.buttonHow.Text = "?";
            this.buttonHow.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.ClientSize = new System.Drawing.Size(354, 255);
            this.Controls.Add(this.buttonHow);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonPreview);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EasyPrint";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboboxPrinter;
        private System.Windows.Forms.ComboBox comboboxPlotstyle;
        private System.Windows.Forms.ComboBox comboboxPaper;
        private System.Windows.Forms.RadioButton radioButtonBlock;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.Button buttonPreview;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonHow;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonPickBlock;
        private System.Windows.Forms.TextBox textBoxCopies;
        private System.Windows.Forms.Label Copies;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxOrientation;
        private System.Windows.Forms.CheckBox All;
        private System.Windows.Forms.Label textBoxBlockName;
    }
}