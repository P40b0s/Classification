using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KwywordsUniqueSetter
{
    public class Connection
    {
        public const string ReserveServerConnectionString = @"Data Source=" + "182.5.202.219" + @";
            Initial Catalog=MRClassification;
            Persist Security Info=True;
            User ID=xphobos;
            Password=iksar33664;
            connection timeout = 30;";
    }
    public class DB
    {
        public async Task<bool> GetKeywords(ObservableCollection<Model> collection)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(@"SELECT [Id]
                                                            ,[KeyWord]
                                                            ,[Weight]
                                                            FROM [MRClassification].[dbo].[Keys] ORDER BY KeyWord");
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (await dr.ReadAsync())
                        {
                            if (dr.HasRows)
                            {
                                collection.Add(new Model() { Id = dr.GetGuid(0), Key = dr.GetString(1), Weight = dr.GetInt32(2) });
                            }
                        }
                    }

                }
                return true;
            }
            catch (SqlException sqex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public void SaveRecords(ObservableCollection<Model> collection)
        {
            try
            {
                foreach(var item in collection)
                {
                    using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand(@"UPDATE Keys SET Weight = @weight WHERE Id = @id");
                        command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = item.Id;
                        command.Parameters.Add("@weight", SqlDbType.Int).Value = item.Weight;
                        command.Connection = conn;
                        int rows = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sex)
            {
            }
            catch (System.Exception ex)
            {
            }
        }

        public void CancelReadyDocument(string number, DateTime signDate)
        {
            try
            {
                    using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand(@"dbo.DeleteReadyFlag");
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@ActNumber", SqlDbType.NVarChar).Value = number;
                        command.Parameters.Add("@SignDate", SqlDbType.Date).Value = signDate;
                        command.Connection = conn;
                        int rows = command.ExecuteNonQuery();
                    }
            }
            catch (SqlException sex)
            {
            }
            catch (System.Exception ex)
            {
            }
        }
    }
}
