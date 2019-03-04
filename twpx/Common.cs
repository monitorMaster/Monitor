using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NVRCsharpDemo;

namespace twpx
{
    class Common
    {
        private static List<Camera> CameraList = new List<Camera>();

        //获取链表
        public List<Camera> GetCL() { return CameraList;  }
        //设备登录
        public void addCL(Camera camera)
        {
            camera.CameraToString();
            CameraList.Add(camera);
            try
            {
                CameraList[GetCount() - 1].login();
                CameraList[GetCount() - 1].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public void addCL(string ip, Int16 port, string username, string password)
        {
            CameraList.Add(new Camera(ip, port, username, password));
        }


        //设备注销(根据索引)
        public void removeCLByI(int i)
        {
            if (i < 0 || i > CameraList.Count) return;
            try
            {
                CameraList[i].logout();
                CameraList.RemoveAt(i);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //根据IP获取设备在设备链表里位置
        public int getCamera(string ip)
        {
            for (int i = 0; i < CameraList.Count; i++)
            {
                if (ip.Equals(CameraList[i].getIp())) return i + 1;
            }
            return 0;
        }

        //获取ip
        public string getIp(int i)
        {
            if (i < 0 || i > CameraList.Count) return null;
            return CameraList[i].getIp();
        }

        //获取长度
        public int GetCount()
        {
            return CameraList.Count;
        }

        //获取userId
        public int GetUserID(int i)
        {
            if (i < 0 || i > CameraList.Count) return 28;
            try
            {
                return CameraList[i].getUserID();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 29;
            }


        }
        //tostring()
        public void CameraToString(int i)
        {
            Console.WriteLine("IP: " + CameraList[i].getIp());
            Console.WriteLine("Port: " + CameraList[i].getPort());
            Console.WriteLine("UserName: " + CameraList[i].getUserName());
            Console.WriteLine("Password: " + CameraList[i].getPassword());
        }

        //指定其中一个预览
        public void Privew(int i, PictureBox pictureBox)
        {
            if (i < 0 || i > CameraList.Count) return;
            try
            {
                CameraList[i].realPlay(pictureBox);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //录像
        public void Record(int i, string FileName)
        {
            if (i < 0 || i > CameraList.Count) return;
            try
            {
                CameraList[i].startRecord(FileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //截图
        public void GetJPEG(int i)
        {
            if (i < 0 || i > CameraList.Count) return;
            try
            {
                CameraList[i].saveJPEG();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void GetBMP(int i)
        {
            if (i < 0 || i > CameraList.Count) return;
            try
            {
                CameraList[i].saveBMP();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        //
    }
}
