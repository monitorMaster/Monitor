using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using NVRCsharpDemo;
using SqlSugar;
using twpx.Dao;
using twpx.Model;

namespace twpx
{
    class Common
    {
        private static List<Camera> CameraList = new List<Camera>();//存储登录设备的链表
        private static int[] intList = new int[9]{-1,-1,-1,-1,-1,-1,-1,-1,-1};//存储当前预览设备在链表中的序号
        private static string operateLog = null;//存放日志
        private static string saveDirectory = "C:\\HikVision\\";//文件根目录
        SqlSugarClient db = SugarDao.GetInstance();
        private bool isRecord = false;//自动录像是否在运行
        public bool m_bInitSDK = false;//SDK 初始化标志


        //int链表添加
        public bool AddInt(int i)
        {
            for(int a=0; a<9; a++)
            {
                if(intList[a] == i)
                {
                    return false;  
                }
                if(intList[a] < 0)
                {
                    intList[a] = i;
                    break;
                }
            }
            return true;
        }
        //int链表清空
        public void ClearInt()
        {
            if (intList[0] < 0)
            {
                return;
            }
            for (int i=0; i<9; i++)
            {
                if (intList[i] < 0)
                {
                    return;
                }
                intList[i] = -1;

            }
        }
        //int链表取最后一个并踢掉
        public int GetIntTail()
        {
            int a = -1;
            if(intList[0] < 0)
            {
                return -1;
            }
            for(int i=8; i<=0; i--)
            {
                if(intList[i] >= 0)
                {
                    a = intList[i];
                    intList[i] = -1;
                    return a;
                }
            }
            return -1;
        }
        //intList ToString
        public void IntListToString()
        {
            if (intList[0] < 0)
            {
                Console.WriteLine("无数据");
                return;
            }
            int m;
            for(int i=0; i<9; i++)
            {
                Console.WriteLine("第" + i + "个: " + intList[i]);
                m = (i + 1) % 9;
                if (intList[m] < 0) break;
            }
        }

        //初始化SDK
        public bool InitSDK()
        {
            m_bInitSDK = CHCNetSDK.NET_DVR_Init();//返回bool类型
            if (m_bInitSDK == false)
            {
                MessageBox.Show("NET_DVR_Init error!");
                Console.WriteLine("NET_DVR_Init error!");
                return false;
            }
            else
            {
                //保存SDK日志 To save the SDK log
                CHCNetSDK.NET_DVR_SetLogToFile(3, GetSaveDirectory(), true);
                return true;
            }
        }
        //SDK注销
        public void cleanSDK()
        {
            StopAllRecord();
            LogOutAll();//注销suoyou
            CHCNetSDK.NET_DVR_Cleanup();
        }
        //读取设备
        public void LoadData()
        {
            var list = db.Queryable<Device>().ToList();
            foreach (var i in list)
            {
                Console.WriteLine(i.ToString());
                //自动登录
                if (AddCL(i.lid, i.lname, i.ip, Convert.ToInt16(i.port), i.username, i.password))
                {
                    i.setStatus();
                }
            }
        }

        //获取设备链表
        public List<Camera> GetCL() { return CameraList;  }
        //清空设备链表
        public bool RemoveAll()
        {
            if (LogOutAll() == 0)
            {
                return true;
            }
            return false;
        }
        //添加登录设备
        public bool AddCL(Camera camera)
        {
            if (CheckRepeat(camera.getIp()))
            {
                return false;
            }
            CameraList.Add(camera);//添加
            //登录
            if (CameraList[GetCount() - 1].login())
            {
                return true;
            }
            else
            {
                CameraList.RemoveAt(GetCount() - 1);
            }
            return false;

        }
        public bool AddCL(string lid, string lname, string ip, Int16 port, string username, string password)
        {
            if (CheckRepeat(ip))
            {
                return false;
            }
            CameraList.Add(new Camera(lid, lname, ip, port, username, password));//添加
            //登录
            if (CameraList[GetCount() - 1].login())
            {
                return true;
            }
            else
            {
                CameraList.RemoveAt(GetCount() - 1);
            }
            return false;
        }
        //设备链表查重
        public bool CheckRepeat(string ip)
        {
            if (GetCamera(ip) >= 0)
            {
                return true;
            }
            return false;
        }

        //设备注销(根据索引)
        public bool RemoveCLByI(int i)
        {
            if (i < 0 || i > CameraList.Count) return false;
            if (CameraList[i].logout())
            {
                CameraList.RemoveAt(i);
                return true;
            }
            return false;
        }
        //设备注销，所有
        public int LogOutAll()
        {
            for (int i = GetCount() - 1; i >= 0; i--)
            {
                if (!RemoveCLByI(i))
                {
                    break;
                }
            }
            return GetCount();
        }

        //根据IP获取设备在设备链表里位置,没有第0位，至少第1位
        public int GetCamera(string ip)
        {
            for (int i = 0; i < CameraList.Count; i++)
            {
                if (ip.Equals(CameraList[i].getIp())) return i;
            }
            return -1;
        }
        //获取ip
        public string GetIp(int i)
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
        //获取lid
        public string GetLid(int i)
        {
            if (i < 0 || i > CameraList.Count) return null;
            return CameraList[i].getLid();
        }
        //获取lname
        public string GetLname(int i)
        {
            if (i < 0 || i > CameraList.Count) return null;
            return CameraList[i].getLname();
        }
        //获取savedirectory
        public string GetSaveDirectory()
        {
            return saveDirectory;
        }
        //tostring()
        public void CameraToString(int i)
        {
            CameraList[i].ToString();
        }
        

        //指定其中一个预览
        public bool Priview(int i, PictureBox pictureBox)
        {
            if (i < 0 || i > CameraList.Count) return false;
            if (CameraList[i].realPlay(pictureBox))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //关闭预览
        public bool StopPriview(int i)
        {
            if (i < 0 || i > CameraList.Count) return false;
            if (CameraList[i].stopPlay())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //关闭所有预览
        public bool StopAllPriview()
        {
            for (int i = GetCount() - 1; i >= 0; i--)
            {
                if (!StopPriview(i))
                {
                    return false;
                }
            }
            return true;
        }
        //关闭int链表中得设备预览
        public void StopPriviewAt()
        {
            if (intList[0] < 0)
            {
                return;
            }
            int m;
            for(int i = 0; i<9; i++)
            {
                if(intList[i] >= 0)
                {
                    if (!StopPriview(intList[i]))
                    {
                        AddLog(" ip: " + CameraList[intList[i]].getIp() + "关闭预览失败");
                        StopAllPriview();
                    }
                }
                m = (i + 1) % 9;
                if(intList[m] < 0)
                {
                    break;
                }
            }
            ClearInt();
        }

        //有参录像
        public string Record(int i)
        {
            if (i < 0 || i > CameraList.Count) return null;
            //获取时间按格式yyyy-MM-dd,路径格式：C:\\HikVision\\设备IP
            string FileDirectory = saveDirectory + CameraList[i].getIp();
            FileDirectory = SetDirectory(FileDirectory);
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
        //无参录象
        public string RecordAt(int i)
        {
            if (i < 0 || i > CameraList.Count) return null;
            //获取时间按格式yyyy-MM-dd,路径格式：C:\\HikVision\\设备IP
            string FileDirectory = saveDirectory + CameraList[i].getIp();
            FileDirectory = SetDirectory(FileDirectory);
            string FileName = FileDirectory + "\\" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".mp4";
            if (CameraList[i].SaveRecord(FileName) != null)
            {
                return FileName;
            }
            return null;
        }
        //启动全部录像
        public bool StartAllRecord()
        {
            if (GetCount() == 0) return false;
            for (int i = 0; i < GetCount(); i++)
            {
                if(!CameraList[i].getBRecord())
                //调用Camera.cs 的无参录像
                if(RecordAt(i) == null)
                {
                        StopAllRecord();
                        return false;
                }
            }
            return true;
        }
        //关闭全部录像
        public void StopAllRecord()
        {
            if (GetCount() == 0) return;
            string fileName;
            for (int i = 0; i < GetCount(); i++)
            {
                if (CameraList[i].getBRecord())
                {
                    fileName = RecordAt(i);//调用Camera.cs 的无参录像
                    saveRecordDirectory(i, fileName);
                }
                    
            }
        }
        //上传录像路径
        public void saveRecordDirectory(int i, string fileName)
        {
            Record record = new Record();
            record.cid = int.Parse(CameraList[i].getLid());
            record.cname = CameraList[i].getLname();
            record.date = DateTime.Now;
            record.path = fileName;
            record.filename = fileName;
            try
            {
                var t2 = db.Insertable(record).ExecuteCommand();
                AddLog("添加数据：" + fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                AddLog(ex.Message);
            }
        }
        //供给计时器自动录像得函数
        public void autoSaveRecord()
        {
            if (isRecord)
            {
                StartAllRecord();//未在运行，停止
            }
            else
            {
                StopAllRecord();//在运行，停止
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
        
        //建立文件夹
        public string SetDirectory(string FileDectory)
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
        public void InitLog()
        {
            operateLog = "* * * * * * * * * * " + DateTime.Now.ToString() + " 程序运行 * * * * * * * * * *";
        }
        //退出日志保存
        public void SaveLog()
        {
            operateLog = operateLog + "\n* * * * * * * * * * " + DateTime.Now.ToString() + " 程序退出 * * * * * * * * * *\n";
            string logName = saveDirectory + "operateLog.txt";
            using (FileStream fs = new FileStream(logName, FileMode.OpenOrCreate, FileAccess.Write))
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
        public string AddLog(string log)
        {
            operateLog = operateLog +"\n" + DateTime.Now.ToString() + "：" + log;
            Console.WriteLine(DateTime.Now.ToString() + "：" + log);
            return operateLog;
        }
        
        //设置布防句
        public void SetAlarmHandle(int i, int alarm)
        {
            CameraList[i].SetM_lAlarmHandle(Convert.ToInt32(alarm));
        }
        //返回布防句柄
        public int GetAlarmHandle(int i)
        {
            return CameraList[i].GetM_lAlarmHandle();

        }
        //布防
        public void SetAlarm(int i, CHCNetSDK.NET_DVR_SETUPALARM_PARAM struAlarmParam)
        {
            CameraList[i].SetAlarm(struAlarmParam);
        }
        //撤防
        public bool CloseAlarm(int i)
        {
            if (CameraList[i].CloseAlarm())
            {
                return true;
            }
            return false;
        }

    }
}
