using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification
{
    class IPSMySqlConnection
    {
        public static string getActText(DateTime data, string number)
        {
            string connect = @"Server=182.5.202.106; Port=3306; Database=r017700; Uid=admin; Pwd=nimda;";

            MySqlConnection connection = new MySqlConnection(connect);
            string responce = String.Empty;
            try
            {

                connection.Open();
            }
            catch (Exception se)
            {
                Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                return se.Message;
            }
            string com = String.Format("SELECT CODE FROM ltb_passings WHERE PASSDATE='{0}' AND PASSNUM='{1}'", data.ToString("yyyy-MM-dd"), number.Replace("№ ", ""));

            var cmd = new MySqlCommand(com, connection);
            // формат даты гггг-мм-дд%
            using (MySqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    com = String.Format("SELECT Value FROM ltb_docs WHERE CODE='{0}'", dr[0].ToString());
                }

            }
            cmd = new MySqlCommand(com, connection);
            using (MySqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    responce = dr[0].ToString();
                }

            }
            connection.Close();
            return responce;
        }
    }
}
