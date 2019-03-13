using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NVRCsharpDemo;
using twpx.Model;
using twpx;
using System.IO;

namespace NVRCsharpDemo
{
    class Camera
    {
        private string ip; //设备IP地址或者域名 Device IP
        Int16 port;//设备服务端口号 Device Port
        private string username;
        private string password;
        private bool m_bRecord = false; //录像标志
        private Int32 UserID = -1; //-1表示失败，其他值表示返回的用户ID值。该用户ID具有唯一性，后续对设备的操作都需要通过此ID实现。
        private Int32 m_lRealHandle=-1;//-1表示失败，其他值作为NET_DVR_StopRealPlay等函数的句柄参数
        private uint dwAChanTotalNum; //设备模拟通道个数，数字（IP）通道最大个数为byIPChanNum + byHighDChanNum*256 
        private uint dwDChanTotalNum;//设备最大数字通道个数，低8位，高8位见byHighDChanNum
        private uint iLastErr = 0; // 用于接收调用NET_DVR_GetLastError获取的错误码
        private int[] iIPDevID = new int[96];
        private int[] iChannelNum = new int[96];
        public CHCNetSDK.NET_DVR_DEVICEINFO_V40 DeviceInfo;// 设备信息结构体
        public CHCNetSDK.NET_DVR_IPPARACFG_V40 m_struIpParaCfgV40; //IP设备资源及IP通道资源配置结构体。
        private long iSelIndex = 0;

        private Int32 m_lTree = 0;
        private string str;
        private string str1;
        private string str2;
        private Int32 i = 0;
        public CHCNetSDK.NET_DVR_IPCHANINFO struChanInfo;
        public CHCNetSDK.NET_DVR_IPCHANINFO_V40 struChanInfoV40;
        private int m_lFindHandle = -1;
        private int m_lPlayHandle = -1;
        private string strResult; //存储查找路径
        private long lHandle;
        //监听布防需要的变量
        private Int32 m_lAlarmHandle = -1;//布防句柄
        private int iPicNumber = 0; //图片序号
        private CHCNetSDK.MSGCallBack m_falarmData = null;
        public delegate void UpdateListBoxCallback(string strAlarmTime, string strDevIP, string strAlarmMsg);
        CHCNetSDK.NET_VCA_TRAVERSE_PLANE m_struTraversePlane = new CHCNetSDK.NET_VCA_TRAVERSE_PLANE();
        CHCNetSDK.NET_VCA_AREA m_struVcaArea = new CHCNetSDK.NET_VCA_AREA();
        CHCNetSDK.NET_VCA_INTRUSION m_struIntrusion = new CHCNetSDK.NET_VCA_INTRUSION();
        CHCNetSDK.UNION_STATFRAME m_struStatFrame = new CHCNetSDK.UNION_STATFRAME();
        CHCNetSDK.UNION_STATTIME m_struStatTime = new CHCNetSDK.UNION_STATTIME();
        //private CHCNetSDK.MSGCallBack_V31 m_falarmData_V31 = null;

        Common Ccommon = new Common();

        

        public Camera(){
            ip = "192.168.1.107";
            port = 8000;
            username = "admin";
            password = "lab12345678";
            //login();
        }

        public Camera(string ip,Int16 port,string username,string password)
        {
            this.ip = ip;
            this.port = port;
            this.username = username;
            this.password = password;
            //login();
        }

        ~Camera()
        {
            logout();
        }

        //监听布防初始化
        public void initAlam()
        {
            byte[] strIP = new byte[16 * 16];
            uint dwValidNum = 0;
            Boolean bEnableBind = false;

            //获取本地PC网卡IP信息
            if (CHCNetSDK.NET_DVR_GetLocalIP(strIP, ref dwValidNum, ref bEnableBind))
            {
                if (dwValidNum > 0)
                {
                    //取第一张网卡的IP地址为默认监听端口
                    CHCNetSDK.NET_DVR_SetValidIP(0, true); //绑定第一张网卡
                }

            }

            //保存SDK日志 To save the SDK log
            //CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
            /*
            for (int i = 0; i < 200; i++)
            {
                m_lAlarmHandle[i] = -1;
            }
            
            //设置报警回调函数
            if (m_falarmData_V31 == null)
            {
                m_falarmData_V31 = new CHCNetSDK.MSGCallBack_V31(MsgCallback_V31);
            }
            CHCNetSDK.NET_DVR_SetDVRMessageCallBack_V31(m_falarmData_V31, IntPtr.Zero);
            */
        }

        //设备登录
        public bool login()
        {
            UserID = CHCNetSDK.NET_DVR_Login_V30(ip, port, username, password, ref DeviceInfo);
            if (UserID < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                Ccommon.AddLog("iLastErr = " + iLastErr);
                return false;
            }
            else
            {
                //登录成功
                dwAChanTotalNum = (uint)DeviceInfo.byChanNum;
                dwDChanTotalNum = (uint)DeviceInfo.byIPChanNum + 256 * (uint)DeviceInfo.byHighDChanNum;
                if (dwDChanTotalNum > 0)
                {
                    deviceInfoIPChannel();
                }
                else
                {
                    for (i = 0; i < dwAChanTotalNum; i++)
                    {
                        deviceListAnalogChannel(i + 1, 1);
                        iChannelNum[i] = i + (int)DeviceInfo.byStartChan;
                    }

                    //comboBoxView.SelectedItem = 1;
                    // MessageBox.Show("This device has no IP channel!");
                }

                Ccommon.AddLog("登录成功");
                return true;
            }
        }

        //设备注销
        public void logout()
        {
            if (m_lRealHandle >= 0)
            {
                CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle);
                m_lRealHandle = -1;
            }
            if (UserID >= 0)
            {
                CHCNetSDK.NET_DVR_Logout(UserID);
                UserID = -1;
            }
        }

        //ip通道配置
        public void deviceInfoIPChannel()
        {
            

            uint dwSize = (uint)Marshal.SizeOf(m_struIpParaCfgV40);

            IntPtr ptrIpParaCfgV40 = Marshal.AllocHGlobal((Int32)dwSize);
            Marshal.StructureToPtr(m_struIpParaCfgV40, ptrIpParaCfgV40, false);

            uint dwReturn = 0;
            int iGroupNo = 0;  //该Demo仅获取第一组64个通道，如果设备IP通道大于64路，需要按组号0~i多次调用NET_DVR_GET_IPPARACFG_V40获取

            if (!CHCNetSDK.NET_DVR_GetDVRConfig(UserID, CHCNetSDK.NET_DVR_GET_IPPARACFG_V40, iGroupNo, ptrIpParaCfgV40, dwSize, ref dwReturn))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                /*
                str = "NET_DVR_GET_IPPARACFG_V40 failed, error code= " + iLastErr;
                获取IP资源配置信息失败，输出错误号 Failed to get configuration of IP channels and output the error code
                DebugInfo(str);
                */
            }
            else
            {

                m_struIpParaCfgV40 = (CHCNetSDK.NET_DVR_IPPARACFG_V40)Marshal.PtrToStructure(ptrIpParaCfgV40, typeof(CHCNetSDK.NET_DVR_IPPARACFG_V40));

                for (i = 0; i < dwAChanTotalNum; i++)
                {
                    deviceListAnalogChannel(i + 1, m_struIpParaCfgV40.byAnalogChanEnable[i]);
                    iChannelNum[i] = i + (int)DeviceInfo.byStartChan;
                }

                byte byStreamType = 0;
                uint iDChanNum = 64;

                if (dwDChanTotalNum < 64)
                {
                    iDChanNum = dwDChanTotalNum; //如果设备IP通道小于64路，按实际路数获取
                }

                for (i = 0; i < iDChanNum; i++)
                {
                    iChannelNum[i + dwAChanTotalNum] = i + (int)m_struIpParaCfgV40.dwStartDChan;
                    byStreamType = m_struIpParaCfgV40.struStreamMode[i].byGetStreamType;

                    dwSize = (uint)Marshal.SizeOf(m_struIpParaCfgV40.struStreamMode[i].uGetStream);
                    switch (byStreamType)
                    {
                        //目前NVR仅支持直接从设备取流 NVR supports only the mode: get stream from device directly
                        case 0:
                            IntPtr ptrChanInfo = Marshal.AllocHGlobal((Int32)dwSize);
                            Marshal.StructureToPtr(m_struIpParaCfgV40.struStreamMode[i].uGetStream, ptrChanInfo, false);
                            struChanInfo = (CHCNetSDK.NET_DVR_IPCHANINFO)Marshal.PtrToStructure(ptrChanInfo, typeof(CHCNetSDK.NET_DVR_IPCHANINFO));

                            //列出IP通道 List the IP channel
                            deviceListIPChannel(i + 1, struChanInfo.byEnable, struChanInfo.byIPID);
                            iIPDevID[i] = struChanInfo.byIPID + struChanInfo.byIPIDHigh * 256 - iGroupNo * 64 - 1;

                            Marshal.FreeHGlobal(ptrChanInfo);
                            break;
                        case 6:
                            IntPtr ptrChanInfoV40 = Marshal.AllocHGlobal((Int32)dwSize);
                            Marshal.StructureToPtr(m_struIpParaCfgV40.struStreamMode[i].uGetStream, ptrChanInfoV40, false);
                            struChanInfoV40 = (CHCNetSDK.NET_DVR_IPCHANINFO_V40)Marshal.PtrToStructure(ptrChanInfoV40, typeof(CHCNetSDK.NET_DVR_IPCHANINFO_V40));

                            //列出IP通道 List the IP channel
                            deviceListIPChannel(i + 1, struChanInfoV40.byEnable, struChanInfoV40.wIPID);
                            iIPDevID[i] = struChanInfoV40.wIPID - iGroupNo * 64 - 1;

                            Marshal.FreeHGlobal(ptrChanInfoV40);
                            break;
                        default:
                            break;
                    }
                }
            }
            Marshal.FreeHGlobal(ptrIpParaCfgV40);
        }

        //
        public void deviceListIPChannel(Int32 iChanNo, byte byOnline, int byIPID)
        {
            str1 = string.Format("IPCamera {0}", iChanNo);
            m_lTree++;

            if (byIPID == 0)
            {
                str2 = "X"; //通道空闲，没有添加前端设备 the channel is idle                  
            }
            else
            {
                if (byOnline == 0)
                {
                    str2 = "offline"; //通道不在线 the channel is off-line
                }
                else
                    str2 = "online"; //通道在线 The channel is on-line
            }

            //listViewIPChannel.Items.Add(new ListViewItem(new string[] { str1, str2 }));将通道添加到列表中 add the channel to the list

        }

        //
        public void deviceListAnalogChannel(Int32 iChanNo, byte byEnable)
        {
            
            str1 = string.Format("Camera {0}", iChanNo);
            m_lTree++;

            if (byEnable == 0)
            {
                str2 = "Disabled"; //通道已被禁用 This channel has been disabled               
            }
            else
            {
                str2 = "Enabled"; //通道处于启用状态 This channel has been enabled
            }

            //将通道添加到列表中 add the channel to the list
            //listViewIPChannel.Items.Add(new ListViewItem(new string[] { str1, str2 }));
        }

        //实时预览
        public bool realPlay(PictureBox pictureBox)
        {
           
            // 检查设备登录状态
            if (UserID < 0)
            {
                MessageBox.Show("请先登录设备!");
                Ccommon.AddLog("请先登录设备!");
                return false;
            }
            // 检查录像状态
            if (m_bRecord)
            {
                MessageBox.Show("请先停止录像!");
                Ccommon.AddLog("请先停止录像!");
                return false;
            }
            if (m_lRealHandle < 0)
            {

                //预览参数结构体。
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();


                //播放窗口的句柄，为NULL表示不解码显示
                lpPreviewInfo.hPlayWnd = pictureBox.Handle;//预览窗口 live view window
                //通道号，目前设备模拟通道号从1开始，数字通道的起始通道号通过NET_DVR_GetDVRConfig（配置命令NET_DVR_GET_IPPARACFG_V40）获取（dwStartDChan)

                lpPreviewInfo.lChannel = iChannelNum[(int)iSelIndex];//预览的设备通道 the device channel number
                //码流类型：0-主码流，1-子码流，2-三码流，3-虚拟码流，以此类推 

                lpPreviewInfo.dwStreamType = 0;
                //连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4- RTP/RTSP，5- RTP/HTTP，6- HRUDP（可靠传输） 
                lpPreviewInfo.dwLinkMode = 0;
                //若设为不阻塞，表示发起与设备的连接就认为连接成功，如果发生码流接收失败、播放失败等情况以预览异常的方式通知上层。在循环播放的时候可以减短停顿的时间，与NET_DVR_RealPlay处理一致。
                //若设为阻塞，表示直到播放操作完成才返回成功与否，网络异常时SDK内部connect失败将会有5s的超时才能够返回，不适合于轮询取流操作。
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                //播放库播放缓冲区最大缓冲帧数，取值范围：1、6（默认，自适应播放模式）、15，置0时默认为1 
                lpPreviewInfo.dwDisplayBufNum = 15; //播放库显示缓冲区最大帧数

                IntPtr pUser = IntPtr.Zero;//用户数据 user data 
                //打开预览 Start live view 
                //[in] NET_DVR_Login_V40等登录接口的返回值 ,[in] 预览参数 ,[in] 码流数据回调函数,[in] 用户数据 

                m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(UserID, ref lpPreviewInfo, null/*RealData*/, pUser);

                //Console.WriteLine(m_lRealHandle);

                if (m_lRealHandle < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string str = "预览失败，输出错误号: " + iLastErr; //预览失败，输出错误号 failed to start live view, and output the error code.
                    //MessageBox.Show(str);
                    Ccommon.AddLog(str);
                    return false;
                }
                else
                {
                    //MessageBox.Show("预览成功");
                    Ccommon.AddLog(str);
                    return true;
                }
            }
            return true;

        }
        //无参预览
        public bool realPlay()
        {
            // 检查设备登录状态
            if (UserID < 0)
            {
                Ccommon.AddLog("请先登录设备!");
                return false;
            }
            // 检查录像状态
            if (m_bRecord)
            {
                Ccommon.AddLog("请先停止录像!");
                return false;
            }
            if (m_lRealHandle < 0)
            {

                //预览参数结构体。
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();


                //播放窗口的句柄，为NULL表示不解码显示
                //lpPreviewInfo.hPlayWnd = null;//预览窗口 live view window
                //通道号，目前设备模拟通道号从1开始，数字通道的起始通道号通过NET_DVR_GetDVRConfig（配置命令NET_DVR_GET_IPPARACFG_V40）获取（dwStartDChan)

                lpPreviewInfo.lChannel = 1;//预览的设备通道 the device channel number
                //码流类型：0-主码流，1-子码流，2-三码流，3-虚拟码流，以此类推 

                lpPreviewInfo.dwStreamType = 0;
                //连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4- RTP/RTSP，5- RTP/HTTP，6- HRUDP（可靠传输） 
                lpPreviewInfo.dwLinkMode = 0;
                //若设为不阻塞，表示发起与设备的连接就认为连接成功，如果发生码流接收失败、播放失败等情况以预览异常的方式通知上层。在循环播放的时候可以减短停顿的时间，与NET_DVR_RealPlay处理一致。
                //若设为阻塞，表示直到播放操作完成才返回成功与否，网络异常时SDK内部connect失败将会有5s的超时才能够返回，不适合于轮询取流操作。
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                //播放库播放缓冲区最大缓冲帧数，取值范围：1、6（默认，自适应播放模式）、15，置0时默认为1 
                lpPreviewInfo.dwDisplayBufNum = 15; //播放库显示缓冲区最大帧数

                IntPtr pUser = IntPtr.Zero;//用户数据 user data 
                //打开预览 Start live view 
                //[in] NET_DVR_Login_V40等登录接口的返回值 ,[in] 预览参数 ,[in] 码流数据回调函数,[in] 用户数据 

                m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(UserID, ref lpPreviewInfo, null/*RealData*/, pUser);

                Console.WriteLine(m_lRealHandle);

                if (m_lRealHandle < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string str = "预览失败，输出错误号: " + iLastErr; //预览失败，输出错误号 failed to start live view, and output the error code.
                    MessageBox.Show(str);
                    return false;
                }
                else
                {
                    MessageBox.Show("预览成功");
                    return true;
                }
            }
            return true;

        }
        
        //停止预览
        public void stopPlay()
        {
            if (!CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string str = "NET_DVR_StopRealPlay failed, error code= " + iLastErr;
                MessageBox.Show(str);
                return;
            }
            MessageBox.Show("NET_DVR_StopRealPlay succ!");
            m_lRealHandle = -1;
            //pictureBox.Invalidate();刷新窗口 refresh the window
            return;
        }

        //录像
        public string startRecord(string FileName)
        {
            //录像保存路径和文件名 the path and file name to save
            string sVideoFileName = FileName;

            
            if (m_bRecord == false)
            {
                //强制I帧 Make a I frame
                int lChannel = 1; //通道号 Channel number
                
                CHCNetSDK.NET_DVR_MakeKeyFrame(UserID, lChannel);


                //开始录像 Start recording
                if (!CHCNetSDK.NET_DVR_SaveRealData(m_lRealHandle, sVideoFileName))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string str = "NET_DVR_SaveRealData failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return null;
                }
                else
                {
                    MessageBox.Show("Succeed to recording...");
                    m_bRecord = true;
                    return sVideoFileName;
                }
            }
            else
            {
                //停止录像 Stop recording
                if (!CHCNetSDK.NET_DVR_StopSaveRealData(m_lRealHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string str = "NET_DVR_StopSaveRealData failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return null;
                }
                else
                {
                    string str = "Successful to stop recording and the saved file is " + sVideoFileName;
                    MessageBox.Show(str);
                    m_bRecord = false;
                    return sVideoFileName;
                }
            }
        }

        //停止录像
        public void stopRecord()
        {
             
        }

        //获取IP
        public string getIp()
        {
            return ip;
        }

        //获取端口号
        public Int16 getPort()
        {
            return port;
        }
        
        //获取用户名
        public string getUserName()
        {
            return username;
        }
        //获取用户密码
        public string getPassword()
        {
            return password;
        }

        //获取userID
        public Int32 getUserID()
        {
            return UserID;
        }

        //获取m_bRecord
        public bool getBRecord()
        {
            return m_bRecord;
        }

        //tostring()
        public void CameraToString()
        {
            Console.WriteLine("IP: " + getIp());
            Console.WriteLine("Port: " + getPort());
            Console.WriteLine("UserName: " + getUserName());
            Console.WriteLine("Password: " + getPassword());
            Console.WriteLine("UserID: " + getUserID());
        }

        //预览
        public void Priview(PictureBox pictureBox)
        {
           
            realPlay(pictureBox);
        }


        //无窗口参录像
        public void SaveRecord(string saveFile)
        {
            realPlay();
            startRecord(saveFile);
        }

        //抓图BMP
        public string saveBMP()
        {
            if (m_lRealHandle < 0)
            {
                MessageBox.Show("Please start live view firstly!"); //BMP抓图需要先打开预览
                return null;
            }
            DateTime date = DateTime.Today;
            string sBmpPicFileName;
            //图片保存路径和文件名 the path and file name to save
            //+ "-" + UserID + "_" + date.ToString("hh:mm:ss")
            sBmpPicFileName = "D:\\" + date.ToString("yyyy-MM-dd")  + ".bmp";

            //BMP抓图 Capture a BMP picture
            if (!CHCNetSDK.NET_DVR_CapturePicture(m_lRealHandle, sBmpPicFileName))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string str = "NET_DVR_CapturePicture failed, error code= " + iLastErr;
                MessageBox.Show(str);
                return null;
            }
            else
            {
                string str = "Successful to capture the BMP file and the saved file is " + sBmpPicFileName;
                MessageBox.Show(str);
            }
            return sBmpPicFileName;
        }

        //抓图JPEG
        public string saveJPEG()
        {
            DateTime date = DateTime.Today;
            string sJpegPicFileName;
            //图片保存路径和文件名 the path and file name to save
            // + "/" + UserID + "_" + date.ToString("hh:mm:ss")
            sJpegPicFileName = "D:\\" + date.ToString("yyyy-MM-dd") + ".jpg";
            int lChannel = 1; //通道号 Channel number
            CHCNetSDK.NET_DVR_JPEGPARA lpJpegPara = new CHCNetSDK.NET_DVR_JPEGPARA();
            lpJpegPara.wPicQuality = 0; //图像质量 Image quality 0-最好
            lpJpegPara.wPicSize = 0xff; //抓图分辨率 Picture size: 2- 4CIF，0xff- Auto(使用当前码流分辨率)，抓图分辨率需要设备支持，更多取值请参考SDK文档

            //JPEG抓图 Capture a JPEG picture
            if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(UserID, lChannel, ref lpJpegPara, sJpegPicFileName))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "NET_DVR_CaptureJPEGPicture failed, error code= " + iLastErr;
                MessageBox.Show(str);
                return null;
            }
            else
            {
                str = "Successful to capture the JPEG file and the saved file is " + sJpegPicFileName;
                MessageBox.Show(str);
            }
            return sJpegPicFileName;
        }

        
        //报警回调函数1
        public bool MsgCallback_V31(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            //通过lCommand来判断接收到的报警信息类型，不同的lCommand对应不同的pAlarmInfo内容
            AlarmMessageHandle(lCommand, ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

            return true; //回调函数需要有返回，表示正常接收到数据
        }
        //报警回调函数2
        public void MsgCallback(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            //通过lCommand来判断接收到的报警信息类型，不同的lCommand对应不同的pAlarmInfo内容
            AlarmMessageHandle(lCommand, ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
        }
        //报警回调函数3
        public void AlarmMessageHandle(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            //通过lCommand来判断接收到的报警信息类型，不同的lCommand对应不同的pAlarmInfo内容
            switch (lCommand)
            {
                case CHCNetSDK.COMM_UPLOAD_FACESNAP_RESULT://人脸抓拍结果信息
                    ProcessCommAlarm_FaceSnap(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                    /*
                case CHCNetSDK.COMM_SNAP_MATCH_ALARM://人脸比对结果信息
                    ProcessCommAlarm_FaceMatch(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                    */
                default:
                    {
                        /*
                        //报警设备IP地址
                        string strIP = ip;

                        //报警信息类型
                        string stringAlarm = "报警上传，信息类型：" + lCommand;

                        if (InvokeRequired)
                        {
                            object[] paras = new object[3];
                            paras[0] = DateTime.Now.ToString(); //当前PC系统时间
                            paras[1] = strIP;
                            paras[2] = stringAlarm;
                            listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
                        }
                        else
                        {
                            //创建该控件的主线程直接更新信息列表 
                            UpdateClientList(DateTime.Now.ToString(), strIP, stringAlarm);
                        }
                        */
                    }
                    break;
            }
        }

        //人脸抓拍结果信息
        private void ProcessCommAlarm_FaceSnap(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            CHCNetSDK.NET_VCA_FACESNAP_RESULT struFaceSnapInfo = new CHCNetSDK.NET_VCA_FACESNAP_RESULT();
            uint dwSize = (uint)Marshal.SizeOf(struFaceSnapInfo);
            struFaceSnapInfo = (CHCNetSDK.NET_VCA_FACESNAP_RESULT)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_VCA_FACESNAP_RESULT));

            //报警设备IP地址
            string strIP = ip;

            //保存抓拍图片数据
            if ((struFaceSnapInfo.dwBackgroundPicLen != 0) && (struFaceSnapInfo.pBuffer2 != IntPtr.Zero))
            {
                iPicNumber++;
                string str = ".\\picture\\FaceSnap_CapPic_[" + strIP + "]_lUerID_[" + pAlarmer.lUserID + "]_" + iPicNumber + ".jpg";
                FileStream fs = new FileStream(str, FileMode.Create);
                int iLen = (int)struFaceSnapInfo.dwBackgroundPicLen;
                byte[] by = new byte[iLen];
                Marshal.Copy(struFaceSnapInfo.pBuffer2, by, 0, iLen);
                fs.Write(by, 0, iLen);
                fs.Close();
            }

            //报警时间：年月日时分秒
            string strTimeYear = ((struFaceSnapInfo.dwAbsTime >> 26) + 2000).ToString();
            string strTimeMonth = ((struFaceSnapInfo.dwAbsTime >> 22) & 15).ToString("d2");
            string strTimeDay = ((struFaceSnapInfo.dwAbsTime >> 17) & 31).ToString("d2");
            string strTimeHour = ((struFaceSnapInfo.dwAbsTime >> 12) & 31).ToString("d2");
            string strTimeMinute = ((struFaceSnapInfo.dwAbsTime >> 6) & 63).ToString("d2");
            string strTimeSecond = ((struFaceSnapInfo.dwAbsTime >> 0) & 63).ToString("d2");
            string strTime = strTimeYear + "-" + strTimeMonth + "-" + strTimeDay + " " + strTimeHour + ":" + strTimeMinute + ":" + strTimeSecond;

            string stringAlarm = "人脸抓拍结果，前端设备IP：" + strIP + "，抓拍时间：" + strTime;
            /*
            if (InvokeRequired)
            {
                object[] paras = new object[3];
                paras[0] = DateTime.Now.ToString(); //当前PC系统时间
                paras[1] = strIP;
                paras[2] = stringAlarm;
                listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
            }
            else
            {
                //创建该控件的主线程直接更新信息列表 
                UpdateClientList(DateTime.Now.ToString(), strIP, stringAlarm);
            }
            */
        }

        //设置布防句柄
        public void SetM_lAlarmHandle(Int32 AlarmHandle)
        {
            m_lAlarmHandle = AlarmHandle;
        }
        //返回布防句柄
        public Int32 GetM_lAlarmHandle()
        {
            return m_lAlarmHandle;
        }
        //布防
        public void SetAlarm(CHCNetSDK.NET_DVR_SETUPALARM_PARAM struAlarmParam)
        {
            m_lAlarmHandle = CHCNetSDK.NET_DVR_SetupAlarmChan_V41(UserID, ref struAlarmParam);
        }
        //撤防
        public bool CloseAlarm()
        {
            if(m_lAlarmHandle >= 0)
            {
                if (!CHCNetSDK.NET_DVR_CloseAlarmChan_V30(m_lAlarmHandle))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }

        /*
        //人脸比对结果信息
        private void ProcessCommAlarm_FaceMatch(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            CHCNetSDK.NET_VCA_FACESNAP_MATCH_ALARM struFaceMatchAlarm = new CHCNetSDK.NET_VCA_FACESNAP_MATCH_ALARM();
            uint dwSize = (uint)Marshal.SizeOf(struFaceMatchAlarm);
            struFaceMatchAlarm = (CHCNetSDK.NET_VCA_FACESNAP_MATCH_ALARM)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_VCA_FACESNAP_MATCH_ALARM));

            //报警设备IP地址
            string strIP = ip;

            //保存抓拍人脸子图图片数据
            if ((struFaceMatchAlarm.struSnapInfo.dwSnapFacePicLen != 0) && (struFaceMatchAlarm.struSnapInfo.pBuffer1 != IntPtr.Zero))
            {
                iPicNumber++;
                string str = ".\\picture\\FaceMatch_FacePic_[" + strIP + "]_lUerID_[" + pAlarmer.lUserID + "]_" + iPicNumber + ".jpg";
                FileStream fs = new FileStream(str, FileMode.Create);
                int iLen = (int)struFaceMatchAlarm.struSnapInfo.dwSnapFacePicLen;
                byte[] by = new byte[iLen];
                Marshal.Copy(struFaceMatchAlarm.struSnapInfo.pBuffer1, by, 0, iLen);
                fs.Write(by, 0, iLen);
                fs.Close();
            }

            //保存比对结果人脸库人脸图片数据
            if ((struFaceMatchAlarm.struBlackListInfo.dwBlackListPicLen != 0) && (struFaceMatchAlarm.struBlackListInfo.pBuffer1 != IntPtr.Zero))
            {
                iPicNumber++;
                string str = ".\\picture\\FaceMatch_BlackListPic_[" + strIP + "]_lUerID_[" + pAlarmer.lUserID + "]" +
                    "_fSimilarity[" + struFaceMatchAlarm.fSimilarity + "]_" + iPicNumber + ".jpg";
                FileStream fs = new FileStream(str, FileMode.Create);
                int iLen = (int)struFaceMatchAlarm.struBlackListInfo.dwBlackListPicLen;
                byte[] by = new byte[iLen];
                Marshal.Copy(struFaceMatchAlarm.struBlackListInfo.pBuffer1, by, 0, iLen);
                fs.Write(by, 0, iLen);
                fs.Close();
            }

            //抓拍时间：年月日时分秒
            string strTimeYear = ((struFaceMatchAlarm.struSnapInfo.dwAbsTime >> 26) + 2000).ToString();
            string strTimeMonth = ((struFaceMatchAlarm.struSnapInfo.dwAbsTime >> 22) & 15).ToString("d2");
            string strTimeDay = ((struFaceMatchAlarm.struSnapInfo.dwAbsTime >> 17) & 31).ToString("d2");
            string strTimeHour = ((struFaceMatchAlarm.struSnapInfo.dwAbsTime >> 12) & 31).ToString("d2");
            string strTimeMinute = ((struFaceMatchAlarm.struSnapInfo.dwAbsTime >> 6) & 63).ToString("d2");
            string strTimeSecond = ((struFaceMatchAlarm.struSnapInfo.dwAbsTime >> 0) & 63).ToString("d2");
            string strTime = strTimeYear + "-" + strTimeMonth + "-" + strTimeDay + " " + strTimeHour + ":" + strTimeMinute + ":" + strTimeSecond;

            string stringAlarm = "人脸比对报警，抓拍设备：" + System.Text.Encoding.UTF8.GetString(struFaceMatchAlarm.struSnapInfo.struDevInfo.struDevIP.sIpV4).TrimEnd('\0') + "，抓拍时间："
                + strTime + "，相似度：" + struFaceMatchAlarm.fSimilarity;

            if (InvokeRequired)
            {
                object[] paras = new object[3];
                paras[0] = DateTime.Now.ToString(); //当前PC系统时间
                paras[1] = strIP;
                paras[2] = stringAlarm;
                listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
            }
            else
            {
                //创建该控件的主线程直接更新信息列表 
                UpdateClientList(DateTime.Now.ToString(), strIP, stringAlarm);
            }
        }
        */
        /*
        //启动监听
        private void startListen()
        {
            string sLocalIP = textBoxListenIP.Text;
            ushort wLocalPort = ushort.Parse(textBoxListenPort.Text);

            if (m_falarmData == null)
            {
                m_falarmData = new CHCNetSDK.MSGCallBack(MsgCallback);
            }

            iListenHandle = CHCNetSDK.NET_DVR_StartListen_V30(sLocalIP, wLocalPort, m_falarmData, IntPtr.Zero);
            if (iListenHandle < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                strErr = "启动监听失败，错误号：" + iLastErr; //启动监听失败，输出错误号
                MessageBox.Show(strErr);
            }
            else
            {
                MessageBox.Show("成功启动监听！");
                btnStopListen.Enabled = true;
                btnStartListen.Enabled = false;
            }
        }
        //关闭监听
        private void stopListen()
        {
            if (!CHCNetSDK.NET_DVR_StopListen_V30(iListenHandle))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                strErr = "停止监听失败，错误号：" + iLastErr; //撤防失败，输出错误号
                MessageBox.Show(strErr);
            }
            else
            {
                MessageBox.Show("停止监听！");
                btnStopListen.Enabled = false;
                btnStartListen.Enabled = true;
            }
        }
        */

    }
}
