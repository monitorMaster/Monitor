using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twpx.Dao
{
    public class SugarDao
    {
        public static string ConnectionString
        {
            get
            {
                string reval = "server=localhost;uid=muamua;pwd=muamua;database=monitor";
                //string reval = "server=123.206.206.93;uid=root;pwd=1997liyiyong;database=video";
                return reval;
            }
        }
        public static SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() {
                ConnectionString = ConnectionString,
                DbType = DbType.MySql,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.SystemTable
            });
           
            return db;
        }
    }

}
