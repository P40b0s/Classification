using Classification.Core.Models;
using Excel;
using Newtonsoft.Json;
using NLog;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Classification.Modules.MainListViewModule
{
    class ExcelExporter : INotifyPropertyChanged
    {
        IEventAggregator eventAggregator;
        protected readonly DB.DbReader dbReader;
        protected readonly DB.DbWriter dbWriter;
        List<AutoClassificationModel> RubricsTriggerList { get; set; }

        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static ObservableCollection<ClassificationModel> CCollection { get; set; }
        public ExcelExporter(IEventAggregator _ea)
        {
            eventAggregator = _ea;
            dbReader = new DB.DbReader(_ea);
            dbWriter = new DB.DbWriter(_ea);
            InitializeCommands();
            RubricsTriggerList = FillRubricsTrigger();
        }


        #region INotifyPropertyChanged
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Команды

        public DelegateCommand ExportToExcelCommand { get; set; }
        public DelegateCommand ImportFromExcelCommand { get; set; }

        private void InitializeCommands()
        {
            ExportToExcelCommand = new DelegateCommand(UploadToExcel);
            ImportFromExcelCommand = new DelegateCommand(ImportFromExcel);

        }
        #endregion

        #region Импорт из файла Excel
        private async void ViewExcelFileRow()
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {

                    if (SelectedFile != null)
                    {
                        ExcelRows = new ObservableCollection<ClassificationModel>();
                        ImportActsMaximum = 0;
                        using (FileStream stream = File.Open(SelectedFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                            excelReader.IsFirstRowAsColumnNames = false;
                            while (excelReader.Read())
                            {
                                Guid id = Guid.NewGuid();
                                int excelId = excelReader.IsDBNull(0) ? 0 : excelReader.GetInt32(0);
                                if (excelId != 0)
                                {
                                    string ActName = excelReader.IsDBNull(5) ? null : excelReader.GetString(5);
                                    string ActNumber = excelReader.IsDBNull(4) ? null : excelReader.GetString(4).Replace('№', ' ').TrimStart(' ');
                                    string SignDate = excelReader.IsDBNull(3) ? null : excelReader.GetString(3);
                                    DateTime SDate;
                                    if (DateTime.TryParse(SignDate, out SDate))
                                    {
                                        ClassificationModel cm = new ClassificationModel(id, ActName, ActNumber, SDate, DateTime.Now);
                                        d.BeginInvoke(new Action(() =>
                                        {
                                            ExcelRows.Add(cm);
                                            ImportActsMaximum++;
                                        }));

                                    }
                                }

                            }
                        }

                    }
                });
            }
            catch (Excel.Exceptions.BiffRecordException ex1) { logger.Fatal(ex1); }
            catch (Excel.Exceptions.HeaderException ex2) { logger.Fatal(ex2); }
            catch (Exception ex) { logger.Fatal(ex); }
        }

        private async void ImportFromExcel()
        {
            try
            {
                await Task.Factory.StartNew(async () =>
                {
                    if (SelectedFile != null)
                    {
                        using (FileStream stream = File.Open(SelectedFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                            excelReader.IsFirstRowAsColumnNames = false;
                            while (excelReader.Read())
                            {
                                Guid id = Guid.NewGuid();
                                int excelId = excelReader.IsDBNull(0) ? 0 : excelReader.GetInt32(0);
                                if (excelId != 0)
                                {
                                    string ActName = excelReader.IsDBNull(5) ? null : excelReader.GetString(5);
                                    string ActNumber = excelReader.IsDBNull(4) ? null : excelReader.GetString(4).Replace('№', ' ').TrimStart(' ');
                                    string SignDate = excelReader.IsDBNull(3) ? null : excelReader.GetString(3);
                                    DateTime SDate;
                                    if (DateTime.TryParse(SignDate, out SDate))
                                    {
                                        ClassificationModel cm = new ClassificationModel(id, ActName, ActNumber, SDate, DateTime.Now);
                                        var model = await AutoClassificatorAsync(cm);
                                        if (dbWriter.ImportExcelRecord(cm))
                                        {
                                            ImportActsValue++;
                                        }
                                    }
                                }

                            }
                        }

                    }
                    dbReader.FillActsCollection(CCollection, true);
                });
            }
            catch (Excel.Exceptions.BiffRecordException ex1) { logger.Fatal(ex1); }
            catch (Excel.Exceptions.HeaderException ex2) { logger.Fatal(ex2); }
            catch (Exception ex) { logger.Fatal(ex); }
        }
        private void LoadImpotList()
        {
            try
            {
                ImportFilesList = new ObservableCollection<FileInfo>(Directory.EnumerateFiles("Import").Select(f => new FileInfo(f)));
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        public ObservableCollection<FileInfo> ImportFilesList
        {
            get { return _ImportFilesList; }
            set
            {
                if (ImportFilesList != value)
                {
                    _ImportFilesList = value;
                    OnPropertyChanged();
                }
            }
        }
        private ObservableCollection<FileInfo> _ImportFilesList { get; set; }

        public ObservableCollection<ClassificationModel> ExcelRows
        {
            get { return _ExcelRows; }
            set
            {
                if (ExcelRows != value)
                {
                    _ExcelRows = value;
                    OnPropertyChanged();
                }
            }
        }
        private ObservableCollection<ClassificationModel> _ExcelRows { get; set; }

        public FileInfo SelectedFile
        {
            get { return _SelectedFile; }
            set
            {
                if (SelectedFile != value)
                {
                    _SelectedFile = value;
                    OnPropertyChanged();
                    ViewExcelFileRow();
                }
            }
        }
        private FileInfo _SelectedFile { get; set; }

        public int ImportActsValue
        {
            get { return _ImportActsValue; }
            set
            {
                if (ImportActsValue != value)
                {
                    _ImportActsValue = value;
                    OnPropertyChanged();

                }
            }
        }
        private int _ImportActsValue { get; set; }

        public int ImportActsMaximum
        {
            get { return _ImportActsMaximum; }
            set
            {
                if (ImportActsMaximum != value)
                {
                    _ImportActsMaximum = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _ImportActsMaximum { get; set; }

        #endregion

        #region Экспорт в файл EXCEL
        Dispatcher d = Dispatcher.CurrentDispatcher;
        private async void UploadToExcel()
        {
            try
            {
                var uploadCollection = await dbReader.GetActsCollectionForUpload();
                int exportCount = await dbReader.ExportToExcelDocumentsCount();
                await Task.Factory.StartNew(() =>
                {
                    if (uploadCollection.Count > 0)
                    {
                        UploadToExcelMaximum = uploadCollection.Where(r => r.IsReady).Count();
                       
                        int z = 0;
                        Directory.CreateDirectory(Path.Combine("Classificated", DateTime.Now.ToString("MMMM")));
                        FileInfo fi = new FileInfo(Path.Combine("Classificated", DateTime.Now.ToString("MMMM"), "На классификацию " + DateTime.Now.ToShortDateString() + ".xlsx"));
                        ExcelPackage pac = new ExcelPackage(fi);
                        pac.Workbook.Worksheets.Add("На классификацию " + DateTime.Now.ToShortDateString());
                        ExcelWorksheet ws = pac.Workbook.Worksheets[1];
                        ws.Name = "На классификацию " + DateTime.Now.ToShortDateString();
                        ws.Cells.Style.Font.Size = 12;
                        ws.Cells.Style.Font.Name = "Calibri";
                        ws.Cells.Style.WrapText = true;
                        ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        for (int i = 0; i < uploadCollection.Count; i++)
                        {
                            var item = uploadCollection[i];
                            if (item.IsReady)
                            {
                                ws.Cells[z + 1, 1].Value = (int)exportCount;
                                ws.Cells[z + 1, 2].Value = (string)item.SignDate.ToShortDateString();
                                ws.Cells[z + 1, 3].Value = (string)item.ActNumber;
                                ws.Cells[z + 1, 4].Value = (string)item.ActName;
                                ws.Cells[z + 1, 5].Value = GetStringFromList(item.Rubrics);
                                ws.Cells[z + 1, 6].Value = GetStringFromList(item.Keys);
                                item.ExcelExportTime = DateTime.Now;
                                z++;
                                exportCount++;
                                UploadToExcelValue = z;
                                d.Invoke(new Action(() =>
                                {
                                    CCollection.Remove(item);
                                }));
                                dbWriter.AddExportedFlag(item);
                            }

                        }
                        ws.Column(1).Width = 5;
                        ws.Column(2).Width = 12;
                        ws.Column(3).Width = 11;
                        ws.Column(4).Width = 56;
                        ws.Column(5).Width = 48;
                        ws.Column(6).Width = 25;
                        ws.DefaultRowHeight = 140;
                        pac.Save();
                        getCounts();
                    }
                });
            }
            catch (Exception ex) { logger.Fatal(ex); }
        }

        string GetStringFromList(ObservableCollection<KeysAndRubricsModel> col)
        {
            List<KeysAndRubricsModel> list = col.OrderBy(o => o.Item).ToList();
            string s = null;
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (i == list.Count - 1)
                    {
                        s += list[i].Item.Trim();
                    }
                    else
                    {
                        s += list[i].Item.Trim() + ";" + Environment.NewLine;
                    }
                }
            }
            return s;
        }



        public bool SerializeInterfaceIsOpen
        {
            get { return _SerializeInterfaceIsOpen; }
            set
            {
                if (SerializeInterfaceIsOpen != value)
                {
                    _SerializeInterfaceIsOpen = value;
                    OnPropertyChanged();
                    UploadToExcelValue = 0;
                    ImportActsValue = 0;
                    if (value)
                        LoadImpotList();
                }
            }
        }
        private bool _SerializeInterfaceIsOpen { get; set; }

        public int UploadToExcelValue
        {
            get { return _UploadToExcelValue; }
            set
            {
                if (UploadToExcelValue != value)
                {
                    _UploadToExcelValue = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _UploadToExcelValue { get; set; }


        public int UploadToExcelMaximum
        {
            get { return _UploadToExcelMaximum; }
            set
            {
                if (UploadToExcelMaximum != value)
                {
                    _UploadToExcelMaximum = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _UploadToExcelMaximum { get; set; }

        #endregion

        #region Количество документов
        public int ReadyItemsCount
        {
            get { return _ReadyItemsCount; }
            set
            {
                if (ReadyItemsCount != value)
                {
                    _ReadyItemsCount = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _ReadyItemsCount { get; set; }

        public int ExportedDocumentsCount
        {
            get { return _ExportedDocumentsCount; }
            set
            {
                if (ExportedDocumentsCount != value)
                {
                    _ExportedDocumentsCount = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _ExportedDocumentsCount { get; set; }

        public int InWorkDocumentsCount
        {
            get { return _InWorkDocumentsCount; }
            set
            {
                if (InWorkDocumentsCount != value)
                {
                    _InWorkDocumentsCount = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _InWorkDocumentsCount { get; set; }

        public int NotReadyKeywordsCount
        {
            get { return _NotReadyKeywordsCount; }
            set
            {
                if (NotReadyKeywordsCount != value)
                {
                    _NotReadyKeywordsCount = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _NotReadyKeywordsCount { get; set; }

        public int NotReadyRubricsCount
        {
            get { return _NotReadyRubricsCount; }
            set
            {
                if (NotReadyRubricsCount != value)
                {
                    _NotReadyRubricsCount = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _NotReadyRubricsCount { get; set; }



        protected async void getCounts()
        {
            ExportedDocumentsCount = await dbReader.ExportToExcelDocumentsCount();
            getLocalCounts();
        }

        protected void getLocalCounts()
        {
            InWorkDocumentsCount = CCollection.Where(d => !d.IsReady).Count();
            ReadyItemsCount = CCollection.Where(d => d.IsReady).Count();
            NotReadyKeywordsCount = CCollection.Where(d => !d.IsReady && !d.IsKeywordsReady).Count();
            NotReadyRubricsCount = CCollection.Where(d => !d.IsReady && !d.IsRubricsReady).Count();
        }

        #endregion

        #region Автоматическая простановка рубрик и ключевых слов

        private List<AutoClassificationModel> FillRubricsTrigger()
        {
            List<AutoClassificationModel> rt = new List<AutoClassificationModel>();
            try
            {

                using (FileStream fs = new FileStream("AutoClassification.list", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        rt = JsonConvert.DeserializeObject<List<AutoClassificationModel>>(sr.ReadToEnd()); ;
                    }
                }
                return rt;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return rt;
            }
        }

        private Dictionary<string, int> morph = new Dictionary<string, int>();
        private void Morphology(string text)
        {
            morph.Clear();
            Iveonik.Stemmers.RussianStemmer stemmer = new Iveonik.Stemmers.RussianStemmer();
            List<string> words = text
                  .Split(Core.Methods.Split.splitArray)
                  .Where(s => s != string.Empty && s.Length >= 4)
                  .Select(s => s.Trim()
                  .ToLower())
                  .ToList();
            foreach (string w in words)
            {
                string stemword = stemmer.Stem(w);
                if (morph.ContainsKey(stemword))
                    morph[stemword]++;
                else
                    morph.Add(stemword, 1);
            }

        }
        private async Task<ClassificationModel> AutoClassificatorAsync(ClassificationModel model)
        {
            var keys = await SetKeywordsToDocuments(model);
            return await SetRubricsToDocuments(keys);
        }

        protected async Task<ClassificationModel> SetRubricsToDocuments(ClassificationModel model, bool saveToBase = false, bool needInvoke = false)
        {
            return await Task<ClassificationModel>.Factory.StartNew(() =>
            {
                try
                {
                    bool rubricsChanged = false;
                    foreach (AutoClassificationModel item in RubricsTriggerList)
                    {
                        if (item.IsRegex)
                        {
                            Regex classificated = new Regex(item.SearchModel);
                            MatchCollection classificatedMatches = classificated.Matches(model.ActName);
                            if (classificatedMatches.Count > 0)
                                rubricsChanged = addRubrics(model, item, needInvoke);

                        }
                        else
                        {
                            switch (item.ContainsType)
                            {
                                default:
                                    {
                                        if (model.ActName.Contains(item.SearchModel))
                                            rubricsChanged = addRubrics(model, item, needInvoke);
                                        break;
                                    }
                                case "Start":
                                    {
                                        if (model.ActName.StartsWith(item.SearchModel))
                                            rubricsChanged = addRubrics(model, item, needInvoke);
                                        break;
                                    }
                                case "Keys":
                                    {
                                        int keysInName = 0;
                                        Morphology(model.ActName);
                                        foreach (string s in item.IdentifyKeys)
                                        {
                                            if (morph.ContainsKey(s))
                                            {
                                                keysInName++;
                                            }
                                        }
                                        if (item.IdentifyKeys.Count == keysInName)
                                            rubricsChanged = addRubrics(model, item, needInvoke);
                                        break;
                                    }
                                case "Keywords":
                                    {
                                        int keysInName = 0;
                                        foreach (var s in model.Keys)
                                        {
                                            if (item.IdentifyKeys.Contains(s.Item))
                                            {
                                                keysInName++;
                                            }
                                        }
                                        if (item.IdentifyKeys.Count == keysInName)
                                            rubricsChanged = addRubrics(model, item, needInvoke);
                                        break;
                                    }
                            }
                        }

                    }
                    if (saveToBase && rubricsChanged)
                        dbWriter.AddReadyRubricToBase(model);
                    return model;
                }
                catch (System.Exception ex)
                {
                    logger.Fatal(ex);
                    return model;
                }
            });
        }

        private bool addRubrics(ClassificationModel model, AutoClassificationModel item, bool needInvoke = false)
        {
            bool rubricsChanged = false;
            try
            {
                if (model.Rubrics.Count < 3)
                {
                    foreach (Guid Rubrid in item.Rubrics)
                    {
                        KeysAndRubricsModel rubric = new KeysAndRubricsModel() { ItemId = Rubrid, Item = DB.DbReader.rubricsDictionary[Rubrid] };
                        if (!model.Rubrics.Contains(rubric))
                        {
                            if(needInvoke)
                            {
                                d.Invoke(new Action(() => { model.Rubrics.Add(new KeysAndRubricsModel() { ItemId = Rubrid, Item = DB.DbReader.rubricsDictionary[Rubrid] });}));
                                rubricsChanged = true;
                            }
                            else
                            {
                                model.Rubrics.Add(new KeysAndRubricsModel() { ItemId = Rubrid, Item = DB.DbReader.rubricsDictionary[Rubrid] });
                                rubricsChanged = true;
                            }
                           
                        }
                    }
                }
                return rubricsChanged;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return rubricsChanged;
            }
        }
        protected async Task<ClassificationModel> SetKeywordsToDocuments(ClassificationModel model, bool saveToBase = false, bool needInvoke = false)
        {

            Guid synonymKey = Guid.Empty;
            return await Task<ClassificationModel>.Factory.StartNew(() =>
            {
                try
                {
                    var keys = DB.DbReader.KeyWords.Select(k => k.SourceText.ToLower()).ToList();
                    List<KeysAndRubricsModel> weightKeys = new List<KeysAndRubricsModel>();
                    List<string> txt = Core.Methods.IPS30.GetActTextFromIPS30(model.SignDate, model.ActNumber)
                       .Split(Core.Methods.Split.splitArray)
                       .Where(s => s != string.Empty && s.Length >= 4)
                       .Select(s => s.Trim()
                       .ToLower())
                       .ToList();

                    foreach (string word in txt)
                    {
                        if (keys.Contains(word))
                        {
                            var item = DB.DbReader.keysDictionary.Where(k => k.Value.Item.ToLower() == word).FirstOrDefault().Value;
                            if (!weightKeys.Contains(item))
                            {
                                item.InCount = 1;
                                weightKeys.Add(item);
                            }
                            else
                            {
                                weightKeys.Where(i => i.ItemId == item.ItemId).FirstOrDefault().InCount++;
                            }

                        }
                        if (DB.DbReader.synonimsDictionary.Keys.Contains(word))
                        {
                            synonymKey = DB.DbReader.synonimsDictionary[word];
                            var item = DB.DbReader.keysDictionary[synonymKey];
                            if (!weightKeys.Contains(item))
                            {
                                item.InCount = 1;
                                weightKeys.Add(item);
                            }
                            else
                            {
                                weightKeys.Where(i => i.ItemId == item.ItemId).FirstOrDefault().InCount++;
                            }
                        }
                    }

                    var orderedKeys = weightKeys.OrderByDescending(o => o.InCount + o.Weight).ToList();
                    bool keysChanged = false;
                    for (int i = 0; i < orderedKeys.Count; i++)
                    {
                        var item = orderedKeys[i];
                        if (item.Weight > 0 && model.Keys.Count <= 5 && !model.Keys.Contains(item))
                        {
                            if(needInvoke)
                            {
                                d.Invoke(new Action(() => { model.Keys.AddSorted(item); }));
                                keysChanged = true;
                            }
                            else
                            {
                                model.Keys.AddSorted(item);
                                keysChanged = true;
                            }
                           
                        }
                    }
                    if (saveToBase && keysChanged)
                        dbWriter.AddReadyKeywordToBase(model);
                    return model;

                }
                catch (System.Exception ex)
                {
                    logger.Info(synonymKey.ToString("D"));
                    logger.Fatal(ex);
                    return model;
                }
            });
        }



        #endregion
    }
}
