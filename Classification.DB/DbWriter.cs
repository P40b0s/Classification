using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classification.Core.Interfaces;
using System.Data.SqlClient;
using Classification.Core.Models;
using NLog;
using System.Data;
using Prism.Events;
using Classification.Core;

namespace Classification.DB
{
    public class DbWriter
    {
        IEventAggregator eventAggregator;
        public DbWriter(IEventAggregator ea)
        {
            eventAggregator = ea;
            eventAggregator.GetEvent<AddSynonymToBaseEvent>().Subscribe(AddSynonymToBase);
        }
        readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void ExportToExcelFlag(Guid Id, bool exported)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveClassificatedDocumentAsync(ClassificationModel item)
        {
            bool ready = false;
            bool keys = await AddReadyKeywordToBaseAsync(item);
            bool rubrics = await AddReadyRubricToBaseAsync(item);
            if (rubrics && keys)
                ready = await AddReadyFlag(item);
            return ready;
        }

        public async void SaveClassificatedDocument(ClassificationModel item)
        {
            bool keys = await AddReadyKeywordToBaseAsync(item);
            bool rubrics = await AddReadyRubricToBaseAsync(item);
            if (rubrics && keys)
                await AddReadyFlag(item);
        }
        /// <summary>
        /// Добавление слова-синонима в базу
        /// </summary>
        /// <param name="syn"></param>
        private async void AddSynonymToBase(KeyValuePair<string, Guid> syn)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand command = new SqlCommand(@"IF NOT EXISTS(SELECT Synonym FROM KeysSynonyms WHERE Synonym = @key)
                                                              BEGIN
	                                                            INSERT INTO KeysSynonyms (Id, Synonym) VALUES (@value, @key)
                                                         	  END");
                    command.Parameters.Add("@key", SqlDbType.NVarChar).Value = syn.Key;
                    command.Parameters.Add("@value", SqlDbType.UniqueIdentifier).Value = syn.Value;

                    command.Connection = conn;
                    int rows = await command.ExecuteNonQueryAsync();
                    if (rows > 0)
                    {
                        logger.Info($"К ключевому слову {syn.Value} добавлен синоним {syn.Key}");
                    }

                }
            }
            catch (SqlException sex)
            {
                logger.Fatal(sex);
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        /// <summary>
        /// Добавление в таблизу записи с новым документом для классификации, при импорте из файла EXCEL
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool ImportExcelRecord(ClassificationModel item)
        {
            bool adding = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                {

                    conn.Open();
                    ///Импортирование самого документа
                    SqlCommand command = new SqlCommand(@"INSERT INTO ClassifiedDocuments (Id, PortionUploadTime, ActName, ActNumber, SignDate) VALUES (@Id, @UploadTime, @ActName, @ActNumber, @SignDate)");
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = item.Id;
                    command.Parameters.Add("@UploadTime", SqlDbType.DateTime).Value = item.PortionUploadTime;
                    command.Parameters.Add("@ActName", SqlDbType.NVarChar).Value = item.ActName;
                    command.Parameters.Add("@ActNumber", SqlDbType.NVarChar).Value = item.ActNumber;
                    command.Parameters.Add("@SignDate", SqlDbType.DateTime).Value = item.SignDate;
                    command.Connection = conn;
                    int rows = command.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        logger.Info($"В базу импортирован документ {item.Id} из файла EXCEL");
                        adding = true;
                    }
                    ///Импортирование ключевых слов
                    foreach (var key in item.Keys)
                    {
                        command = new SqlCommand(@"INSERT INTO KeysValues (DocumentId, KeyId) VALUES (@DocumentId, @KeyId)");
                        command.Parameters.Add("@DocumentId", SqlDbType.UniqueIdentifier).Value = item.Id;
                        command.Parameters.Add("@KeyId", SqlDbType.UniqueIdentifier).Value = key.ItemId;
                        command.Connection = conn;
                        command.ExecuteNonQuery();
                    }


                    ///Импортирование рубрик
                    foreach (var rubric in item.Rubrics)
                    {
                        command = new SqlCommand(@"INSERT INTO RubricsValues (DocumentId, RubricId) VALUES (@DocumentId, @RubricId)");
                        command.Parameters.Add("@DocumentId", SqlDbType.UniqueIdentifier).Value = item.Id;
                        command.Parameters.Add("@RubricId", SqlDbType.UniqueIdentifier).Value = rubric.ItemId;
                        command.Connection = conn;
                        command.ExecuteNonQuery();
                    }
                }
                return adding;
            }
            catch (SqlException sex)
            {
                logger.Fatal(sex);
                return adding;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return adding;
            }
        }

        /// <summary>
        /// Добавление к документу даты экспорта в EXCEL при успешной выгрузке в EXCEL
        /// </summary>
        /// <param name="item"></param>
        public async void AddExportedFlag(ClassificationModel item)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand command = new SqlCommand(@"UPDATE ClassifiedDocuments SET ExcelExportTime = @export
		                                                         FROM ClassifiedDocuments as c
		                                                         WHERE Id = @id");
                    command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = item.Id;
                    command.Parameters.Add("@export", SqlDbType.DateTime).Value = item.ExcelExportTime;
                    command.Connection = conn;
                    int rows = await command.ExecuteNonQueryAsync();
                }
            }
            catch (SqlException sex)
            {
                logger.Fatal(sex);
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        /// <summary>
        /// Добавление к документу флага, что он готов к выгрузке
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task<bool> AddReadyFlag(ClassificationModel item)
        {
            try
            {
                DateTime ChangeTime = DateTime.Now;
                bool ready = item.Keys.Count >= 3 && item.Rubrics.Count >= 1;
                using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand command = new SqlCommand(@"UPDATE ClassifiedDocuments SET IsReadyToExport = @ready, ChangeTime = @ChangeTime
		                                                         FROM ClassifiedDocuments as c
		                                                         WHERE Id = @id");
                    command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = item.Id;
                    command.Parameters.Add("@ChangeTime", SqlDbType.DateTime).Value = ChangeTime;
                    command.Parameters.Add("@ready", SqlDbType.Bit).Value = ready;

                    command.Connection = conn;
                    int rows = await command.ExecuteNonQueryAsync();
                    if (rows > 0)
                    {
                        item.IsReady = ready;
                        item.ChangeTime = ChangeTime;
                        logger.Info($"Документу {item} выставлен флаг IsReadyToExport = {ready}, Строк обработано {rows}");
                    }

                }
                return true;
            }
            catch (SqlException sex)
            {
                logger.Fatal(sex);
                return false;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return false;
            }
        }

        #region Добавление в базу ключевых слов и рубрик
    
        public async Task<bool> AddReadyKeywordToBaseAsync(ClassificationModel item)
        {
            bool ready = false;
            try
            {

                using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand command = new SqlCommand(@"DELETE FROM KeysValues
		                                                      WHERE DocumentId = @id");
                    command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = item.Id;
                    command.Connection = conn;
                    int deleterows = await command.ExecuteNonQueryAsync();
                    logger.Info($"Удалено {deleterows} ключевых слов из документа {item} на замену");
                }
                var collection = item.Keys.OrderBy(o => o.Item);
                foreach (var key in collection)
                {
                    using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                    {
                        await conn.OpenAsync();
                        SqlCommand command = new SqlCommand(@"INSERT INTO KeysValues
                                                                 (DocumentId, KeyId)
                                                                 VALUES (@id, @KeyId)");
                        command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = item.Id;
                        command.Parameters.Add("@KeyId", SqlDbType.UniqueIdentifier).Value = key.ItemId;
                        command.Connection = conn;
                        int rows = await command.ExecuteNonQueryAsync();
                        logger.Info($"К документу {item} добавлено ключевое слово: {key.Item}");
                    }
                }
                ready = true;
                return ready;
            }
            catch (SqlException sex)
            {
                logger.Fatal(sex);
                logger.Info(item.Keys);
                return ready;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return ready;
            }
        }

        public bool AddReadyKeywordToBase(ClassificationModel item)
        {
            bool ready = false;
            try
            {

                using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(@"DELETE FROM KeysValues
		                                                      WHERE DocumentId = @id");
                    command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = item.Id;
                    command.Connection = conn;
                    int deleterows = command.ExecuteNonQuery();
                    logger.Info($"Удалено {deleterows} ключевых слов из документа {item} на замену");
                }
                var collection = item.Keys.OrderBy(o => o.Item);
                foreach (var key in collection)
                {
                    using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand(@"INSERT INTO KeysValues
                                                                 (DocumentId, KeyId)
                                                                 VALUES (@id, @KeyId)");
                        command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = item.Id;
                        command.Parameters.Add("@KeyId", SqlDbType.UniqueIdentifier).Value = key.ItemId;
                        command.Connection = conn;
                        int rows = command.ExecuteNonQuery();
                        logger.Info($"К документу {item} добавлено ключевое слово: {key.Item}");
                    }
                }
                ready = true;
                return ready;
            }
            catch (SqlException sex)
            {
                logger.Fatal(sex);
                logger.Info(item.Keys);
                return ready;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return ready;
            }
        }

        public async Task<bool> AddReadyRubricToBaseAsync(ClassificationModel item)
        {
            bool ready = false;
            try
            {

                using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand command = new SqlCommand(@"DELETE FROM RubricsValues
		                                                      WHERE DocumentId = @id");
                    command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = item.Id;
                    command.Connection = conn;
                    int deleterows = await command.ExecuteNonQueryAsync();
                    logger.Info($"Удалено {deleterows} рубрик из документа {item} на замену");
                }
                var collection = item.Rubrics.OrderBy(o => o.Item);
                foreach (var rubric in collection)
                {
                    using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                    {
                        await conn.OpenAsync();
                        SqlCommand command = new SqlCommand(@"INSERT INTO RubricsValues
                                                                 (DocumentId, RubricId)
                                                                 VALUES (@id, @RubricId)");
                        command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = item.Id;
                        command.Parameters.Add("@RubricId", SqlDbType.UniqueIdentifier).Value = rubric.ItemId;
                        command.Connection = conn;
                        int rows = await command.ExecuteNonQueryAsync();
                        logger.Info($"К документу {item} добавлена рубрика: {rubric.Item}");
                    }

                }
                ready = true;
                return ready;
            }
            catch (SqlException sex)
            {
                logger.Fatal(sex);
                return ready;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return ready;
            }
        }

        public bool AddReadyRubricToBase(ClassificationModel item)
        {
            bool ready = false;
            try
            {

                using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(@"DELETE FROM RubricsValues
		                                                      WHERE DocumentId = @id");
                    command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = item.Id;
                    command.Connection = conn;
                    int deleterows = command.ExecuteNonQuery();
                    logger.Info($"Удалено {deleterows} рубрик из документа {item} на замену");
                }
                var collection = item.Rubrics.OrderBy(o => o.Item);
                foreach (var rubric in collection)
                {
                    using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand(@"INSERT INTO RubricsValues
                                                                 (DocumentId, RubricId)
                                                                 VALUES (@id, @RubricId)");
                        command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = item.Id;
                        command.Parameters.Add("@RubricId", SqlDbType.UniqueIdentifier).Value = rubric.ItemId;
                        command.Connection = conn;
                        int rows = command.ExecuteNonQuery();
                        logger.Info($"К документу {item} добавлена рубрика: {rubric.Item}");
                    }

                }
                ready = true;
                return ready;
            }
            catch (SqlException sex)
            {
                logger.Fatal(sex);
                return ready;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return ready;
            }
        }
        #endregion
    }
}
