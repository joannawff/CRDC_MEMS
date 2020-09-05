using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace CRDC_MEMS
{
    public partial class DataSolution : Form
    {
        public string path;
        public string filepath;
        public string content;
        public string content_1;

        int judge = 0;
        int[] node_a = new int[300];
        int[] node_b = new int[300];

        static int N_OF_COL = 14;           //每行分割数量
        static int N_OF_ROW = 1;            //比对的前后行数
        static int GAP_THRESHOLD = 131;     //比对的前后行数的Z值差

        public DataSolution()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 文件导入
        /// </summary>
        private void File_import_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog P_File_Folder = new FolderBrowserDialog();
            if (P_File_Folder.ShowDialog() == DialogResult.OK)
            {
                path = P_File_Folder.SelectedPath;
                MessageBox.Show("文件导入成功！");

                // 处理文件
                Process(path, is_for_H:true);
            }

        }

        private void Data_solution_H_Click(object sender, EventArgs e)
        {
            DirectoryInfo thefolder = new DirectoryInfo(path);
            int sucorerr_flag = 0;

            foreach (FileInfo file in thefolder.GetFiles())
            {
                content = "";
                filepath = file.DirectoryName + @"\" + file.Name;
                //var fout = file.DirectoryName + @"\out_" + file.Name;
                var fout = file.DirectoryName + @"\Data_Solution.txt";
                //var flog = file.DirectoryName + @"\log_" + file.Name;
                var flog = file.DirectoryName + @"\Data_err.txt";
                //StreamWriter sw = new StreamWriter(flog);

                //错误数据追加到Data_err文件中，跟下面的写入数据流不能冲突
                if(flog == filepath)
                {
                    sucorerr_flag = 1;
                    MessageBox.Show("Data_err.txt文件已存在，本次处理终止！", "文件提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
                }
                StreamWriter sw = File.AppendText(flog);

                int[] last_values = new int[N_OF_COL];

                Dictionary<int, bool> isPass = new Dictionary<int, bool>();
                List<string> rows = new List<string>();
                Dictionary<int, List<int>> row_values = new Dictionary<int, List<int>>();

                int row_index = 0;
                using (StreamReader reader = new StreamReader(filepath))
                {
                    string line = "";
                    while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
                    {
                        string[] tokens = line.Trim(',').Replace(",,", ",").Split(',');
                        //新建文件，存储node_totalnum:，存储时间+电压
                        //赋值N_OF_COL
                        if ((tokens.Length == 2)&&(tokens[0].Contains("node_totalnum")))
                        {

                        }

                        //判断当前行格式是否正确
                        if (tokens.Length != N_OF_COL)
                        {
                            Console.WriteLine("Failed to pass: " + line);
                            continue;
                        }
                        
                        int last_index = 0;
                        bool pass = true;
                        //通过行数全部储存
                        List<int> values = new List<int>();
                        for (int i = 1; i < tokens.Length; i++)
                        {
                            string[] array = tokens[i].Split('-');
                            // judge current row's format
                            if (int.TryParse(array[0], out int id) && id > last_index && int.TryParse(array[array.Length - 1], out int val))
                            {
                                pass = true;
                                last_index = id;
                                values.Add(val);
                            }
                            else
                            {
                                pass = false;
                                //break;
                            }
                        }
                        //合格的前提
                        if (pass)
                        {
                            //把合格的行数拼接好放到rows里面
                            rows.Add(string.Join(",", tokens));
                            //row_values里面添加“行索引+values（Z坐标的值，13个）”
                            row_values.Add(row_index, values);
                            //isPass添加“行索引+是否通过的结果”
                            isPass.Add(row_index++, true);
                        }
                    }
                    reader.Close();
                }
                
                //整个文件读取，按行分割，再检查行
                using (StreamWriter writer = File.AppendText(fout))
                {
                    //rows里面为合格的行
                    for (int i = 0; i < rows.Count; i++)
                    {
                        //如果是第0行，直接输出
                        if (i == 0)
                        {
                            writer.WriteLine(rows[i]);
                            continue;
                        }
                        //如果改行Z轴数量不符，输出到错误列表
                        if (row_values[i].Count != N_OF_COL - 1)
                        {
                            sw.WriteLine(rows[i]);
                            continue;
                        }

                        Dictionary<int, int> value_gaps = new Dictionary<int, int>();
                        bool pass = true;

                        //此处有疑问，应该是比对前后行，此处比对从第0行到第i+1行内容
                        for (int j = i - N_OF_ROW; j <= i + N_OF_ROW; j++)
                        {
                            if (!isPass.ContainsKey(j) || row_values[j].Count != N_OF_COL - 1)
                            {
                                continue;
                            }
                            if (i == j)
                            {
                                continue;
                            }
                            for (int k = 0; k < row_values[i].Count; k++)
                            {
                                //？前如果成立返回：前的值，如果不成立返回：后的值
                                int type = row_values[i][k] - row_values[j][k] >= GAP_THRESHOLD ? 1 : row_values[j][k] - row_values[i][k] >= GAP_THRESHOLD ? 2 : 3;

                                if (!value_gaps.ContainsKey(k))
                                {
                                    value_gaps.Add(k, type);
                                }
                                //如果同时不等于3，代表前后值同时超过GAP_THRESHOLD
                                else if (value_gaps[k] == type && type != 3)
                                {
                                    //Console.WriteLine(value_gaps[k] + "\t" + type);
                                    pass = false;
                                    Console.WriteLine("Error key: " + k + "\t" + type);
                                    break;
                                }
                                else
                                {
                                    //pass = false;
                                }
                            }
                        }
                        if (pass)
                        {
                            writer.WriteLine(rows[i]);
                        }
                        else
                        {
                            Console.WriteLine("Failed to pass: " + rows[i]);
                            sw.WriteLine(rows[i]);
                        }
                    }
                    writer.Flush();
                    writer.Close();
                }
                sw.Flush();
                sw.Close();
            }
            if(sucorerr_flag == 0)
            {
                MessageBox.Show("数据解算成功！");
            }
            else
            {
                sucorerr_flag = 0;
            }
        }

        private void Process(string path, bool is_for_H = true)
        {
            string path_out = path + Config.F_OUT_SUFFIX;
            if (!Directory.Exists(path_out))
            {
                Directory.CreateDirectory(path_out);
            }

            // GlobalItem首行记录，SingleItem单行详细内容记录
            List<GlobalItem> gloves = new List<GlobalItem>();
            List<SingleItem> itemlist = new List<SingleItem>();
            //新建类
            Processor helper = new Processor();

            // 加载文件
            helper.LoadFile(path, gloves, itemlist);

            List<SingleItem> itemlist_preprocessed = new List<SingleItem>(); // V 合格的行
            List<SingleItem> itemlist_failed = new List<SingleItem>(); // V 合格的行
            // 预处理文件
            helper.Preprocess(itemlist, itemlist_preprocessed, itemlist_failed, is_for_H);
            helper.Output(Path.Combine(path_out, Config.F_PRE), itemlist_preprocessed); // 成功
            helper.Output(Path.Combine(path_out, Config.F_FAILED), itemlist_failed); // 失败

            // 电源申压文件
            helper.OutputPowerV(Path.Combine(path_out, Config.F_PowerV), gloves);

            // 压缩
            List<SingleItem> itemlist_avg = new List<SingleItem>();
            List<SingleItem> itemlist_compressed = new List<SingleItem>();
            helper.Compress(itemlist_preprocessed, itemlist_compressed, itemlist_avg);

            helper.Output(Path.Combine(path_out, Config.F_DEBUG_CUM), itemlist_compressed);
            helper.Output(Path.Combine(path_out, Config.F_DEBUG_AVG), itemlist_avg);

            // 节段电压文件
            helper.OutputNodeV(Path.Combine(path_out, Config.F_NodeV), itemlist_compressed);

            // 节段温度文件
            helper.OutputNodeT(Path.Combine(path_out, Config.F_NodeT), itemlist_compressed);

            // Result1
            helper.OutputResult1(Path.Combine(path_out, Config.F_RES1), itemlist_compressed, is_for_H);

            // Result2
            helper.OutputResult2(Path.Combine(path_out, Config.F_RES2), itemlist_compressed, is_for_H);

            // Result3
            helper.OutputResult3(Path.Combine(path_out, Config.F_RES3), itemlist_compressed, is_for_H);

            MessageBox.Show("结束！");
        }

        private void ProcessV2(string path, bool is_for_H = true)
        {
            string path_out = path + Config.F_OUT_SUFFIX;
            if (!Directory.Exists(path_out))
            {
                // create output
                Directory.CreateDirectory(path_out);
            }
            else
            {
                // clear output
                Directory.Delete(path_out);
            }

            // GlobalItem首行记录，SingleItem单行详细内容记录
            List<GlobalItem> gloves = new List<GlobalItem>();
            ProcessorV2 helper = new ProcessorV2();

            Queue<SingleItem> queue = new Queue<SingleItem>();

            DirectoryInfo thefolder = new DirectoryInfo(path);
            foreach (FileInfo file in thefolder.GetFiles())
            {
                var filepath = file.DirectoryName + @"\" + file.Name;
                GlobalItem glove = new GlobalItem();
                bool first_start = true;
                using (StreamReader reader = new StreamReader(filepath))
                {
                    string line = "";
                    while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
                    {
                        if (line.StartsWith("node_totalnum"))
                        {
                            glove = new GlobalItem();
                            glove.Initial(line);
                            gloves.Add(glove);
                            continue;
                        }
                        if (queue.Count > Config.CUMULATE_COUNT) // q.Count > 100
                        {
                            queue.Dequeue();
                        }

                        SingleItem item = new SingleItem();
                        if (!item.Initial(glove.NodeTotalNum, line))
                        {
                            continue;
                        }

                        if (!helper.Preprocess(0, queue.ToList(), first_start, is_for_H))
                        {
                            continue;
                        }

                        if (!item.flag)
                        {
                            queue.Enqueue(item);
                        }


                        if (string.IsNullOrWhiteSpace(glove.Key))
                        {
                            glove.Key = $"{item.Date},{item.Hour},0,0";
                        }

                        helper.OutputPowerV(filename: Path.Combine(path_out, Config.F_PowerV),
                                            glove: glove,
                                            first: first_start,
                                            append: true);
                        first_start = false;
                    }
                }

                //List<SingleItem> itemlist = new List<SingleItem>();
                

                // 加载文件
                //helper.LoadFile(path, gloves, itemlist);

                //List<SingleItem> itemlist_preprocessed = new List<SingleItem>(); // V 合格的行
                //List<SingleItem> itemlist_failed = new List<SingleItem>(); // V 合格的行
                                                                           // 预处理文件
                //helper.Preprocess(itemlist, itemlist_preprocessed, itemlist_failed, is_for_H);
                //helper.Output(Path.Combine(path_out, Config.F_PRE), itemlist_preprocessed); // 成功
                //helper.Output(Path.Combine(path_out, Config.F_FAILED), itemlist_failed); // 失败

                // 电源申压文件
                //helper.OutputPowerV(Path.Combine(path_out, Config.F_PowerV), gloves);

                // 压缩
                //List<SingleItem> itemlist_avg = new List<SingleItem>();
                //List<SingleItem> itemlist_compressed = new List<SingleItem>();
                helper.Compress(itemlist_preprocessed, itemlist_compressed, itemlist_avg);

                helper.Output(Path.Combine(path_out, Config.F_DEBUG_CUM), itemlist_compressed);
                helper.Output(Path.Combine(path_out, Config.F_DEBUG_AVG), itemlist_avg);

                // 节段电压文件
                helper.OutputNodeV(Path.Combine(path_out, Config.F_NodeV), itemlist_compressed);

                // 节段温度文件
                helper.OutputNodeT(Path.Combine(path_out, Config.F_NodeT), itemlist_compressed);

                // Result1
                helper.OutputResult1(Path.Combine(path_out, Config.F_RES1), itemlist_compressed, is_for_H);

                // Result2
                helper.OutputResult2(Path.Combine(path_out, Config.F_RES2), itemlist_compressed, is_for_H);

                // Result3
                helper.OutputResult3(Path.Combine(path_out, Config.F_RES3), itemlist_compressed, is_for_H);

                MessageBox.Show("结束！");
            }
        }

        private void 文件导入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog P_File_Folder = new FolderBrowserDialog();
            if (P_File_Folder.ShowDialog() == DialogResult.OK)
            {
                path = P_File_Folder.SelectedPath;
                MessageBox.Show("文件导入成功！");

                // 处理文件
                Process(path, is_for_H: false);
            }
        }
    }
    

}
