using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Windows;
using System.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Classification
{
    class SQLMethods
    {
        private SqlConnection getSqlConnection
        {
            get
            {
                return new SqlConnection("Data Source=182.5.202.220;Initial Catalog=Classification;User ID=phobos;Password=iksar");
            }

        }

        public async void AddingDataToTables(int id, DateTime signdata, string actnumber, string actname)
        {
            try
            {
                using (SqlConnection conn = getSqlConnection)
                {
                    try
                    {
                        await conn.OpenAsync();
                    }
                    catch (SqlException se)
                    {
                        Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                    }
                    SqlCommand command = new SqlCommand(@"INSERT INTO ActsForClassification (Id, ActSignData, ActNumber, ActName) values (@id, @signdata, @actnumber, @actname)");
                    int identifier = int.Parse(DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + id.ToString());
                    command.Parameters.Add("@id", SqlDbType.Int, 10, "Id").Value = identifier;
                    command.Parameters.Add("@signdata", SqlDbType.Date, 10, "ActSignData").Value = signdata;
                    command.Parameters.Add("@actnumber", SqlDbType.NVarChar, 50, "ActNumber").Value = actnumber;
                    command.Parameters.Add("@actname", SqlDbType.NVarChar, 4000, "ActName").Value = actname;
                    command.Connection = conn;
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (System.Exception ex)
            {
                Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

        }

        public List<TextBlockSelection> GetEtalonKeywords()
        {
            List<TextBlockSelection> list = new List<TextBlockSelection>();
            try
            {
                using (SqlConnection conn = getSqlConnection)
                {

                    try
                    {
                        conn.Open();
                    }
                    catch (SqlException se)
                    {
                        Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                    }
                    SqlCommand cmd = new SqlCommand(@"Select MainKey From KeysHighLight");
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            if (dr.HasRows)
                            {
                                list.Add(new TextBlockSelection(dr.GetString(0), string.Empty));
                            }
                        }
                    }

                }
                return list;
            }

            catch (Exception ex)
            {
                Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return null;
            }
        }

        public List<TextBlockSelection> GetEtalonRubrics()
        {
                List<TextBlockSelection> list = new List<TextBlockSelection>();
                try
                {

                    using (SqlConnection conn = getSqlConnection)
                    {

                        try
                        {
                            conn.Open();
                        }
                        catch (SqlException se)
                        {
                            Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                        }
                        SqlCommand cmd = new SqlCommand(@"Select Rubrics From ClassificationsRubrics");
                        cmd.Connection = conn;
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dr.Read())
                            {
                                if (dr.HasRows)
                                {
                                    list.Add(new TextBlockSelection(dr.GetString(0), string.Empty));
                                }
                            }
                        }

                    }
                    return list;
                }

                catch (Exception ex)
                {
                    Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    return null;
                }           
        }

        //public int AddingDataToTables(int id, DateTime signdata, string actnumber, string actname)
        //{
        //    SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //    SQ.Open();
        //    string acttext = "Документ не найден!";
        //    acttext = MySqlDat.Connect(signdata, actnumber).Replace('\'', ' ');
        //    string sql = @"insert into ActsForClassification (id, ActSignData, ActNumber, ActName, ActText) values (@id, @signdate, @actnumber, @actname , @acttext)";           
        //    SQLiteCommand command = new SQLiteCommand(sql, SQ);
        //    command.Parameters.Add("@id", DbType.Int16).Value = id;
        //    command.Parameters.Add("@signdate", DbType.Date).Value = signdata;
        //    command.Parameters.Add("@actnumber", DbType.String, 8).Value = actnumber;
        //    command.Parameters.Add("@actname", DbType.String, 300).Value = actname;
        //    command.Parameters.Add("@acttext", DbType.String).Value = acttext;
        //    return command.ExecuteNonQuery();
        //}
        //public List<SearchAndDestroy> GetEtalonKeywords
        //{
        //    get
        //    {
        //        SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //        SQ.Open();
        //        List<SearchAndDestroy> list = new List<SearchAndDestroy>();
        //        SQLiteCommand command = new SQLiteCommand(SQ);
        //        command.CommandText = @"Select MainKey From KeysHighLight";
        //        command.CommandType = CommandType.Text;
        //        SQLiteDataReader reader = command.ExecuteReader();
        //        if (reader.HasRows)
        //        {
        //            while (reader.Read())
        //            {
        //                if (!reader.IsDBNull(0))
        //                {
        //                    list.Add(new SearchAndDestroy(reader.GetString(0), string.Empty));
        //                }

        //            }
        //        }
        //        command.Dispose();
        //        SQ.Close();
        //        return list;
        //    }
        //}

        //public List<SearchAndDestroy> GetEtalonRubrics
        //{
        //    get
        //    {
        //        SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //        SQ.Open();
        //        List<SearchAndDestroy> list = new List<SearchAndDestroy>();
        //        SQLiteCommand command = new SQLiteCommand(SQ);
        //        command.CommandText = @"Select Rubrics From ClassificationsRubrics";
        //        command.CommandType = CommandType.Text;
        //        SQLiteDataReader reader = command.ExecuteReader();
        //        if (reader.HasRows)
        //        {
        //            while (reader.Read())
        //            {
        //                if (!reader.IsDBNull(0))
        //                {
        //                    list.Add(new SearchAndDestroy(reader.GetString(0), string.Empty));
        //                }

        //            }
        //        }
        //        command.Dispose();
        //        SQ.Close();
        //        return list;
        //    }
        //}


        public async Task<string> GetMainKeyWordFromBase(string key)
        {
            string k = string.Empty;
            try
            {
                using (SqlConnection conn = getSqlConnection)
                {

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                    }
                    catch (SqlException se)
                    {
                        Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                    }
                    SqlCommand cmd = new SqlCommand(@"SELECT MainKey FROM KeysHighLight where MainKey = @key or key1 = @key or key2 = @key or key3 = @key or key4 = @key or key5 = @key");
                    cmd.Parameters.Add("@key", SqlDbType.NVarChar, 50).Value = key;
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (await dr.ReadAsync())
                        {
                            if (dr.HasRows)
                            {
                                k = dr.GetString(0);
                            }
                        }
                    }

                }
                return k;
            }

            catch (Exception ex)
            {
                Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return k;
            }

        }
        //public void AddingKeysOrRubricsToBase(List<string> arr, string TableName)
        //{
        //    SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //    SQ.Open();
        //    string sql = string.Empty;
        //    foreach (string s in arr)
        //    {
        //        sql += @"insert into ClassificationsRubrics ('" + TableName + "') values ('" + s.Trim() + "');";
        //    }
        //    SQLiteCommand command = new SQLiteCommand(sql, SQ);
        //    command.ExecuteNonQuery();
        //    command.Dispose();
        //    SQ.Close();

        //}

        //public string GetMainKeyWordFromBase(string key)
        //{
        //    string s = string.Empty;
        //    if (key.Length> 0)
        //    {
        //    SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //    SQ.Open();           
        //    key = key.Trim().ToLower();
        //    var mkey = new StringBuilder(key);
        //    mkey[0] = char.ToUpper(mkey[0]);
        //    string mainkey = mkey.ToString();
        //    SQLiteCommand command = new SQLiteCommand(SQ);
        //    command.CommandText = @"SELECT KeysHighLight.MainKey FROM KeysHighLight where KeysHighLight.[MainKey] = '" + mainkey + "' or KeysHighLight.[key1] = '" + key + "' or KeysHighLight.[key2] = '" + key + "' or KeysHighLight.[key3] = '" + key + "' or KeysHighLight.[key4] = '" + key + "'";
        //    command.CommandType = CommandType.Text;
        //    SQLiteDataReader reader = command.ExecuteReader();
        //    if (reader.HasRows)
        //    {
        //        while (reader.Read())
        //        {
        //            if (!reader.IsDBNull(0))
        //            {
        //                s = reader.GetString(0);
        //            }

        //        }
        //    }
        //    command.Dispose();
        //    SQ.Close();
        //    }
        //    return s;

        //}


        public async Task<List<string>> LoadAllKeywords()
        {
            List<string> list = new List<string>();
            try
            {
                using (SqlConnection conn = getSqlConnection)
                {

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                    }
                    catch (SqlException se)
                    {
                        Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                    }
                    SqlCommand cmd = new SqlCommand(@"Select KeysHighLight.MainKey From KeysHighLight union
                                    select KeysHighLight.key1 from KeysHighLight union
                                    select KeysHighLight.key2 from KeysHighLight union
                                    select KeysHighLight.key3 from KeysHighLight union
                                    select KeysHighLight.key4 from KeysHighLight union
                                    select KeysHighLight.key5 from KeysHighLight   order by KeysHighLight.MainKey");
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (await dr.ReadAsync())
                        {
                            if (dr.HasRows)
                            {
                                if (!dr.IsDBNull(0))
                                {
                                    list.Add(dr.GetString(0));
                                }
                                
                            }
                        }
                    }

                }
                return list;
            }

            catch (Exception ex)
            {
                Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return null;
            }
        }



        //        public List<string> LoadAllKeywords
        //        {
        //            get
        //            {
        //                SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //                SQ.Open();
        //                List<string> list = new List<string>();
        //                SQLiteCommand command = new SQLiteCommand(SQ);
        //                command.CommandText = @"Select KeysHighLight.MainKey From KeysHighLight union
        //                                    select KeysHighLight.key1 from KeysHighLight union
        //                                    select KeysHighLight.key2 from KeysHighLight union
        //                                    select KeysHighLight.key3 from KeysHighLight union
        //                                    select KeysHighLight.key4 from KeysHighLight order by KeysHighLight.MainKey";
        //                command.CommandType = CommandType.Text;
        //                SQLiteDataReader reader = command.ExecuteReader();
        //                if (reader.HasRows)
        //                {
        //                    while (reader.Read())
        //                    {
        //                        if (!reader.IsDBNull(0))
        //                        {
        //                            list.Add((reader.GetString(0)));
        //                        }

        //                    }
        //                }
        //                command.Dispose();
        //                SQ.Close();
        //                return list;
        //            }
        //        }

        public async void DeleteDataFromTables()
        {
            try
            {
                using (SqlConnection conn = getSqlConnection)
                {
                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                    }
                    catch (SqlException se)
                    {
                        Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                    }
                    SqlCommand command = new SqlCommand(@"Delete From ActsForClassification");
                    command.Connection = conn;
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (System.Exception ex)
            {
                Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

        }

        public async void UpdateIsReady(int id, bool ready = true)
        {
            try
            {
                using (SqlConnection conn = getSqlConnection)
                {
                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                    }
                    catch (SqlException se)
                    {
                        Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                    }
                    SqlCommand command = new SqlCommand(@"update ActsForClassification SET IsReady = @ready where ActsForClassification.id= @id");
                    command.Parameters.Add("@id", SqlDbType.Int, 10, "Id").Value = id;
                    command.Parameters.Add("@ready", SqlDbType.Bit, 1, "IsReady").Value = ready ? 1 : (object)DBNull.Value;
                    command.Connection = conn;
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (System.Exception ex)
            {
                Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public async void SetKeywords(int id, List<string> list)
        {
            object k1 = DBNull.Value;
            object k2 = DBNull.Value;
            object k3 = DBNull.Value;
            object k4 = DBNull.Value;
            object k5 = DBNull.Value;
            object k6 = DBNull.Value;
            int index = list.Count() - 1;
            try
            {
                using (SqlConnection conn = getSqlConnection)
                {
                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                    }
                    catch (SqlException se)
                    {
                        Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                    }
                    SqlCommand command = new SqlCommand(@"IF EXISTS(SELECT * FROM ActsKeyWords WHERE Id = @id) 
                                                              BEGIN
                                                                UPDATE ActsKeyWords SET key1 = @key1, key2 = @key2, key3 = @key3, key4 = @key4, key5 = @key5, key6 = @key6 WHERE Id = @id
                                                              END
                                                              ELSE
                                                              BEGIN
                                                                INSERT INTO ActsKeyWords (id, key1, key2, key3, key4, key5, key6) values (@id, @key1, @key2, @key3, @key4, @key5, @key6)
                                                              END", conn);
                    if (list.Count() == 0)
                    {

                        command.Parameters.Add("@id", SqlDbType.Int, 10, "Id").Value = id;
                        command.Parameters.Add("@key1", SqlDbType.NVarChar, 50, "key1").Value = k1;
                        command.Parameters.Add("@key2", SqlDbType.NVarChar, 50, "key2").Value = k2;
                        command.Parameters.Add("@key3", SqlDbType.NVarChar, 50, "key3").Value = k3;
                        command.Parameters.Add("@key4", SqlDbType.NVarChar, 50, "key4").Value = k4;
                        command.Parameters.Add("@key5", SqlDbType.NVarChar, 50, "key5").Value = k5;
                        command.Parameters.Add("@key6", SqlDbType.NVarChar, 50, "key6").Value = k6;
                        await command.ExecuteNonQueryAsync();
                        //command.Dispose();
                    }
                    else
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@id", SqlDbType.Int, 10, "Id").Value = id;
                        int end = 0;
                        for (int i = 0; i < list.Count(); i++)
                        {
                            command.Parameters.Add("@key" + (i+1).ToString(), SqlDbType.NVarChar, 50, "key" + (i+1).ToString()).Value = list[i];
                            end++;
                        }
                        for (int i = end; i < 6; i++)
                        {
                            command.Parameters.Add("@key" + (i + 1).ToString(), SqlDbType.NVarChar, 50, "key" + (i + 1).ToString()).Value = DBNull.Value;
                        }
                        command.ExecuteNonQuery();
                        //command.Dispose();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

        }

        public async void SetRubrics(int id, List<string> list)
        {
            object rub1 = DBNull.Value;
            object rub2 = DBNull.Value;
            object rub3 = DBNull.Value;

            int index = list.Count() - 1;
            try
            {
                using (SqlConnection conn = getSqlConnection)
                {
                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                    }
                    catch (SqlException se)
                    {
                        Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                    }
                    SqlCommand command = new SqlCommand(@"IF EXISTS(SELECT * FROM ActsRubrics WHERE Id = @id) 
                                                              BEGIN
                                                                UPDATE ActsRubrics SET rub1 = @rub1, rub2 = @rub2, rub3 = @rub3 WHERE Id = @id
                                                              END
                                                              ELSE
                                                              BEGIN
                                                                INSERT INTO ActsRubrics (id, rub1, rub2, rub3) values (@id, @rub1, @rub2, @rub3)
                                                              END", conn);
                    if (list.Count() == 0)
                    {

                        command.Parameters.Add("@id", SqlDbType.Int, 10, "Id").Value = id;
                        command.Parameters.Add("@rub1", SqlDbType.NVarChar, 400, "rub1").Value = rub1;
                        command.Parameters.Add("@rub2", SqlDbType.NVarChar, 400, "rub2").Value = rub2;
                        command.Parameters.Add("@rub3", SqlDbType.NVarChar, 400, "rub3").Value = rub3;
                        await command.ExecuteNonQueryAsync();
                        command.Dispose();
                    }
                    else
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@id", SqlDbType.Int, 10, "Id").Value = id;
                        int end = 0;
                        for (int i = 0; i < list.Count(); i++)
                        {
                            command.Parameters.Add("@rub" + (i + 1).ToString(), SqlDbType.NVarChar, 400, "rub" + (i + 1).ToString()).Value = list[i];
                            end++;
                        }
                        for (int i = end; i < 3; i++)
                        {
                            command.Parameters.Add("@rub" + (i + 1).ToString(), SqlDbType.NVarChar, 400, "rub" + (i + 1).ToString()).Value = DBNull.Value;
                        }
                        await command.ExecuteNonQueryAsync();
                        command.Dispose();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

        }




        //public void DeleteDataFromTables()
        //{

        //    //удалять ключевые слова и рубрики они не очищаются
        //    SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //    SQ.Open();
        //    string sql = @"Delete From ActsForClassification";
        //    SQLiteCommand command = new SQLiteCommand(sql, SQ);
        //    command.ExecuteNonQuery();
        //    command.Dispose();
        //    SQ.Close();
        //}


        //public void UpdateIsReady(int id)
        //{
        //    SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //    SQ.Open();
        //    string sql = @"update ActsForClassification SET IsReady = 'true' where ActsForClassification.id='" + id + "'";
        //    SQLiteCommand command = new SQLiteCommand(sql, SQ);
        //    command.ExecuteNonQuery();
        //    command.Dispose();
        //    SQ.Close();
        //}

        //public void SetKeywords(int id, List<string> list)
        //{
        //    SQLiteConnection SQ = new SQLiteConnection(SQLDataSource, true);
        //    SQ.Open();

        //    int index = list.Count() - 1;
        //    if (list.Count() == 0)
        //    {
        //        string sql = @"insert or replace into ActsKeyWords (id, key1, key2, key3, key4, key5, key6) values (" + id + ", null, null, null, null, null, null) ";
        //        SQLiteCommand command = new SQLiteCommand(sql, SQ);
        //        command.ExecuteNonQuery();
        //        command.Dispose();
        //        SQ.Close();
        //    }
        //    else
        //    {
        //        string k1 = null;
        //        string k2 = null;
        //        string k3 = null;
        //        string k4 = null;
        //        string k5 = null;
        //        string k6 = null;
        //        switch (index)
        //        {
        //            case 0:
        //                {
        //                    k1 = list[0];
        //                    break;
        //                }
        //            case 1:
        //                {
        //                    k1 = list[0];
        //                    k2 = list[1];
        //                    break;
        //                }
        //            case 2:
        //                {
        //                    k1 = list[0];
        //                    k2 = list[1];
        //                    k3 = list[2];
        //                    break;
        //                }
        //            case 3:
        //                {
        //                    k1 = list[0];
        //                    k2 = list[1];
        //                    k3 = list[2];
        //                    k4 = list[3];
        //                    break;
        //                }
        //            case 4:
        //                {
        //                    k1 = list[0];
        //                    k2 = list[1];
        //                    k3 = list[2];
        //                    k4 = list[3];
        //                    k5 = list[4];
        //                    break;
        //                }
        //            case 5:
        //                {
        //                    k1 = list[0];
        //                    k2 = list[1];
        //                    k3 = list[2];
        //                    k4 = list[3];
        //                    k5 = list[4];
        //                    k6 = list[5];
        //                    break;
        //                }

        //        }
        //        string sql = @"insert or replace into ActsKeyWords (id, key1, key2, key3, key4, key5, key6) values (" + id + ", '" + k1 + "', '" + k2 + "', '" + k3 + "', '" + k4 + "', '" + k5 + "', '" + k6 + "') ";
        //        SQLiteCommand command = new SQLiteCommand(sql, SQ);
        //        command.ExecuteNonQuery();
        //        command.Dispose();
        //        SQ.Close();
        //    }

        //}

        //public void SetRubrics(int id, List<string> list)
        //{
        //    SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //    SQ.Open();
        //    int index = list.Count() - 1;
        //    if (list.Count() == 0)
        //    {
        //        string sql = @"insert or replace into ActsRubrics (id, rub1, rub2, rub3) values (" + id + ", null, null, null) ";
        //        SQLiteCommand command = new SQLiteCommand(sql, SQ);
        //        command.ExecuteNonQuery();
        //        command.Dispose();
        //        SQ.Close();
        //    }
        //    else
        //    {
        //        string rub1 = null;
        //        string rub2 = null;
        //        string rub3 = null;
        //        switch (index)
        //        {
        //            case 0:
        //                {
        //                    rub1 = list[0];
        //                    break;
        //                }
        //            case 1:
        //                {
        //                    rub1 = list[0];
        //                    rub2 = list[1];
        //                    break;
        //                }
        //            case 2:
        //                {
        //                    rub1 = list[0];
        //                    rub2 = list[1];
        //                    rub3 = list[2];
        //                    break;
        //                }
        //        }
        //        string sql = @"insert or replace into ActsRubrics (id, rub1, rub2, rub3) values (" + id + ", '" + rub1 + "', '" + rub2 + "', '" + rub3 + "') ";
        //        SQLiteCommand command = new SQLiteCommand(sql, SQ);
        //        command.ExecuteNonQuery();
        //        command.Dispose();
        //        SQ.Close();
        //    }

        //}

        //public List<DirectoryInfo> GetSettings
        //{
        //    get
        //    {
        //        SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //        SQ.Open();
        //        List<DirectoryInfo> list = new List<DirectoryInfo>();
        //        SQLiteCommand command = new SQLiteCommand(SQ);
        //        command.CommandText = @"SELECT Settings.LoadPath, Settings.ExportPath, Settings.BackUp FROM Settings";
        //        command.CommandType = CommandType.Text;
        //        SQLiteDataReader reader = command.ExecuteReader();
        //        if (reader.HasRows)
        //        {
        //            reader.Read();
        //            list.Add(new DirectoryInfo(reader.GetString(0)));
        //            list.Add(new DirectoryInfo(reader.GetString(1)));
        //            list.Add(new DirectoryInfo(reader.GetString(2)));
        //        }
        //        command.Dispose();
        //        SQ.Close();
        //        return list;
        //    }
        //    set { }
        //}


        public async Task<string> GetActsCount()
        {
            string count = "ОШИБКА!";
            try
            {
                using (SqlConnection conn = getSqlConnection)
                {

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                    }
                    catch (SqlException se)
                    {
                        Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                    }
                    SqlCommand cmd = new SqlCommand(@"SELECT count(*) FROM ActsForClassification WHERE IsReady = '1'");
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (await dr.ReadAsync())
                        {
                            if (dr.HasRows)
                            {
                                count = dr.GetInt32(0).ToString();
                            }
                        }
                    }

                }
                return count;
            }

            catch (Exception ex)
            {
                Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return null;
            }
        }

        //public string GetActsCount
        //{
        //    get
        //    {
        //        SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //        SQ.Open();
        //        int i = 0;
        //        SQLiteCommand command = new SQLiteCommand(SQ);
        //        command.CommandText = @"SELECT count(*) FROM ActsForClassification WHERE ActsForClassification.IsReady = 'true'";
        //        command.CommandType = CommandType.Text;
        //        SQLiteDataReader reader = command.ExecuteReader();
        //        if (reader.HasRows)
        //        {
        //            reader.Read();
        //            i = reader.GetInt16(0);
        //        }
        //        command.Dispose();
        //        SQ.Close();
        //        return "Документов выгружено: " + i;
        //    }
        //    set { }
        //}



        public async Task<ObservableCollection<ReadyListBoxClass>> ReadFromTable(bool IsReady = false)
        {
            ObservableCollection<ReadyListBoxClass> coll = new ObservableCollection<ReadyListBoxClass>();
            try
            {
                using (SqlConnection conn = getSqlConnection)
                {
                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                    }
                    catch (SqlException se)
                    {
                        Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                    }
                    SqlCommand cmd = new SqlCommand(@"SELECT id, ActSignData, ActNumber, ActName, IsReady  FROM ActsForClassification WHERE ActsForClassification.IsReady = @ready", conn);
                    if (IsReady == false)
                    {
                        cmd = new SqlCommand(@"SELECT id, ActSignData, ActNumber, ActName, IsReady  FROM ActsForClassification WHERE ActsForClassification.IsReady IS NULL", conn);
                    }
                    else
                    {
                        cmd.Parameters.Add("@ready", SqlDbType.Bit, 1, "IsReady").Value = 1;
                    }
                    
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            if (dr.HasRows)
                            {
                                ReadyListBoxClass rc = new ReadyListBoxClass();
                                rc.IndexNum = dr.GetInt32(0);
                                rc.SignDate = dr.GetDateTime(1).ToString("dd.MM.yyyy");
                                rc.Number = dr.GetString(2);
                                rc.ActName = dr.GetString(3);
                                rc.Check = dr.IsDBNull(4) ? false : true;
                                rc.Keywordd = await GetKeys(dr.GetInt32(0));    
                                rc.Rubriki = await GetRubrics(dr.GetInt32(0));  
                                coll.Add(rc);
                            }
                        }
                    }

                }
                return coll;
            }

            catch (Exception ex)
            {
                Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return null;
            }
        }

        //public ObservableCollection<ReadyListBoxClass> ReadFromTable
        //{
        //    get
        //    {
        //        SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //        SQ.Open();
        //        ObservableCollection<ReadyListBoxClass> coll = new ObservableCollection<ReadyListBoxClass>();
        //        SQLiteCommand command = new SQLiteCommand(SQ);
        //        command.CommandText = @"SELECT id, ActSignData, ActNumber, ActName, IsReady  FROM ActsForClassification WHERE ActsForClassification.IsReady = 'false'";
        //        command.CommandType = CommandType.Text;
        //        SQLiteDataReader reader = command.ExecuteReader();
        //        if (reader.HasRows)
        //        {
        //            while (reader.Read())
        //            {
        //                coll.Add(new ReadyListBoxClass(reader.GetInt16(0), reader.GetDateTime(1).ToString("dd.MM.yyyy"), reader.GetString(2), reader.GetString(3), bool.Parse(reader.GetString(4)), GetRubrics(reader.GetInt16(0)), GetKeys(reader.GetInt16(0))));
        //            }
        //        }
        //        command.Dispose();
        //        SQ.Close();
        //        return coll;
        //    }
        //    set { }
        //}

        public async void ApplyngKeysToBaseKeyword(string key, string MainKey)
        {
            try
            {
                using (SqlConnection conn = getSqlConnection)
                {
                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                    }
                    catch (SqlException se)
                    {
                        Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                    }
                    SqlCommand cmd = new SqlCommand(@"SELECT key1, key2, key3, key4, key5 FROM KeysHighLight WHERE MainKey = @mainkey");
                    cmd.Parameters.Add("@mainkey", SqlDbType.NVarChar, 50, "MainKey").Value = MainKey;
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (await dr.ReadAsync())
                        {
                            if (dr.HasRows)
                            {
                                using (SqlConnection conn1 = getSqlConnection)
                                {
                                    try
                                    {
                                        await conn1.OpenAsync().ConfigureAwait(false);
                                    }
                                    catch (SqlException se)
                                    {
                                        Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                                    }
                                    for (int i = 0; i <= dr.FieldCount - 1; i++)
                                    {
                                        if (dr.IsDBNull(i))
                                        {
                                            SqlCommand cmm = new SqlCommand("UPDATE KeysHighLight SET key" + (i + 1) + " = @key WHERE MainKey =  @mainkey", conn1);
                                            cmm.Parameters.Add("@key", SqlDbType.NVarChar, 50).Value = key;
                                            cmm.Parameters.Add("@mainkey", SqlDbType.NVarChar, 50, "MainKey").Value = MainKey;
                                            await cmm.ExecuteNonQueryAsync();
                                            cmm.Dispose();
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

        }

        //public void ApplyngKeysToBaseKeyword(string key, string MainKey)
        //{
        //    SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //    SQ.Open();
        //    SQLiteCommand command = new SQLiteCommand(SQ);
        //    command.CommandText = @"SELECT KeysHighLight.[key1], KeysHighLight.[key2], KeysHighLight.[key3], KeysHighLight.[key4] FROM KeysHighLight WHERE KeysHighLight.MainKey = '" + MainKey + "'";
        //    command.CommandType = CommandType.Text;
        //    SQLiteDataReader reader = command.ExecuteReader();
        //    if (reader.HasRows)
        //    {
        //        reader.Read();
        //        for (int i = 0; i <= reader.FieldCount - 1; i++)
        //        {
        //            bool bb = reader.IsDBNull(i);
        //            if (reader.IsDBNull(i))
        //            {
        //                string sql = @"UPDATE KeysHighLight SET key" + (i + 1) + " = '" + key + "' WHERE KeysHighLight.MainKey = '" + MainKey + "'";
        //                command = new SQLiteCommand(sql, SQ);
        //                command.ExecuteNonQuery();
        //                command.Dispose();
        //                SQ.Close();
        //                break;
        //            }
        //        }
        //    }
        //    SQ.Close();
        //}



        public async Task<string> GetActText(int id)
        {
            string acttext = "ДОКУМЕНТ НЕ НАЙДЕН!";
            try
            {
                using (SqlConnection conn = getSqlConnection)
                {

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                    }
                    catch (SqlException se)
                    {
                        Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                    }
                    SqlCommand cmd = new SqlCommand(@"Select ActSignData, ActNumber FROM ActsForClassification WHERE Id = @id");
                    cmd.Parameters.Add("@id", SqlDbType.Int, 10, "Id").Value = id;
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (await dr.ReadAsync())
                        {
                            if (dr.HasRows)
                            {
                                acttext = IPSMySqlConnection.getActText(dr.GetDateTime(0), dr.GetString(1));
                            }
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return ex.Message;
            }
            return acttext;

        }

        //public string GetActText(int id)
        //{
        //    SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //    SQ.Open();
        //    string txt = null;
        //    SQLiteCommand command = new SQLiteCommand(SQ);
        //    command.CommandText = @"SELECT ActsForClassification.ActText FROM ActsForClassification WHERE  ActsForClassification.id = " + id + "";
        //    command.CommandType = CommandType.Text;
        //    SQLiteDataReader reader = command.ExecuteReader();
        //    if (reader.HasRows)
        //    {
        //        reader.Read();
        //        if (!reader.IsDBNull(0))
        //        {
        //            txt = reader.GetString(0);
        //        }
        //    }
        //    command.Dispose();
        //    SQ.Close();
        //    return txt;
        //}

        public async void RefreshTable(ObservableCollection<ReadyListBoxClass> coll)
        {
            await Task.Factory.StartNew(() =>
            {
                foreach (ReadyListBoxClass cl in coll)
                {
                    cl.Keywordd = GetKeys(cl.IndexNum).Result;
                    cl.Rubriki = GetRubrics(cl.IndexNum).Result;
                }
            });
        }


        public async Task<List<string>> GetRubrics(int id)
        {
            List<string> result = new List<string>();
            try
            {
                using (SqlConnection conn = getSqlConnection)
                {

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                    }
                    catch (SqlException se)
                    {
                        Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                    }
                    SqlCommand cmd = new SqlCommand(@"Select rub1, rub2, rub3 FROM ActsRubrics WHERE Id = @id");
                    cmd.Parameters.Add("@id", SqlDbType.Int, 10, "Id").Value = id;
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (await dr.ReadAsync())
                        {
                            if (dr.HasRows)
                            {
                                for (int i = 0; i <= dr.FieldCount - 1; i++)
                                {
                                    if (!dr.IsDBNull(i))
                                    {
                                        result.Add(dr.GetString(i));
                                    }
                                }

                            }

                        }
                    }

                }
            }

            catch (Exception ex)
            {
                Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return result;
            }
            return result;

        }

        //private List<string> GetRubrics(int id)
        //{
        //    SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //    SQ.Open();
        //    List<string> result = new List<string>();
        //    SQLiteCommand command = new SQLiteCommand(SQ);
        //    command.CommandText = @"SELECT * FROM ActsRubrics WHERE ActsRubrics.id = '" + id + "'";
        //    command.CommandType = CommandType.Text;
        //    SQLiteDataReader reader = command.ExecuteReader();
        //    if (reader.HasRows)
        //    {
        //        reader.Read();
        //        for (int i = 1; i <= reader.GetValues().Count - 1; i++)
        //        {
        //            if (!reader.IsDBNull(i))
        //            {
        //                result.Add(reader.GetValue(i).ToString());
        //            }

        //        }
        //        result.Sort();
        //    }
        //    command.Dispose();
        //    SQ.Close();
        //    return result;
        //}

        public async Task<List<string>> GetKeys(int id)
        {
            List<string> result = new List<string>();
            try
            {
                using (SqlConnection conn = getSqlConnection)
                {

                    try
                    {
                        await conn.OpenAsync().ConfigureAwait(false);
                    }
                    catch (SqlException se)
                    {
                        Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, se.Message);
                    }
                    SqlCommand cmd = new SqlCommand(@"Select key1, key2, key3, key4, key5, key6 FROM ActsKeyWords WHERE Id = @id");
                    cmd.Parameters.Add("@id", SqlDbType.Int, 10, "Id").Value = id;
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (await dr.ReadAsync())
                        {
                            if (dr.HasRows)
                            {
                                for (int i = 0; i <= dr.FieldCount - 1; i++)
                                {
                                    if (!dr.IsDBNull(i))
                                    {
                                        result.Add(dr.GetString(i));
                                    }
                                }

                            }

                        }
                    }

                }
            }

            catch (Exception ex)
            {
                Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return result;
            }
            return result;

        }

        //private List<string> GetKeys(int id)
        //{
        //    SQLiteConnection SQ = new SQLiteConnection(SQLDataSource);
        //    SQ.Open();
        //    List<string> result = new List<string>();
        //    SQLiteCommand command = new SQLiteCommand(SQ);
        //    command.CommandText = @"SELECT * FROM ActsKeyWords WHERE ActsKeyWords.id = '" + id + "'";
        //    command.CommandType = CommandType.Text;
        //    SQLiteDataReader reader = command.ExecuteReader();
        //    if (reader.HasRows)
        //    {
        //        reader.Read();
        //        for (int i = 1; i <= reader.GetValues().Count - 1; i++)
        //            if (!reader.IsDBNull(i))
        //            {
        //                result.Add(reader.GetValue(i).ToString());
        //            }
        //        result.Sort();
        //    }
        //    command.Dispose();
        //    SQ.Close();
        //    return result;
        //}

        public static void FindDouble(ObservableCollection<ReadyListBoxClass> coll, string actname, int id)
        {
            int d = 0;
            SQLMethods sq = new SQLMethods();
            string pattern = @"^О\s[А-Я]{1}[а-я]{4,20}\s[А-Я]{1}.[А-Я]{1}.";
            string patternOB = @"^Об\s[А-Я]{1}[а-я]{4,20}\s[А-Я]{1}.[А-Я]{1}.";
            Regex newReg = new Regex(pattern);
            string text = actname;
            MatchCollection matches = newReg.Matches(text);

            if (matches.Count == 0)
            {
                newReg = new Regex(patternOB);
                text = actname;
                matches = newReg.Matches(text);
            }
            if (matches.Count > 0)
            {
                for (int i = 0; i < coll.Count; i++)
                {
                    text = coll[i].ActName;
                    matches = newReg.Matches(text);
                    if (matches.Count > 0)
                    {
                        List<string> list = coll.Where(x => x.IndexNum == id).FirstOrDefault().Rubriki;
                        coll[i].Rubriki = list;
                        sq.SetRubrics(coll[i].IndexNum, list);
                        d++;
                    }

                }
            }

            else
            {
                for (int i = 0; i < coll.Count; i++)
                {
                    if (coll[i].ActName.Contains(actname))
                    {
                        List<string> list = coll.Where(x => x.IndexNum == id).FirstOrDefault().Rubriki;
                        coll[i].Rubriki = list;
                        sq.SetRubrics(coll[i].IndexNum, list);
                        d++;
                    }
                }
            }
            MessageBox.Show("Установлены рубрики для " + d.ToString() + " документов");
        }
    }
}
