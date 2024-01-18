using Masuit.Tools.Strings;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LvPlayer.Utils
{
    class SqliteUtil
    {
        public static string dbfile = @"URI=file:app.db";
        public static SQLiteConnection cnn;

        /// <summary>
        /// 连接数据库
        /// </summary>
        public static void connectDb()
        {
            cnn = new SQLiteConnection(dbfile);
            cnn.Open();
        }
        /// <summary>
        /// 关闭sqllite连接
        /// </summary>
        public static void closeDb()
        {
            cnn.Close();
        }
        /// <summary>
        /// 初始化所有的表
        /// </summary>
        public static void initTable()
        {
            string sql = "Create table ListChache (Id integer primary key AUTOINCREMENT, Title text,CacheTime text,VideoLength text,VideoImg text,VideoClass text);";
            SQLiteCommand cmd = new SQLiteCommand(sql, cnn);
            cmd.ExecuteNonQuery();
        }

    }
}
