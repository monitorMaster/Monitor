using SqlSugar;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using twpx.Dao;

namespace twpx
{
    public partial class Screen : Form
    {
        int currentCamera = -1;//当前所选位置
        bool isFullScreen = false;//标记是否全屏
        bool isAllRecord = false;//标记是否开启全部录像
        SqlSugarClient db = SugarDao.GetInstance();
        Common Scommon = new Common(); //公共类
        List<PictureBox> pictureBoxList = new List<PictureBox>(); //存储pictureBox

        public Screen()
        {
            InitializeComponent();//初始化
            LoadData();//读取设备列表
            priview();//自动打开预览
            KeyPreview = true;//优先响应键盘事件
        }

        //注释函数
        public void DebugInfo(string str)
        {
            if (str.Length > 0)
            {
                str += "\n";
                textBox1.AppendText(str);
            }
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
        //读取数据
        public void LoadData()
        {
            listView1.Items.Clear();
            var list = Scommon.GetCL();

            for(int i=0; i<Scommon.GetCount(); i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = Scommon.GetLid(i);
                item.SubItems.Add(Scommon.GetLname(i));
                item.SubItems.Add(Scommon.GetIp(i));
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
            Scommon.StopPriviewAt();
            int a = Scommon.GetCount();
            if (a == 1)
            {
                setVisibleToFalse();
                pictureBox1.Visible = true;
                Scommon.Priview(0, pictureBox1);
                Scommon.AddInt(0);
            }
            if (a > 1 && a < 5)
            {
                setVisibleToFalse();
                for (int b = 0; b < a; b++)
                {
                    pictureBoxList[b + 1].Visible = true;
                    Scommon.Priview(b, pictureBoxList[b + 1]);
                    Scommon.AddInt(b);
                }
            }
            if (a > 4 && a < 10)
            {
                setVisibleToFalse();
                for (int b = 0; b < a; b++)
                {
                    pictureBoxList[b + 5].Visible = true;
                    Scommon.Priview(b, pictureBoxList[b + 5]);
                    Scommon.AddInt(b);
                }
            }
            if(a > 9)
            {
                setVisibleToFalse();
                for (int b = 0; b < 9; b++)
                {
                    pictureBoxList[b + 5].Visible = true;
                    Scommon.Priview(b, pictureBoxList[b + 5]);
                    Scommon.AddInt(b);
                }
            }
        }

        //把所有pictureBox属性Visible设置为false
        private void setVisibleToFalse()
        {
            for (int i = 0; i < 13; i++)
            {
                pictureBoxList[i].Visible = false;
            }
        }
        //列表选择触发
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 9)
            {
                MessageBox.Show("抱歉，预览设备选择过多，不能多于9个!");
                Scommon.AddLog("抱歉，预览设备选择过多!");
            }

            setVisibleToFalse();//设置所有picturebox属性不可见
            Scommon.StopPriviewAt();//关闭上次的预览设备
            if (listView1.SelectedItems.Count == 1)
            {
                pictureBox1.Visible = true;
                //这次选择的和上次一样
                if (currentCamera == Scommon.GetCamera(listView1.SelectedItems[0].SubItems[2].Text))
                    return;
                else
                    if(currentCamera >= 0)
                {
                    Scommon.StopPriview(currentCamera);//关闭前一个
                }
                currentCamera = Scommon.GetCamera(listView1.SelectedItems[0].SubItems[2].Text);
                if(currentCamera >= 0)
                {
                    if(Scommon.Priview(currentCamera, pictureBox1))
                    Scommon.AddInt(currentCamera);
                    Scommon.IntListToString();
                }
                else
                {
                    DebugInfo("选择预览失败");
                    Scommon.AddLog("选择预览失败");
                }
                
            }

            if (listView1.SelectedItems.Count > 1 && listView1.SelectedItems.Count < 5)
            {
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    currentCamera = Scommon.GetCamera(listView1.SelectedItems[i].SubItems[2].Text);
                    if(currentCamera < 0)
                    {
                        Scommon.StopPriviewAt();
                        MessageBox.Show("预览失败，请重试");
                        DebugInfo("预览失败，请重试");
                        Scommon.AddLog("预览失败，请重试");
                        return;
                    }
                    pictureBoxList[i + 1].Visible = true;//设置相应picturebox属性可见
                    Scommon.Priview(currentCamera, pictureBoxList[i + 1]);//四画面，1，2，3，4
                    Scommon.AddInt(currentCamera);//记录打开预览的设备在链表的序号
                }
            }

            if (listView1.SelectedItems.Count > 4 && listView1.SelectedItems.Count < 10)
            {
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    currentCamera = Scommon.GetCamera(listView1.SelectedItems[i].SubItems[2].Text);
                    if (currentCamera < 0)
                    {
                        Scommon.StopPriviewAt();
                        MessageBox.Show("预览失败，请重试");
                        DebugInfo("预览失败，请重试");
                        Scommon.AddLog("预览失败，请重试");
                    }
                    pictureBoxList[i + 5].Visible = true;//设置相应picturebox属性可见
                    Scommon.Priview(currentCamera, pictureBoxList[i + 5]);//九画面，5，6，7，8，9，10，11，12，13
                    Scommon.AddInt(currentCamera);//记录打开预览的设备在链表的序号
                }
            }
        }

        //登录/注销
        private void button1_Click(object sender, EventArgs e)
        {

        }
        //开始/停止全部录像
        private void button2_Click(object sender, EventArgs e)
        {
            if (!isAllRecord)
            {
                if (Scommon.StartAllRecord())
                {
                    button2.Text = "停止录像";
                    isAllRecord = true;
                }
            }
            else
            {
                Scommon.StopAllRecord();
                button2.Text = "开始录像";
                isAllRecord = false;
            }
        }
        //测试直接录像
        private void button2_Click2(object sender, EventArgs e)
        {
           
        }

        //抓图JPG
        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1) return;//只能选择一个预览
            Scommon.GetJPEG(currentCamera);
        }
        //抓图BMP
        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1) return;//只能选择一个预览
            Scommon.GetBMP(currentCamera);
        }
        //退出
        private void button5_Click(object sender, EventArgs e)
        {
            Close();
        }
        //预览画面，主页面
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
