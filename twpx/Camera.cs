using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NVRCsharpDemo;
using twpx.Model;

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
        private Int32 m_lPort = -1;
        private uint iLastErr = 0; // 用于接收调用NET_DVR_GetLastError获取的错误码
        private IntPtr m_ptrRealHandle;
        private int[] iIPDevID = new int[96];
        private int[] iChannelNum = new int[96];
        public CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo;// 设备信息结构体
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
        private bool m_bReverse;
        private bool m_bPause;
        private string strResult; //存储查找路径

        

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

        //设备登录
        public void login()
        {
            UserID = CHCNetSDK.NET_DVR_Login_V30(ip, port, username, password, ref DeviceInfo);
            if (UserID < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                Console.WriteLine("iLastErr = " + iLastErr);
            }
            else
            {
                //登录成功
                Console.WriteLine("userId = " + UserID);

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

                MessageBox.Show("登录成功.");
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
        public void realPlay(PictureBox pictureBox)
        {
           
            // 检查设备登录状态
            if (UserID < 0)
            {
                MessageBox.Show("请先登录设备!");
                return;
            }
            // 检查录像状态
            if (m_bRecord)
            {
                MessageBox.Show("请先停止录像!");
                return;
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
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    MessageBox.Show("预览成功");
                    return;
                }
            }

        }

        public bool realPlay()
        {
            // 检查设备登录状态
            if (UserID < 0)
            {
                MessageBox.Show("请先登录设备!");
                return false;
            }
            // 检查录像状态
            if (m_bRecord)
            {
                MessageBox.Show("请先停止录像!");
                return false;
            }
            if (m_lRealHandle < 0)
            {

                //预览参数结构体。
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();


                //播放窗口的句柄，为NULL表示不解码显示
                //lpPreviewInfo.hPlayWnd = pictureBox.Handle;预览窗口 live view window
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
                    MessageBox.Show(str);
                    return false;
                }
                else
                {
                    MessageBox.Show("预览成功");
                    return true;
                }
            }
            return false;
        }

        //停止预览
        public void stopPlay(PictureBox pictureBox)
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
            pictureBox.Invalidate();//刷新窗口 refresh the window
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


        //单独调用录像
        public void SaveRecord(string saveFile)
        {
            login();
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


        //录像文件查找
        // 参数开始年月日-结束年月日
        public void findFile(int y1, int m1, int d1, int y2, int m2, int d2)
        {
            //DateTimePicker dateTimeStart = new DateTimePicker();//dateTime选择
            //DateTimePicker dateTimeEnd = new DateTimePicker();//
            DateTime dateTimeStart = new DateTime(y1, m1, d1, 0, 0, 0);
            DateTime dateTimeEnd = new DateTime(y2, m2, d2, 23, 50, 59);

            CHCNetSDK.NET_DVR_FILECOND_V40 struFileCond_V40 = new CHCNetSDK.NET_DVR_FILECOND_V40();
            struFileCond_V40.lChannel = iChannelNum[0]; //通道号 Channel number
            struFileCond_V40.dwFileType = 0xff; //0xff-全部，0-定时录像，1-移动侦测，2-报警触发，...
            struFileCond_V40.dwIsLocked = 0xff; //0-未锁定文件，1-锁定文件，0xff表示所有文件（包括锁定和未锁定）

            //设置录像查找的开始时间 Set the starting time to search video files
            struFileCond_V40.struStartTime.dwYear = (uint)dateTimeStart.Year;
            struFileCond_V40.struStartTime.dwMonth = (uint)dateTimeStart.Month;
            struFileCond_V40.struStartTime.dwDay = (uint)dateTimeStart.Day;
            struFileCond_V40.struStartTime.dwHour = (uint)dateTimeStart.Hour;
            struFileCond_V40.struStartTime.dwMinute = (uint)dateTimeStart.Minute;
            struFileCond_V40.struStartTime.dwSecond = (uint)dateTimeStart.Second;

            //设置录像查找的结束时间 Set the stopping time to search video files
            struFileCond_V40.struStopTime.dwYear = (uint)dateTimeEnd.Year;
            struFileCond_V40.struStopTime.dwMonth = (uint)dateTimeEnd.Month;
            struFileCond_V40.struStopTime.dwDay = (uint)dateTimeEnd.Day;
            struFileCond_V40.struStopTime.dwHour = (uint)dateTimeEnd.Hour;
            struFileCond_V40.struStopTime.dwMinute = (uint)dateTimeEnd.Minute;
            struFileCond_V40.struStopTime.dwSecond = (uint)dateTimeEnd.Second;

            //开始录像文件查找 Start to search video files 
            m_lFindHandle = CHCNetSDK.NET_DVR_FindFile_V40(UserID, ref struFileCond_V40);
            //  找不到
            if (m_lFindHandle < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string str = "NET_DVR_FindFile_V40 failed, error code= " + iLastErr; //预览失败，输出错误号
                MessageBox.Show(str);
                return;
            }
            else
            {
                CHCNetSDK.NET_DVR_FINDDATA_V30 struFileData = new CHCNetSDK.NET_DVR_FINDDATA_V30();
                while (true)
                {
                    //逐个获取查找到的文件信息 Get file information one by one.
                    int result = CHCNetSDK.NET_DVR_FindNextFile_V30(m_lFindHandle, ref struFileData);

                    if (result == CHCNetSDK.NET_DVR_ISFINDING)  //正在查找请等待 Searching, please wait
                    {
                        continue;
                    }
                    else if (result == CHCNetSDK.NET_DVR_FILE_SUCCESS) //获取文件信息成功 Get the file information successfully
                    {
                        string str1 = struFileData.sFileName;

                        string str2 = Convert.ToString(struFileData.struStartTime.dwYear) + "-" +
                            Convert.ToString(struFileData.struStartTime.dwMonth) + "-" +
                            Convert.ToString(struFileData.struStartTime.dwDay) + " " +
                            Convert.ToString(struFileData.struStartTime.dwHour) + ":" +
                            Convert.ToString(struFileData.struStartTime.dwMinute) + ":" +
                            Convert.ToString(struFileData.struStartTime.dwSecond);

                        string str3 = Convert.ToString(struFileData.struStopTime.dwYear) + "-" +
                            Convert.ToString(struFileData.struStopTime.dwMonth) + "-" +
                            Convert.ToString(struFileData.struStopTime.dwDay) + " " +
                            Convert.ToString(struFileData.struStopTime.dwHour) + ":" +
                            Convert.ToString(struFileData.struStopTime.dwMinute) + ":" +
                            Convert.ToString(struFileData.struStopTime.dwSecond);

                        strResult = str1;
                        Console.WriteLine(strResult);
                        //listViewFile.Items.Add(new ListViewItem(new string[] { str1, str2, str3 }));//将查找的录像文件添加到列表中

                    }
                    else if (result == CHCNetSDK.NET_DVR_FILE_NOFIND || result == CHCNetSDK.NET_DVR_NOMOREFILE)
                    {
                        break; //未查找到文件或者查找结束，退出   No file found or no more file found, search is finished 
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        // 录像回放(文件名，播放窗口)
        public void playBack(string sPlayBackFileName, PictureBox VideoPlayWnd)
        {
            Timer timerPlayback = new Timer();
            if (sPlayBackFileName == null)
            {
                MessageBox.Show("Please select one file firstly!");//先选择回放的文件
                return;
            }

            if (m_lPlayHandle >= 0)
            {
                //如果已经正在回放，先停止回放
                if (!CHCNetSDK.NET_DVR_StopPlayBack(m_lPlayHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string str = "NET_DVR_StopPlayBack failed, error code= " + iLastErr; //停止回放失败，输出错误号
                    MessageBox.Show(str);
                    return;
                }

                m_bReverse = false;
                //btnReverse.Text = "Reverse";
                //labelReverse.Text = "切换为倒放";

                m_bPause = false;
                //btnPause.Text = "||";
                //labelPause.Text = "暂停";

                m_lPlayHandle = -1;
                //PlaybackprogressBar.Value = 0;
            }

            //按文件名回放
            m_lPlayHandle = CHCNetSDK.NET_DVR_PlayBackByName(UserID, sPlayBackFileName, VideoPlayWnd.Handle);
            Console.WriteLine("UserID = " + UserID + ", m_lPlayHandle = " + m_lPlayHandle);
            if (m_lPlayHandle < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string str = "NET_DVR_PlayBackByName failed, error code= " + iLastErr;
                MessageBox.Show(str);
                return;
            }

            uint iOutValue = 0;
            if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lPlayHandle, CHCNetSDK.NET_DVR_PLAYSTART, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string str = "NET_DVR_PLAYSTART failed, error code= " + iLastErr; //回放控制失败，输出错误号
                MessageBox.Show(str);
                return;
            }
            timerPlayback.Interval = 1000;
            timerPlayback.Enabled = true;
        }
        
        //录像回放汇总
        public void playBack2(int y1, int m1, int d1, int y2, int m2, int d2, PictureBox VideoPlayWnd)
        {
            findFile(y1, m1, d1, y2, m2, d2);
            playBack(strResult, VideoPlayWnd);
        }


    }
}
