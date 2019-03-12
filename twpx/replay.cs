using NVRCsharpDemo;
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
    public partial class replay : Form
    {
        private bool m_bPause = false;
        private bool play = false;
        private string path = null;
        public replay()
        {
            InitializeComponent();
        }

        public replay(String str)
        {
            InitializeComponent();
            path = str;
        }

        private void replay_Load(object sender, EventArgs e)
        {
            if (path == null)
            {
               
                Close();
                MessageBox.Show("未选择有效文件");
                return;
            }
            Boolean flag = PlayCtrl.PlayM4_OpenFile(0, path);
            Console.WriteLine(flag ? "文件打开成功" : "文件打开失败");
            PlayCtrl.PlayM4_SetPicQuality(0, 1);
            PlayCtrl.PlayM4_SetPlayPos(0, 0);
            label3.Text = GetTime(PlayCtrl.PlayM4_GetFileTime(0));
        }


        private string GetTime(uint time)
        {
            float h = time / 3600;
            float m = time / 60 - h * 60;
            float s = time - m * 60 - h * 3600;
            return h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00");
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (!play&&PlayCtrl.PlayM4_Play(0, pictureBox1.Handle))
            {
                button1.Text = "暂停";
                play = true;
                Console.WriteLine("播放成功");
            }
            else
            {
                if(!m_bPause)
                {
                    PlayCtrl.PlayM4_Pause(0, 1);
                    button1.Text = "继续播放";
                    m_bPause = true;
                    Console.WriteLine("暂停成功");
                }
                else
                {
                    PlayCtrl.PlayM4_Pause(0, 0);
                    button1.Text = "暂停";
                    m_bPause = false;
                    Console.WriteLine("恢复播放成功");
                }
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (PlayCtrl.PlayM4_Stop(0)) {
                play = false;
                m_bPause = false;
                button1.Text = "播放";
                //pictureBox1.Image = null ;
                progressBar1.Value = 0;
                label4.Text = "00:00:00";
                Console.WriteLine("停止播放成功");
            }
        }

        

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!play || m_bPause) return;
            progressBar1.Value = (int)(PlayCtrl.PlayM4_GetPlayPos(0) * 100);
            label4.Text = GetTime(PlayCtrl.PlayM4_GetPlayedTime(0));
            Console.WriteLine(PlayCtrl.PlayM4_GetPlayedTime(0));
            Console.WriteLine(PlayCtrl.PlayM4_GetPlayedTimeEx(0));
            Console.WriteLine(PlayCtrl.PlayM4_GetFileTime(0));
            if (PlayCtrl.PlayM4_GetPlayedTimeEx(0) >= PlayCtrl.PlayM4_GetFileTime(0)*1000)
            {
                progressBar1.Value = 100;
               play = false;
                m_bPause = false;
                button2_Click(null, null);
            }
        }
    }
}
