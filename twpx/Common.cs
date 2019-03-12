using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NVRCsharpDemo;
using System.Timers;

namespace twpx
{
    class Common
    {
        private static List<Camera> CameraList = new List<Camera>();//存储登录设备的链表
        private static string operateLog = null;//存放日志
        private static string saveDirectory = "C:\\HikVision\\";//文件根目录
        DateTime currentTime = new DateTime();
        
        //获取链表
        public List<Camera> GetCL() { return CameraList;  }
         //添加登录设备
        public void addCL(Camera camera)
        {
            CameraList.Add(camera);//添加
            try
            {
                CameraList[GetCount() - 1].login();//登录
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public void addCL(string ip, Int16 port, string username, string password)
        {
            CameraList.Add(new Camera(ip, port, username, password));//添加
            try
            {
                CameraList[GetCount() - 1].login();//登录操作
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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
            if (i < 0 || i > CameraList.Count) return -1;
            try
            {
                return CameraList[i].getUserID();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                
            }
            return -1;

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
        public string Record(int i)
        {
            if (i < 0 || i > CameraList.Count) return null;
            //获取时间按格式yyyy-MM-dd,路径格式：C:\\HikVision\\设备IP
            string FileDirectory = saveDirectory + CameraList[i].getIp();
            FileDirectory = setDirectory(FileDirectory);
            string FileName = FileDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".mp4";
            try
            {
                CameraList[i].startRecord(FileName);
                return FileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        //测试直接录象
        public void Record()
        {
            string sVideoFileName;
            sVideoFileName = "D:\\test.mp4";
            CameraList[0].SaveRecord(sVideoFileName);
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
        
        //建立文件夹
        public string setDirectory(string FileDectory)
        {
            try
            {
                if (!Directory.Exists(FileDectory))
                {
                    // Create the directory it does not exist.
                    Directory.CreateDirectory(FileDectory);
                }
                return FileDectory;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        //初始化日志
        public void initLog()
        {
            operateLog = "* * * * * * * * * * " + DateTime.Now.ToString() + " 程序运行 * * * * * * * * * *";
        }
        //退出日志保存
        public void saveLog()
        {
            operateLog = operateLog + "\n* * * * * * * * * * " + DateTime.Now.ToString() + " 程序退出 * * * * * * * * * *\n";
            using (FileStream fs = new FileStream(@"d:\test.txt", FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("{0}\n", operateLog , DateTime.Now);
                    sw.Flush();
                }
            }
        }
        //返回log
        public string GetOperateLog()
        {
            return operateLog;
        }
        //记录日志
        public string addLog(string log)
        {
            operateLog = operateLog +"\n" + DateTime.Now.ToString() + "：" + log;
            return operateLog;
        }
        //启动全部录像
        public void allRecord()
        {
            string FileDirectory;
            string FileName;
            for (int i=0; i<GetCount(); i++)
            {
                FileDirectory = saveDirectory + CameraList[i].getIp();
                FileDirectory = setDirectory(FileDirectory);
                FileName = FileDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".mp4";
                CameraList[i].SaveRecord(FileName);  
            }
        }
        //设置布防句柄
        public void setAlarmHandle(int i, int alarm)
        {
            CameraList[i].setM_lAlarmHandle(Convert.ToInt32(alarm));
        }
        //返回布防句柄
        public int getAlarmHandle(int i)
        {
            return CameraList[i].getM_lAlarmHandle();

        }
        //布防
        public void setAlarm(int i, CHCNetSDK.NET_DVR_SETUPALARM_PARAM struAlarmParam)
        {
            CameraList[i].setAlarm(struAlarmParam);
        }
        //撤防
        public bool closeAlarm(int i)
        {
            if (CameraList[i].closeAlarm())
            {
                return true;
            }
            return false;
        }

    }
}
