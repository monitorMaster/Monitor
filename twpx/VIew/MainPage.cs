using System;
using System.Windows.Forms;
using NVRCsharpDemo;
using System.Diagnostics;
using twpx.VIew;

namespace twpx
{
    public partial class Form1 : Form
    {
        Common Fcommon = new Common();
        private long timeCount = 0;

        public Form1()
        {
            InitializeComponent();
            timer1.Start();
            Fcommon.AddLog("timer1计时器开启");
        }

        
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void 打开ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var frm = new Manage1();
            //frm.MdiParent = this;
            frm.Show();
        }

        private void 打开ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var frm = new Manage2();
            //frm.MdiParent = this;
            frm.Show();
        }

        private void 打开ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            var frm = new View1();
            //frm.MdiParent = this;
            frm.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void 录像回放ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new Find();
            //frm.MdiParent = this;
            frm.Show();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void 设备管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new Devices();
            frm.Show();
        }

        private void 监控预览ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new Screen();
            frm.Show();
        }

        private void 用户管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 设备管理ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var whd = new replay();
            whd.Show();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            var whd = new VideoList();
            whd.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var frm = new Devices();
            frm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var frm = new Screen();
            frm.Show();
        }

        private void 一键报警ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.hao123.com/haoserver/tefudh.htm"); // 使用本机默认浏览器打开网址
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timeCount == 10)
            {
                timer1.Stop();
                Fcommon.AddLog("timer1停止");
            }
            Console.WriteLine("Now time is " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
            timeCount++;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            new Sysgl().Show();
        }
    }
}
