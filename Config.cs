/***
 * Config.cs 是全局公共类，包含了以下信息：
 * 1. 全局静态变量，如节段前的字段数量（5个，时间~电压）， 单个节段的字段数量（10个，X~航向角）
 * 2. 统筹对象GlobalItem，包含3个内容：
 *  1) 存储节点数量node_totalnum，
 *  2) 电压voltage，
 *  3) key（格式是“日期,时,0,0”，来源于每个文件的第一条时间）。
 *    该对象用于两个地方：
 *  1）每行判断节段数量是否满足要求，
 *  2）生成PowerV文件
 * 3. 单行记录对象SingleItem，包含基本信息和节段信息，除了文件中要求的，还另外增加了格式为DateTime的Time，它的格式是（日期时分秒），用于生成Result3文件时通过一定的时间间隔来输出。
 * 4. 节段记录对象Node，包含每个节段的基本信息
 * */


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRDC_MEMS
{
    public class Config
    {
        public static int PRE_LEN = 5;              //节点前基本信息的长度，日期~节点电压
        public static int NODE_LEN = 11;            //一段节点的长度，节点号~航向角
        public static int N_OF_ROW = 1;             //比对的前后行数
        public static int GAP_THRESHOLD = 131;      //比对的前后行数的Z值差
        public static int CUMULATE_COUNT = 100;     //累计前面的行数
        public static int TIME_INTERVAL = 300;      //间隔为300s，即5分钟

        // output filenames
        public static string F_OUT_SUFFIX = "_out_new";
        public static string F_DEBUG_CUM = "Debug_cumulate.txt";
        public static string F_DEBUG_AVG = "Debug_average.txt";
        public static string F_PRE = "Pre_file.txt";
        public static string F_FAILED = "Pre_file_failed.txt";
        public static string F_PowerV = "PowerV.txt";
        public static string F_NodeV = "NodeV.txt";
        public static string F_NodeT = "NodeT.txt";
        public static string F_RES1 = "Result_1.txt";
        public static string F_RES2 = "Result_2.txt";
        public static string F_RES3 = "Result_3.txt";
    }
    
    // 首行记录：节点数量，供电电压
    public class GlobalItem
    {
        public string Key { get; set; }
        public int NodeTotalNum { get; set; } // 节点数量
        public double PowerVoltage { get; set; } //供电电压
        public string Device_SerNumber { get; set; }

        // 初始化
        public GlobalItem()
        {
            this.NodeTotalNum = 0;
            this.PowerVoltage = 0;
            this.Device_SerNumber = "";
        }

        // 赋值
        // node_totalnum:1, PowerVoltage:0.69
        public void Initial(string line)
        {
            Dictionary<string, string> dict = line.Split(',').Select(t => t.Split(':')).ToDictionary(key => key[0].Trim(), value => value[1].Trim());

            if (dict.ContainsKey("node_totalnum") && int.TryParse(dict["node_totalnum"], out int num))
            {
                this.NodeTotalNum = num;
            }

            if (dict.ContainsKey("PowerVoltage") && double.TryParse(dict["PowerVoltage"], out double voltage))
            {
                this.PowerVoltage = voltage;
            }

            if (dict.ContainsKey("Device_SerNumber"))
            {
                this.Device_SerNumber = dict["Device_SerNumber"];
            }
        }
    }

    // 单行记录：日期，时，分，秒，节点电压，节点list
    // 20200717,10,00,00,0.21, <1,-13389,-432,-176,487,-315,192,-10893,-8482,4926,>
    public class SingleItem
    {
        public DateTime Time { get; set; } // 时间，由日期、时、分、秒组成，用于输出给定时间间隔的记录
        public string Date { get; set; } // 日期
        public string Hour { get; set; } // 时
        public string Minitue { get; set; } // 分
        public string Second { get; set; } //秒
        public double Voltage { get; set; } // 节点电压
        public bool flag { get; set; } // flat to store if processed or not

        public List<Node> nodes;

        // 初始化
        public SingleItem()
        {
            this.Voltage = 0;
            this.nodes = new List<Node>();
        }

        // 赋值
        public bool Initial(int node_num, string line)
        {
            var tokens = line.Split(',');
            if (tokens.Length < Config.PRE_LEN + node_num * Config.NODE_LEN)
            {
                Console.WriteLine("");
                return false;
            }

            this.Date = tokens[0];
            this.Hour = tokens[1];
            this.Minitue = tokens[2];
            this.Second = tokens[3];
            this.Time = DateTime.ParseExact($"{this.Date}{this.Hour}{this.Minitue}{this.Second}", "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
            if (double.TryParse(tokens[4], out double volt))
            {
                this.Voltage = Math.Round(volt,2);
            }
            this.flag = false;

            if (this.nodes == null)
            {
                this.nodes = new List<Node>();
            }

            for (int i = 0; i < node_num; i++)
            {
                Node node = new Node();
                if (!node.Initial(tokens.Skip(Config.PRE_LEN + i * Config.NODE_LEN).Take(Config.NODE_LEN).ToList()))
                {
                    Console.WriteLine($"wrong node: {string.Join("\t",tokens)}");
                    continue;
                }
                this.nodes.Add(node);
            }

            if (this.nodes == null || this.nodes.Count == 0)
            {
                Console.WriteLine($"wrong line: {line}");
                return false;
            }

            return true;
        }

        // 输出string
        public string ItemToString()
        {
            return $"{this.Date},{this.Hour},{this.Minitue},{this.Second},{this.Voltage},{string.Join(",", this.nodes.Select(t => t.NodeToString()))}";
            //return this.Date + "\t" + this.Hour + "\t" + this.Minitue + "\t" + this.Second + "\t" + this.Voltage.ToString();
        }

        // 浅拷贝，仅拷贝内容，不拷贝地址
        public object ShallowCopy()
        {
            return this.MemberwiseClone();
        }

        public List<Node> ShallowCopyForNodes()
        {
            return new List<Node>(this.nodes.Select(t => (Node)t.ShallowCopy()));
        }
    }

    //单节点记录：节点号，X，Y，Z，磁X，磁Y，磁Z，俯仰角，横滚角，航向角
    public class Node
    {
        public int Id { get; set; } // 节点号
        public double Temp { get; set; } // temperature
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double MagX { get; set; } // 磁X
        public double MagY { get; set; } // 磁Y
        public double MagZ { get; set; } // 磁Z
        public double PitchAngle { get; set; } // 俯仰角
        public double RollAngle { get; set; } // 横滚角
        public double CourseAngle { get; set; } // 航向角

        public bool Initial(List<string> tokens)
        {
            if (tokens.Count < Config.NODE_LEN)
            {
                return false;
            }
            if (int.TryParse(tokens[0], out int id)) { this.Id = id; }
            if (double.TryParse(tokens[1], out double temp)) { this.Temp = temp; }
            if (double.TryParse(tokens[2], out double x)) { this.X = x; }
            if (double.TryParse(tokens[3], out double y)) { this.Y = y; }
            if (double.TryParse(tokens[4], out double z)) { this.Z = z; }
            if (double.TryParse(tokens[5], out double magx)) { this.MagX = magx; }
            if (double.TryParse(tokens[6], out double magy)) { this.MagY = magy; }
            if (double.TryParse(tokens[7], out double magz)) { this.MagZ = magz; }
            if (double.TryParse(tokens[8], out double pitch)) { this.PitchAngle = pitch; }
            if (double.TryParse(tokens[9], out double roll)) { this.RollAngle = roll; }
            if (double.TryParse(tokens[10], out double course)) { this.CourseAngle = course; }
            return true;
        }

        // 浅拷贝，仅拷贝内容，不拷贝地址
        public object ShallowCopy()
        {
            return this.MemberwiseClone();
        }

        public string NodeToString()
        {
            return $"{this.Id},{this.Temp},{this.X},{this.Y},{this.Z},{this.MagX},{this.MagY},{this.MagZ},{this.PitchAngle},{this.RollAngle},{this.CourseAngle}";
        }
    }
}
