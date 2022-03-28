using Classification.Core;
using Classification.Core.Models;
using Classification.Modules.DocumentTextViewerModule.Models;
using Classification.Modules.DocumentTextViewerModule.Synonyms;
using MySql.Data.MySqlClient;
using NLog;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.Windows.Threading;
using System.Windows;

namespace Classification.Modules.DocumentTextViewerModule.ViewModels
{
    class ViewDocumentTextViewerModuleViewModel : IncrementSearch, INotifyPropertyChanged
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        private List<TextInlineSelection> KeywordsEtalon { get; set; }
        public ObservableCollection<TextInlineSelection> Keywords { get; set; } 
        private string TextFromIPS { get; set; }
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
        IEventAggregator eventAggregator;
        public ViewDocumentTextViewerModuleViewModel(IEventAggregator ea)
        {
            Keywords = new ObservableCollection<TextInlineSelection>();
            KeywordsEtalon = new List<TextInlineSelection>();
            eventAggregator = ea;
            SubscribeEvents();
            InitializeCommands();
        }

        //#region RoutedEvents
        //public delegate void TextChangedEventHandler();
        //public event TextChangedEventHandler TextChangedEvent;
        //public void RiseTextChangedEvent()
        //{
        //    if(TextChangedEvent != null)
        //    {
        //        TextChangedEvent();
        //    }
        //}

        //#endregion

        #region Commands
        public DelegateCommand SearchKeywordForSynonymCommand { get; set; }
        public DelegateCommand CloseSynonymWindowCommand { get; set; }
        public DelegateCommand AddSynonymCommand { get; set; }
        private void InitializeCommands()
        {
            SearchKeywordForSynonymCommand = new DelegateCommand(SearchKeywordForSynonym);
            CloseSynonymWindowCommand = new DelegateCommand(() => AddSynonymWordPopUpIsOpen = false);
            AddSynonymCommand = new DelegateCommand(AddSynonym);
        }
        #endregion

        #region Events
        private void SubscribeEvents()
        {
            eventAggregator.GetEvent<ActSelectionChangedEvent>().Subscribe(d => GetTextFromIps30(d.SignDate, d.ActNumber));
            eventAggregator.GetEvent<KeywordsIsLoaded>().Subscribe(GetKeysForSynonyms);
        }
        #endregion

       


        public string DocumentText
        {
            get { return _DocumentText; }
            set
            {
                if (DocumentText != value)
                {
                    _DocumentText = value;
                    OnPropertyChanged();
                    IsTextChanged = true;
                    IsTextChanged = false;
                }
            }
        }
        private string _DocumentText { get; set; }

        public bool IsTextChanged
        {
            get { return _IsTextChanged; }
            set
            {
                if (IsTextChanged != value)
                {
                    _IsTextChanged = value;
                    OnPropertyChanged();

                }
            }
        }
        private bool _IsTextChanged { get; set; }


        public WordPositionStruct SelectedWord
        {
            get { return _SelectedWord; }
            set
            {
                if (SelectedWord != value)
                {
                    _SelectedWord = value;
                    OnPropertyChanged();
                    bool addOk = false;
                    if (DB.DbReader.synonimsDictionary.ContainsKey(value.Word.ToLower()))
                    {
                        eventAggregator.GetEvent<AddingKeyEvent>().Publish(DB.DbReader.synonimsDictionary[value.Word.ToLower()]);
                        addOk = true;
                    }
                    if (!addOk && KeywordsEtalon.Select(s => s.SourceText.ToLower()).Contains(value.Word.ToLower()))
                    {
                        var key = KeywordsEtalon.Where(s => s.SourceText.ToLower() == value.Word.ToLower()).FirstOrDefault();
                        if (key != null)
                        {
                            eventAggregator.GetEvent<AddingKeyEvent>().Publish(key.Id);
                            addOk = true;
                        }
                    }
                    if (!addOk)
                    {
                        AddSynonymWordPopUpIsOpen = true;
                        SynonymSearchBox = SelectedWord.Word;
                    }


                }
            }
        }
        private WordPositionStruct _SelectedWord { get; set; }


        private void GetTextFromIps30(DateTime signDate, string actNumber)
        {
            string response = Core.Methods.IPS30.GetActTextFromIPS30(signDate, actNumber);
            DocumentText = response;
            TextFromIPS = response;
        }

        #region Работа с синонимами ключевых слов
        public bool AddSynonymWordPopUpIsOpen
        {
            get { return _AddSynonymWordPopUpIsOpen; }
            set
            {
                if (AddSynonymWordPopUpIsOpen != value)
                {
                    _AddSynonymWordPopUpIsOpen = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _AddSynonymWordPopUpIsOpen { get; set; }

        public string SynonymSearchBox
        {
            get { return _SynonymSearchBox; }
            set
            {
                if (SynonymSearchBox != value)
                {
                    _SynonymSearchBox = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _SynonymSearchBox { get; set; }

        public TextInlineSelection SelectedKeywordForSynonym
        {
            get { return _SelectedKeywordForSynonym; }
            set
            {
                if (SelectedKeywordForSynonym != value)
                {
                    _SelectedKeywordForSynonym = value;
                    OnPropertyChanged();
                }
            }
        }
        private TextInlineSelection _SelectedKeywordForSynonym { get; set; }

        private void SearchKeywordForSynonym()
        {
            if (SynonymSearchBox != null)
                Search(SynonymSearchBox, Keywords, KeywordsEtalon);
        }
        Dispatcher d = Dispatcher.CurrentDispatcher;
        private void GetKeysForSynonyms(IEnumerable<TextInlineSelection> collection)
        {
           
            foreach (var item in collection)
            {
                d.Invoke(new Action(() => {
                    Keywords.Add(item);
                }));
               
                KeywordsEtalon.Add(item);
            }
        }
        private void AddSynonym()
        {
            try
            {
                if (SelectedKeywordForSynonym != null && SelectedWord != null)
                {
                    if (!DB.DbReader.synonimsDictionary.ContainsKey(SelectedWord.Word.ToLower()))
                    {
                        DB.DbReader.synonimsDictionary.Add(SelectedWord.Word.ToLower(), SelectedKeywordForSynonym.Id);
                        eventAggregator.GetEvent<AddSynonymToBaseEvent>().Publish(new KeyValuePair<string, Guid>(SelectedWord.Word.ToLower(), SelectedKeywordForSynonym.Id));
                        DocumentText = TextFromIPS;
                        AddSynonymWordPopUpIsOpen = false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        #endregion


    }
}
