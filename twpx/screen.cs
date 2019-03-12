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
        Boolean isFullScreen = false;//标记是否全屏
        SqlSugarClient db = SugarDao.GetInstance();
        //公共类
        Common Scommon = new Common(); 
        //存储pictureBox
        List<PictureBox> pictureBoxList = new List<PictureBox>();
        public Screen()
        {
            InitializeComponent();//初始化
            LoadData();//读取设备列表
            priview();//自动打开预览
            this.KeyPreview = true;//优先响应键盘事件
        }

        //pictureBox添加到链表中
        private void addPictureBox()
        {
            pictureBoxList.Add(pictureBox1);
            pictureBoxList.Add(pictureBox2);
            pictureBoxList.Add(pictureBox3);
            pictureBoxList.Add(pictureBox4);
            pictureBoxList.Add(pictureBox5);
            pictureBoxList.Add(pictureBox6);
            pictureBoxList.Add(pictureBox7);
            pictureBoxList.Add(pictureBox8);
            pictureBoxList.Add(pictureBox9);
            pictureBoxList.Add(pictureBox10);
            pictureBoxList.Add(pictureBox11);
            pictureBoxList.Add(pictureBox12);
            pictureBoxList.Add(pictureBox13);
            pictureBoxList.Add(pictureBox14);
        }

        //按Esc退出全屏
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                fullSreen();
            }
        }
        //全屏显示或者退出全屏显示
        private void fullSreen()
        {
            if (isFullScreen)
            {

            }
            else
            {

            }
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
            //初始化pictureBox链表
            addPictureBox();
            
        }

        private void Screen_Load(object sender, EventArgs e)
        {

        }

        //自动预览
        private void priview()
        {
            if(Scommon.GetCount() == 1)
            {
                setVisibleToFalse();
                this.pictureBox1.Visible = true;
                Scommon.Privew(0, pictureBox1);
            }
            if(Scommon.GetCount() > 1 && Scommon.GetCount() < 5)
            {
                setVisibleToFalse();
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    this.pictureBoxList[i + 1].Visible = true;
                    Scommon.Privew(i, pictureBoxList[i + 1]);
                }
            }
            if(Scommon.GetCount() > 4)
            {
                setVisibleToFalse();
                for (int i = 0; i < 9; i++)
                {
                    this.pictureBoxList[i + 5].Visible = true;
                    Scommon.Privew(i, pictureBoxList[i + 5]);
                }
            }
        }


        //把所有pictureBox属性Visible设置为false
        private void setVisibleToFalse()
        {
            for (int i = 0; i < 13; i++)
            {
                this.pictureBoxList[i].Visible = false;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count == 1)
            {
                setVisibleToFalse();
                this.pictureBox1.Visible = true;
                if (i == Scommon.getCamera(this.listView1.SelectedItems[0].SubItems[1].Text)) return;//这次选择的和上次一样
                i = Scommon.getCamera(this.listView1.SelectedItems[0].SubItems[1].Text);
                Scommon.Privew(i - 1, pictureBox1);
            }
            
            if(listView1.SelectedItems.Count > 1 && listView1.SelectedItems.Count < 5)
            {
                setVisibleToFalse();
                for(int i=0; i<listView1.SelectedItems.Count; i++)
                {
                    this.pictureBoxList[i + 1].Visible = true;
                    Scommon.Privew(i, pictureBoxList[i + 1]);
                }
            }

            if(listView1.SelectedItems.Count > 4 && listView1.SelectedItems.Count < 10)
            {
                setVisibleToFalse();
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    this.pictureBoxList[i + 5].Visible = true;
                    Scommon.Privew(i, pictureBoxList[i + 5]);
                }
            }

            if(listView1.SelectedItems.Count > 9)
            {
                Console.WriteLine("抱歉，选择过多!");
            }

        }

        //登录/注销
        private void button1_Click(object sender, EventArgs e)
        {

        }
        //开始/停止录像
        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1) return;//只能选择一个预览
            Console.WriteLine(Scommon.getCamera(this.listView1.SelectedItems[0].SubItems[1].Text)); 
            if (Scommon.getCamera(this.listView1.SelectedItems[0].SubItems[1].Text) == 0) return;
            i = Scommon.getCamera(this.listView1.SelectedItems[0].SubItems[1].Text);
            Scommon.Record(i - 1);
        }
        //测试直接录像
        private void button2_Click2(object sender, EventArgs e)
        {
            Scommon.Record();
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
