namespace DataManagerWindow
{
    partial class DataManager
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
            this.HelpButton = new System.Windows.Forms.Button();
            this.CancelBotton = new System.Windows.Forms.Button();
            this.OkBotton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Catalog_Number = new System.Windows.Forms.GroupBox();
            this.CatalogText6 = new System.Windows.Forms.TextBox();
            this.CatalogText5 = new System.Windows.Forms.TextBox();
            this.CatalogText4 = new System.Windows.Forms.TextBox();
            this.CatalogText3 = new System.Windows.Forms.TextBox();
            this.CatalogText2 = new System.Windows.Forms.TextBox();
            this.CatalogText1 = new System.Windows.Forms.TextBox();
            this.DSR = new System.Windows.Forms.GroupBox();
            this.DsrText6 = new System.Windows.Forms.TextBox();
            this.DsrText5 = new System.Windows.Forms.TextBox();
            this.DsrText4 = new System.Windows.Forms.TextBox();
            this.DsrText3 = new System.Windows.Forms.TextBox();
            this.DsrText2 = new System.Windows.Forms.TextBox();
            this.DsrText1 = new System.Windows.Forms.TextBox();
            this.Barcode = new System.Windows.Forms.GroupBox();
            this.BarcodeText6 = new System.Windows.Forms.TextBox();
            this.BarcodeText5 = new System.Windows.Forms.TextBox();
            this.BarcodeText4 = new System.Windows.Forms.TextBox();
            this.BarcodeText3 = new System.Windows.Forms.TextBox();
            this.BarcodeText2 = new System.Windows.Forms.TextBox();
            this.BarcodeText1 = new System.Windows.Forms.TextBox();
            this.Tag_Number = new System.Windows.Forms.GroupBox();
            this.SelectTag6 = new System.Windows.Forms.CheckBox();
            this.SelectTag5 = new System.Windows.Forms.CheckBox();
            this.SelectTag4 = new System.Windows.Forms.CheckBox();
            this.SelectTag3 = new System.Windows.Forms.CheckBox();
            this.SelectTag2 = new System.Windows.Forms.CheckBox();
            this.SelectTag1 = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.RecordSelectTagList = new System.Windows.Forms.GroupBox();
            this.rSelectTag6 = new System.Windows.Forms.RadioButton();
            this.rSelectTag5 = new System.Windows.Forms.RadioButton();
            this.rSelectTag4 = new System.Windows.Forms.RadioButton();
            this.rSelectTag3 = new System.Windows.Forms.RadioButton();
            this.rSelectTag2 = new System.Windows.Forms.RadioButton();
            this.rSelectTag1 = new System.Windows.Forms.RadioButton();
            this.BarCRC = new System.Windows.Forms.TextBox();
            this.ColumnList = new System.Windows.Forms.TextBox();
            this.RecordListCRC = new System.Windows.Forms.TextBox();
            this.RecordList = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.DataBytes = new System.Windows.Forms.Label();
            this.HexCountBar = new System.Windows.Forms.TextBox();
            this.SerialNoLabel = new System.Windows.Forms.Label();
            this.DisplayHexRadioButtonList = new System.Windows.Forms.GroupBox();
            this.sRecordTag6 = new System.Windows.Forms.RadioButton();
            this.sRecordTag5 = new System.Windows.Forms.RadioButton();
            this.sRecordTag4 = new System.Windows.Forms.RadioButton();
            this.sRecordTag3 = new System.Windows.Forms.RadioButton();
            this.sRecordTag2 = new System.Windows.Forms.RadioButton();
            this.sRecordTag1 = new System.Windows.Forms.RadioButton();
            this.HexDataTextBox = new System.Windows.Forms.TextBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.RefreshButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.Catalog_Number.SuspendLayout();
            this.DSR.SuspendLayout();
            this.Barcode.SuspendLayout();
            this.Tag_Number.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.RecordSelectTagList.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.DisplayHexRadioButtonList.SuspendLayout();
            this.SuspendLayout();
            // 
            // HelpButton
            // 
            this.HelpButton.Location = new System.Drawing.Point(450, 391);
            this.HelpButton.Name = "HelpButton";
            this.HelpButton.Size = new System.Drawing.Size(82, 32);
            this.HelpButton.TabIndex = 0;
            this.HelpButton.Text = "Help";
            this.HelpButton.UseVisualStyleBackColor = true;
            this.HelpButton.Click += new System.EventHandler(this.HelpBotton_Click);
            // 
            // CancelBotton
            // 
            this.CancelBotton.Location = new System.Drawing.Point(336, 391);
            this.CancelBotton.Name = "CancelBotton";
            this.CancelBotton.Size = new System.Drawing.Size(82, 32);
            this.CancelBotton.TabIndex = 0;
            this.CancelBotton.Text = "Cancel";
            this.CancelBotton.UseVisualStyleBackColor = true;
            this.CancelBotton.Click += new System.EventHandler(this.CancelBotton_Click);
            // 
            // OkBotton
            // 
            this.OkBotton.Location = new System.Drawing.Point(218, 391);
            this.OkBotton.Name = "OkBotton";
            this.OkBotton.Size = new System.Drawing.Size(82, 32);
            this.OkBotton.TabIndex = 0;
            this.OkBotton.Text = "OK";
            this.OkBotton.UseVisualStyleBackColor = true;
            this.OkBotton.Click += new System.EventHandler(this.OkBotton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(3, -1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(547, 383);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.progressBar1);
            this.tabPage1.Controls.Add(this.Catalog_Number);
            this.tabPage1.Controls.Add(this.DSR);
            this.tabPage1.Controls.Add(this.Barcode);
            this.tabPage1.Controls.Add(this.Tag_Number);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(539, 357);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Edit Data";
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click_1);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(11, 327);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(511, 15);
            this.progressBar1.TabIndex = 4;
            // 
            // Catalog_Number
            // 
            this.Catalog_Number.Controls.Add(this.CatalogText6);
            this.Catalog_Number.Controls.Add(this.CatalogText5);
            this.Catalog_Number.Controls.Add(this.CatalogText4);
            this.Catalog_Number.Controls.Add(this.CatalogText3);
            this.Catalog_Number.Controls.Add(this.CatalogText2);
            this.Catalog_Number.Controls.Add(this.CatalogText1);
            this.Catalog_Number.Location = new System.Drawing.Point(345, 16);
            this.Catalog_Number.Name = "Catalog_Number";
            this.Catalog_Number.Size = new System.Drawing.Size(177, 298);
            this.Catalog_Number.TabIndex = 3;
            this.Catalog_Number.TabStop = false;
            this.Catalog_Number.Text = "Catalog No.";
            // 
            // CatalogText6
            // 
            this.CatalogText6.Location = new System.Drawing.Point(6, 266);
            this.CatalogText6.Name = "CatalogText6";
            this.CatalogText6.Size = new System.Drawing.Size(165, 20);
            this.CatalogText6.TabIndex = 0;
            this.CatalogText6.TextChanged += new System.EventHandler(this.CatalogText6_TextChanged);
            // 
            // CatalogText5
            // 
            this.CatalogText5.Location = new System.Drawing.Point(6, 216);
            this.CatalogText5.Name = "CatalogText5";
            this.CatalogText5.Size = new System.Drawing.Size(165, 20);
            this.CatalogText5.TabIndex = 0;
            this.CatalogText5.TextChanged += new System.EventHandler(this.CatalogText5_TextChanged);
            // 
            // CatalogText4
            // 
            this.CatalogText4.Location = new System.Drawing.Point(6, 169);
            this.CatalogText4.Name = "CatalogText4";
            this.CatalogText4.Size = new System.Drawing.Size(165, 20);
            this.CatalogText4.TabIndex = 0;
            this.CatalogText4.TextChanged += new System.EventHandler(this.CatalogText4_TextChanged);
            // 
            // CatalogText3
            // 
            this.CatalogText3.Location = new System.Drawing.Point(6, 126);
            this.CatalogText3.Name = "CatalogText3";
            this.CatalogText3.Size = new System.Drawing.Size(165, 20);
            this.CatalogText3.TabIndex = 0;
            this.CatalogText3.TextChanged += new System.EventHandler(this.CatalogText3_TextChanged);
            // 
            // CatalogText2
            // 
            this.CatalogText2.Location = new System.Drawing.Point(6, 79);
            this.CatalogText2.Name = "CatalogText2";
            this.CatalogText2.Size = new System.Drawing.Size(165, 20);
            this.CatalogText2.TabIndex = 0;
            this.CatalogText2.TextChanged += new System.EventHandler(this.CatalogText2_TextChanged);
            // 
            // CatalogText1
            // 
            this.CatalogText1.Location = new System.Drawing.Point(6, 36);
            this.CatalogText1.Name = "CatalogText1";
            this.CatalogText1.Size = new System.Drawing.Size(165, 20);
            this.CatalogText1.TabIndex = 0;
            this.CatalogText1.TextChanged += new System.EventHandler(this.CatalogText1_TextChanged);
            // 
            // DSR
            // 
            this.DSR.Controls.Add(this.DsrText6);
            this.DSR.Controls.Add(this.DsrText5);
            this.DSR.Controls.Add(this.DsrText4);
            this.DSR.Controls.Add(this.DsrText3);
            this.DSR.Controls.Add(this.DsrText2);
            this.DSR.Controls.Add(this.DsrText1);
            this.DSR.Location = new System.Drawing.Point(233, 16);
            this.DSR.Name = "DSR";
            this.DSR.Size = new System.Drawing.Size(95, 298);
            this.DSR.TabIndex = 2;
            this.DSR.TabStop = false;
            this.DSR.Text = "D.S.R.";
            this.DSR.Enter += new System.EventHandler(this.DSR_Enter);
            // 
            // DsrText6
            // 
            this.DsrText6.Location = new System.Drawing.Point(6, 265);
            this.DsrText6.Name = "DsrText6";
            this.DsrText6.Size = new System.Drawing.Size(82, 20);
            this.DsrText6.TabIndex = 0;
            this.DsrText6.TextChanged += new System.EventHandler(this.DsrText6_TextChanged);
            // 
            // DsrText5
            // 
            this.DsrText5.Location = new System.Drawing.Point(6, 215);
            this.DsrText5.Name = "DsrText5";
            this.DsrText5.Size = new System.Drawing.Size(82, 20);
            this.DsrText5.TabIndex = 0;
            this.DsrText5.TextChanged += new System.EventHandler(this.DsrText5_TextChanged);
            // 
            // DsrText4
            // 
            this.DsrText4.Location = new System.Drawing.Point(6, 167);
            this.DsrText4.Name = "DsrText4";
            this.DsrText4.Size = new System.Drawing.Size(82, 20);
            this.DsrText4.TabIndex = 0;
            this.DsrText4.TextChanged += new System.EventHandler(this.DsrText4_TextChanged);
            // 
            // DsrText3
            // 
            this.DsrText3.Location = new System.Drawing.Point(6, 124);
            this.DsrText3.Name = "DsrText3";
            this.DsrText3.Size = new System.Drawing.Size(82, 20);
            this.DsrText3.TabIndex = 0;
            this.DsrText3.TextChanged += new System.EventHandler(this.DsrText3_TextChanged);
            // 
            // DsrText2
            // 
            this.DsrText2.Location = new System.Drawing.Point(6, 78);
            this.DsrText2.Name = "DsrText2";
            this.DsrText2.Size = new System.Drawing.Size(82, 20);
            this.DsrText2.TabIndex = 0;
            this.DsrText2.TextChanged += new System.EventHandler(this.DsrText2_TextChanged);
            // 
            // DsrText1
            // 
            this.DsrText1.Location = new System.Drawing.Point(6, 36);
            this.DsrText1.Name = "DsrText1";
            this.DsrText1.Size = new System.Drawing.Size(82, 20);
            this.DsrText1.TabIndex = 0;
            this.DsrText1.TextChanged += new System.EventHandler(this.DsrText1_TextChanged);
            // 
            // Barcode
            // 
            this.Barcode.Controls.Add(this.BarcodeText6);
            this.Barcode.Controls.Add(this.BarcodeText5);
            this.Barcode.Controls.Add(this.BarcodeText4);
            this.Barcode.Controls.Add(this.BarcodeText3);
            this.Barcode.Controls.Add(this.BarcodeText2);
            this.Barcode.Controls.Add(this.BarcodeText1);
            this.Barcode.Location = new System.Drawing.Point(107, 16);
            this.Barcode.Name = "Barcode";
            this.Barcode.Size = new System.Drawing.Size(111, 298);
            this.Barcode.TabIndex = 1;
            this.Barcode.TabStop = false;
            this.Barcode.Text = "Barcode";
            // 
            // BarcodeText6
            // 
            this.BarcodeText6.Location = new System.Drawing.Point(5, 264);
            this.BarcodeText6.Name = "BarcodeText6";
            this.BarcodeText6.Size = new System.Drawing.Size(100, 20);
            this.BarcodeText6.TabIndex = 0;
            this.BarcodeText6.TextChanged += new System.EventHandler(this.BarcodeText6_TextChanged);
            // 
            // BarcodeText5
            // 
            this.BarcodeText5.Location = new System.Drawing.Point(5, 214);
            this.BarcodeText5.Name = "BarcodeText5";
            this.BarcodeText5.Size = new System.Drawing.Size(100, 20);
            this.BarcodeText5.TabIndex = 0;
            this.BarcodeText5.TextChanged += new System.EventHandler(this.BarcodeText5_TextChanged);
            // 
            // BarcodeText4
            // 
            this.BarcodeText4.Location = new System.Drawing.Point(5, 167);
            this.BarcodeText4.Name = "BarcodeText4";
            this.BarcodeText4.Size = new System.Drawing.Size(100, 20);
            this.BarcodeText4.TabIndex = 0;
            this.BarcodeText4.TextChanged += new System.EventHandler(this.BarcodeText4_TextChanged);
            // 
            // BarcodeText3
            // 
            this.BarcodeText3.Location = new System.Drawing.Point(5, 124);
            this.BarcodeText3.Name = "BarcodeText3";
            this.BarcodeText3.Size = new System.Drawing.Size(100, 20);
            this.BarcodeText3.TabIndex = 0;
            this.BarcodeText3.TextChanged += new System.EventHandler(this.BarcodeText3_TextChanged);
            // 
            // BarcodeText2
            // 
            this.BarcodeText2.Location = new System.Drawing.Point(5, 78);
            this.BarcodeText2.Name = "BarcodeText2";
            this.BarcodeText2.Size = new System.Drawing.Size(100, 20);
            this.BarcodeText2.TabIndex = 0;
            this.BarcodeText2.TextChanged += new System.EventHandler(this.BarcodeText2_TextChanged);
            // 
            // BarcodeText1
            // 
            this.BarcodeText1.Location = new System.Drawing.Point(5, 36);
            this.BarcodeText1.Name = "BarcodeText1";
            this.BarcodeText1.Size = new System.Drawing.Size(100, 20);
            this.BarcodeText1.TabIndex = 0;
            this.BarcodeText1.TextChanged += new System.EventHandler(this.BarcodeText1_TextChanged);
            // 
            // Tag_Number
            // 
            this.Tag_Number.Controls.Add(this.SelectTag6);
            this.Tag_Number.Controls.Add(this.SelectTag5);
            this.Tag_Number.Controls.Add(this.SelectTag4);
            this.Tag_Number.Controls.Add(this.SelectTag3);
            this.Tag_Number.Controls.Add(this.SelectTag2);
            this.Tag_Number.Controls.Add(this.SelectTag1);
            this.Tag_Number.Controls.Add(this.label6);
            this.Tag_Number.Controls.Add(this.label5);
            this.Tag_Number.Controls.Add(this.label4);
            this.Tag_Number.Controls.Add(this.label3);
            this.Tag_Number.Controls.Add(this.label2);
            this.Tag_Number.Controls.Add(this.label1);
            this.Tag_Number.Location = new System.Drawing.Point(11, 16);
            this.Tag_Number.Name = "Tag_Number";
            this.Tag_Number.Size = new System.Drawing.Size(81, 298);
            this.Tag_Number.TabIndex = 0;
            this.Tag_Number.TabStop = false;
            this.Tag_Number.Text = "Tag Number";
            this.Tag_Number.Enter += new System.EventHandler(this.Tag_Number_Enter);
            // 
            // SelectTag6
            // 
            this.SelectTag6.AutoSize = true;
            this.SelectTag6.Location = new System.Drawing.Point(41, 263);
            this.SelectTag6.Name = "SelectTag6";
            this.SelectTag6.Size = new System.Drawing.Size(15, 14);
            this.SelectTag6.TabIndex = 5;
            this.SelectTag6.UseVisualStyleBackColor = true;
            this.SelectTag6.CheckedChanged += new System.EventHandler(this.SelectTag6_CheckedChanged);
            // 
            // SelectTag5
            // 
            this.SelectTag5.AutoSize = true;
            this.SelectTag5.Location = new System.Drawing.Point(41, 220);
            this.SelectTag5.Name = "SelectTag5";
            this.SelectTag5.Size = new System.Drawing.Size(15, 14);
            this.SelectTag5.TabIndex = 5;
            this.SelectTag5.UseVisualStyleBackColor = true;
            this.SelectTag5.CheckedChanged += new System.EventHandler(this.SelectTag5_CheckedChanged);
            // 
            // SelectTag4
            // 
            this.SelectTag4.AutoSize = true;
            this.SelectTag4.Location = new System.Drawing.Point(41, 174);
            this.SelectTag4.Name = "SelectTag4";
            this.SelectTag4.Size = new System.Drawing.Size(15, 14);
            this.SelectTag4.TabIndex = 5;
            this.SelectTag4.UseVisualStyleBackColor = true;
            this.SelectTag4.CheckedChanged += new System.EventHandler(this.SelectTag4_CheckedChanged);
            // 
            // SelectTag3
            // 
            this.SelectTag3.AutoSize = true;
            this.SelectTag3.Location = new System.Drawing.Point(41, 131);
            this.SelectTag3.Name = "SelectTag3";
            this.SelectTag3.Size = new System.Drawing.Size(15, 14);
            this.SelectTag3.TabIndex = 5;
            this.SelectTag3.UseVisualStyleBackColor = true;
            this.SelectTag3.CheckedChanged += new System.EventHandler(this.SelectTag3_CheckedChanged);
            // 
            // SelectTag2
            // 
            this.SelectTag2.AutoSize = true;
            this.SelectTag2.Location = new System.Drawing.Point(41, 85);
            this.SelectTag2.Name = "SelectTag2";
            this.SelectTag2.Size = new System.Drawing.Size(15, 14);
            this.SelectTag2.TabIndex = 5;
            this.SelectTag2.UseVisualStyleBackColor = true;
            this.SelectTag2.CheckedChanged += new System.EventHandler(this.SelectTag2_CheckedChanged);
            // 
            // SelectTag1
            // 
            this.SelectTag1.AutoSize = true;
            this.SelectTag1.Location = new System.Drawing.Point(41, 42);
            this.SelectTag1.Name = "SelectTag1";
            this.SelectTag1.Size = new System.Drawing.Size(15, 14);
            this.SelectTag1.TabIndex = 5;
            this.SelectTag1.UseVisualStyleBackColor = true;
            this.SelectTag1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 261);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(13, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "6";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 218);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 171);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "4";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "3";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "1";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.Controls.Add(this.RecordSelectTagList);
            this.tabPage2.Controls.Add(this.BarCRC);
            this.tabPage2.Controls.Add(this.ColumnList);
            this.tabPage2.Controls.Add(this.RecordListCRC);
            this.tabPage2.Controls.Add(this.RecordList);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(539, 357);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Record List";
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click_1);
            // 
            // RecordSelectTagList
            // 
            this.RecordSelectTagList.Controls.Add(this.rSelectTag6);
            this.RecordSelectTagList.Controls.Add(this.rSelectTag5);
            this.RecordSelectTagList.Controls.Add(this.rSelectTag4);
            this.RecordSelectTagList.Controls.Add(this.rSelectTag3);
            this.RecordSelectTagList.Controls.Add(this.rSelectTag2);
            this.RecordSelectTagList.Controls.Add(this.rSelectTag1);
            this.RecordSelectTagList.Location = new System.Drawing.Point(8, 31);
            this.RecordSelectTagList.Name = "RecordSelectTagList";
            this.RecordSelectTagList.Size = new System.Drawing.Size(71, 317);
            this.RecordSelectTagList.TabIndex = 6;
            this.RecordSelectTagList.TabStop = false;
            this.RecordSelectTagList.Text = "Select Tag";
            this.RecordSelectTagList.Enter += new System.EventHandler(this.RecordSelectTagList_Enter);
            // 
            // rSelectTag6
            // 
            this.rSelectTag6.Location = new System.Drawing.Point(9, 257);
            this.rSelectTag6.Name = "rSelectTag6";
            this.rSelectTag6.Size = new System.Drawing.Size(56, 24);
            this.rSelectTag6.TabIndex = 0;
            this.rSelectTag6.Text = "Tag 6";
            this.rSelectTag6.CheckedChanged += new System.EventHandler(this.rSelectTag6_CheckedChanged);
            // 
            // rSelectTag5
            // 
            this.rSelectTag5.AutoSize = true;
            this.rSelectTag5.Location = new System.Drawing.Point(9, 219);
            this.rSelectTag5.Name = "rSelectTag5";
            this.rSelectTag5.Size = new System.Drawing.Size(53, 17);
            this.rSelectTag5.TabIndex = 0;
            this.rSelectTag5.TabStop = true;
            this.rSelectTag5.Text = "Tag 5";
            this.rSelectTag5.UseVisualStyleBackColor = true;
            this.rSelectTag5.CheckedChanged += new System.EventHandler(this.rSelectTag5_CheckedChanged_1);
            // 
            // rSelectTag4
            // 
            this.rSelectTag4.AutoSize = true;
            this.rSelectTag4.Location = new System.Drawing.Point(9, 175);
            this.rSelectTag4.Name = "rSelectTag4";
            this.rSelectTag4.Size = new System.Drawing.Size(53, 17);
            this.rSelectTag4.TabIndex = 0;
            this.rSelectTag4.TabStop = true;
            this.rSelectTag4.Text = "Tag 4";
            this.rSelectTag4.UseVisualStyleBackColor = true;
            this.rSelectTag4.CheckedChanged += new System.EventHandler(this.rSelectTag4_CheckedChanged_1);
            // 
            // rSelectTag3
            // 
            this.rSelectTag3.AutoSize = true;
            this.rSelectTag3.Location = new System.Drawing.Point(9, 135);
            this.rSelectTag3.Name = "rSelectTag3";
            this.rSelectTag3.Size = new System.Drawing.Size(53, 17);
            this.rSelectTag3.TabIndex = 0;
            this.rSelectTag3.TabStop = true;
            this.rSelectTag3.Text = "Tag 3";
            this.rSelectTag3.UseVisualStyleBackColor = true;
            this.rSelectTag3.CheckedChanged += new System.EventHandler(this.rSelectTag3_CheckedChanged_1);
            // 
            // rSelectTag2
            // 
            this.rSelectTag2.AutoSize = true;
            this.rSelectTag2.Location = new System.Drawing.Point(9, 95);
            this.rSelectTag2.Name = "rSelectTag2";
            this.rSelectTag2.Size = new System.Drawing.Size(53, 17);
            this.rSelectTag2.TabIndex = 0;
            this.rSelectTag2.TabStop = true;
            this.rSelectTag2.Text = "Tag 2";
            this.rSelectTag2.UseVisualStyleBackColor = true;
            this.rSelectTag2.CheckedChanged += new System.EventHandler(this.radioButton5_CheckedChanged);
            // 
            // rSelectTag1
            // 
            this.rSelectTag1.AutoSize = true;
            this.rSelectTag1.Location = new System.Drawing.Point(9, 53);
            this.rSelectTag1.Name = "rSelectTag1";
            this.rSelectTag1.Size = new System.Drawing.Size(53, 17);
            this.rSelectTag1.TabIndex = 0;
            this.rSelectTag1.TabStop = true;
            this.rSelectTag1.Text = "Tag 1";
            this.rSelectTag1.UseVisualStyleBackColor = true;
            this.rSelectTag1.CheckedChanged += new System.EventHandler(this.rSelectTag1_CheckedChanged_1);
            // 
            // BarCRC
            // 
            this.BarCRC.Location = new System.Drawing.Point(462, 31);
            this.BarCRC.Name = "BarCRC";
            this.BarCRC.Size = new System.Drawing.Size(60, 20);
            this.BarCRC.TabIndex = 1;
            this.BarCRC.TextChanged += new System.EventHandler(this.BarCRC_TextChanged);
            // 
            // ColumnList
            // 
            this.ColumnList.Location = new System.Drawing.Point(94, 31);
            this.ColumnList.Name = "ColumnList";
            this.ColumnList.Size = new System.Drawing.Size(428, 20);
            this.ColumnList.TabIndex = 1;
            // 
            // RecordListCRC
            // 
            this.RecordListCRC.Location = new System.Drawing.Point(462, 53);
            this.RecordListCRC.Multiline = true;
            this.RecordListCRC.Name = "RecordListCRC";
            this.RecordListCRC.Size = new System.Drawing.Size(60, 295);
            this.RecordListCRC.TabIndex = 0;
            this.RecordListCRC.TextChanged += new System.EventHandler(this.RecordList_TextChanged);
            // 
            // RecordList
            // 
            this.RecordList.Location = new System.Drawing.Point(94, 53);
            this.RecordList.Multiline = true;
            this.RecordList.Name = "RecordList";
            this.RecordList.Size = new System.Drawing.Size(428, 295);
            this.RecordList.TabIndex = 0;
            this.RecordList.TextChanged += new System.EventHandler(this.RecordList_TextChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.Transparent;
            this.tabPage3.Controls.Add(this.DataBytes);
            this.tabPage3.Controls.Add(this.HexCountBar);
            this.tabPage3.Controls.Add(this.SerialNoLabel);
            this.tabPage3.Controls.Add(this.DisplayHexRadioButtonList);
            this.tabPage3.Controls.Add(this.HexDataTextBox);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(539, 357);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Hex Data";
            this.tabPage3.Click += new System.EventHandler(this.tabPage3_Click);
            // 
            // DataBytes
            // 
            this.DataBytes.AutoSize = true;
            this.DataBytes.Location = new System.Drawing.Point(303, 15);
            this.DataBytes.Name = "DataBytes";
            this.DataBytes.Size = new System.Drawing.Size(33, 13);
            this.DataBytes.TabIndex = 8;
            this.DataBytes.Text = "Bytes";
            this.DataBytes.Click += new System.EventHandler(this.DataBytes_Click);
            // 
            // HexCountBar
            // 
            this.HexCountBar.Location = new System.Drawing.Point(93, 41);
            this.HexCountBar.Name = "HexCountBar";
            this.HexCountBar.Size = new System.Drawing.Size(429, 20);
            this.HexCountBar.TabIndex = 7;
            // 
            // SerialNoLabel
            // 
            this.SerialNoLabel.AutoSize = true;
            this.SerialNoLabel.Location = new System.Drawing.Point(90, 16);
            this.SerialNoLabel.Name = "SerialNoLabel";
            this.SerialNoLabel.Size = new System.Drawing.Size(50, 13);
            this.SerialNoLabel.TabIndex = 6;
            this.SerialNoLabel.Text = "Serial No";
            this.SerialNoLabel.Click += new System.EventHandler(this.SerialNoLabel_Click);
            // 
            // DisplayHexRadioButtonList
            // 
            this.DisplayHexRadioButtonList.Controls.Add(this.sRecordTag6);
            this.DisplayHexRadioButtonList.Controls.Add(this.sRecordTag5);
            this.DisplayHexRadioButtonList.Controls.Add(this.sRecordTag4);
            this.DisplayHexRadioButtonList.Controls.Add(this.sRecordTag3);
            this.DisplayHexRadioButtonList.Controls.Add(this.sRecordTag2);
            this.DisplayHexRadioButtonList.Controls.Add(this.sRecordTag1);
            this.DisplayHexRadioButtonList.Location = new System.Drawing.Point(8, 41);
            this.DisplayHexRadioButtonList.Name = "DisplayHexRadioButtonList";
            this.DisplayHexRadioButtonList.Size = new System.Drawing.Size(69, 307);
            this.DisplayHexRadioButtonList.TabIndex = 5;
            this.DisplayHexRadioButtonList.TabStop = false;
            this.DisplayHexRadioButtonList.Text = "Select Tag";
            this.DisplayHexRadioButtonList.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // sRecordTag6
            // 
            this.sRecordTag6.AutoSize = true;
            this.sRecordTag6.Location = new System.Drawing.Point(9, 250);
            this.sRecordTag6.Name = "sRecordTag6";
            this.sRecordTag6.Size = new System.Drawing.Size(53, 17);
            this.sRecordTag6.TabIndex = 1;
            this.sRecordTag6.TabStop = true;
            this.sRecordTag6.Text = "Tag 6";
            this.sRecordTag6.UseVisualStyleBackColor = true;
            this.sRecordTag6.CheckedChanged += new System.EventHandler(this.sRecordTag6_CheckedChanged_1);
            // 
            // sRecordTag5
            // 
            this.sRecordTag5.AutoSize = true;
            this.sRecordTag5.Location = new System.Drawing.Point(9, 209);
            this.sRecordTag5.Name = "sRecordTag5";
            this.sRecordTag5.Size = new System.Drawing.Size(53, 17);
            this.sRecordTag5.TabIndex = 0;
            this.sRecordTag5.TabStop = true;
            this.sRecordTag5.Text = "Tag 5";
            this.sRecordTag5.UseVisualStyleBackColor = true;
            this.sRecordTag5.CheckedChanged += new System.EventHandler(this.rSelectTag5_CheckedChanged);
            // 
            // sRecordTag4
            // 
            this.sRecordTag4.AutoSize = true;
            this.sRecordTag4.Location = new System.Drawing.Point(9, 165);
            this.sRecordTag4.Name = "sRecordTag4";
            this.sRecordTag4.Size = new System.Drawing.Size(53, 17);
            this.sRecordTag4.TabIndex = 0;
            this.sRecordTag4.TabStop = true;
            this.sRecordTag4.Text = "Tag 4";
            this.sRecordTag4.UseVisualStyleBackColor = true;
            this.sRecordTag4.CheckedChanged += new System.EventHandler(this.rSelectTag4_CheckedChanged);
            // 
            // sRecordTag3
            // 
            this.sRecordTag3.AutoSize = true;
            this.sRecordTag3.Location = new System.Drawing.Point(9, 125);
            this.sRecordTag3.Name = "sRecordTag3";
            this.sRecordTag3.Size = new System.Drawing.Size(53, 17);
            this.sRecordTag3.TabIndex = 0;
            this.sRecordTag3.TabStop = true;
            this.sRecordTag3.Text = "Tag 3";
            this.sRecordTag3.UseVisualStyleBackColor = true;
            this.sRecordTag3.CheckedChanged += new System.EventHandler(this.rSelectTag3_CheckedChanged);
            // 
            // sRecordTag2
            // 
            this.sRecordTag2.AutoSize = true;
            this.sRecordTag2.Location = new System.Drawing.Point(9, 85);
            this.sRecordTag2.Name = "sRecordTag2";
            this.sRecordTag2.Size = new System.Drawing.Size(53, 17);
            this.sRecordTag2.TabIndex = 0;
            this.sRecordTag2.TabStop = true;
            this.sRecordTag2.Text = "Tag 2";
            this.sRecordTag2.UseVisualStyleBackColor = true;
            this.sRecordTag2.CheckedChanged += new System.EventHandler(this.rSelectTag2_CheckedChanged);
            // 
            // sRecordTag1
            // 
            this.sRecordTag1.AutoSize = true;
            this.sRecordTag1.Location = new System.Drawing.Point(9, 43);
            this.sRecordTag1.Name = "sRecordTag1";
            this.sRecordTag1.Size = new System.Drawing.Size(53, 17);
            this.sRecordTag1.TabIndex = 0;
            this.sRecordTag1.TabStop = true;
            this.sRecordTag1.Text = "Tag 1";
            this.sRecordTag1.UseVisualStyleBackColor = true;
            this.sRecordTag1.CheckedChanged += new System.EventHandler(this.rSelectTag1_CheckedChanged);
            // 
            // HexDataTextBox
            // 
            this.HexDataTextBox.Location = new System.Drawing.Point(93, 63);
            this.HexDataTextBox.Multiline = true;
            this.HexDataTextBox.Name = "HexDataTextBox";
            this.HexDataTextBox.Size = new System.Drawing.Size(429, 285);
            this.HexDataTextBox.TabIndex = 0;
            this.HexDataTextBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // RefreshButton
            // 
            this.RefreshButton.Location = new System.Drawing.Point(24, 391);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(90, 32);
            this.RefreshButton.TabIndex = 2;
            this.RefreshButton.Text = "Refresh";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // DataManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(562, 432);
            this.Controls.Add(this.RefreshButton);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.OkBotton);
            this.Controls.Add(this.CancelBotton);
            this.Controls.Add(this.HelpButton);
            this.Name = "DataManager";
            this.Text = "DataView";
            this.Load += new System.EventHandler(this.DataView_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.Catalog_Number.ResumeLayout(false);
            this.Catalog_Number.PerformLayout();
            this.DSR.ResumeLayout(false);
            this.DSR.PerformLayout();
            this.Barcode.ResumeLayout(false);
            this.Barcode.PerformLayout();
            this.Tag_Number.ResumeLayout(false);
            this.Tag_Number.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.RecordSelectTagList.ResumeLayout(false);
            this.RecordSelectTagList.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.DisplayHexRadioButtonList.ResumeLayout(false);
            this.DisplayHexRadioButtonList.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button HelpButton;
        private System.Windows.Forms.Button CancelBotton;
        private System.Windows.Forms.Button OkBotton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox Catalog_Number;
        private System.Windows.Forms.GroupBox DSR;
        private System.Windows.Forms.GroupBox Barcode;
        private System.Windows.Forms.GroupBox Tag_Number;
        private System.Windows.Forms.TextBox DsrText6;
        private System.Windows.Forms.TextBox DsrText5;
        private System.Windows.Forms.TextBox DsrText4;
        private System.Windows.Forms.TextBox DsrText3;
        private System.Windows.Forms.TextBox DsrText2;
        private System.Windows.Forms.TextBox DsrText1;
        private System.Windows.Forms.TextBox BarcodeText6;
        private System.Windows.Forms.TextBox BarcodeText5;
        private System.Windows.Forms.TextBox BarcodeText4;
        private System.Windows.Forms.TextBox BarcodeText3;
        private System.Windows.Forms.TextBox BarcodeText2;
        private System.Windows.Forms.TextBox BarcodeText1;
        private System.Windows.Forms.TextBox CatalogText6;
        private System.Windows.Forms.TextBox CatalogText5;
        private System.Windows.Forms.TextBox CatalogText4;
        private System.Windows.Forms.TextBox CatalogText3;
        private System.Windows.Forms.TextBox CatalogText2;
        private System.Windows.Forms.TextBox CatalogText1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox SelectTag6;
        private System.Windows.Forms.CheckBox SelectTag5;
        private System.Windows.Forms.CheckBox SelectTag4;
        private System.Windows.Forms.CheckBox SelectTag3;
        private System.Windows.Forms.CheckBox SelectTag2;
        private System.Windows.Forms.CheckBox SelectTag1;
        private System.Windows.Forms.TextBox HexDataTextBox;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.GroupBox DisplayHexRadioButtonList;
        private System.Windows.Forms.TextBox ColumnList;
        private System.Windows.Forms.TextBox RecordList;
        private System.Windows.Forms.Label SerialNoLabel;
        private System.Windows.Forms.TextBox HexCountBar;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.RadioButton sRecordTag5;
        private System.Windows.Forms.RadioButton sRecordTag4;
        private System.Windows.Forms.RadioButton sRecordTag3;
        private System.Windows.Forms.RadioButton sRecordTag2;
        private System.Windows.Forms.RadioButton sRecordTag1;
        private System.Windows.Forms.GroupBox RecordSelectTagList;
        private System.Windows.Forms.RadioButton rSelectTag5;
        private System.Windows.Forms.RadioButton rSelectTag4;
        private System.Windows.Forms.RadioButton rSelectTag3;
        private System.Windows.Forms.RadioButton rSelectTag2;
        private System.Windows.Forms.RadioButton rSelectTag1;
        private System.Windows.Forms.TextBox RecordListCRC;
        private System.Windows.Forms.RadioButton sRecordTag6;
        private System.Windows.Forms.RadioButton rSelectTag6;
        private System.Windows.Forms.TextBox BarCRC;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label DataBytes;
    }
}