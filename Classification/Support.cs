using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification
{
    class Support
    {

        public static void LoggingError(string classForException, string moduleName, string log)
        {
            try
            {
                File.AppendAllText(Path.Combine(@"logs", DateTime.Now.ToShortDateString() + ".txt"), String.Format("NameSpace: {0} Module: {1} Date:{2} Exception:{3}", classForException, moduleName, DateTime.Now.ToString(" dd.MM.yyyy-HH:mm:ss "), log + Environment.NewLine));
            }
            catch (System.Exception ex)
            {
                File.AppendAllText(@"logs\ERROR_WCF.TXT", ex.Message);
            }

        }
    }
}
