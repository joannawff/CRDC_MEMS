/***
 * Processor.cs 是公共的处理类，兼容水平和竖直解算，通过布尔类型参数is_for_H来控制（true=水平，false=竖直），基本包含以下操作：
 * 1. 加载文件LoadFile，逐行加载，获取List<GlobalItem>和List<SingleItem>
 * 2. 预处理Preprocess，解算过程，每行前后对比，前后行对比，合格行放入itemlist_filtered，失败行放入itemlist_failed
 * 2. 压缩Compress，压缩过程，包含两个步骤：
 *  1） 同一时间（精确到秒）内取平均，
 *  2）每行是前N行（目前取前100行，由Config.CUMULATE_COUNT控制）的累计之和
 * 3. 输出，包括7个输出函数：
 *  1）通用Output，逐行输出预处理和压缩文件
 *  2）电源申压，OutputPowerV，取值来源GolbalItem
 *  3）节段电压，OutputNodeV，取值来源压缩文件
 *  4）节段温度，OutputNodeT，目前用X代替，取值来源压缩文件
 *  5）OutputResult1，根据水平（Z）或竖直（X，Y）筛选列，取值来源压缩文件
 *  6）OutputResult2，X或Z形变取值来源压缩文件
 *  7）OutputResult3，X或Z形变，一定时间间隔（目前取300s=5min，由Config.TIME_INTERVAL控制）输出，取值来源压缩文件
 * */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRDC_MEMS
{
    public class Processor
    {
        // 加载文件
        public void LoadFileV3(string filepath, List<GlobalItem> gloves, List<SingleItem> itemlist)
        {
            if (itemlist == null)
            {
                itemlist = new List<SingleItem>();
            }

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
                        gloves.Add(glove);
                        continue;
                    }

                    SingleItem item = new SingleItem();
                    if (item.Initial(glove.NodeTotalNum, line))
                    {
                        itemlist.Add(item);
                    }
                    //读取第一行属性的时候附时间使用
                    if (string.IsNullOrWhiteSpace(glove.Key))
                    {
                        glove.Key = $"{item.Date},{item.Hour},0,0";
                    }
                }
            }
        }

        // 加载文件
        public void LoadFile(string path, List<GlobalItem> gloves, List<SingleItem> itemlist)
        {
            if (itemlist == null)
            {
                itemlist = new List<SingleItem>();
            }

            DirectoryInfo thefolder = new DirectoryInfo(path);
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
                            gloves.Add(glove);
                            continue;
                        }
                        
                        SingleItem item = new SingleItem();
                        if (item.Initial(glove.NodeTotalNum, line))
                        {
                            itemlist.Add(item);
                        }

                        if (string.IsNullOrWhiteSpace(glove.Key))
                        {
                            glove.Key = $"{item.Date},{item.Hour},0,0";
                        }
                    }
                }
            }
        }

        // 
        public bool PreprocessSignalLine(int i, List<SingleItem> itemlist, bool first_start, bool is_for_H = true)
        {
            var item_current = itemlist[i];
            bool pass = true;
            //// 前后列对比
            for (int k = 0; k < item_current.nodes.Count; k++)
            {
                var node = item_current.nodes[k];
                if (k < item_current.nodes.Count - 1 && node.Id >= item_current.nodes[k + 1].Id)
                {
                    return false;
                }
            }

            //如果是第一个可以输出行，直接输出
            if (first_start)
            {
                return true;
            }

            //如果改行Z轴数量不符，输出到错误列表
            //如果改行Z轴数量不符，输出到错误列表

            Dictionary<int, int> value_gaps_x = new Dictionary<int, int>();
            Dictionary<int, int> value_gaps_y = new Dictionary<int, int>();
            Dictionary<int, int> value_gaps_z = new Dictionary<int, int>();

            // 前后行对比，从当前行往前推 Config.N_OF_ROW 行，到往后推 Config.N_OF_ROW 行
            for (int j = i - Config.N_OF_ROW; j <= i + Config.N_OF_ROW && j < itemlist.Count; j++)
            {
                // 待对比行
                var item_compare = itemlist[j];
                if (i == j)
                {
                    continue;
                }
                for (int k = 0; k < item_current.nodes.Count; k++)
                {
                    //？前如果成立返回：前的值，如果不成立返回：后的值
                    //bool block_x = Math.Abs(item_current.nodes[k].X - item_compare.nodes[k].X) >= Config.GAP_THRESHOLD ? true : false;
                    //bool block_y = Math.Abs(item_current.nodes[k].Y - item_compare.nodes[k].Y) >= Config.GAP_THRESHOLD ? true : false;
                    //bool block_z = Math.Abs(item_current.nodes[k].Z - item_compare.nodes[k].Z) >= Config.GAP_THRESHOLD ? true : false;

                    int type_x = item_current.nodes[k].X - item_compare.nodes[k].X >= Config.GAP_THRESHOLD ? 1 :
                                item_compare.nodes[k].X - item_current.nodes[k].X >= Config.GAP_THRESHOLD ? 2 : 3;
                    int type_y = item_current.nodes[k].Y - item_compare.nodes[k].Y >= Config.GAP_THRESHOLD ? 1 :
                                item_compare.nodes[k].Y - item_current.nodes[k].Y >= Config.GAP_THRESHOLD ? 2 : 3;
                    int type_z = item_current.nodes[k].Z - item_compare.nodes[k].Z >= Config.GAP_THRESHOLD ? 1 :
                                item_compare.nodes[k].Z - item_current.nodes[k].Z >= Config.GAP_THRESHOLD ? 2 : 3;

                    if (is_for_H) // 水平看Z轴
                    {
                        if (!value_gaps_z.ContainsKey(k))
                        {
                            value_gaps_z.Add(k, type_z);
                        }
                        else if (value_gaps_z[k] == type_z && type_z != 3)
                        {
                            pass = false;
                            break;
                        }
                    }
                    else // 竖直看X和Y轴
                    {
                        if (!value_gaps_x.ContainsKey(k) || !value_gaps_y.ContainsKey(k))
                        {
                            value_gaps_x.Add(k, type_x);
                            value_gaps_y.Add(k, type_y);
                        }
                        else if ((value_gaps_x[k] == type_x && type_x != 3) ||
                                    (value_gaps_y[k] == type_y && type_y != 3))
                        {
                            pass = false;
                            break;
                        }
                    }
                }
            }
            item_current.flag = true;
            return pass;
        }

        // 预处理
        public void Preprocess(List<SingleItem> itemlist, List<SingleItem> itemlist_filtered, List<SingleItem> itemlist_failed, bool is_for_H = true)
        {
            bool first_start = true;
            for (int i = 0; i < itemlist.Count; i++)
            {
                // 当前行（包括很多节点数据）
                var item_current = itemlist[i];
                
                // 每行的前后列ID序号检验
                bool pass = true;
                for (int k = 0; k < item_current.nodes.Count; k++)
                {
                    //当前行当前节段的数据
                    var node = item_current.nodes[k];
                    //当前行的当前节段值不等于对应序号，则处理
                    if (item_current.nodes[k].Id != k + 1)
                    {
                        pass = false;
                        break;
                    }
                    //if (k < item_current.nodes.Count && node.Id - 1 >= item_current.nodes[k + 1].Id)
                    //{
                    //    pass = false;
                    //    break;
                    //}
                }

                // 节段号出现问题的进行删除
                if (!pass)
                {
                    itemlist_failed.Add(item_current);
                    continue;
                }

                // 如果是第一个可以输出行，直接输出
                if (first_start)
                {
                    itemlist_filtered.Add(item_current);
                    first_start = false;
                    continue;
                }

                Dictionary<int, int> value_gaps_x = new Dictionary<int, int>();
                Dictionary<int, int> value_gaps_y = new Dictionary<int, int>();
                Dictionary<int, int> value_gaps_z = new Dictionary<int, int>();
                
                //***此处对比行数有问题***//
                
                // 前后行对比，从当前行往前推 Config.N_OF_ROW 行，到往后推 Config.N_OF_ROW 行
                for (int j = i - Config.N_OF_ROW; j <= i + Config.N_OF_ROW && j < itemlist.Count; j++)
                {
                    // 待对比行
                    var item_compare = itemlist[j]; 
                    // 过滤掉当前行
                    if (i == j)
                    {
                        continue;
                    }
                    // 当前行的每个数据进行提取
                    for (int k = 0; k < item_current.nodes.Count; k++)
                    {
                        int type_x = item_current.nodes[k].X - item_compare.nodes[k].X >= Config.GAP_THRESHOLD ? 1 :
                                    item_compare.nodes[k].X - item_current.nodes[k].X >= Config.GAP_THRESHOLD ? 2 : 3;
                        int type_y = item_current.nodes[k].Y - item_compare.nodes[k].Y >= Config.GAP_THRESHOLD ? 1 :
                                    item_compare.nodes[k].Y - item_current.nodes[k].Y >= Config.GAP_THRESHOLD ? 2 : 3;
                        int type_z = item_current.nodes[k].Z - item_compare.nodes[k].Z >= Config.GAP_THRESHOLD ? 1 :
                                    item_compare.nodes[k].Z - item_current.nodes[k].Z >= Config.GAP_THRESHOLD ? 2 : 3;

                        if (is_for_H)   // 水平看Z轴
                        {
                            if (!value_gaps_z.ContainsKey(k))
                            {
                                value_gaps_z.Add(k, type_z);
                            }
                            else if (value_gaps_z[k] == type_z && type_z != 3)//？？？？？
                            {
                                pass = false;
                                break;
                            }
                        }
                        else            // 竖直看X和Y轴
                        {
                            if (!value_gaps_x.ContainsKey(k) || !value_gaps_y.ContainsKey(k))
                            {
                                value_gaps_x.Add(k, type_x);
                                value_gaps_y.Add(k, type_y);
                            }
                            else if ((value_gaps_x[k] == type_x && type_x != 3) ||
                                        (value_gaps_y[k] == type_y && type_y != 3))
                            {
                                pass = false;
                                break;
                            }
                           
                        }
                    }
                }

                // 存储水平合格行
                if (pass)
                {
                    itemlist_filtered.Add(item_current);
                }
                else
                {
                    itemlist_failed.Add(item_current);
                    Console.WriteLine($"Failed to pass: {item_current.ItemToString()}");
                }
            }
        }

        // 按时间压缩，itemlist_avg存储按照时间压缩后的数据，itemlist_filtered存储按时间压缩后并滤波后的数据
        public void Compress(List<SingleItem> itemlist, List<SingleItem> itemlist_filtered, List<SingleItem> itemlist_avg)
        {
            HashSet<string> keys = new HashSet<string>(); // 时_分_秒
            string last_key = string.Empty;
            SingleItem last_item = new SingleItem();

            int last_count = 0; // 对于当前时段，总共有多少行 -> 同一秒合并时用
            //int total_count = 0; // 总共累计了多少合并航 -> 前100行使用

            for (int i = 0; i < itemlist.Count; i++)
            {
                // 获取当前行的内容和时间KEY
                var item = itemlist[i];
                var key = $"{item.Hour}_{item.Minitue}_{item.Second}";

                // 第一行
                if (string.IsNullOrWhiteSpace(last_key))
                {
                    last_key = key;
                    last_item = (SingleItem)item.ShallowCopy();             //？？？？
                    last_count += 1;
                    continue;
                }

                // 当前key和上一行不一样，表示上一行的合并结束，开始初始化新行的合并
                if (last_key != key)
                {
                    last_item.Voltage = Math.Round(1.0 * last_item.Voltage / last_count, 2);
                    var item_avg = (SingleItem)last_item.ShallowCopy();
                    for (int k = 0; k < last_item.nodes.Count; k++)
                    {
                        // 把累计后的值取出进行平均处理
                        var node = last_item.nodes[k];
                        node.X = Math.Round(1.0 * node.X / last_count, 4);
                        node.Y = Math.Round(1.0 * node.Y / last_count, 4);
                        node.Z = Math.Round(1.0 * node.Z / last_count, 4);
                        node.MagX = Math.Round(1.0 * node.MagX / last_count, 4);
                        node.MagY = Math.Round(1.0 * node.MagY / last_count, 4);
                        node.MagZ = Math.Round(1.0 * node.MagZ / last_count, 4);
                        node.PitchAngle = Math.Round(1.0 * node.PitchAngle / last_count, 4);
                        node.RollAngle = Math.Round(1.0 * node.RollAngle / last_count, 4);
                        node.CourseAngle = Math.Round(1.0 * node.CourseAngle / last_count, 4);
                        node.Temp = Math.Round(1.0 * node.Temp / last_count, 4);
                        // 更新item_avg内的累计值为均值
                        item_avg.nodes[k] = (Node)node.ShallowCopy();
                    }
                    itemlist_avg.Add(item_avg);

                    //前100行求均值
                    itemlist_filtered.Add(Cumulate(itemlist_avg, itemlist_avg.Count - Config.CUMULATE_COUNT, itemlist_avg.Count - 1));

                    last_key = key;
                    last_item = (SingleItem)item.ShallowCopy();
                    last_count = 1;
                    //total_count += 1;

                    continue;
                }

                // 当前的key和上一个key一样，表示当前合并尚未结束，则继续累计以求均值
                if (last_item.nodes.Count != item.nodes.Count)      //？？？？
                {
                    continue;
                }

                // 每行的节段电压
                last_item.Voltage += item.Voltage;
                for (int k = 0; k < last_item.nodes.Count; k++)
                {
                    last_item.nodes[k].X += item.nodes[k].X;
                    last_item.nodes[k].Y += item.nodes[k].Y;
                    last_item.nodes[k].Z += item.nodes[k].Z;
                    last_item.nodes[k].MagX += item.nodes[k].MagX;
                    last_item.nodes[k].MagY += item.nodes[k].MagY;
                    last_item.nodes[k].MagZ += item.nodes[k].MagZ;
                    last_item.nodes[k].PitchAngle += item.nodes[k].PitchAngle;
                    last_item.nodes[k].RollAngle += item.nodes[k].RollAngle;
                    last_item.nodes[k].CourseAngle += item.nodes[k].CourseAngle;
                    last_item.nodes[k].Temp += item.nodes[k].Temp;
                }

                last_count += 1;
            }
        }

        // 使用相同时间压缩后的成果进行平均处理，累计前100行进行求取均值的结果
        public SingleItem Cumulate(List<SingleItem> itemlist, int start, int end)
        {
            // itemlist为每秒压缩后的数据
            SingleItem item_cumulate = new SingleItem();
            int left = start < 0 ? 0 : start;
            int right = end >= itemlist.Count ? itemlist.Count - 1 : end;

            for (int i = left; i <= right; i++)
            {
                var item = itemlist[i];
                if (item_cumulate.nodes.Count == 0)
                {
                    item_cumulate = (SingleItem)item.ShallowCopy();
                    item_cumulate.nodes = item.ShallowCopyForNodes();
                    continue;
                }

                item_cumulate.Date = item.Date;
                item_cumulate.Hour = item.Hour;
                item_cumulate.Minitue = item.Minitue;
                item_cumulate.Second = item.Second;
                item_cumulate.Time = item.Time;
                item_cumulate.Voltage += item.Voltage;

                for (int k = 0; k < item.nodes.Count; k++)
                {
                    item_cumulate.nodes[k].X += item.nodes[k].X;
                    item_cumulate.nodes[k].Y += item.nodes[k].Y;
                    item_cumulate.nodes[k].Z += item.nodes[k].Z;
                    item_cumulate.nodes[k].MagX += item.nodes[k].MagX;
                    item_cumulate.nodes[k].MagY += item.nodes[k].MagY;
                    item_cumulate.nodes[k].MagZ += item.nodes[k].MagZ;
                    item_cumulate.nodes[k].PitchAngle += item.nodes[k].PitchAngle;
                    item_cumulate.nodes[k].RollAngle += item.nodes[k].RollAngle;
                    item_cumulate.nodes[k].CourseAngle += item.nodes[k].CourseAngle;
                    item_cumulate.nodes[k].Temp += item.nodes[k].Temp;
                }
                //item_cumulate.nodes[k].X = item_cumulate.nodes[k].X / item.nodes.Count;

            }

            item_cumulate.Voltage = Math.Round(item_cumulate.Voltage / (right - left + 1), 2);
            for (int k = 0; k < item_cumulate.nodes.Count; k++)
            {
                item_cumulate.nodes[k].X = Math.Round(item_cumulate.nodes[k].X / (right - left + 1), 0);
                item_cumulate.nodes[k].Y = Math.Round(item_cumulate.nodes[k].Y / (right - left + 1), 0);
                item_cumulate.nodes[k].Z = Math.Round(item_cumulate.nodes[k].Z / (right - left + 1), 0);
                item_cumulate.nodes[k].MagX = Math.Round(item_cumulate.nodes[k].MagX / (right - left + 1), 0);
                item_cumulate.nodes[k].MagY = Math.Round(item_cumulate.nodes[k].MagY / (right - left + 1), 0);
                item_cumulate.nodes[k].MagZ = Math.Round(item_cumulate.nodes[k].MagZ / (right - left + 1), 0);
                item_cumulate.nodes[k].PitchAngle = Math.Round(item_cumulate.nodes[k].PitchAngle / (right - left + 1), 0);
                item_cumulate.nodes[k].RollAngle = Math.Round(item_cumulate.nodes[k].RollAngle / (right - left + 1), 0);
                item_cumulate.nodes[k].CourseAngle = Math.Round(item_cumulate.nodes[k].CourseAngle / (right - left + 1), 0);
                item_cumulate.nodes[k].Temp = Math.Round(item_cumulate.nodes[k].Temp / (right - left + 1), 0);
            }

            return item_cumulate;
        }

        // 通用的输出函数
        public void Output(string filename, List<SingleItem> itemlist, bool append = false)
        {
            using (StreamWriter sw = new StreamWriter(filename, append))
            {
                foreach (var item in itemlist)
                {
                    sw.WriteLine(item.ItemToString());
                }
            }
        }

        // 电源申压文件
        public void OutputPowerV(string filename, List<GlobalItem> gloves, bool append = false)
        {
            using (StreamWriter sw = new StreamWriter(filename, append))
            {
                if (!append)
                {
                    sw.WriteLine($"node_totalnum:{gloves[0].NodeTotalNum},Device_SerNumber:{gloves[0].Device_SerNumber}");
                }
                foreach (var item in gloves)
                {
                    sw.WriteLine($"{item.Key},PowerVoltage:{item.PowerVoltage}");
                }
            }
        }
        // 电源申压文件
        public void OutputPowerV(string filename, GlobalItem glove, bool first = false, bool append = true)
        {
            using (StreamWriter sw = new StreamWriter(filename, append))
            {
                if (first)
                {
                    sw.WriteLine($"node_totalnum:{glove.NodeTotalNum},Device_SerNumber:{glove.Device_SerNumber}");
                }
                else
                {
                    sw.WriteLine($"{glove.Key},PowerVoltage:{glove.PowerVoltage}");
                }
            }
        }


        // 节段电压文件
        public void OutputNodeV(string filename, List<SingleItem> itemlist, bool append = false)
        {
            using (StreamWriter sw = new StreamWriter(filename, append))
            {
                foreach (var item in itemlist)
                {
                    sw.WriteLine($"{item.Date},{item.Hour},{item.Minitue},{item.Second},{item.Voltage}");
                }
            }
        }

        // 节段温度文件
        public void OutputNodeT(string filename, List<SingleItem> itemlist, bool append = false)
        {
            using (StreamWriter sw = new StreamWriter(filename, append))
            {
                foreach (var item in itemlist)
                {
                    sw.WriteLine($"{item.Date},{item.Hour},{item.Minitue},{item.Second},{string.Join(",", item.nodes.Select(t => $"{t.Id},{t.Temp}"))}");
                }
            }
        }

        // Result1
        public void OutputResult1(string filename, List<SingleItem> itemlist, bool is_for_H = true, bool append = false)
        {
            using (StreamWriter sw = new StreamWriter(filename, append))
            {
                foreach (var item in itemlist)
                {
                    if (is_for_H)
                    {
                        sw.WriteLine($"{item.Date},{item.Hour},{item.Minitue},{item.Second},{item.Voltage},{string.Join(",", item.nodes.Select(t => $"{t.Id},{t.Z},{t.PitchAngle},{t.RollAngle},{t.CourseAngle}"))}");
                    }
                    else
                    {
                        sw.WriteLine($"{item.Date},{item.Hour},{item.Minitue},{item.Second},{item.Voltage},{string.Join(",", item.nodes.Select(t => $"{t.Id},{t.X},{t.Y},{t.PitchAngle},{t.RollAngle},{t.CourseAngle}"))}");
                    }
                }
            }
        }

        // Result2
        public void OutputResult2(string filename, List<SingleItem> itemlist, bool is_for_H = true, bool append = false)
        {
            using (StreamWriter sw = new StreamWriter(filename, append))
            {
                foreach (var item in itemlist)
                {
                    if (is_for_H)
                    {
                        sw.WriteLine($"{item.Date},{item.Hour},{item.Minitue},{item.Second},{string.Join(",", item.nodes.Select(t => $"{t.Id},{t.Z + 100}"))}");
                    }
                    else
                    {
                        sw.WriteLine($"{item.Date},{item.Hour},{item.Minitue},{item.Second},{string.Join(",", item.nodes.Select(t => $"{t.Id},{t.X + 100},{t.Y + 100}"))}");
                    }
                }
            }
        }

        // Result3
        public void OutputResult3(string filename, List<SingleItem> itemlist, bool is_for_H = true, bool append = false)
        {
            using (StreamWriter sw = new StreamWriter(filename, append))
            {
                DateTime last_dt = itemlist[0].Time;
                var item = itemlist[0];

                if (is_for_H)
                {
                    sw.WriteLine($"{item.Date},{item.Hour},{item.Minitue},{item.Second},{string.Join(",", item.nodes.Select(t => $"{t.Id},{t.Z + 100}"))}");
                }
                else
                {
                    sw.WriteLine($"{item.Date},{item.Hour},{item.Minitue},{item.Second},{string.Join(",", item.nodes.Select(t => $"{t.Id},{t.X + 100},{t.Y + 100}"))}");
                }

                for (int i = 1; i < itemlist.Count; i++)
                {
                    item = itemlist[i];
                    if ((item.Time - last_dt).TotalSeconds < Config.TIME_INTERVAL)
                    {
                        continue;
                    }

                    if (is_for_H)
                    {
                        sw.WriteLine($"{item.Date},{item.Hour},{item.Minitue},{item.Second},{string.Join(",", item.nodes.Select(t => $"{t.Id},{t.Z + 100}"))}");
                    }
                    else
                    {
                        sw.WriteLine($"{item.Date},{item.Hour},{item.Minitue},{item.Second},{string.Join(",", item.nodes.Select(t => $"{t.Id},{t.X + 100}"))}");
                    }

                    last_dt = item.Time;
                }
            }
        }
    }
}
