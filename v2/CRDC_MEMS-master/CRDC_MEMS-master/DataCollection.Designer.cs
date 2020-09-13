namespace CRDC_MEMS
{
    partial class DataCollection
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton_Unicode = new System.Windows.Forms.RadioButton();
            this.radioButton_UTF8 = new System.Windows.Forms.RadioButton();
            this.radioButton_ASCII = new System.Windows.Forms.RadioButton();
            this.radioButton_Hex = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox_Status = new System.Windows.Forms.PictureBox();
            this.button_Switch = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_CheckBits = new System.Windows.Forms.ComboBox();
            this.comboBox_StopBits = new System.Windows.Forms.ComboBox();
            this.comboBox_DataBits = new System.Windows.Forms.ComboBox();
            this.comboBox_BaudRate = new System.Windows.Forms.ComboBox();
            this.comboBox_Port = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button_Send = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBox_Send = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox_Receive = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.memory_refresh = new System.Windows.Forms.Button();
            this.memory_save = new System.Windows.Forms.Button();
            this.memory_del = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBox_memory = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Status)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.HotTrack = true;
            this.tabControl1.ItemSize = new System.Drawing.Size(142, 25);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(684, 432);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(29, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(651, 424);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "端口配置";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton_Unicode);
            this.groupBox2.Controls.Add(this.radioButton_UTF8);
            this.groupBox2.Controls.Add(this.radioButton_ASCII);
            this.groupBox2.Controls.Add(this.radioButton_Hex);
            this.groupBox2.Location = new System.Drawing.Point(37, 60);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(586, 98);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "编码方式";
            // 
            // radioButton_Unicode
            // 
            this.radioButton_Unicode.AutoSize = true;
            this.radioButton_Unicode.Location = new System.Drawing.Point(475, 42);
            this.radioButton_Unicode.Name = "radioButton_Unicode";
            this.radioButton_Unicode.Size = new System.Drawing.Size(65, 16);
            this.radioButton_Unicode.TabIndex = 3;
            this.radioButton_Unicode.TabStop = true;
            this.radioButton_Unicode.Text = "Unicode";
            this.radioButton_Unicode.UseVisualStyleBackColor = true;
            // 
            // radioButton_UTF8
            // 
            this.radioButton_UTF8.AutoSize = true;
            this.radioButton_UTF8.Location = new System.Drawing.Point(335, 42);
            this.radioButton_UTF8.Name = "radioButton_UTF8";
            this.radioButton_UTF8.Size = new System.Drawing.Size(53, 16);
            this.radioButton_UTF8.TabIndex = 2;
            this.radioButton_UTF8.TabStop = true;
            this.radioButton_UTF8.Text = "UTF-8";
            this.radioButton_UTF8.UseVisualStyleBackColor = true;
            // 
            // radioButton_ASCII
            // 
            this.radioButton_ASCII.AutoSize = true;
            this.radioButton_ASCII.Location = new System.Drawing.Point(195, 42);
            this.radioButton_ASCII.Name = "radioButton_ASCII";
            this.radioButton_ASCII.Size = new System.Drawing.Size(53, 16);
            this.radioButton_ASCII.TabIndex = 1;
            this.radioButton_ASCII.TabStop = true;
            this.radioButton_ASCII.Text = "ASCII";
            this.radioButton_ASCII.UseVisualStyleBackColor = true;
            // 
            // radioButton_Hex
            // 
            this.radioButton_Hex.AutoSize = true;
            this.radioButton_Hex.Location = new System.Drawing.Point(55, 42);
            this.radioButton_Hex.Name = "radioButton_Hex";
            this.radioButton_Hex.Size = new System.Drawing.Size(59, 16);
            this.radioButton_Hex.TabIndex = 0;
            this.radioButton_Hex.TabStop = true;
            this.radioButton_Hex.Text = "16进制";
            this.radioButton_Hex.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox_Status);
            this.groupBox1.Controls.Add(this.button_Switch);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBox_CheckBits);
            this.groupBox1.Controls.Add(this.comboBox_StopBits);
            this.groupBox1.Controls.Add(this.comboBox_DataBits);
            this.groupBox1.Controls.Add(this.comboBox_BaudRate);
            this.groupBox1.Controls.Add(this.comboBox_Port);
            this.groupBox1.Location = new System.Drawing.Point(37, 180);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(586, 142);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "端口属性设置";
            // 
            // pictureBox_Status
            // 
            this.pictureBox_Status.Location = new System.Drawing.Point(540, 80);
            this.pictureBox_Status.Name = "pictureBox_Status";
            this.pictureBox_Status.Size = new System.Drawing.Size(32, 32);
            this.pictureBox_Status.TabIndex = 11;
            this.pictureBox_Status.TabStop = false;
            // 
            // button_Switch
            // 
            this.button_Switch.Location = new System.Drawing.Point(406, 85);
            this.button_Switch.Name = "button_Switch";
            this.button_Switch.Size = new System.Drawing.Size(121, 20);
            this.button_Switch.TabIndex = 10;
            this.button_Switch.Text = "开启";
            this.button_Switch.UseVisualStyleBackColor = true;
            this.button_Switch.Click += new System.EventHandler(this.button_Switch_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(337, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "检验方式";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(159, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "停止位";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(533, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "数据位";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(337, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "波特率";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(159, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "端口";
            // 
            // comboBox_CheckBits
            // 
            this.comboBox_CheckBits.FormattingEnabled = true;
            this.comboBox_CheckBits.Items.AddRange(new object[] {
            "None",
            "Odd",
            "Even",
            "Mark",
            "Space"});
            this.comboBox_CheckBits.Location = new System.Drawing.Point(210, 85);
            this.comboBox_CheckBits.Name = "comboBox_CheckBits";
            this.comboBox_CheckBits.Size = new System.Drawing.Size(121, 20);
            this.comboBox_CheckBits.TabIndex = 4;
            // 
            // comboBox_StopBits
            // 
            this.comboBox_StopBits.FormattingEnabled = true;
            this.comboBox_StopBits.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.comboBox_StopBits.Location = new System.Drawing.Point(32, 85);
            this.comboBox_StopBits.Name = "comboBox_StopBits";
            this.comboBox_StopBits.Size = new System.Drawing.Size(121, 20);
            this.comboBox_StopBits.TabIndex = 3;
            // 
            // comboBox_DataBits
            // 
            this.comboBox_DataBits.FormattingEnabled = true;
            this.comboBox_DataBits.Items.AddRange(new object[] {
            "8",
            "7",
            "6"});
            this.comboBox_DataBits.Location = new System.Drawing.Point(406, 44);
            this.comboBox_DataBits.Name = "comboBox_DataBits";
            this.comboBox_DataBits.Size = new System.Drawing.Size(121, 20);
            this.comboBox_DataBits.TabIndex = 2;
            // 
            // comboBox_BaudRate
            // 
            this.comboBox_BaudRate.FormattingEnabled = true;
            this.comboBox_BaudRate.Items.AddRange(new object[] {
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "43000",
            "56000",
            "57600",
            "115200"});
            this.comboBox_BaudRate.Location = new System.Drawing.Point(210, 44);
            this.comboBox_BaudRate.Name = "comboBox_BaudRate";
            this.comboBox_BaudRate.Size = new System.Drawing.Size(121, 20);
            this.comboBox_BaudRate.TabIndex = 1;
            // 
            // comboBox_Port
            // 
            this.comboBox_Port.FormattingEnabled = true;
            this.comboBox_Port.Location = new System.Drawing.Point(32, 44);
            this.comboBox_Port.Name = "comboBox_Port";
            this.comboBox_Port.Size = new System.Drawing.Size(121, 20);
            this.comboBox_Port.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage2.Controls.Add(this.button_Send);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(29, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(651, 424);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "通讯测试";
            // 
            // button_Send
            // 
            this.button_Send.Location = new System.Drawing.Point(471, 347);
            this.button_Send.Name = "button_Send";
            this.button_Send.Size = new System.Drawing.Size(120, 23);
            this.button_Send.TabIndex = 0;
            this.button_Send.Text = "测试通讯";
            this.button_Send.Click += new System.EventHandler(this.button_Send_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBox_Send);
            this.groupBox4.Location = new System.Drawing.Point(37, 187);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(580, 137);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "发送端";
            // 
            // textBox_Send
            // 
            this.textBox_Send.Location = new System.Drawing.Point(6, 20);
            this.textBox_Send.Multiline = true;
            this.textBox_Send.Name = "textBox_Send";
            this.textBox_Send.Size = new System.Drawing.Size(568, 111);
            this.textBox_Send.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox_Receive);
            this.groupBox3.Location = new System.Drawing.Point(37, 35);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(580, 137);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "接收端";
            // 
            // textBox_Receive
            // 
            this.textBox_Receive.Location = new System.Drawing.Point(6, 20);
            this.textBox_Receive.Multiline = true;
            this.textBox_Receive.Name = "textBox_Receive";
            this.textBox_Receive.Size = new System.Drawing.Size(568, 111);
            this.textBox_Receive.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage3.Controls.Add(this.memory_refresh);
            this.tabPage3.Controls.Add(this.memory_save);
            this.tabPage3.Controls.Add(this.memory_del);
            this.tabPage3.Controls.Add(this.groupBox5);
            this.tabPage3.Location = new System.Drawing.Point(29, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(651, 424);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "数据采集";
            // 
            // memory_refresh
            // 
            this.memory_refresh.Location = new System.Drawing.Point(61, 377);
            this.memory_refresh.Name = "memory_refresh";
            this.memory_refresh.Size = new System.Drawing.Size(88, 23);
            this.memory_refresh.TabIndex = 5;
            this.memory_refresh.Text = "刷新";
            this.memory_refresh.UseVisualStyleBackColor = true;
            this.memory_refresh.Click += new System.EventHandler(this.memory_refresh_Click);
            // 
            // memory_save
            // 
            this.memory_save.Location = new System.Drawing.Point(340, 377);
            this.memory_save.Name = "memory_save";
            this.memory_save.Size = new System.Drawing.Size(120, 23);
            this.memory_save.TabIndex = 4;
            this.memory_save.Text = "本地下载";
            this.memory_save.UseVisualStyleBackColor = true;
            this.memory_save.Click += new System.EventHandler(this.memory_save_Click);
            // 
            // memory_del
            // 
            this.memory_del.Location = new System.Drawing.Point(478, 377);
            this.memory_del.Name = "memory_del";
            this.memory_del.Size = new System.Drawing.Size(120, 23);
            this.memory_del.TabIndex = 3;
            this.memory_del.Text = "清空COLLECTOR";
            this.memory_del.UseVisualStyleBackColor = true;
            this.memory_del.Click += new System.EventHandler(this.memory_del_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBox_memory);
            this.groupBox5.Location = new System.Drawing.Point(29, 25);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(602, 323);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "CRDC_MEMS_COLLECTOR_FILES";
            // 
            // textBox_memory
            // 
            this.textBox_memory.Location = new System.Drawing.Point(6, 20);
            this.textBox_memory.Multiline = true;
            this.textBox_memory.Name = "textBox_memory";
            this.textBox_memory.Size = new System.Drawing.Size(590, 297);
            this.textBox_memory.TabIndex = 0;
            // 
            // DataCollection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(684, 432);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(700, 471);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(700, 471);
            this.Name = "DataCollection";
            this.Text = "DataCollection";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Status)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButton_Unicode;
        private System.Windows.Forms.RadioButton radioButton_UTF8;
        private System.Windows.Forms.RadioButton radioButton_ASCII;
        private System.Windows.Forms.RadioButton radioButton_Hex;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox_Status;
        private System.Windows.Forms.Button button_Switch;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_CheckBits;
        private System.Windows.Forms.ComboBox comboBox_StopBits;
        private System.Windows.Forms.ComboBox comboBox_DataBits;
        private System.Windows.Forms.ComboBox comboBox_BaudRate;
        private System.Windows.Forms.ComboBox comboBox_Port;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBox_Send;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox_Receive;
        private System.Windows.Forms.Button button_Send;
        private System.Windows.Forms.Button memory_save;
        private System.Windows.Forms.Button memory_del;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBox_memory;
        private System.Windows.Forms.Button memory_refresh;
    }
}