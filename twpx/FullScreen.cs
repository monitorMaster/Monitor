using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using twpx.VIew;

namespace twpx
{
    public partial class FullScreen : Form
    {
        Common FScommon = new Common();
        List<PictureBox> pictureBoxList = new List<PictureBox>();//存储pictureBox1,pictureBox2,pictureBox3,pictureBox4
        int currentCamera = 0;//当前位置
        private long timeCount = 0;//计时次数

        public FullScreen()
        {
            InitializeComponent();
            InitPictureBoxList();
            timer1.Start();//启动计时器
            Console.WriteLine("计时器开始");
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        //初始化pictureBox
        public void InitPictureBoxList()
        {
            pictureBoxList.Add(pictureBox1);
            pictureBoxList.Add(pictureBox2);
            pictureBoxList.Add(pictureBox3);
            pictureBoxList.Add(pictureBox4);
        }
        //还有一个计时器
        //计时器控制自动预览
        public void AutoPriview()
        {
            FScommon.StopPriviewAt();
            int a = FScommon.GetCount();
            int m = 4;
            if(a < 5)
            {
                m = a;
            }
            for(int i=0; i< m; i++)
            {
                FScommon.Priview(currentCamera, pictureBoxList[i]);
                currentCamera = (currentCamera + 1) % a; 
                FScommon.AddInt(i);
            }
        }

        private void FullScreen_Load(object sender, EventArgs e)
        {
            //regist the form closing event.
            //注册窗体关闭事件。
            FormClosing += new FormClosingEventHandler(FullScreen_Closing);
        }
        private void FullScreen_Closing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timeCount == 3)
            {
                timer1.Stop();
                FScommon.AddLog("timer1停止");
                FScommon.StopPriviewAt();
            }
            AutoPriview();
            Console.WriteLine("timeCount = " + timeCount);
            timeCount++;
        }
    }
}
