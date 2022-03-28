using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.Windows.Documents;
using System.Windows.Media;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;
using MahApps.Metro.Controls;
using MahApps.Metro;
using Microsoft.Windows;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using NLog;
using System.Windows.Threading;

namespace Classification
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

   
    public partial class MainWindow : MetroWindow
    {

        public string AText;
        List<string> RubrikiBufer;
        List<string> KeyWordsBufer;
        SQLMethods sq = new SQLMethods();
        AddingRubricsAndKeywords items = new AddingRubricsAndKeywords();
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public MainWindow()
        {
            InitializeComponent();
            try
            {

                this.Loaded += (s, e) =>
                {
                    ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(x => x.Name == "Orange"), Theme.Light);
                    KeysListBox.ItemsSource = items.KeyList;
                    OtraslyListBox.ItemsSource = items.Otraslyst;
                    ContextMenuListBox.ItemsSource = items.RichTextKeyList;
                    load();
                    //SpisokListBox.SelectedIndex = Properties.Settings.Default.ListBoxLastIndex;
                    //SpisokListBox.ScrollIntoView(SpisokListBox.SelectedItem);
                    ExcelImportExport.bg.ProgressChanged += bg_ProgressChanged;
                    
                    #region добавление рубрик и ключевых слов ао нажатию enter и выделенных
                    Settings_Button.Click += (s1, e1) =>
                    {
                        FlySettings.IsOpen = !FlySettings.IsOpen;
                    };
                    KeysListBox.KeyDown += (s1, e1) =>
                        {
                            switch (e1.Key)
                            {
                                case (Key.Enter):
                                    if (SpisokListBox.SelectedIndex >= 0)
                                    {
                                        int id = collection[SpisokListBox.SelectedIndex].IndexNum;
                                        if (collection[SpisokListBox.SelectedIndex].Keywordd.Count < 6 && collection[SpisokListBox.SelectedIndex].Keywordd.IndexOf((KeysListBox.Items[KeysListBox.SelectedIndex] as TextBlockSelection).TextBefore.ToString()) == -1)
                                        {
                                            collection[SpisokListBox.SelectedIndex].Keywordd.Add((KeysListBox.Items[KeysListBox.SelectedIndex] as TextBlockSelection).TextBefore.ToString());
                                            collection[SpisokListBox.SelectedIndex].Keywordd.Sort();
                                            sq.SetKeywords(id, collection[SpisokListBox.SelectedIndex].Keywordd);
                                            SpisokListBox.Items.Refresh();
                                            KeySearchBox.Clear();
                                            KeySearchBox.Focus();
                                        }
                                    }
                                    break;

                                case (Key.Escape):
                                    KeySearchBox.Clear();
                                    break;
                            }
                        };
                    KeySearchBox.KeyDown += (s1, e1) =>
                    {
                        switch (e1.Key)
                        {
                            case (Key.Enter):
                                if (SpisokListBox.SelectedIndex >= 0)
                                {
                                    int id = collection[SpisokListBox.SelectedIndex].IndexNum;
                                    if (collection[SpisokListBox.SelectedIndex].Keywordd.Count < 6 && KeysListBox.Items.Count > 0 && collection[SpisokListBox.SelectedIndex].Keywordd.IndexOf((KeysListBox.Items[0] as TextBlockSelection).TextBefore.ToString()) == -1)
                                    {
                                        collection[SpisokListBox.SelectedIndex].Keywordd.Add((KeysListBox.Items[0] as TextBlockSelection).TextBefore.ToString());
                                        collection[SpisokListBox.SelectedIndex].Keywordd.Sort();
                                        sq.SetKeywords(id, collection[SpisokListBox.SelectedIndex].Keywordd);
                                        SpisokListBox.Items.Refresh();
                                        KeySearchBox.Clear();
                                        KeySearchBox.Focus();
                                    }
                                }
                                break;

                            case (Key.Escape):
                                KeySearchBox.Clear();

                                break;
                        }
                    };
                    OtraslyListBox.KeyDown += (s1, e1) =>
                        {
                            switch (e1.Key)
                            {
                                case (Key.Enter):
                                    if (SpisokListBox.SelectedIndex >= 0)
                                    {
                                        int id = collection[SpisokListBox.SelectedIndex].IndexNum;
                                        if (collection[SpisokListBox.SelectedIndex].Rubriki.Count < 3 && (collection[SpisokListBox.SelectedIndex].Rubriki.IndexOf((OtraslyListBox.Items[OtraslyListBox.SelectedIndex] as TextBlockSelection).TextBefore.ToString()) == -1))
                                        {
                                            collection[SpisokListBox.SelectedIndex].Rubriki.Add((OtraslyListBox.Items[OtraslyListBox.SelectedIndex] as TextBlockSelection).TextBefore.ToString());
                                            collection[SpisokListBox.SelectedIndex].Rubriki.Sort();
                                            sq.SetRubrics(id, collection[SpisokListBox.SelectedIndex].Rubriki);
                                            SpisokListBox.Items.Refresh();
                                            OtraslySearchBox.Clear();
                                            OtraslySearchBox.Focus();
                                        }
                                    }
                                    break;

                                case (Key.Escape):
                                    OtraslySearchBox.Clear();

                                    break;
                            }
                        };
                    OtraslySearchBox.KeyDown += (s1, e1) =>
                    {
                        switch (e1.Key)
                        {
                            case (Key.Enter):
                                if (SpisokListBox.SelectedIndex >= 0)
                                {
                                    int id = collection[SpisokListBox.SelectedIndex].IndexNum;
                                    if (collection[SpisokListBox.SelectedIndex].Rubriki.Count < 3 && OtraslyListBox.Items.Count > 0 && (collection[SpisokListBox.SelectedIndex].Rubriki.IndexOf((OtraslyListBox.Items[0] as TextBlockSelection).TextBefore.ToString()) == -1))
                                    {
                                        collection[SpisokListBox.SelectedIndex].Rubriki.Add((OtraslyListBox.Items[0] as TextBlockSelection).TextBefore.ToString());
                                        collection[SpisokListBox.SelectedIndex].Rubriki.Sort();
                                        sq.SetRubrics(id, collection[SpisokListBox.SelectedIndex].Rubriki);
                                        SpisokListBox.Items.Refresh();
                                        OtraslySearchBox.Clear();
                                        OtraslySearchBox.Focus();
                                    }
                                }
                                break;

                            case (Key.Escape):
                                OtraslySearchBox.Clear();

                                break;
                        }
                    };

                    SpisokListBox.KeyDown += (s1, e1) =>
                    {
                        switch (e1.Key)
                        {
                            case (Key.F1):
                                {
                                    RubrikiBufer = collection[SpisokListBox.SelectedIndex].Rubriki;
                                    KeyWordsBufer = collection[SpisokListBox.SelectedIndex].Keywordd;
                                    break;
                                }
                            case (Key.F2):
                                {
                                    if (KeyWordsBufer != null && KeyWordsBufer.Count > 0)
                                    {
                                        collection[SpisokListBox.SelectedIndex].Keywordd = KeyWordsBufer;
                                        sq.SetKeywords(collection[SpisokListBox.SelectedIndex].IndexNum, KeyWordsBufer);
                                    }
                                    if (RubrikiBufer != null && RubrikiBufer.Count > 0)
                                    {
                                        collection[SpisokListBox.SelectedIndex].Rubriki = RubrikiBufer;
                                        sq.SetRubrics(collection[SpisokListBox.SelectedIndex].IndexNum, RubrikiBufer);
                                    }


                                    break;
                                }
                            case (Key.Delete):
                                {
                                    int id = collection[SpisokListBox.SelectedIndex].IndexNum;
                                    collection[SpisokListBox.SelectedIndex].Keywordd.Clear();
                                    collection[SpisokListBox.SelectedIndex].Rubriki.Clear();
                                    SpisokListBox.Items.Refresh();
                                    sq.SetKeywords(id, collection[SpisokListBox.SelectedIndex].Keywordd);
                                    sq.SetRubrics(id, collection[SpisokListBox.SelectedIndex].Rubriki);
                                    break;
                                }
                        }
                    };


                    #endregion

                    #region
                    FindDubleButton.Click += (s1, e1) =>
                        {
                            int id = collection[SpisokListBox.SelectedIndex].IndexNum;
                            SQLMethods.FindDouble(collection, CorrectTextBox.Text, id);
                            SpisokListBox.Items.Refresh();
                        };
                    #endregion
                    this.Closing += (s1, e1) =>
                    {
                      //  Process.Start("SQLLITEBackUp.exe");
                    };
                  


                    SpisokListBox.SelectionChanged += async (s1, e1) =>
                    {
                        int id = collection[SpisokListBox.SelectedIndex].IndexNum;
                       
                        if (e1.RemovedItems.Count > 0 && SpisokListBox.SelectedIndex != -1)
                        {

                            CorrectTextBox.Text = collection[SpisokListBox.SelectedIndex].ActName;
                            Paragraph p = new Paragraph();
                            FlowDocument fd = new FlowDocument();
                            p.Inlines.Add(new Run(await sq.GetActText(id)));
                            fd.Blocks.Add(p);
                            ActText.Document = fd;
                            //int i = (await sq.GetActText(id)).Length;
                            //if (i <= 200000)
                            {
                                RefreshWords();
                                RunTextHighLight();
                            }                           
                            //HighLight hl = new HighLight(ActText);
                            //hl.RefreshWords();
                            //hl.RunTextHighLight();
                            //ActText = hl.RTB;
                        }
                        Properties.Settings.Default.ListBoxLastIndex = SpisokListBox.SelectedIndex;
                        Properties.Settings.Default.Save();
                        sq.RefreshTable(collection);
                        SpisokListBox.Items.Refresh();
                    };
                    RefreshWords();
                    ActText.MouseDoubleClick  += async (s1, e1) =>
                    {
                        if (ActText.Selection.Text.Length > 0 && !ActText.Selection.Text.Contains("\r"))
                        {
                            int id = collection[SpisokListBox.SelectedIndex].IndexNum;



                            string ss = await sq.GetMainKeyWordFromBase(ActText.Selection.Text);
                            if (ss.Length > 0)
                            {
                                if (collection[SpisokListBox.SelectedIndex].Keywordd.IndexOf(ss) == -1 && collection[SpisokListBox.SelectedIndex].Keywordd.Count < 6 && KeysListBox.Items.Count > 0)
                                {
                                    collection[SpisokListBox.SelectedIndex].Keywordd.Add(ss);
                                    collection[SpisokListBox.SelectedIndex].Keywordd.Sort();
                                    sq.SetKeywords(id, collection[SpisokListBox.SelectedIndex].Keywordd);
                                    SpisokListBox.Items.Refresh();
                                }
                            }
                            else
                            {

                                if (ActText.Selection.Text.Trim().Length > 2)
                                {
                                    cmenu.IsOpen = true;
                                    ContextMenuKeySearchBox.Text = ActText.Selection.Text.Trim().Remove(ActText.Selection.Text.Trim().Length - 2);
                                }

                            }
                        }
                    };
                };

                ContextMenuOk.Click += (s2, e2) =>
                {
                    if (ContextMenuListBox.SelectedIndex != -1)
                    {
                        AddWordToBase(ActText.Selection.Text.Trim(), (ContextMenuListBox.SelectedItem as TextBlockSelection).TextBefore);
                        cmenu.IsOpen = false;
                    }
                    ContextMenuListBox.SelectedIndex = -1;
                };
                ContextMenuCancel.Click += (s2, e2) => { cmenu.IsOpen = false; };

                keyslistbox.MouseDoubleClick += async (s1, s2) =>
                {
                    if (keyslistbox.SelectedIndex != -1)
                    {
                        int id = collection[SpisokListBox.SelectedIndex].IndexNum;
                        string ss = await sq.GetMainKeyWordFromBase(keyslistbox.SelectedItem.ToString());
                        if (collection[SpisokListBox.SelectedIndex].Keywordd.IndexOf(ss) == -1)
                        {
                            collection[SpisokListBox.SelectedIndex].Keywordd.Add(ss);
                            collection[SpisokListBox.SelectedIndex].Keywordd.Sort();
                            sq.SetKeywords(id, collection[SpisokListBox.SelectedIndex].Keywordd);
                            SpisokListBox.Items.Refresh();
                        }
                    }

                };
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void load()
        {
            SpisokListBox.ItemsSource = await sq.ReadFromTable();
            ActsCount.Text = await sq.GetActsCount();
        }
        private ObservableCollection<ReadyListBoxClass> collection
        {
            get
            {
                return SpisokListBox.ItemsSource.Cast<ReadyListBoxClass>() as ObservableCollection<ReadyListBoxClass>;
            }
        }
        private async void RefreshWords(bool b = true)
        {
            if (b)
            {
                string[] specialWords = (await sq.LoadAllKeywords()).ToArray();
                tags = new List<string>(specialWords);
                // возможные разделители слов                   
            }
            else
            {
                tags = new List<string>();
            }
            char[] chrs = {
                 '.',
                 ')',
                 '(',
                 '[',
                 ']',
                 '>',
                 '<',
                 ':',
                 ';',
                 '\n',
                 '\t',
                 '\r',
                 '"',
                 ',',
                 '|',
                 '-'
                          };
            specials = new List<char>(chrs);

        }
        private void AddWordToBase(string key, string MainKey)
        {
            sq.ApplyngKeysToBaseKeyword(key.ToLower(), MainKey);
            new Thread(() =>
            {
                RefreshWords(false);
                tags.Add(key);
                Dispatcher.Invoke(new Action(() => { RunTextHighLight(false); }));
            }).Start();
        }

        //добавление в листбоксы и в базу рубрик и ключевых слов по двойному клику мышки
        private void ListBoxMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ClickCount == 2 && SpisokListBox.SelectedIndex >= 0)
            {
                ObservableCollection<ReadyListBoxClass> collection = SpisokListBox.ItemsSource.Cast<ReadyListBoxClass>() as ObservableCollection<ReadyListBoxClass>;
                int id = collection[SpisokListBox.SelectedIndex].IndexNum;
                if (OtraslyListBox.IsKeyboardFocusWithin == true)
                {
                    if (collection[SpisokListBox.SelectedIndex].Rubriki.Count < 3 && (collection[SpisokListBox.SelectedIndex].Rubriki.IndexOf((OtraslyListBox.Items[OtraslyListBox.SelectedIndex] as TextBlockSelection).TextBefore.ToString()) == -1))
                    {
                        var t = OtraslyListBox.SelectedItem as TextBlockSelection;
                        collection[SpisokListBox.SelectedIndex].Rubriki.Add(t.TextBefore.ToString());
                        collection[SpisokListBox.SelectedIndex].Rubriki.Sort();
                        sq.SetRubrics(id, collection[SpisokListBox.SelectedIndex].Rubriki);
                        SpisokListBox.Items.Refresh();
                        OtraslySearchBox.Clear();
                        OtraslySearchBox.Focus();
                    }
                }

                else
                {
                    if (KeysListBox.IsKeyboardFocusWithin == true)
                    {
                        if (collection[SpisokListBox.SelectedIndex].Keywordd.Count < 6 && (collection[SpisokListBox.SelectedIndex].Keywordd.IndexOf((KeysListBox.Items[KeysListBox.SelectedIndex] as TextBlockSelection).TextBefore.ToString()) == -1))
                        {
                            var t = KeysListBox.SelectedItem as TextBlockSelection;
                            collection[SpisokListBox.SelectedIndex].Keywordd.Add(t.TextBefore.ToString());
                            collection[SpisokListBox.SelectedIndex].Keywordd.Sort();
                            sq.SetKeywords(id, collection[SpisokListBox.SelectedIndex].Keywordd);
                            SpisokListBox.Items.Refresh();
                            KeySearchBox.Clear();
                            KeySearchBox.Focus();
                        }
                    }

                }
            }
        }

        void bg_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            _ProgressBar.Value = e.ProgressPercentage;
        }

        #region События для полей рубрик и ключевых слов (инкриментный поиск, добавление в список)
        private void KeyTextChanged(object sender, TextChangedEventArgs e)
        {
            items.increment(KeySearchBox.Text, items.KeyList, items.KeyListEtalon());
            items.increment(ContextMenuKeySearchBox.Text, items.RichTextKeyList, items.KeyListEtalon());
            KeysListBox.Items.Refresh();
            ContextMenuListBox.Items.Refresh();

        }
        private void KeySearchKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case (Key.Down):
                    KeysListBox.Focus();
                    break;
            }
        }
        private void OtraslySearchBoxKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case (Key.Down):
                    OtraslyListBox.Focus();
                    break;
            }
        }
        private void OtraslySearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            items.increment(OtraslySearchBox.Text, items.Otraslyst, items.OtraslystEtalon());
            OtraslyListBox.Items.Refresh();
        }
        #endregion

        


        private void ButtonDelKeywords_Click(object sender, RoutedEventArgs e)
        {
            //экспорировать данные в эксель
            try
            {

                //if (ExportTo.Text.Count() > 0 && ExportFrom.Text.Count() > 0)
                //{
                //    ObservableCollection<ReadyListBoxClass> collection = SpisokListBox.ItemsSource.Cast<ReadyListBoxClass>() as ObservableCollection<ReadyListBoxClass>;
                //    for (int i = int.Parse(ExportFrom.Text); i <= int.Parse(ExportTo.Text); i++)
                //    {
                //        if (collection[i].Rubriki.Count() == 0)
                //        {
                //            SpisokListBox.SelectedItem = collection.Where(x => x.IndexNum == i).Cast<ReadyListBoxClass>();
                //            throw new Exception("В документе № " + collection[i].IndexNum + "не найдены рубрики!");
                //        }
                //        if (collection[i].Keywordd.Count() == 0)
                //        {
                //            SpisokListBox.SelectedItem = collection.Where(x => x.IndexNum == i).Cast<ReadyListBoxClass>();
                //            throw new Exception("В документе № " + collection[i].IndexNum + "не найдены ключевые слова!");
                //        }
                //    }
                if (int.Parse(ExportFrom.Text)  > 0 && int.Parse(ExportTo.Text) > 0)
                {           
                    ProgressTextBlock.Text = "Обработка ячеек...";
                    _ProgressBar.Maximum = int.Parse(ExportTo.Text) - int.Parse(ExportFrom.Text);
                    ExcelImportExport.ExportToExcel(new ClassForExport(int.Parse(ExportFrom.Text), int.Parse(ExportTo.Text), Path.Combine("Готовые", "На классификацию " + DateTime.Now.ToShortDateString() + ".xlsx")));
                }
                //}

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #region копирование рубрик и ключевых слов, удаление.
        private void CopyRubriki(object sender, RoutedEventArgs e)
        {
            ObservableCollection<ReadyListBoxClass> collection = SpisokListBox.ItemsSource.Cast<ReadyListBoxClass>() as ObservableCollection<ReadyListBoxClass>;
            if (collection[SpisokListBox.SelectedIndex].Rubriki != null)
            {
                RubrikiBufer = collection[SpisokListBox.SelectedIndex].Rubriki;
            }
        }

        private void PasteRubriki(object sender, RoutedEventArgs e)
        {
            if (RubrikiBufer.Count > 0)
            {
                ObservableCollection<ReadyListBoxClass> collection = SpisokListBox.ItemsSource.Cast<ReadyListBoxClass>() as ObservableCollection<ReadyListBoxClass>;
                collection[SpisokListBox.SelectedIndex].Rubriki = RubrikiBufer;
                sq.SetRubrics(collection[SpisokListBox.SelectedIndex].IndexNum, RubrikiBufer);
                SpisokListBox.Items.Refresh();
            }
        }

        private void CopyKeyWords(object sender, RoutedEventArgs e)
        {
            ObservableCollection<ReadyListBoxClass> collection = SpisokListBox.ItemsSource.Cast<ReadyListBoxClass>() as ObservableCollection<ReadyListBoxClass>;
            if (collection[SpisokListBox.SelectedIndex].Keywordd != null)
            {
                KeyWordsBufer = collection[SpisokListBox.SelectedIndex].Keywordd;
            }

        }

        private void PasteKeyWorsd(object sender, RoutedEventArgs e)
        {
            if (KeyWordsBufer.Count > 0)
            {
                ObservableCollection<ReadyListBoxClass> collection = SpisokListBox.ItemsSource.Cast<ReadyListBoxClass>() as ObservableCollection<ReadyListBoxClass>;
                collection[SpisokListBox.SelectedIndex].Keywordd = KeyWordsBufer;
                sq.SetKeywords(collection[SpisokListBox.SelectedIndex].IndexNum, KeyWordsBufer);
                SpisokListBox.Items.Refresh();
            }
        }

        private void DelKeysOrRubricsClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SpisokListBox.SelectedIndex != -1)
            {
                ObservableCollection<ReadyListBoxClass> collection = SpisokListBox.ItemsSource.Cast<ReadyListBoxClass>() as ObservableCollection<ReadyListBoxClass>;
                int id = collection[SpisokListBox.SelectedIndex].IndexNum;
                Button but = (Button)sender;
                var t = but.DataContext;
                if (SpisokListBox.SelectedIndex != -1 && (collection[SpisokListBox.SelectedIndex].Keywordd.Contains(t) || collection[SpisokListBox.SelectedIndex].Rubriki.Contains(t)))
                {
                    collection[SpisokListBox.SelectedIndex].Keywordd.Remove(t.ToString());
                    collection[SpisokListBox.SelectedIndex].Rubriki.Remove(t.ToString());
                    sq.SetKeywords(id, collection[SpisokListBox.SelectedIndex].Keywordd);
                    sq.SetRubrics(id, collection[SpisokListBox.SelectedIndex].Rubriki);
                }
                SpisokListBox.Items.Refresh();
            }

        }
        #endregion

        //Кнопка экспорта в Excel
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Pass ps = new Pass();
            ps.Owner = this;
            ps.Show();
        }


        #region выделение слов в ричтекстбоксе
        static List<string> tags = new List<string>();
        static List<char> specials = new List<char>();
        static string text;

        private List<string> words()
        {
            List<string> list = new List<string>();
            foreach (Tag s in m_tags)
            {
                list.Add(s.Word.ToLower());
            }
            //for (int i = 0; i <= list.Count; i++)
            //{

            //}
            list = list.Distinct().ToList();
            list.Sort();
            return list;
        }

        public static bool IsKnownTag(string tag)
        {
            return tags.Exists(delegate(string s) { return s.ToLower().Equals(tag.ToLower()); });
        }
        private static bool GetSpecials(char i)
        {
            foreach (var item in specials)
            {
                if (item.Equals(i))
                {
                    return true;
                }
            }
            return false;
        }
        new struct Tag
        {
            public TextPointer StartPosition;
            public TextPointer EndPosition;
            public string Word;
        }
        List<Tag> m_tags = new List<Tag>();
        delegate void CheckWordsInRunDelegate(Run theRun);

        //internal bool CheckWordsInRun(Run theRun)
        //{
        //    int sIndex = 0;
        //    int eIndex = 0;
        //    for (int i = 0; i < text.Length; i++)
        //    {
        //        if (Char.IsWhiteSpace(text[i]) | GetSpecials(text[i]))
        //        {
        //            if (i > 0 && !(Char.IsWhiteSpace(text[i - 1]) | GetSpecials(text[i - 1])))
        //            {
        //                eIndex = i - 1;
        //                string word = text.Substring(sIndex, eIndex - sIndex + 1);
        //                if (IsKnownTag(word))
        //                {
        //                    Dispatcher.Invoke(new Action(() =>
        //                    {
        //                        Tag t = new Tag();
        //                        t.StartPosition = theRun.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
        //                        t.EndPosition = theRun.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
        //                        t.Word = word;
        //                        m_tags.Add(t);
        //                        bg.ReportProgress(m_tags.Count + 1);

        //                    }));
        //                }
        //            }
        //            sIndex = i + 1;
        //        }

        //    }
        //    string lastWord = text.Substring(sIndex, text.Length - sIndex);
        //    if (IsKnownTag(lastWord))
        //    {
        //        Tag t = new Tag();
        //        t.StartPosition = theRun.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
        //        t.EndPosition = theRun.ContentStart.GetPositionAtOffset(text.Length, LogicalDirection.Backward); //fix 1
        //        t.Word = lastWord;
        //        m_tags.Add(t);
        //    }
        //    return true;
        //}

        internal void CheckWordsInRun(Run theRun) //do not hightlight keywords in this method
        {
            //How, let's go through our text and save all tags we have to save.               
            int sIndex = 0;
            int eIndex = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsWhiteSpace(text[i]) | GetSpecials(text[i]))
                {
                    if (i > 0 && !(Char.IsWhiteSpace(text[i - 1]) | GetSpecials(text[i - 1])))
                    {
                        eIndex = i - 1;
                        string word = text.Substring(sIndex, eIndex - sIndex + 1);
                        //Заменить метод для слов входящих в словарь на IsKnownTag
                        if (IsKnownTag(word))
                        {
                            Tag t = new Tag();
                            t.StartPosition = theRun.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                            t.EndPosition = theRun.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                            t.Word = word;
                            m_tags.Add(t);
                        }
                    }
                    sIndex = i + 1;
                }
            }
            //How this works. But wait. If the word is last word in my text I'll never hightlight it, due I'm looking for separators. Let's add some fix for this case
            string lastWord = text.Substring(sIndex, text.Length - sIndex);
            if (IsKnownTag(lastWord))
            {
                Tag t = new Tag();
                t.StartPosition = theRun.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                t.EndPosition = theRun.ContentStart.GetPositionAtOffset(text.Length, LogicalDirection.Backward); //fix 1
                t.Word = lastWord;
                m_tags.Add(t);
            }
        }


        //BackgroundWorker bg = new BackgroundWorker();
        //private void clear(TextRange docclearrange)
        //{
        //    m_tags.Clear();
        //    ActText.Dispatcher.Invoke(new Action(() => { docclearrange.ClearAllProperties(); }));
        //}
        //private void RichBox_HighLight(object sender, DoWorkEventArgs e)
        //{
            
        //    bool b = (bool)e.Argument;
        //    if (ActText.Document == null)
        //        return;
        //    TextRange documentRange = new TextRange(ActText.Document.ContentStart, ActText.Document.ContentEnd);
        //    if (b)
        //    {
        //        clear(documentRange);
        //    }
        //    TextPointer navigator = ActText.Document.ContentStart;
        //    int c = 0;
        //    while (navigator.CompareTo(ActText.Document.ContentEnd) < 0)
        //    {
        //        TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
        //        if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
        //        {
        //            c++;
        //            ActText.Dispatcher.Invoke(new Action(() =>
        //            {
        //                text = ((Run)navigator.Parent).Text;
        //            }));
        //            if (text != "")
        //            {
        //                CheckWordsInRun((Run)navigator.Parent);                   
        //            }
        //        }
        //        navigator = navigator.GetNextContextPosition(LogicalDirection.Forward); 
        //    }
        //}

        delegate void checkText();
        private async void RunTextHighLight(bool IsClear = true)
        {
            try
            {
                await Dispatcher.BeginInvoke(DispatcherPriority.Background, new checkText(() =>
                {
                    bindicator.IsBusy = true;
                }));              
                await Task.Factory.StartNew(async () =>
                {
                    if (ActText.Document == null)
                        return;
                  
                    TextRange documentRange = new TextRange(ActText.Document.ContentStart, ActText.Document.ContentEnd);

                    string txt = documentRange.Text;

                    await Dispatcher.BeginInvoke(DispatcherPriority.Background, new checkText(() =>
                    {                        
                       if (IsClear)
                       {
                            documentRange.ClearAllProperties();
                            m_tags.Clear();
                        }                     
                    }));
                    //Now let's create navigator to go though the text, find all the keywords but do not hightlight
                    TextPointer navigator = ActText.Document.ContentStart;

                    while (navigator.CompareTo(ActText.Document.ContentEnd) < 0)
                    {
                        TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);

                        if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                        {
                            await Dispatcher.BeginInvoke(DispatcherPriority.Background, new checkText(() =>
                            {
                                text = ((Run)navigator.Parent).Text; //fix 2
                            }));
                            if (text != "")
                            {
                                CheckWordsInRun((Run)navigator.Parent);
                            }
                        }
                        navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
                    }
                    //only after all keywords are found, then we highlight them
                    for (int i = 0; i < m_tags.Count; i++)
                    {
                        try
                        {
                            await Dispatcher.BeginInvoke(DispatcherPriority.Background, new checkText(() =>
                            {
                                TextRange range = new TextRange(m_tags[i].StartPosition, m_tags[i].EndPosition);
                                range.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.GreenYellow));
                                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                                bindicator.BusyContent = String.Format("Найдено ключевых слов: {0}", i);
                            }));
                        }
                        catch (Exception ex) { logger.Fatal(ex); }
                    }
                    await Dispatcher.BeginInvoke(DispatcherPriority.Background, new checkText(() =>
                    {
                        bindicator.IsBusy = false;
                    }));                 
                });

            }
            catch (Exception ex) { logger.Fatal(ex); };
        }
        //private void RunTextHighLight_old(bool clear = true)
        //{
        //    bg.WorkerReportsProgress = true;
        //    bg.WorkerSupportsCancellation = true;

        //    bg.DoWork += RichBox_HighLight;
        //    if (!bg.IsBusy)
        //    {
        //        bindicator.IsBusy = true;
        //        bg.RunWorkerAsync(clear);
        //    }          

        //    bg.ProgressChanged += (s1, e1) =>
        //    {
        //        bindicator.BusyContent = String.Format("Слов найдено {0}", e1.ProgressPercentage);    
        //    };
            
        //    bg.RunWorkerCompleted += (s1, e1) =>
        //    {
        //        Task.Factory.StartNew(() => { 
        //        Parallel.For(0, m_tags.Count, f =>
        //        {
        //            ActText.Dispatcher.BeginInvoke(new Action(() =>
        //             {
        //                 try
        //                 {
        //                     bindicator.BusyContent = String.Format("Раскрашиваю.... {0}", f);
        //                     TextRange range = new TextRange(m_tags[f].StartPosition, m_tags[f].EndPosition);
        //                     range.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.GreenYellow));
        //                     range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
        //                 }
        //                 catch { }
        //             }));
        //        });
        //        });
        //        bindicator.IsBusy = false;
        //        keyslistbox.ItemsSource = words();
        //        bg.DoWork -= RichBox_HighLight;
        //    };
        //}

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //sq.AddingKeysOrRubricsToBase(items.Otraslyst.Select(s => s.TextBefore).ToList(), "Rubrics");
            //sq.AddingKeysOrRubricsToBase(items.KeyList.Select(s => s.TextBefore).ToList(), "ClassificationKeywords");
        }
    }
}
