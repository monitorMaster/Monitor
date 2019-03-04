using SqlSugar;
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

namespace twpx
{
    public partial class Screen : Form
    {
        int i = 0;//当前所选位置
        SqlSugarClient db = SugarDao.GetInstance();
        Common Scommon = new Common();
        public Screen()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            Console.WriteLine("链表长度=" + Scommon.GetCount());
            listView1.Items.Clear();
            var list = Scommon.GetCL();

            for(int i=0; i<Scommon.GetCount(); i++)
            {
                Console.WriteLine(Scommon.getIp(i));
                ListViewItem item = new ListViewItem();
                item.Text = i.ToString();
                item.SubItems.Add(Scommon.getIp(i));
                listView1.Items.Add(item);
            }
            
        }

        private void Screen_Load(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1) return;//只能选择一个预览
            if (i == Scommon.getCamera(this.listView1.SelectedItems[0].SubItems[1].Text)) return;//这次选择的和上次一样
            i = Scommon.getCamera(this.listView1.SelectedItems[0].SubItems[1].Text);
            Scommon.Privew(i-1, pictureBox1);
        }

        //登录/注销
        private void button1_Click(object sender, EventArgs e)
        {

        }
        //开始/停止录像
        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1) return;//只能选择一个预览
            if (i == Scommon.getCamera(this.listView1.SelectedItems[0].SubItems[1].Text)) return;//这次选择的和上次一样
            i = Scommon.getCamera(this.listView1.SelectedItems[0].SubItems[1].Text);
            Scommon.Record(i - 1, "D://video.mp4");
        }
        //抓图JPG
        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1) return;//只能选择一个预览
            if (i == Scommon.getCamera(this.listView1.SelectedItems[0].SubItems[1].Text)) return;//这次选择的和上次一样
            i = Scommon.getCamera(this.listView1.SelectedItems[0].SubItems[1].Text);
            Scommon.GetJPEG(i - 1);
        }
        //抓图BMP
        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1) return;//只能选择一个预览
            if (i == Scommon.getCamera(this.listView1.SelectedItems[0].SubItems[1].Text)) return;//这次选择的和上次一样
            i = Scommon.getCamera(this.listView1.SelectedItems[0].SubItems[1].Text);
            Scommon.GetBMP(i - 1);
        }
        //退出
        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //预览画面，主页面
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
