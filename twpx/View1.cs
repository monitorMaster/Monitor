using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace twpx
{
    public partial class View1 : Form
    {
        public View1()
        {
            InitializeComponent();
        }

        /*private void panel1_Paint(object sender, PaintEventArgs e)
        {
       
        }*/ /// <summary>
        /// 动态创建面板
         /// </summary>
        /// <param name="xy">Panel的XY坐标</param>
        /// <param name="wh">Panel的大小</param>
        private Panel CreatePanel(Point xy, Size wh)
        {
            Panel panel = new Panel();
            panel.BackColor = System.Drawing.Color.Transparent;
            panel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panel.Location = xy;
            panel.Name = string.Concat("pVideo");
            panel.Size = wh;
            panel.TabIndex = 0;
            panel.BackColor = Color.Black;
            return panel;
        }

        private void View1_Load(object sender, EventArgs e)
        {
            /// <summary>
            /// 动态创建面板
            /// </summary>
            /// <param name="xy">Panel的XY坐标</param>
            /// <param name="wh">Panel的大小</param>
       
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
