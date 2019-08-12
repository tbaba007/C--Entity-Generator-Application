namespace EntityGenerator
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbDatabaseAuthentication = new System.Windows.Forms.ComboBox();
            this.txtConnectionString = new System.Windows.Forms.TextBox();
            this.txtscriptoutput = new System.Windows.Forms.TextBox();
            this.cmboutput = new System.Windows.Forms.ComboBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.txtactivity = new System.Windows.Forms.TextBox();
            this.txtoutput = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtprojectname = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label7 = new System.Windows.Forms.Label();
            this.lblactivationstatus = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtEntitiesName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.ChkGenerateControllers = new System.Windows.Forms.CheckBox();
            this.ChkViews = new System.Windows.Forms.CheckBox();
            this.btnbrowsepath = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnclearDatabase = new System.Windows.Forms.Button();
            this.chkerrlog = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Database Authentication:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "ConnectionString";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 162);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Scripts Output Path";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 192);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Output Language";
            // 
            // cmbDatabaseAuthentication
            // 
            this.cmbDatabaseAuthentication.FormattingEnabled = true;
            this.cmbDatabaseAuthentication.Items.AddRange(new object[] {
            "True",
            "False"});
            this.cmbDatabaseAuthentication.Location = new System.Drawing.Point(143, 38);
            this.cmbDatabaseAuthentication.Name = "cmbDatabaseAuthentication";
            this.cmbDatabaseAuthentication.Size = new System.Drawing.Size(194, 21);
            this.cmbDatabaseAuthentication.TabIndex = 4;
            this.cmbDatabaseAuthentication.Text = "---Select---";
            this.cmbDatabaseAuthentication.SelectedIndexChanged += new System.EventHandler(this.cmbDatabaseAuthentication_SelectedIndexChanged);
            // 
            // txtConnectionString
            // 
            this.txtConnectionString.Location = new System.Drawing.Point(143, 97);
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.Size = new System.Drawing.Size(194, 20);
            this.txtConnectionString.TabIndex = 5;
            this.txtConnectionString.TextChanged += new System.EventHandler(this.txtConnectionString_TextChanged);
            // 
            // txtscriptoutput
            // 
            this.txtscriptoutput.Location = new System.Drawing.Point(143, 159);
            this.txtscriptoutput.Name = "txtscriptoutput";
            this.txtscriptoutput.ReadOnly = true;
            this.txtscriptoutput.Size = new System.Drawing.Size(194, 20);
            this.txtscriptoutput.TabIndex = 6;
            // 
            // cmboutput
            // 
            this.cmboutput.FormattingEnabled = true;
            this.cmboutput.Items.AddRange(new object[] {
            "Aspx",
            "Mvc",
            "Android"});
            this.cmboutput.Location = new System.Drawing.Point(143, 189);
            this.cmboutput.Name = "cmboutput";
            this.cmboutput.Size = new System.Drawing.Size(194, 21);
            this.cmboutput.TabIndex = 7;
            this.cmboutput.Text = "---Select---";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(429, 39);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(290, 154);
            this.checkedListBox1.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(423, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(296, 174);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Database List";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(302, 306);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(195, 58);
            this.btnGenerate.TabIndex = 10;
            this.btnGenerate.Text = "Generate Scripts From Database";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // txtactivity
            // 
            this.txtactivity.BackColor = System.Drawing.Color.Black;
            this.txtactivity.ForeColor = System.Drawing.Color.White;
            this.txtactivity.Location = new System.Drawing.Point(1, 260);
            this.txtactivity.Multiline = true;
            this.txtactivity.Name = "txtactivity";
            this.txtactivity.ReadOnly = true;
            this.txtactivity.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtactivity.Size = new System.Drawing.Size(295, 104);
            this.txtactivity.TabIndex = 11;
            // 
            // txtoutput
            // 
            this.txtoutput.Location = new System.Drawing.Point(559, 353);
            this.txtoutput.Multiline = true;
            this.txtoutput.Name = "txtoutput";
            this.txtoutput.Size = new System.Drawing.Size(162, 12);
            this.txtoutput.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Project/ Application Name\r\n";
            // 
            // txtprojectname
            // 
            this.txtprojectname.Location = new System.Drawing.Point(143, 66);
            this.txtprojectname.Name = "txtprojectname";
            this.txtprojectname.Size = new System.Drawing.Size(194, 20);
            this.txtprojectname.TabIndex = 14;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip1.Text = "About ";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(724, 24);
            this.menuStrip1.TabIndex = 17;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Buxton Sketch", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(554, 324);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 26);
            this.label7.TabIndex = 19;
            this.label7.Text = "label7";
            // 
            // lblactivationstatus
            // 
            this.lblactivationstatus.AutoSize = true;
            this.lblactivationstatus.Location = new System.Drawing.Point(556, 10);
            this.lblactivationstatus.Name = "lblactivationstatus";
            this.lblactivationstatus.Size = new System.Drawing.Size(0, 13);
            this.lblactivationstatus.TabIndex = 22;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 133);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(69, 13);
            this.label9.TabIndex = 23;
            this.label9.Text = "EntitiesName";
            // 
            // txtEntitiesName
            // 
            this.txtEntitiesName.Location = new System.Drawing.Point(143, 130);
            this.txtEntitiesName.Name = "txtEntitiesName";
            this.txtEntitiesName.Size = new System.Drawing.Size(194, 20);
            this.txtEntitiesName.TabIndex = 24;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 216);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(0, 13);
            this.label10.TabIndex = 25;
            // 
            // ChkGenerateControllers
            // 
            this.ChkGenerateControllers.AutoSize = true;
            this.ChkGenerateControllers.Location = new System.Drawing.Point(429, 234);
            this.ChkGenerateControllers.Name = "ChkGenerateControllers";
            this.ChkGenerateControllers.Size = new System.Drawing.Size(146, 17);
            this.ChkGenerateControllers.TabIndex = 26;
            this.ChkGenerateControllers.Text = "Generate Mvc Controllers";
            this.ChkGenerateControllers.UseVisualStyleBackColor = true;
            // 
            // ChkViews
            // 
            this.ChkViews.AutoSize = true;
            this.ChkViews.Location = new System.Drawing.Point(587, 234);
            this.ChkViews.Name = "ChkViews";
            this.ChkViews.Size = new System.Drawing.Size(125, 17);
            this.ChkViews.TabIndex = 27;
            this.ChkViews.Text = "Generate Mvc Views";
            this.ChkViews.UseVisualStyleBackColor = true;
            // 
            // btnbrowsepath
            // 
            this.btnbrowsepath.Location = new System.Drawing.Point(343, 157);
            this.btnbrowsepath.Name = "btnbrowsepath";
            this.btnbrowsepath.Size = new System.Drawing.Size(28, 23);
            this.btnbrowsepath.TabIndex = 28;
            this.btnbrowsepath.UseVisualStyleBackColor = true;
            this.btnbrowsepath.Click += new System.EventHandler(this.btnbrowsepath_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 228);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(148, 26);
            this.button1.TabIndex = 29;
            this.button1.Text = "Clear Generated Scripts";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnclearDatabase
            // 
            this.btnclearDatabase.Location = new System.Drawing.Point(495, 206);
            this.btnclearDatabase.Name = "btnclearDatabase";
            this.btnclearDatabase.Size = new System.Drawing.Size(148, 26);
            this.btnclearDatabase.TabIndex = 30;
            this.btnclearDatabase.Text = "Reset DB List";
            this.btnclearDatabase.UseVisualStyleBackColor = true;
            this.btnclearDatabase.Click += new System.EventHandler(this.btnclearDatabase_Click);
            // 
            // chkerrlog
            // 
            this.chkerrlog.AutoSize = true;
            this.chkerrlog.Location = new System.Drawing.Point(429, 262);
            this.chkerrlog.Name = "chkerrlog";
            this.chkerrlog.Size = new System.Drawing.Size(149, 17);
            this.chkerrlog.TabIndex = 31;
            this.chkerrlog.Text = "Generate Error Log(Table)";
            this.chkerrlog.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(724, 369);
            this.Controls.Add(this.chkerrlog);
            this.Controls.Add(this.btnclearDatabase);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnbrowsepath);
            this.Controls.Add(this.ChkViews);
            this.Controls.Add(this.ChkGenerateControllers);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtEntitiesName);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblactivationstatus);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.txtprojectname);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtoutput);
            this.Controls.Add(this.txtactivity);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.cmboutput);
            this.Controls.Add(this.txtscriptoutput);
            this.Controls.Add(this.txtConnectionString);
            this.Controls.Add(this.cmbDatabaseAuthentication);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EntityGenerator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbDatabaseAuthentication;
        private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.TextBox txtscriptoutput;
        private System.Windows.Forms.ComboBox cmboutput;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TextBox txtactivity;
        private System.Windows.Forms.TextBox txtoutput;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtprojectname;
        //private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        //private Microsoft.VisualBasic.PowerPacks.LineShape lineShape2;
        //private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblactivationstatus;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtEntitiesName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox ChkGenerateControllers;
        private System.Windows.Forms.CheckBox ChkViews;
        private System.Windows.Forms.Button btnbrowsepath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnclearDatabase;
        private System.Windows.Forms.CheckBox chkerrlog;
    }
}

