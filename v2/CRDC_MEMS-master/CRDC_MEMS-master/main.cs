using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRDC_MEMS
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new DataCollection().ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new DataSolution().ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new Configure().ShowDialog();
        }
    }
}
