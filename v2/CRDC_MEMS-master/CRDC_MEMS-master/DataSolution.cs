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
                bool is_for_H = true;
                // 处理文件
                switch (Config.Version)
                {
                    case "v1": Process(path, is_for_H: is_for_H); break;
                    case "v2": ProcessV2(path, is_for_H: is_for_H); break;
                    case "v3": ProcessV3(path, is_for_H: is_for_H); break;
                    case "v4": ProcessV4(path, is_for_H: is_for_H); break;
                    default: Process(path, is_for_H: is_for_H); break;
                }
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
            ProcessorV2 helper = new ProcessorV2();
            string path_out = path + Config.F_OUT_SUFFIX;
            if (Directory.Exists(path_out))
            {
                //Directory.CreateDirectory(path_out);
                //Directory.Delete(path_out);
                helper.DeleteDirectory(path_out);
            }
            Directory.CreateDirectory(path_out);

            // GlobalItem首行记录，SingleItem单行详细内容记录
            List<GlobalItem> gloves = new List<GlobalItem>();
            Queue<SingleItem> queue_src = new Queue<SingleItem>(); // 原始
            Queue<SingleItem> queue = new Queue<SingleItem>(); // 压缩后
            DirectoryInfo thefolder = new DirectoryInfo(path);

            bool first_start = true; //原始文件的第一行
            bool first_start_cum = true; //累积文件的第一行
            string last_key = string.Empty;
            SingleItem last_item = new SingleItem();
            int last_count = 0; // 对于当前时段，总共有多少行 -> 同一秒合并时用
            DateTime last_dt = DateTime.Today;
            foreach (FileInfo file in thefolder.GetFiles())
            {
                var filepath = file.DirectoryName + @"\" + file.Name;
                GlobalItem glove = new GlobalItem();
                
                using (StreamReader reader = new StreamReader(filepath))
                {
                    string line = "";
                    while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
                    {
                        if (line.StartsWith("node_totalnum"))
                        {
                            glove = new GlobalItem();
                            glove.Initial(line);
                            var raw_key = file.Name.Replace(".txt", "");
                            glove.Key = $"{raw_key.Substring(0,8)},{raw_key.Substring(8,2)},0,0";
                            helper.OutputPowerV(filename: Path.Combine(path_out, Config.F_PowerV),
                                                glove: glove,
                                                first: first_start,
                                                append: true);
                            gloves.Add(glove);
                            continue;
                        }
                        if (queue.Count > Config.CUMULATE_COUNT) // q.Count > 100
                        {
                            queue.Dequeue();
                            queue_src.Dequeue();
                        }

                        SingleItem item = new SingleItem();
                        if (!item.Initial(glove.NodeTotalNum, line))
                        {
                            continue;
                        }

                        //将原始文件压入队列进行预处理：上下左右检验，每次只要检验队列的最后一行即可
                        queue_src.Enqueue(item);
                        if (!helper.Preprocess(queue_src.Count - 1, queue_src.ToList(), first_start, is_for_H))
                        {
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(glove.Key))
                        {
                            glove.Key = $"{item.Date},{item.Hour},0,0";
                        }

                        if (first_start)
                        {
                            last_dt = item.Time;
                            first_start = false;
                        }

                        // 合并
                        var key = $"{item.Hour}_{item.Minitue}_{item.Second}"; // 时_分_秒

                        // 第一行
                        if (string.IsNullOrWhiteSpace(last_key))
                        {
                            last_key = key;
                            last_item = (SingleItem)item.ShallowCopy();
                            last_count += 1;
                            continue;
                        }

                        // 当前key和上一行不一样，表示上一行的合并结束，开始初始化新行的合并,放入队列中
                        if (last_key != key)
                        {
                            last_item.Voltage = Math.Round(1.0 * last_item.Voltage / last_count, 2);
                            var item_avg = (SingleItem)last_item.ShallowCopy();
                            for (int k = 0; k < last_item.nodes.Count; k++)
                            {
                                var node = last_item.nodes[k];
                                node.Temp = Math.Round(1.0 * node.Temp / last_count, 0);
                                node.X = Math.Round(1.0 * node.X / last_count, 0);
                                node.Y = Math.Round(1.0 * node.Y / last_count, 0);
                                node.Z = Math.Round(1.0 * node.Z / last_count, 0);
                                node.MagX = Math.Round(1.0 * node.MagX / last_count, 0);
                                node.MagY = Math.Round(1.0 * node.MagY / last_count, 0);
                                node.MagZ = Math.Round(1.0 * node.MagZ / last_count, 0);
                                node.PitchAngle = Math.Round(1.0 * node.PitchAngle / last_count, 0);
                                node.RollAngle = Math.Round(1.0 * node.RollAngle / last_count, 0);
                                node.CourseAngle = Math.Round(1.0 * node.CourseAngle / last_count, 0);
                                item_avg.nodes[k] = (Node)node.ShallowCopy();
                            }

                            queue.Enqueue(item_avg);

                            // 输出合并结果
                            helper.Output(Path.Combine(path_out, Config.F_DEBUG_AVG), item_avg, append: true);
                            // 输出累计结果
                            SingleItem cumItem = helper.Cumulate(itemlist: queue.ToList(),
                                                                start: 0,
                                                                end: queue.Count - 1);
                            helper.Output(Path.Combine(path_out, Config.F_DEBUG_CUM), cumItem, append: true);
                            
                            // 节段电压文件
                            helper.OutputNodeV(Path.Combine(path_out, Config.F_NodeV), cumItem, append: true);
                            // 节段温度文件
                            helper.OutputNodeT(Path.Combine(path_out, Config.F_NodeT), cumItem, append: true);
                            // Result1
                            helper.OutputResult1(Path.Combine(path_out, Config.F_RES1), cumItem, is_for_H, append: true);
                            // Result2
                            helper.OutputResult2(Path.Combine(path_out, Config.F_RES2), cumItem, is_for_H, append: true);

                            // Result3
                            //第一条记录，或者不足时间间隔（5min）
                            if (first_start_cum || (cumItem.Time - last_dt).TotalSeconds >= Config.TIME_INTERVAL)
                            {
                                helper.OutputResult3(filename: Path.Combine(path_out, Config.F_RES3), 
                                                     item: cumItem, 
                                                     first: first_start_cum,
                                                     is_for_H: is_for_H, 
                                                     append: true);
                                last_dt = item.Time;
                            }
                            first_start_cum = false;

                            last_key = key;
                            last_item = (SingleItem)item.ShallowCopy();
                            last_count = 1;
                            continue;
                        }

                        // 当前的key和上一个key一样，表示当前合并尚未结束，则继续累计以求均值
                        if (last_item.nodes.Count != item.nodes.Count)
                        {
                            continue;
                        }

                        last_item.Voltage += item.Voltage;
                        for (int k = 0; k < last_item.nodes.Count; k++)
                        {
                            last_item.nodes[k].Temp += item.nodes[k].Temp;
                            last_item.nodes[k].X += item.nodes[k].X;
                            last_item.nodes[k].Y += item.nodes[k].Y;
                            last_item.nodes[k].Z += item.nodes[k].Z;
                            last_item.nodes[k].MagX += item.nodes[k].MagX;
                            last_item.nodes[k].MagY += item.nodes[k].MagY;
                            last_item.nodes[k].MagZ += item.nodes[k].MagZ;
                            last_item.nodes[k].PitchAngle += item.nodes[k].PitchAngle;
                            last_item.nodes[k].RollAngle += item.nodes[k].RollAngle;
                            last_item.nodes[k].CourseAngle += item.nodes[k].CourseAngle;
                        }

                        last_count += 1;
                    }
                }
            }
            MessageBox.Show("结束！");
        }

        private void ProcessV3(string path, bool is_for_H = true)
        {
            string path_out = path + Config.F_OUT_SUFFIX;
            if (!Directory.Exists(path_out))
            {
                Directory.CreateDirectory(path_out);
            }

            //新建类
            Processor helper = new Processor();

            // 加载文件
            DirectoryInfo thefolder = new DirectoryInfo(path);
            bool isFirst = true;
            foreach (FileInfo file in thefolder.GetFiles())
            {
                // GlobalItem首行记录，SingleItem单行详细内容记录
                List<GlobalItem> gloves = new List<GlobalItem>();
                List<SingleItem> itemlist = new List<SingleItem>();
                var filepath = file.DirectoryName + @"\" + file.Name;
                helper.LoadFileV3(filepath, gloves, itemlist);                      //加载所有数据

                List<SingleItem> itemlist_preprocessed = new List<SingleItem>();    // 合格的行
                List<SingleItem> itemlist_failed = new List<SingleItem>();          // 不合格的行
                                                                           
                helper.Preprocess(itemlist, itemlist_preprocessed, itemlist_failed, is_for_H);                  // 预处理文件
                helper.Output(Path.Combine(path_out, Config.F_PRE), itemlist_preprocessed, append: !isFirst);   // 成功
                helper.Output(Path.Combine(path_out, Config.F_FAILED), itemlist_failed, append: !isFirst);      // 失败

                // 电源申压文件
                helper.OutputPowerV(Path.Combine(path_out, Config.F_PowerV), gloves, append: !isFirst);

                // 压缩
                List<SingleItem> itemlist_avg = new List<SingleItem>();
                List<SingleItem> itemlist_compressed = new List<SingleItem>();
                helper.Compress(itemlist_preprocessed, itemlist_compressed, itemlist_avg);

                helper.Output(Path.Combine(path_out, Config.F_DEBUG_CUM), itemlist_compressed, append: !isFirst);
                helper.Output(Path.Combine(path_out, Config.F_DEBUG_AVG), itemlist_avg, append: !isFirst);

                // 节段电压文件
                helper.OutputNodeV(Path.Combine(path_out, Config.F_NodeV), itemlist_compressed, append: !isFirst);

                // 节段温度文件
                helper.OutputNodeT(Path.Combine(path_out, Config.F_NodeT), itemlist_compressed, append: !isFirst);

                // Result1
                helper.OutputResult1(Path.Combine(path_out, Config.F_RES1), itemlist_compressed, is_for_H, append: !isFirst);

                // Result2
                helper.OutputResult2(Path.Combine(path_out, Config.F_RES2), itemlist_compressed, is_for_H, append: !isFirst);

                // Result3
                helper.OutputResult3(Path.Combine(path_out, Config.F_RES3), itemlist_compressed, is_for_H, append: !isFirst);

                isFirst = false;
            }
            MessageBox.Show("结束！");
        }

        private void ProcessV4(string path, bool is_for_H = true)
        {
            ProcessorV4 helper = new ProcessorV4();
            string path_out = path + Config.F_OUT_SUFFIX;
            if (Directory.Exists(path_out))
            {
                helper.DeleteFilesInDirectory(path_out);
            }
            else
            {
                Directory.CreateDirectory(path_out);
            }

            // GlobalItem首行记录，SingleItem单行详细内容记录
            List<GlobalItem> gloves = new List<GlobalItem>();
            Queue<SingleItem> queue_src = new Queue<SingleItem>(); // 原始
            Queue<SingleItem> queue = new Queue<SingleItem>(); // 压缩后
            DirectoryInfo thefolder = new DirectoryInfo(path);

            bool first_start = true; //原始文件的第一行
            bool first_start_cum = true; //累积文件的第一行，最终文件
            string last_key = string.Empty;
            SingleItem last_item = new SingleItem();
            int last_count = 0; // 对于当前时段，总共有多少行 -> 同一秒合并时用
            DateTime last_dt = DateTime.Today;

            //List<SingleItem> itemsForAvg = new List<SingleItem>(); // 100个大小
            List<SingleItem> itemsForCum = new List<SingleItem>(); // 100个大小
            List<SingleItem> itemsForPre = new List<SingleItem>(); // 用于放预处理，空间大小为2*Config.N_OR_ROW+1=3
            
            // 逐行读取
            foreach (FileInfo file in thefolder.GetFiles())
            {
                var filepath = file.DirectoryName + @"\" + file.Name;
                GlobalItem glove = new GlobalItem();

                using (StreamReader reader = new StreamReader(filepath))
                {
                    string line = "";
                    while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
                    {
                        //Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"read {line}");
                        //Console.ResetColor();
                        if (line.StartsWith("node_totalnum"))
                        {
                            // glove输出申源电压
                            glove = new GlobalItem();
                            glove.Initial(line);
                            // 从文件名中截取时分秒
                            var raw_key = file.Name.Replace(".txt", "");
                            glove.Key = $"{raw_key.Substring(0, 8)},{raw_key.Substring(8, 2)},0,0";
                            helper.OutputPowerV(filename: Path.Combine(path_out, Config.F_PowerV),
                                                glove: glove,
                                                first: first_start);
                            gloves.Add(glove);
                            continue;
                        }

                        // 初始化，line -> SingleItem
                        SingleItem item = new SingleItem();
                        if (!item.Initial(glove.NodeTotalNum, line))
                        {
                            continue;
                        }

                        itemsForPre.Add(item);
                        // 边界处理，至少2行（N_OF_ROW+1），比如第一行，需要向下对比1行
                        if (itemsForPre.Count < Config.N_OF_ROW + 1)
                        {
                            continue;
                        }

                        // 如果是两行，则不预处理，直接加入首行，index是待预处理的行，当前是第1行，则预处理第0行
                        int index = 0;
                        // 预处理，上下左右检验
                        if (itemsForPre.Count > Config.N_OF_ROW + 1)
                        {
                            int last = itemsForPre.Count - 1;
                            int first = 0;
                            index = last - Config.N_OF_ROW;
                            if (!helper.Preprocess(itemlist: itemsForPre,
                                                    first: first,
                                                    last: last,
                                                    index: index,
                                                    is_for_H: is_for_H))
                            {
                                itemsForPre.RemoveAt(index);
                            }
                        }
                        
                        // 预处理成功，则待加入同一秒合并
                        var candidate = (SingleItem)itemsForPre[index].ShallowCopy();
                        var candidate_key = $"{candidate.Hour}_{candidate.Minitue}_{candidate.Second}"; // 时_分_秒
                        Console.WriteLine($"candiate is {index}: {candidate.ItemToString()}");

                        if (first_start)
                        {
                            last_dt = candidate.Time; ////////////////////////////////////
                        }

                        // 当前key和上一行不一样，表示上一行的合并结束，开始初始化新行的合并
                        var key = $"{item.Hour}_{item.Minitue}_{item.Second}"; // 时_分_秒
                        if (!string.IsNullOrWhiteSpace(last_key) && last_key != candidate_key)
                        {
                            Console.WriteLine($"last_key={last_key},candidate_key={candidate_key},current_key={item.Hour}_{item.Minitue}_{item.Second}");
                            last_item.Voltage = Math.Round(1.0 * last_item.Voltage / last_count, 2);
                            var item_avg = (SingleItem)last_item.ShallowCopy();
                            for (int k = 0; k < last_item.nodes.Count; k++)
                            {
                                var node = last_item.nodes[k];
                                node.Temp = Math.Round(1.0 * node.Temp / last_count, 4);
                                node.X = Math.Round(1.0 * node.X / last_count, 4);
                                node.Y = Math.Round(1.0 * node.Y / last_count, 4);
                                node.Z = Math.Round(1.0 * node.Z / last_count, 4);
                                node.MagX = Math.Round(1.0 * node.MagX / last_count, 4);
                                node.MagY = Math.Round(1.0 * node.MagY / last_count, 4);
                                node.MagZ = Math.Round(1.0 * node.MagZ / last_count, 4);
                                node.PitchAngle = Math.Round(1.0 * node.PitchAngle / last_count, 4);
                                node.RollAngle = Math.Round(1.0 * node.RollAngle / last_count, 4);
                                node.CourseAngle = Math.Round(1.0 * node.CourseAngle / last_count, 4);
                                item_avg.nodes[k] = (Node)node.ShallowCopy();
                            }
                            // 输出合并结果
                            helper.Output(filename: Path.Combine(path_out, Config.F_DEBUG_AVG), item: item_avg);

                            // 累计前100行结果
                            // 滑动重置，从后往前挪，然后删除最后一个
                            if (itemsForCum.Count >= Config.CUMULATE_COUNT)
                            {
                                int cum_id = 0;
                                for (; cum_id < itemsForCum.Count - 1; cum_id++)
                                {
                                    itemsForCum[cum_id] = (SingleItem)itemsForCum[cum_id + 1].ShallowCopy();
                                }
                                itemsForCum.RemoveAt(cum_id);
                            }
                            itemsForCum.Add(item_avg);
                            helper.Cumulate(itemlist: itemsForCum,
                                            start: 0,
                                            end: itemsForCum.Count - 1);
                            // 输出累计结果
                            helper.Output(filename: Path.Combine(path_out, Config.F_DEBUG_CUM), item: itemsForCum.Last());

                            var cumItem = itemsForCum.Last();
                            // 节段电压文件
                            helper.OutputNodeV(Path.Combine(path_out, Config.F_NodeV), cumItem);
                            // 节段温度文件
                            helper.OutputNodeT(Path.Combine(path_out, Config.F_NodeT), cumItem);
                            // Result1
                            helper.OutputResult1(Path.Combine(path_out, Config.F_RES1), cumItem, is_for_H);
                            // Result2
                            helper.OutputResult2(Path.Combine(path_out, Config.F_RES2), cumItem, is_for_H);
                            // Result3
                            //第一条记录，或者不足时间间隔（5min）
                            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!");
                            Console.WriteLine($"cumItem.Time:{cumItem.Time},last_dt:{last_dt}");
                            if (first_start_cum || (cumItem.Time - last_dt).TotalSeconds >= Config.TIME_INTERVAL)
                            {
                                helper.OutputResult3(filename: Path.Combine(path_out, Config.F_RES3),
                                                     item: cumItem,
                                                     is_for_H: is_for_H);
                                last_dt = cumItem.Time;
                            }
                            first_start_cum = false;

                            last_key = key;
                            last_item = (SingleItem)candidate.ShallowCopy();
                            last_count = 1;
                            continue;
                        }

                        // 合并
                        if (string.IsNullOrWhiteSpace(last_key))
                        {
                            last_key = candidate_key;
                            last_item = (SingleItem)candidate.ShallowCopy();
                            last_count += 1;
                            Console.WriteLine($"add item {candidate.ItemToString()}");
                            continue;
                        }

                        last_item.Voltage += candidate.Voltage;
                        for (int k = 0; k < last_item.nodes.Count; k++)
                        {
                            last_item.nodes[k].Temp += candidate.nodes[k].Temp;
                            last_item.nodes[k].X += candidate.nodes[k].X;
                            last_item.nodes[k].Y += candidate.nodes[k].Y;
                            last_item.nodes[k].Z += candidate.nodes[k].Z;
                            last_item.nodes[k].MagX += candidate.nodes[k].MagX;
                            last_item.nodes[k].MagY += candidate.nodes[k].MagY;
                            last_item.nodes[k].MagZ += candidate.nodes[k].MagZ;
                            last_item.nodes[k].PitchAngle += candidate.nodes[k].PitchAngle;
                            last_item.nodes[k].RollAngle += candidate.nodes[k].RollAngle;
                            last_item.nodes[k].CourseAngle += candidate.nodes[k].CourseAngle;
                        }
                        Console.WriteLine($"add item {candidate.ItemToString()}");
                        last_count += 1;

                        //如果超过了2*N_OR_ROW+1，则重置itemsForPre
                        if (itemsForPre.Count >= 2 * Config.N_OF_ROW + 1)
                        {
                            int pre_id = 0;
                            for (; pre_id < itemsForPre.Count - 1; pre_id++)
                            {
                                itemsForPre[pre_id] = (SingleItem)itemsForPre[pre_id + 1].ShallowCopy();
                            }
                            itemsForPre.RemoveAt(pre_id);
                        }
                    }
                }
            }
            MessageBox.Show("结束！");
        }

        private void 文件导入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog P_File_Folder = new FolderBrowserDialog();
            if (P_File_Folder.ShowDialog() == DialogResult.OK)
            {
                path = P_File_Folder.SelectedPath;
                MessageBox.Show("文件导入成功！");

                // 处理文件
                var is_for_H = false;
                switch (Config.Version)
                {
                    case "v1": Process(path, is_for_H: is_for_H); break;
                    case "v2": ProcessV2(path, is_for_H: is_for_H); break;
                    case "v3": ProcessV3(path, is_for_H: is_for_H); break;
                    case "v4": ProcessV4(path, is_for_H: is_for_H); break;
                    default: Process(path, is_for_H: is_for_H); break;
                }
            }
        }
    }
    

}
