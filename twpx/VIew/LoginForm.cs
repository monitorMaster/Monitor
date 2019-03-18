using System;
using System.Windows.Forms;

namespace twpx.VIew
{
    public partial class LoginForm : Form
    {
        Common Lcommon = new Common();
        public LoginForm()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Lcommon.GetCount() == 0)
            {
                MessageBox.Show("没有设备在线!");
                Lcommon.AddLog("没有设备在线!");
                return;
            }
            new FullScreen().Show();
            Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Form1().Show();
            Hide();
        }
    }
}
