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

namespace twpx.VIew
{
    public partial class Jxlgl : Form
    {
        SqlSugarClient db = SugarDao.GetInstance();
        public Jxlgl()
        {
            InitializeComponent();
            LoadData();
        }

        // 加载数据
        public void LoadData()
        {
            listView1.Items.Clear();
            var list = db.Queryable<Building>().ToList();
            foreach (var i in list)
            {
                ListViewItem item = new ListViewItem();
                item.Text = i.Id.ToString();
                item.SubItems.Add(i.Name);
                item.SubItems.Add(i.Address);
                listView1.Items.Add(item);
            }
        }

        // 添加/更新信息
        public void saveData()
        {
            var building = new Building();
            var result = 0;
            building.Name = textBox1.Text.Trim();
            building.Address = textBox3.Text.Trim();
            if ("".Equals(textBox2.Text) || textBox2.Text == null)
            {
                result = db.Insertable(building).ExecuteCommand();
            }
            else
            {
                building.Id = int.Parse(textBox2.Text);
                result = db.Updateable(building).ExecuteCommand();
            }
            LoadData();
        }
        // 清除信息
        private void clearBlank()
        {
            textBox2.Text = "";
            textBox1.Text = "";
            textBox3.Text = "";
        }
        // 清除信息按钮
        private void button5_Click(object sender, EventArgs e)
        {
            clearBlank();
        }

        // 添加按钮
        private void button1_Click(object sender, EventArgs e)
        {
            saveData();
        }

        // 删除按钮
        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("未选中项！");
                return;
            }
            int id = int.Parse(listView1.SelectedItems[0].Text);
            var result = db.Deleteable<Building>().In(id).ExecuteCommand();
            if (result == 1) LoadData();
        }
        // 刷新按钮
        private void button4_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        // 批量删除
        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count == 0)
            {
                MessageBox.Show("未选中项！");
                return;
            }
            int[] ids = new int[listView1.CheckedItems.Count];
            for (int i = 0; i < listView1.CheckedItems.Count; i++)
            {
                ids[i] = int.Parse(listView1.CheckedItems[i].Text);
            }
            var result = db.Deleteable<Building>().In(ids).ExecuteCommand();
            if (result == listView1.CheckedItems.Count) LoadData();
        }

        // 选中项变化
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                clearBlank();
                return;
            }
            textBox2.Text = listView1.SelectedItems[0].Text;
            textBox1.Text = listView1.SelectedItems[0].SubItems[1].Text;
            textBox3.Text = listView1.SelectedItems[0].SubItems[2].Text;
        }

        // END
    }
}
