using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Classification.Core
{
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        public AsyncObservableCollection() { }
        public AsyncObservableCollection(IEnumerable<T> list) : base(list) { }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_inBachUpdate)
            {
                if (SynchronizationContext.Current == _synchronizationContext)
                {
                    RiseCollectionChanged(e);
                }
                else
                {
                    _synchronizationContext.Send(RiseCollectionChanged, e);
                }
            }
        }
        private void RiseCollectionChanged(object param)
        {
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (!_inBachUpdate)
            {
                if (SynchronizationContext.Current == _synchronizationContext)
                {
                    RisePropertyChanged(e);
                }
                else
                {
                    _synchronizationContext.Send(RisePropertyChanged, e);
                }
            }
        }

        private void RisePropertyChanged(object param)
        {
            base.OnPropertyChanged((PropertyChangedEventArgs)param);
        }

        #region BachUpdate
        private bool _inBachUpdate;

        #endregion

        #region Сортировка массива
        public void AddSorted(T item)
        {
            int i = 0;
            if (DefaultComparer != null)
            {
                while (i < this.Count && DefaultComparer.Compare(this[i], item) < 0)
                    i++;
                this.Insert(i, item);
            }
        }
        public void AddSortedWithoutNotificate(T item)
        {
            _inBachUpdate = true;
            int i = 0;
            if (DefaultComparer != null)
            {
                while (i < this.Count && DefaultComparer.Compare(this[i], item) < 0)
                    i++;
                this.Items.Insert(i, item);
            }
            _inBachUpdate = false;
        }
        public void FullCollectionNotificate()
        {
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public IComparer<T> DefaultComparer { get; set; }
        public void Sort(IComparer<T> comparer)
        {
            var sortableList = new List<T>(this);
            sortableList.Sort(comparer);

            for (int i = 0; i < sortableList.Count; i++)
            {
                int oldindex = this.IndexOf(sortableList[i]);
                RemoveAt(oldindex);
                Insert(i, sortableList[i]);
            }
            this.DefaultComparer = comparer;
        }
        public void Sort()
        {
            if (DefaultComparer != null)
                Sort(DefaultComparer);
        }
        #endregion

    }
}
