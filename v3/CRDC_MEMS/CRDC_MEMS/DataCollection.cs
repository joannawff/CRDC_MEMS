using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace CRDC_MEMS
{
    public partial class DataCollection : Form
    {
        //创建文件流
        StreamWriter file_sw;
        //存储文件路径
        string file_path;
        string file_path_base;
        //存储文件内容
        public string file_content = "";
        //存储文件名内容
        public string filename_content = "";                //接收所有传过来的内容，包括其它文件名等
        public string[] filename_content1 = null;           //接收所有内容进行后的分割，包括其它文件名等
        public string[] filename_content_array = null;      //选则txt文件进行保存
        //livedemo演示使用数据存储
        int TemLiveDemo_flag = 0;                           //get_tem_livedemo获取结果标志位
        int LiveDemo_flag = 2;                              //判断当前状态的标志位，1为正在读取，2为停止
        int LiveDemo_state = 0;
        int LiveDemo_state_return = 0;
        public string livedemo_content = "";
        public string[] livedemo_content_detail = null;
        //nodeinfo节点设备数据
        int nodeinfo_flag = 0;                              //get_nodeinfo获取结果标志位
        int nodeinfo_state = 0;
        int nodeinfo_state_return = 0;
        public string nodeinfo_content = "";
        public string[] nodeinfo_content_detail = null;
        //voltage电压数据
        int voltage_flag = 0;                              //get_voltage获取结果标志位
        int voltage_state = 0;
        int voltage_state_return = 0;
        public string voltage_content = "";
        public string[] voltage_content_detail = null;
        //所有文件名字符串数组
        string[] file_name = new string[] { "20-05-15-15.txt", "20-05-15-16.txt" };
        //当前文件的传输编号
        int file_num = 0;
        //当前文件名称的传输编号
        int filename_number = 0;
        //当前文件删除的编号
        int file_del_number = 0;
        //当前文件的传输状态，接收文件具体内容时使用
        int file_state = 0;
        //当前文件名的传输状态，接收文件名的时候使用
        int filename_state = 0;
        //当前文件的删除状态，删除文件时使用
        int file_del_state = 0;

        //定义端口类
        private SerialPort ComDevice = new SerialPort();
        public DataCollection()
        {
            InitializeComponent();
            InitralConfig();
        }
        /// <summary>
        /// 配置初始化
        /// </summary>
        private void InitralConfig()
        {
            //查询主机上存在的串口
            comboBox_Port.Items.AddRange(SerialPort.GetPortNames());

            if (comboBox_Port.Items.Count > 0)
            {
                comboBox_Port.SelectedIndex = 0;
            }
            else
            {
                comboBox_Port.Text = "未检测到串口";
            }
            radioButton_Hex.Enabled = false;
            radioButton_UTF8.Enabled = false;
            radioButton_Unicode.Enabled =  false;
            radioButton_ASCII.Checked = true;
            comboBox_BaudRate.SelectedIndex = 11;
            comboBox_DataBits.SelectedIndex = 0;
            comboBox_StopBits.SelectedIndex = 0;
            comboBox_CheckBits.SelectedIndex = 0;
            pictureBox_Status.BackgroundImage = Properties.Resources.red;

            //向ComDevice.DataReceived（是一个事件）注册一个方法Com_DataReceived，当端口类接收到信息时时会自动调用Com_DataReceived方法
            ComDevice.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived);
        }

        /// <summary>
        /// 一旦ComDevice.DataReceived事件发生，就将从串口接收到的数据显示到接收端对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //开辟接收缓冲区
            byte[] ReDatas = new byte[ComDevice.BytesToRead];
            //从串口读取数据
            ComDevice.Read(ReDatas, 0, ReDatas.Length);
            //实现数据的解码与显示
            AddData(ReDatas);
        }

        /// <summary>
        /// 解码过程
        /// </summary>
        /// <param name="data">串口通信的数据编码方式因串口而异，需要查询串口相关信息以获取</param>
        public void AddData(byte[] data)
        {
            //16进制格式选项
            if (radioButton_Hex.Checked)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sb.AppendFormat("{0:x2}" + " ", data[i]);
                }
                AddContent(sb.ToString().ToUpper());
            }
            //解码，把格式转换成string格式
            else if (radioButton_ASCII.Checked)
            {
                AddContent(new ASCIIEncoding().GetString(data));
                //AddContent("换行\r\n");

                //获取文件内容：如果显示的是文件内容，就进行存储到路径中的txt中
                if (file_state == 1)
                {
                    file_content = file_content + new ASCIIEncoding().GetString(data);
                    //使用下面这行写入txt会造成很多换行符，分成一节一节的内容
                    //file_sw.WriteLine(new ASCIIEncoding().GetString(data));
                }
                //获取文件名：如果显示的是文件名内容，保存到filename_content变量中
                if (filename_state == 1)
                {
                    filename_content = filename_content + new ASCIIEncoding().GetString(data);
                }
                //如果显示的是删除文件后的结果，根据结果进行继续删除还是处理
                if (file_del_state == 1)
                {
                    if((new ASCIIEncoding().GetString(data)).Contains("del_OK"))
                    {
                        file_del_number++;
                        if(file_del_number < filename_content_array.Length)
                        {
                            memory_del_Click(memory_del, new EventArgs());
                        }
                        else
                        {
                            file_del_number = 0;
                            file_del_state = 0;
                            //memory_refresh_Click(memory_refresh, new EventArgs());
                            MessageBox.Show("采集器清空完毕，请刷新查看！", "文件提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                    }
                    else if((new ASCIIEncoding().GetString(data)).Contains("del_ERROR"))
                    {
                        MessageBox.Show("文件删除错误！", "文件提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        file_del_number = 0;
                        file_del_state = 0;
                        //memory_refresh_Click(memory_refresh, new EventArgs());
                        MessageBox.Show("采集器清空完毕，请刷新查看！", "文件提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        MessageBox.Show("文件删除出现未知错误！", "文件提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        file_del_number = 0;
                        file_del_state = 0;
                        //memory_refresh_Click(memory_refresh, new EventArgs());
                        MessageBox.Show("采集器清空完毕，请刷新查看！", "文件提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }

                //判断显示的内容是否包含“END”字符，表明文件传输完毕
                //文件内容传输完毕，数据流清空，文件关闭，文件传输状态重置，文件序号加1
                if ((file_state == 1) && ((new ASCIIEncoding().GetString(data)).Contains("#")))
                {
                    AddContent("本地下载完毕！\r\n");
                    file_sw.WriteLine(file_content);
                    file_content = "";
                    file_sw.Flush();
                    file_sw.Close();

                    file_state = 0;
                    file_num++;

                    if(file_num < filename_content_array.Length)
                    {
                        memory_save_Click_2();
                    }
                    else
                    {
                        file_num = 0;
                    }
                }
                //文件名传输完毕
                if ((filename_state == 1) && ((new ASCIIEncoding().GetString(data)).Contains("END")))
                {
                    filename_number = 0;
                    //Console.WriteLine(filename_content);
                    
                    filename_content = filename_content.Replace("//","%");
                    filename_content1 = filename_content.Split('%','\n','\r');

                    for (int i = 0; i < filename_content1.Length; i++)
                    {
                        if (filename_content1[i].Contains(".txt"))
                        { filename_number++; }
                    }
                    filename_content_array = new string[filename_number];
                    filename_number = 0;
                    for (int i = 0;i < filename_content1.Length;i++)
                    {
                        if(filename_content1[i].Contains(".txt"))
                        {
                            filename_content_array[filename_number] = filename_content1[i];
                            filename_number++;
                        }
                    }
                    Console.WriteLine(filename_content_array);

                    filename_state = 0;
                }
                //检查文件数量，如果还有文件并且前一个文件发送完毕，立刻开始下一个文件的发送
                if ((file_state == 1) && (file_num > 0) && (file_num < file_name.Length) && (file_state == 0))
                {
                    button_Send_Click_content();
                }
                //如果文件全部传输完毕，则文件数量重置
                if ((file_num == file_name.Length) && (file_state == 0))
                {
                    file_num = 0;
                    memory_save.Enabled = true;
                }

                //livedemo界面使用数据
                if (LiveDemo_state == 1)
                {
                    //LiveDemo_state = 0;
                    livedemo_content = livedemo_content + new ASCIIEncoding().GetString(data);
                    //Console.WriteLine("livedemo_content\r\n",livedemo_content.Trim());
                    if((new ASCIIEncoding().GetString(data)).Contains("#"))
                    {
                        LiveDemo_state = 0;
                        LiveDemo_state_return = 1;
                    }
                }

                //nodeinfo使用数据
                if (nodeinfo_state == 1)
                {
                    nodeinfo_content = nodeinfo_content + new ASCIIEncoding().GetString(data);
                    if ((new ASCIIEncoding().GetString(data)).Contains("#"))
                    {
                        nodeinfo_state = 0;
                        nodeinfo_state_return = 1;
                    }
                }

                //voltage使用数据
                if (voltage_state == 1)
                {
                    voltage_content = voltage_content + new ASCIIEncoding().GetString(data);
                    if ((new ASCIIEncoding().GetString(data)).Contains("#"))
                    {
                        voltage_flag = 0;
                        voltage_state_return = 1;
                    }
                }
            }
            //UTF8格式
            else if (radioButton_UTF8.Checked)
            {
                AddContent(new UTF8Encoding().GetString(data));
            }
            //Unicode格式
            else if (radioButton_Unicode.Checked)
            {
                AddContent(new UnicodeEncoding().GetString(data));
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sb.AppendFormat("{0:x2}" + " ", data[i]);
                }
                AddContent(sb.ToString().ToUpper());
            }
        }

        /// <summary>
        /// 接收端对话框显示消息,如果
        /// </summary>
        /// <param name="content"></param>
        private void AddContent(string content)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                if(content.Contains("communicate_test")||content.Contains("63 6F 6D 6D 75 6E 69 63 61 74 65 5F 74 65 73 74"))   //16进制与10进制同含义
                {
                    //textBox_memory.AppendText(content);
                    textBox_Receive.AppendText(content);
                }
                else
                {
                    textBox_memory.AppendText(content);
                }
                
            }));
        }
        /// <summary>
        /// 开启端口按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Switch_Click(object sender, EventArgs e)
        {
            if (comboBox_Port.Items.Count <= 0)
            {
                MessageBox.Show("未发现可用串口，请检查硬件设备");
                return;
            }

            if (ComDevice.IsOpen == false)
            {
                //设置串口相关属性
                ComDevice.PortName = comboBox_Port.SelectedItem.ToString();
                ComDevice.BaudRate = Convert.ToInt32(comboBox_BaudRate.SelectedItem.ToString());
                ComDevice.Parity = (Parity)Convert.ToInt32(comboBox_CheckBits.SelectedIndex.ToString());
                ComDevice.DataBits = Convert.ToInt32(comboBox_DataBits.SelectedItem.ToString());
                ComDevice.StopBits = (StopBits)Convert.ToInt32(comboBox_StopBits.SelectedItem.ToString());
                try
                {
                    //开启串口
                    ComDevice.Open();
                    button_Send.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "未能成功开启串口", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                button_Switch.Text = "关闭";
                pictureBox_Status.BackgroundImage = Properties.Resources.green;
            }
            else
            {
                try
                {
                    //关闭串口
                    ComDevice.Close();
                    button_Send.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "串口关闭错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                button_Switch.Text = "开启";
                pictureBox_Status.BackgroundImage = Properties.Resources.red;
            }

            comboBox_Port.Enabled = !ComDevice.IsOpen;
            comboBox_BaudRate.Enabled = !ComDevice.IsOpen;
            comboBox_DataBits.Enabled = !ComDevice.IsOpen;
            comboBox_StopBits.Enabled = !ComDevice.IsOpen;
            comboBox_CheckBits.Enabled = !ComDevice.IsOpen;
        }
        /// <summary>
        /// 删除文件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void memory_del_Click(object sender, EventArgs e)
        {
            file_del_state = 1;
            if(filename_content_array.Length > 0)
            {
                byte[] sendData = null;
                //Console.WriteLine(filename_content_array);
                sendData = Encoding.ASCII.GetBytes(filename_content_array[file_del_number] + "@\r\n");
                SendData(sendData);
            }
            else
            {
                MessageBox.Show("无数据文件！", "文件提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        /// <summary>
        /// 本地下载文件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void memory_save_Click(object sender, EventArgs e)
        {
            //memory_save.Enabled = false;
            if(filename_content_array.Length > 0)
            {
                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.Description = "请选择存储路径！";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (string.IsNullOrEmpty(dialog.SelectedPath))
                    {
                        MessageBox.Show(this, "文件夹路径不能为空", "提示");
                        //memory_save.Enabled = true;
                    }
                    else
                    {
                        file_path_base = dialog.SelectedPath;
                        memory_save_Click_2();
                    }
                }
                //else
                //{ memory_save.Enabled = true; }
            }
            else
            { MessageBox.Show("没有数据文件可供下载！", "文件提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }

        }
        public void memory_save_Click_2()
        {
            file_state = 1;
            file_content = "";
            byte[] sendData = null;
            sendData = Encoding.ASCII.GetBytes(filename_content_array[file_num] + "*\r\n");
            //新建txt文件
            file_path = file_path_base + "\\" + filename_content_array[file_num];
            //如果不存在这个文件就创建，并添加数据
            if (!System.IO.File.Exists(file_path))
            {
                file_sw = new StreamWriter(file_path, false, Encoding.Default);
            }
            //如果存在。。。
            else
            {
                MessageBox.Show("文件\"" + filename_content_array[file_num] + "\"已存在，将覆盖该文件！", "文件提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                file_sw = new StreamWriter(file_path, false, Encoding.Default);
            }
            SendData(sendData);

        }
        /// <summary>
        /// 测试通讯按键，将消息编码并发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Send_Click(object sender, EventArgs e)
        {
            byte[] sendData = null;
            if (textBox_Receive.Text.Length > 0)
            {
                textBox_Receive.AppendText("\n");
            }
            textBox_Send.Text = "communicate_test";
            sendData = Encoding.ASCII.GetBytes(textBox_Send.Text.Trim() + "\r\n");
            SendData(sendData);
        }
        public void button_Send_Click_content()
        {
            byte[] sendData = null;
            //16进制选项类型文件
            if (radioButton_Hex.Checked)
            {
                sendData = strToHexByte(textBox_Send.Text.Trim() + "\r\n");
            }
            //发送文件名，注意添加“#*\r\n”，其它几种类型还未修改
            else if (radioButton_ASCII.Checked)
            {
                sendData = Encoding.ASCII.GetBytes(textBox_Send.Text.Trim() + "\r\n");
            }
            else if (radioButton_UTF8.Checked)
            {
                sendData = Encoding.UTF8.GetBytes(textBox_Send.Text.Trim() + "\r\n");
            }
            else if (radioButton_Unicode.Checked)
            {
                sendData = Encoding.Unicode.GetBytes(textBox_Send.Text.Trim() + "\r\n");
            }
            else
            {
                sendData = strToHexByte(textBox_Send.Text.Trim() + "\r\n");
            }
            SendData(sendData);
        }
        /// <summary>
        /// 此函数将编码后的消息传递给串口
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SendData(byte[] data)
        {
            if (ComDevice.IsOpen)
            {
                try
                {
                    //将消息传递给串口
                    if(data == null)
                    {
                        MessageBox.Show("发送格式有误，请检查！", "发送提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                    ComDevice.Write(data, 0, data.Length);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "发送失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("串口未开启", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
        /// <summary>comboBox_BaudRate
        /// 16进制编码
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private byte[] strToHexByte(string hexString)
        {
            try
            {
                hexString = hexString.Replace(" ", "");
                if ((hexString.Length % 2) != 0) hexString += " ";
                byte[] returnBytes = new byte[hexString.Length / 2];
                for (int i = 0; i < returnBytes.Length; i++)
                    returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Replace(" ", ""), 16);
                return returnBytes;
            }
            catch
            { Console.WriteLine("err:格式错误"); return null; }

        }
        //数据采集“刷新”按键事件
        private void memory_refresh_Click(object sender, EventArgs e)
        {
            filename_number = 0;
            filename_content = "";
            filename_content1 = null;
            filename_content_array = null;
            filename_state = 1;
            textBox_memory.Text = null;
            if (textBox_memory.Text.Length > 0)
            {
                textBox_memory.AppendText("\n");
            }
            byte[] sendData = null;
            sendData = Encoding.ASCII.GetBytes("sd_filesname\r\n");
            SendData(sendData);
        }
        //实时演示界面事件
        private void LiveDemo_Click(object sender, EventArgs e)
        {
            //获取设备信息并显示在窗体上
            if (!this.Text.Contains("_"))
            {
                get_nodeinfo();
                while (true)
                {
                    if (nodeinfo_flag == 1)
                    {
                        this.Text = "DataCollection" + " " + nodeinfo_content_detail[5] + "_" + nodeinfo_content_detail[1];
                        break;
                    }
                }
            }
            //如果刚按下开始按键，或者
            if (LiveDemo_flag == 2)
            {
                //LiveDemo.Text = "STOP";
                //LiveDemo_flag = 1;              //读取数据
                byte[] sendData = null;
                sendData = Encoding.ASCII.GetBytes("Ctem_test\r\n");
                
                //设置标志位
                if (SendData(sendData))
                {
                    LiveDemo_state = 1;
                    while (true)
                    {
                        if (LiveDemo_state_return == 1)
                        {
                            LiveDemo_state_return = 0;

                            livedemo_content_detail = livedemo_content.Split(',', '\n', '\r');
                            int livedemo_length = livedemo_content_detail.Length;
                            if (livedemo_length >= 13 * int.Parse(nodeinfo_content_detail[1]))
                            {
                                textBox2.Text = (double.Parse(livedemo_content_detail[1])/100.0).ToString();

                                textBox6.Text = livedemo_content_detail[2];
                                textBox7.Text = livedemo_content_detail[3];
                                textBox8.Text = livedemo_content_detail[4];
                                textBox23.Text = livedemo_content_detail[5];
                                textBox22.Text = livedemo_content_detail[6];
                                textBox21.Text = livedemo_content_detail[7];

                                textBox9.Text = livedemo_content_detail[2 + 13];
                                textBox10.Text = livedemo_content_detail[3 + 13];
                                textBox11.Text = livedemo_content_detail[4 + 13];
                                textBox20.Text = livedemo_content_detail[5 + 13];
                                textBox19.Text = livedemo_content_detail[6 + 13];
                                textBox18.Text = livedemo_content_detail[7 + 13];

                                textBox12.Text = livedemo_content_detail[2 + 26];
                                textBox13.Text = livedemo_content_detail[3 + 26];
                                textBox14.Text = livedemo_content_detail[4 + 26];
                                textBox17.Text = livedemo_content_detail[5 + 26];
                                textBox16.Text = livedemo_content_detail[6 + 26];
                                textBox15.Text = livedemo_content_detail[7 + 26];

                                textBox3.Text = (Math.Round((Config.NODE_Length * 1.0) * Math.Sin(Math.Asin((1.0 * int.Parse(livedemo_content_detail[4])) / 13107.2)), 2)).ToString();
                                textBox4.Text = (Math.Round((Config.NODE_Length * 1.0) * Math.Sin(Math.Asin((1.0 * int.Parse(livedemo_content_detail[4+13])) / 13107.2)), 2)).ToString();
                                textBox5.Text = (Math.Round((Config.NODE_Length * 1.0) * Math.Sin(Math.Asin((1.0 * int.Parse(livedemo_content_detail[4+26])) / 13107.2)), 2)).ToString();
                            }
                            livedemo_content = "";
                            Array.Clear(livedemo_content_detail, 0, livedemo_content_detail.Length);
                            break;
                        }
                    }
                }
                //获取电压数据
                get_voltage();
                while(true)
                {
                    if(voltage_flag == 1)
                    {
                        voltage_flag = 0;
                        textBox1.Text = (11.0 * double.Parse(voltage_content_detail[1])).ToString();
                        break;
                    }
                }
            }
            else if(LiveDemo_flag == 1)
            {
                LiveDemo.Text = "START";
                LiveDemo_flag = 2;              //停止读取数据
            }
        }
        //获取电源电压
        public void get_voltage()
        {
            byte[] sendData = null;
            sendData = Encoding.ASCII.GetBytes("voltage_test\r\n");
            if (SendData(sendData))
            {
                voltage_state = 1;
                while (true)
                {
                    if (voltage_state_return == 1)
                    {
                        voltage_state_return = 0;
                        voltage_content = voltage_content.Replace(" ", "");
                        voltage_content_detail = voltage_content.Split(',',':', '\n', '\r');
                        voltage_flag = 1;
                        break;
                    }
                }
            }
        }
        //获取温度和直连演示为同一个代码
        public void get_tem_livedemo()
        {
            byte[] sendData = null;
            sendData = Encoding.ASCII.GetBytes("Ctem_test\r\n");
            if (SendData(sendData))
            {
                LiveDemo_state = 1;
                while (true)
                {
                    if (LiveDemo_state_return == 1)
                    {
                        LiveDemo_state_return = 0;
                        livedemo_content = livedemo_content.Replace(" ", "");
                        livedemo_content_detail = livedemo_content.Split(',', '\n', '\r');
                        TemLiveDemo_flag = 1;
                        break;
                    }
                }
            }
        }
        //获取节点设备信息
        public void get_nodeinfo()
        {
            byte[] sendData = null;
            sendData = Encoding.ASCII.GetBytes("node_info\r\n");
            if (SendData(sendData))
            {
                nodeinfo_state = 1;
                while(true)
                { 
                    if(nodeinfo_state_return == 1)
                    {
                        nodeinfo_state_return = 0;
                        nodeinfo_content = nodeinfo_content.Replace(" ", "");
                        nodeinfo_content_detail = nodeinfo_content.Split(',',':','\r','\n');
                        nodeinfo_flag = 1;
                        break;
                    }
                }
            }
        }
        //功能配置页面，点击事件发生后触发更改界面名称内容
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //获取设备信息并显示在窗体上
            if (!this.Text.Contains("_"))
            {
                get_nodeinfo();
                while (true)
                {
                    if (nodeinfo_flag == 1)
                    {
                        this.Text = "DataCollection" + " " + nodeinfo_content_detail[5] + "_" + nodeinfo_content_detail[1];
                        break;
                    }
                }
            }
            //ComboBOX初始化
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            for(int i = 1;i <= int.Parse(nodeinfo_content_detail[1]);i++)
            {
                comboBox1.Items.Add(i);
                comboBox2.Items.Add(i);
                comboBox3.Items.Add(i);
                comboBox4.Items.Add(i);
            }
        }
    }






}
