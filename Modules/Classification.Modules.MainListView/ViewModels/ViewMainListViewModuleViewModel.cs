using Classification.Core;
using Classification.Core.Enums;
using Classification.Core.Models;
using Newtonsoft.Json;
using NLog;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Classification.Modules.MainListViewModule.ViewModels
{
    class ViewMainListViewModuleViewModel : ExcelExporter
    {
        /// <summary>
        /// Максимальное количество ключей в документе
        /// </summary>
        private const int MaximumKeys = 6;
        /// <summary>
        /// Максимальное количество рубрик в документе
        /// </summary>
        private const int MaximumRubrics = 3;
        private const string JSONPath = "AutoClassification.list";
        IEventAggregator eventAggregator;
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        private Timer RefreshTimer { get; set; }
        public ViewMainListViewModuleViewModel(IEventAggregator _ea) : base(_ea)
        {
            eventAggregator = _ea;
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                RefreshTimer = new Timer(RefreshTimerCallback);
                RefreshTimer.Change(30000, 30000);
                CCollection = new ObservableCollection<ClassificationModel>();
                dbReader.FillActsCollection(CCollection);
                InitializeCommands();
                SubscribeEvents();
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        #region Команды
        public DelegateCommand<object> RemoveKeyCommand { get; set; }
        public DelegateCommand<object> RemoveRubricCommand { get; set; }
        public DelegateCommand SaveDocumentCommand { get; set; }
        public DelegateCommand SearchDocumentAnalogCommand { get; set; }
        public DelegateCommand SearchRegexDocumentAnalogCommand { get; set; }
        public DelegateCommand OpenSerializationInterfaceCommand { get; set; }

        private void InitializeCommands()
        {
            RemoveKeyCommand = new DelegateCommand<object>(RemoveKey);
            RemoveRubricCommand = new DelegateCommand<object>(RemoveRubric);
            SaveDocumentCommand = new DelegateCommand(SaveDocument);
            OpenSerializationInterfaceCommand = new DelegateCommand(OpenSerializationInterface);
        }
        #endregion

        #region Эвенты
        private void SubscribeEvents()
        {
            eventAggregator.GetEvent<AddingKeyEvent>().Subscribe(AddingKey);
            eventAggregator.GetEvent<AddingRubricEvent>().Subscribe(AddingRubric);
            eventAggregator.GetEvent<AllActsIsLoadedEvent>().Subscribe(getCounts);
        }
        #endregion

        #region Добавление и удаление ключей и рубрик
        private void AddingKey(Guid Id)
        {
            try
            {
                if (SelectedItem != null && SelectedItem.Keys.Count < MaximumKeys)
                {
                    var DocId = SelectedItem.Id;
                    var KeyId = Id;
                    if (SelectedItem.Keys.Where(k => k.ItemId == Id).Count() == 0)
                    {
                        SelectedItem.Keys.AddSorted(new KeysAndRubricsModel() { ItemId = KeyId, Item = DB.DbReader.keysDictionary[KeyId].Item });
                    }
                    else
                    {
                        throw new ArgumentException($"Ключевое слово {KeyId} уже добавлено к документу");
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        private void AddingRubric(Guid Id)
        {
            try
            {
                if (SelectedItem != null && SelectedItem.Rubrics.Count < MaximumRubrics)
                {
                    var DocId = SelectedItem.Id;
                    var rubId = Id;
                    if (SelectedItem.Rubrics.Where(r => r.ItemId == Id).Count() == 0)
                    {
                        SelectedItem.Rubrics.AddSorted(new KeysAndRubricsModel() { ItemId = rubId, Item = DB.DbReader.rubricsDictionary[rubId] });
                    }
                    else
                    {
                        throw new ArgumentException($"Рубрика {rubId} уже добавлена к рубрикам документа");
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }



        private void RemoveRubric(object item)
        {
            try
            {
                Guid itemId = (Guid)item;
                if (SelectedItem != null)
                {
                    var rub = SelectedItem.Rubrics.Where(k => k.ItemId == itemId).FirstOrDefault();
                    if (rub != null)
                    {
                        SelectedItem.Rubrics.Remove(rub);
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        private void RemoveKey(object item)
        {
            try
            {
                Guid itemId = (Guid)item;
                if (SelectedItem != null)
                {
                    var key = SelectedItem.Keys.Where(k => k.ItemId == itemId).FirstOrDefault();
                    if (key != null)
                    {
                        SelectedItem.Keys.Remove(key);
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        #endregion

        #region Сохранение документа
        private async void SaveDocument()
        {
            bool ready = await dbWriter.SaveClassificatedDocumentAsync(SelectedItem);
            getLocalCounts();
        }
        private async void SaveDocument(ClassificationModel item)
        {
            bool ready = await dbWriter.SaveClassificatedDocumentAsync(item);
        }


        #endregion

        #region Выставление рубрик и ключевых слов по похожим документам
        private void SearchDocumentAnalog()
        {
            int count = 0;
            try
            {
                for (int i = 0; i < CCollection.Count; i++)
                {
                    if (CCollection[i].ActName.Contains(CorrectedActName))
                    {
                        if (CCollection[i].Keys.Count == 0)
                        {
                            foreach (var key in SelectedItem.Keys)
                            {
                                CCollection[i].Keys.Add(key);
                            }
                        }
                        if (CCollection[i].Rubrics.Count == 0)
                        {
                            foreach (var rubric in SelectedItem.Rubrics)
                            {
                                CCollection[i].Rubrics.Add(rubric);
                            }
                        }
                        SaveDocument(CCollection[i]);
                        count++;
                    }
                }
                logger.Info($"Установлены рубрики и (или) ключевые слова для {count} документов");
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        #region Encoding\Decoding

        private string ToBase64(string pattern)
        {
            var txt = System.Text.Encoding.UTF8.GetBytes(pattern);
            return System.Convert.ToBase64String(txt);
        }
        private string FromBase64(string pattern)
        {
            var bytes = System.Convert.FromBase64String(pattern);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        private void SerializeAutoClassificationClass()
        {
            try
            {
                List<AutoClassificationModel> auto = new List<AutoClassificationModel>();
                AutoClassificationModel reg1 = new AutoClassificationModel();
                reg1.SearchModel = @"^О\s[А-Я]{1}[а-я]{4,20}\s[А-Я]{1}.[А-Я]{1}.";
                reg1.IsRegex = true;
                reg1.Rubrics.Add(new Guid("95FE40AE-D464-4516-8832-912C58E4DCF2"));
                auto.Add(reg1);
                AutoClassificationModel reg2 = new AutoClassificationModel();
                reg2.SearchModel = @"^Об\s[А-Я]{1}[а-я]{4,20}\s[А-Я]{1}.[А-Я]{1}.";
                reg2.IsRegex = true;
                reg2.Rubrics.Add(new Guid("95FE40AE-D464-4516-8832-912C58E4DCF2"));
                auto.Add(reg2);
                AutoClassificationModel acm2 = new AutoClassificationModel();
                acm2.SearchModel = "О реорганизации";
                acm2.IsRegex = false;
                acm2.Rubrics.Add(new Guid("13617FAC-9E75-4D40-9841-20FB02FD2D95"));
                acm2.Rubrics.Add(new Guid("08AB5425-4568-4FA1-B40F-967584ED25C7"));
                auto.Add(acm2);
                //SerializeToFile(auto);
                string json = JsonConvert.SerializeObject(auto, Formatting.Indented);
                File.WriteAllText(JSONPath, json);
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }


        #endregion


        Dispatcher d = Dispatcher.CurrentDispatcher;
        #endregion

        private async void RefreshTimerCallback(object obj)
        {
            if (await dbReader.RefreshItems(CCollection))
            {
                getLocalCounts();
            }
        }

        #region Открытие интерфейса сериализации\десериализации
        private void OpenSerializationInterface()
        {
            SerializeInterfaceIsOpen = true;
            UploadToExcelMaximum = ReadyItemsCount;
        }
        #endregion



        public ClassificationModel SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (SelectedItem != value)
                {
                    _SelectedItem = value;
                    OnPropertyChanged();
                    if (value != null)
                    {
                        CorrectedActName = SelectedItem.ActName;
                        eventAggregator.GetEvent<ActSelectionChangedEvent>().Publish(SelectedItem);
                        SearchRubricsAndKeywords(value);
                    }

                }
            }
        }
        private ClassificationModel _SelectedItem { get; set; }

        private async void SearchRubricsAndKeywords(ClassificationModel value)
        {

            await base.SetKeywordsToDocuments(value, true, true);
            await base.SetRubricsToDocuments(value, true, true);

        }


        public bool ShowReadyDocumentsInListView
        {
            get { return _ShowReadyDocumentsInListView; }
            set
            {
                if (ShowReadyDocumentsInListView != value)
                {
                    _ShowReadyDocumentsInListView = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _ShowReadyDocumentsInListView { get; set; }

        public string CorrectedActName
        {
            get { return _CorrectedActName; }
            set
            {
                if (CorrectedActName != value)
                {
                    _CorrectedActName = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _CorrectedActName { get; set; }




    }
}
