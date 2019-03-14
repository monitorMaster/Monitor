﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using twpx.Dao;
using twpx.Model;
using NVRCsharpDemo;
using System.Diagnostics;
using System.Timers;

namespace twpx
{
    public partial class Form1 : Form
    {

        public bool m_bInitSDK = false;
        Common Fcommon = new Common();

        public Form1()
        {
            InitializeComponent();
            m_bInitSDK = CHCNetSDK.NET_DVR_Init();//返回bool类型
            if (m_bInitSDK == false)
            {
                MessageBox.Show("NET_DVR_Init error!");
                Console.WriteLine("NET_DVR_Init error!");
                return;
            }
            else
            {
                //保存SDK日志 To save the SDK log
                CHCNetSDK.NET_DVR_SetLogToFile(3, "D:\\SdkLog\\", true);

            }
            //SqlSugarClient db = SugarDao.GetInstance();
            //var list = db.Queryable<Device>().ToList();

        }
        /*
        //计时触发函数 保存录像
        private static void TimedEvent(object source, ElapsedEventArgs e)
        {
            for (int i = 0; i < Fcommon.GetCount(); i++)
            {
                if
            }
        }
        */
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
            Fcommon.AllRecord();
        }
    }
}
