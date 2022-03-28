using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Core.Methods
{
    public class IPS30
    {
        readonly static Logger logger = LogManager.GetCurrentClassLogger();
        public static string GetActTextFromIPS30(DateTime data, string number)
        {
            string connect = @"Server=182.5.202.106; Port=3306; Database=r017700; Uid=admin; Pwd=nimda;";
            string responce = "Ошибка получения текста из базы ИПС 3.0";
            try
            {
                MySqlConnection connection = new MySqlConnection(connect);

                connection.Open();
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
            catch (MySqlException sqex)
            {
                logger.Fatal(sqex);
                return responce;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return responce;
            }

        }
    }
}
