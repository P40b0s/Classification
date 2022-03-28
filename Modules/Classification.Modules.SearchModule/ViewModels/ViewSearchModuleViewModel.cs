using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Classification.Core.Models;
using Prism.Commands;
using Classification.Core;

namespace Classification.Modules.SearchModule.ViewModels
{
    class ViewSearchModuleViewModel : IncrementSearch, INotifyPropertyChanged
    {
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
        public ViewSearchModuleViewModel(IEventAggregator _eventAggregator)
        {
            eventAggregator = _eventAggregator;
            Keywords = new ObservableCollection<TextInlineSelection>();
            KeywordsEtalon = new List<TextInlineSelection>();
            Rubrics = new ObservableCollection<TextInlineSelection>();
            RubricsEtalon = new List<TextInlineSelection>();
            CommandsInitialization();
            SubscribeEvents();
        }

        #region EventAggregator
        private void SubscribeEvents()
        {
            eventAggregator.GetEvent<KeywordsIsLoaded>().Subscribe(FillKeywords);
            eventAggregator.GetEvent<RubricsIsLoaded>().Subscribe(FillRubrics);
        }
        #endregion

        private void FillKeywords(IEnumerable<TextInlineSelection> collection)
        {
            foreach (var item in collection)
            {
                Keywords.Add(item);
                KeywordsEtalon.Add(item);
            }
        }
        private void FillRubrics(IEnumerable<TextInlineSelection> collection)
        {
            foreach (var item in collection)
            {
                Rubrics.Add(item);
                RubricsEtalon.Add(item);
            }
        }

        #region Команды
        public DelegateCommand AddKeyCommand { get; set; }
        public DelegateCommand AddRubricCommand { get; set; }
        public DelegateCommand SearchKeywordCommand { get; set; }
        public DelegateCommand SearchRubricCommand { get; set; }

        private void CommandsInitialization()
        {
            AddKeyCommand = new DelegateCommand(AddKey);
            AddRubricCommand = new DelegateCommand(AddRubric);
            SearchKeywordCommand = new DelegateCommand(SearchKeyword);
            SearchRubricCommand = new DelegateCommand(SearchRubric);
        }
        #endregion

        private void SearchKeyword()
        {
            if(KeySearchBox != null)
            Search(KeySearchBox, Keywords, KeywordsEtalon);
            if (Keywords.Count == 1)
            {
                eventAggregator.GetEvent<AddingKeyEvent>().Publish(Keywords.ElementAt(0).Id);
                KeySearchBox = string.Empty;
            }
        }
        private void SearchRubric()
        {
           if(RubricSearchBox != null)
            Search(RubricSearchBox, Rubrics, RubricsEtalon);
           if(Rubrics.Count == 1)
            {
                eventAggregator.GetEvent<AddingRubricEvent>().Publish(Rubrics.ElementAt(0).Id);
                RubricSearchBox = string.Empty;
            }
                
        }

        private void AddKey()
        {
            if (SelectedKey != null)
            {
                eventAggregator.GetEvent<AddingKeyEvent>().Publish(SelectedKey.Id);
                KeySearchBox = string.Empty;
            }
               
        }
        private void AddRubric()
        {
            if (SelectedRubric != null)
            {
                eventAggregator.GetEvent<AddingRubricEvent>().Publish(SelectedRubric.Id);
                RubricSearchBox = string.Empty;
            }
              
        }

        public TextInlineSelection SelectedKey
        {
            get { return _SelectedKey; }
            set
            {
                if (SelectedKey != value)
                {
                    _SelectedKey = value;
                    OnPropertyChanged();
                }
            }
        }
        private TextInlineSelection _SelectedKey { get; set; }


        public TextInlineSelection SelectedRubric
        {
            get { return _SelectedRubric; }
            set
            {
                if (SelectedRubric != value)
                {
                    _SelectedRubric = value;
                    OnPropertyChanged();
                }
            }
        }
        private TextInlineSelection _SelectedRubric { get; set; }

        public string KeySearchBox
        {
            get { return _KeySearchBox; }
            set
            {
                if (KeySearchBox != value)
                {
                    _KeySearchBox = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _KeySearchBox { get; set; }

        public string RubricSearchBox
        {
            get { return _RubricSearchBox; }
            set
            {
                if (RubricSearchBox != value)
                {
                    _RubricSearchBox = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _RubricSearchBox { get; set; }

        private List<TextInlineSelection> KeywordsEtalon { get; set; }
        private List<TextInlineSelection> RubricsEtalon { get; set; }
        public ObservableCollection<TextInlineSelection> Keywords { get; set; }
        public ObservableCollection<TextInlineSelection> Rubrics { get; set; }
    }

}
