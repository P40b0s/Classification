using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Data.SqlClient;
using System.Data;
using Classification.Core.Models;
using Classification.Core;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Threading;
using Prism.Events;
using System.Windows.Threading;

namespace Classification.DB
{
    public class DbReader
    {
        IEventAggregator eventAggregator;
        public DbReader(IEventAggregator _ea)
        {
            eventAggregator = _ea;
            KeyWords = new List<TextInlineSelection>();
            Rubrics = new List<TextInlineSelection>();
            keysDictionary = new Dictionary<Guid, KeysAndRubricsModel>();
            rubricsDictionary = new Dictionary<Guid, string>();
            synonimsDictionary = new Dictionary<string, Guid>();
        }
        /// <summary>
        /// Коллекция ключевых слов
        /// </summary>
        public static List<TextInlineSelection> KeyWords { get; set; }
        /// <summary>
        /// Коллекция рубрик
        /// </summary>
        private List<TextInlineSelection> Rubrics { get; set; }
        /// <summary>
        /// Словарь ключевых слов
        /// </summary>
        public static Dictionary<Guid, KeysAndRubricsModel> keysDictionary { get; set; }
        /// <summary>
        /// Словарь рубрик
        /// </summary>
        public static Dictionary<Guid, string> rubricsDictionary { get; set; }
        /// <summary>
        /// Словарь слов-синонимов
        /// </summary>
        public static Dictionary<string, Guid> synonimsDictionary { get; set; }
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        Dispatcher d = Dispatcher.CurrentDispatcher;


        #region Заполнение коллекций

        public async void FillActsCollection(ObservableCollection<ClassificationModel> classificationCollection, bool clear = false)
        {
            try
            {
                if (clear)
                {
                    if (classificationCollection != null && classificationCollection.Count > 0)
                        await d.BeginInvoke(new Action(() => { classificationCollection.Clear(); }));

                }
                else
                {
                    await GetKeywords();
                    await GetRubrics();
                    GetKeywordsSynonyms();
                }

                using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(
@"SELECT [Id]
      ,[ActName]
      ,[ActNumber]
      ,[SignDate]
      ,[ChangeTime]
      ,[IsReadyToExport]
 FROM [MRClassification].[dbo].[ClassifiedDocuments]
 WHERE ExcelExportTime IS NULL
 ORDER BY SignDate");
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        int count = 1;
                        while (await dr.ReadAsync())
                        {
                            if (dr.HasRows)
                            {
                                var cl = new ClassificationModel(
                                        dr.GetGuid(0),
                                        dr.GetString(1),
                                        dr.GetString(2),
                                        dr.GetDateTime(3),
                                        dr.IsDBNull(4) ? (DateTime?)null : dr.GetDateTime(4),
                                        dr.IsDBNull(5) ? false : dr.GetBoolean(5)
                                        );

                                var rubrics = await GetRubricsForDocumentAsync(cl.Id);
                                foreach (var item in rubrics)
                                {
                                    cl.Rubrics.AddSorted(item);
                                }
                                var keywords = await GetKeysForDocumentAsync(cl.Id);

                                foreach (var item in keywords)
                                {
                                    cl.Keys.AddSorted(item);
                                }

                                cl.ItemNumber = count;
                                if (clear)
                                    d.Invoke(new Action(() => { classificationCollection.Add(cl); }));
                                else
                                    classificationCollection.Add(cl);
                                count++;
                            }
                        }
                    }
                    eventAggregator.GetEvent<AllActsIsLoadedEvent>().Publish();
                }
            }
            catch (SqlException sqex)
            {
                logger.Fatal(sqex);
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        /// <summary>
        /// Получение коллекции обработанных документов для выгрузки в файл EXCEL
        /// </summary>
        /// <returns></returns>
        public async Task<List<ClassificationModel>> GetActsCollectionForUpload()
        {
            List<ClassificationModel> classificationCollection = new List<ClassificationModel>();
            try
            {
                return await Task<List<ClassificationModel>>.Factory.StartNew(() =>
                {


                    using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(
    @"SELECT [Id]
      ,[ActName]
      ,[ActNumber]
      ,[SignDate]
      ,[ChangeTime]
      ,[IsReadyToExport]
 FROM [MRClassification].[dbo].[ClassifiedDocuments]
 WHERE ExcelExportTime IS NULL
 ORDER BY SignDate");
                        cmd.Connection = conn;
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            int count = 1;
                            while (dr.Read())
                            {
                                if (dr.HasRows)
                                {
                                    var cl = new ClassificationModel(
                                            dr.GetGuid(0),
                                            dr.GetString(1),
                                            dr.GetString(2),
                                            dr.GetDateTime(3),
                                            dr.IsDBNull(4) ? (DateTime?)null : dr.GetDateTime(4),
                                            dr.IsDBNull(5) ? false : dr.GetBoolean(5)
                                            );
                                    var rubrics = GetRubricsForDocument(cl.Id);
                                    foreach (var item in rubrics)
                                    {
                                        cl.Rubrics.Add(item);
                                    }
                                    var keywords = GetKeysForDocument(cl.Id);

                                    foreach (var item in keywords)
                                    {
                                        cl.Keys.Add(item);
                                    }

                                    cl.ItemNumber = count;
                                    classificationCollection.Add(cl);
                                    count++;
                                }
                            }
                        }
                    }
                    return classificationCollection;
                });
            }
            catch (SqlException sqex)
            {
                logger.Fatal(sqex);
                return classificationCollection;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                return classificationCollection;
            }
        }
        /// <summary>
        /// Заполнение словаря ключевыми словами сделанными пользователями
        /// </summary>
        /// <returns></returns>
        public async Task<List<KeysAndRubricsModel>> GetKeysForDocumentAsync(Guid docId)
        {
            List<KeysAndRubricsModel> collection = new List<KeysAndRubricsModel>();
            return await Task<List<KeysAndRubricsModel>>.Factory.StartNew(() =>
            {
                Guid keyId = Guid.Empty;
                try
                {
                    using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(@"SELECT KeyId FROM KeysValues WHERE DocumentId = @DocumentId");
                        cmd.Parameters.Add("@DocumentId", SqlDbType.UniqueIdentifier).Value = docId;
                        cmd.Connection = conn;
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dr.Read())
                            {
                                if (dr.HasRows)
                                {
                                    keyId = dr.GetGuid(0);
                                    collection.Add(new KeysAndRubricsModel() { ItemId = keyId, Item = keysDictionary[keyId].Item });
                                }
                            }
                        }
                    }
                    return collection;
                }
                catch (SqlException sqex)
                {
                    logger.Fatal(sqex);
                    logger.Fatal(keyId);
                    return collection;
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex);
                    logger.Fatal(keyId);
                    return collection;
                }
            });
        }
        public List<KeysAndRubricsModel> GetKeysForDocument(Guid docId)
        {
            List<KeysAndRubricsModel> collection = new List<KeysAndRubricsModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT KeyId FROM KeysValues WHERE DocumentId = @DocumentId");
                    cmd.Parameters.Add("@DocumentId", SqlDbType.UniqueIdentifier).Value = docId;
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            if (dr.HasRows)
                            {
                                Guid keyId = dr.GetGuid(0);
                                collection.Add(new KeysAndRubricsModel() { ItemId = keyId, Item = keysDictionary[keyId].Item });
                            }
                        }
                    }
                }
                return collection;
            }
            catch (SqlException sqex)
            {
                logger.Fatal(sqex);
                return collection;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                return collection;
            }
        }
        /// <summary>
        /// Заполнение словаря рубриками сделанными пользователями
        /// </summary>
        /// <returns></returns>
        public async Task<List<KeysAndRubricsModel>> GetRubricsForDocumentAsync(Guid docId)
        {
            List<KeysAndRubricsModel> collection = new List<KeysAndRubricsModel>();
            return await Task<List<KeysAndRubricsModel>>.Factory.StartNew(() =>
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(@"SELECT RubricId FROM RubricsValues WHERE DocumentId = @DocumentId");
                        cmd.Parameters.Add("@DocumentId", SqlDbType.UniqueIdentifier).Value = docId;
                        cmd.Connection = conn;
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dr.Read())
                            {
                                if (dr.HasRows)
                                {
                                    Guid rubricId = dr.GetGuid(0);
                                    collection.Add(new KeysAndRubricsModel() { ItemId = rubricId, Item = rubricsDictionary[rubricId] });
                                }
                            }
                        }
                    }
                    return collection;
                }
                catch (SqlException sqex)
                {
                    logger.Fatal(sqex);
                    return collection;
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex);
                    return collection;
                }
            });
        }
        public List<KeysAndRubricsModel> GetRubricsForDocument(Guid docId)
        {
            List<KeysAndRubricsModel> collection = new List<KeysAndRubricsModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT RubricId FROM RubricsValues WHERE DocumentId = @DocumentId");
                    cmd.Parameters.Add("@DocumentId", SqlDbType.UniqueIdentifier).Value = docId;
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            if (dr.HasRows)
                            {
                                Guid rubricId = dr.GetGuid(0);
                                collection.Add(new KeysAndRubricsModel() { ItemId = rubricId, Item = rubricsDictionary[rubricId] });
                            }
                        }
                    }
                }
                return collection;
            }
            catch (SqlException sqex)
            {
                logger.Fatal(sqex);
                return collection;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                return collection;
            }
        }


        /// <summary>
        /// Получение списка основных ключевых слов
        /// </summary>
        /// <param name="items"></param>
        public async Task<bool> GetKeywords()
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
                                int weight = dr.IsDBNull(2) ? 1 : dr.GetInt32(2);
                                if (KeyWords != null)
                                    KeyWords.Add(new TextInlineSelection(dr.GetGuid(0), dr.GetString(1), string.Empty));
                                if (!keysDictionary.ContainsKey(dr.GetGuid(0)))
                                    keysDictionary.Add(dr.GetGuid(0), new KeysAndRubricsModel() { Item = dr.GetString(1), ItemId = dr.GetGuid(0), Weight = weight });
                            }
                        }
                    }

                }
                eventAggregator.GetEvent<KeywordsIsLoaded>().Publish(KeyWords);

                return true;
            }
            catch (SqlException sqex)
            {
                logger.Fatal(sqex);
                return false;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                return false;
            }
        }
        /// <summary>
        /// Получение списка слов-синонимов для основных ключевых слов
        /// </summary>
        /// <param name="items"></param>
        public async void GetKeywordsSynonyms()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(@"SELECT [Synonym],[Id]
                                                            FROM [MRClassification].[dbo].[KeysSynonyms]");
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (await dr.ReadAsync())
                        {
                            if (dr.HasRows)
                            {
                                synonimsDictionary.Add(dr.GetString(0), dr.GetGuid(1));
                            }
                        }
                    }

                }
            }
            catch (SqlException sqex)
            {
                logger.Fatal(sqex);
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        /// <summary>
        /// Получение списка рубрик для классификации
        /// </summary>
        /// <param name="items"></param>
        public async Task<bool> GetRubrics()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(@"SELECT [Id]
                                                                ,[Rubric]
                                                            FROM [MRClassification].[dbo].[Rubrics] ORDER BY Rubric");
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (await dr.ReadAsync())
                        {
                            if (dr.HasRows)
                            {
                                if (Rubrics != null)
                                    Rubrics.Add(new TextInlineSelection(dr.GetGuid(0), dr.GetString(1), string.Empty));
                                if (!rubricsDictionary.ContainsKey(dr.GetGuid(0)))
                                    rubricsDictionary.Add(dr.GetGuid(0), dr.GetString(1));
                            }
                        }
                    }
                    eventAggregator.GetEvent<RubricsIsLoaded>().Publish(Rubrics);
                    return true;
                }
            }
            catch (SqlException sqex)
            {
                logger.Fatal(sqex);
                return false;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                return false;
            }
        }
        #endregion

        public async Task<string> GetAutoClassificationModel()
        {
            string result = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(@"SELECT JSON
                                                            FROM [MRClassification].[dbo].[AutoClassification]");
                    cmd.Connection = conn;
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (await dr.ReadAsync())
                        {
                            if (dr.HasRows)
                            {
                                result = dr.GetString(0);
                            }
                        }
                    }

                }
                return result;
            }
            catch (SqlException sqex)
            {
                logger.Fatal(sqex);
                return result;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                return result;
            }
        }

        #region Количество документов
        public async Task<int> ExportToExcelDocumentsCount()
        {
            return await Task<int>.Factory.StartNew(() =>
            {
                int count = 0;
                try
                {
                    using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(@"SELECT COUNT(ExcelExportTime)
                                                            FROM [MRClassification].[dbo].[ClassifiedDocuments]
                                                            WHERE YEAR(ExcelExportTime) = YEAR(GETDATE())");
                        cmd.Connection = conn;
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dr.Read())
                            {
                                if (dr.HasRows)
                                {
                                    count = dr.GetInt32(0);
                                }
                            }
                        }
                    }
                    return count;
                }
                catch (SqlException sqex)
                {
                    logger.Fatal(sqex);
                    return count;
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex);
                    return count;
                }
            });


        }

        #endregion

        #region Обновление ключевых слов и рубрик документов из базы
        public async Task<bool> RefreshItems(ObservableCollection<ClassificationModel> col)
        {
            return await Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(Connection.ReserveServerConnectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(@"SELECT [Id]
                                                                ,[ChangeTime]
                                                                ,[IsReadyToExport]
                                                                 FROM [MRClassification].[dbo].[ClassifiedDocuments]
                                                                 WHERE ExcelExportTime IS NULL AND ChangeTime > (SELECT CONVERT(NVARCHAR, GETDATE(), 105))");
                        cmd.Connection = conn;
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dr.Read())
                            {
                                if (dr.HasRows)
                                {
                                    Guid g = dr.GetGuid(0);
                                    DateTime? ct = dr.IsDBNull(1) ? (DateTime?)null : dr.GetDateTime(1);
                                    bool ready = dr.IsDBNull(2) ? false : dr.GetBoolean(2);

                                    var item = col.Where(d => d.Id == g).FirstOrDefault();
                                    if (item != null && ct.HasValue)
                                    {
                                        if (!item.ChangeTime.HasValue || (item.ChangeTime < ct.Value))
                                        {
                                            //RefreshKeys(g, item);
                                            //RefreshRubrics(g, item);
                                            item.IsReady = ready;
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return false;
                }
                catch (SqlException sqex)
                {
                    logger.Fatal(sqex);
                    return false;
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex);
                    return false;
                }
            });
        }


        #endregion
    }
}

