using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using twpx.VIew;

namespace twpx
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Common Pcommon = new Common();
            Pcommon.InitLog();//初始化日志
            Pcommon.InitSDK();//初始化SDK
            Pcommon.LoadData();//读取设备信息并登录
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
            Pcommon.cleanSDK();//注销SDK
            Pcommon.SaveLog();//保存日志
        }


    }
}
