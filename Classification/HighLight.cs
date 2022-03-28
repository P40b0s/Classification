using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace Classification
{
    class HighLight
    {
        SQLMethods sq = new SQLMethods();
       public RichTextBox RTB { get; set; }
        public HighLight(RichTextBox rtb)
        {
            RTB = rtb;
            
        }
        

        public async void RefreshWords(bool b = true)
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

        internal bool CheckWordsInRun(Run theRun)
        {
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
                        if (IsKnownTag(word))
                        {

                            Tag t = new Tag();
                            t.StartPosition = theRun.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                            t.EndPosition = theRun.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                            t.Word = word;
                            m_tags.Add(t);
                            bg.ReportProgress(m_tags.Count + 1);
                        }
                    }
                    sIndex = i + 1;
                }
                if (bg.CancellationPending)
                {
                    return false;
                }
            }
            string lastWord = text.Substring(sIndex, text.Length - sIndex);
            if (IsKnownTag(lastWord))
            {
                Tag t = new Tag();
                t.StartPosition = theRun.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                t.EndPosition = theRun.ContentStart.GetPositionAtOffset(text.Length, LogicalDirection.Backward); //fix 1
                t.Word = lastWord;
                m_tags.Add(t);
            }
            return true;
        }


        BackgroundWorker bg = new BackgroundWorker();
        private void clear(TextRange docclearrange)
        {
            m_tags.Clear();
            docclearrange.ClearAllProperties();
        }
        private void RichBox_HighLight(object sender, DoWorkEventArgs e)
        {

            bool b = (bool)e.Argument;
            if (RTB.Document == null)
                return;
            TextRange documentRange = new TextRange(RTB.Document.ContentStart, RTB.Document.ContentEnd);
            if (b)
            {
                clear(documentRange);
            }
            TextPointer navigator = RTB.Document.ContentStart;
            int c = 0;
            while (navigator.CompareTo(RTB.Document.ContentEnd) < 0)
            {
                TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                {
                    c++;

                    text = ((Run)navigator.Parent).Text;
                    if (text != "")
                    {
                        if (!CheckWordsInRun((Run)navigator.Parent))
                        {
                            e.Cancel = true;
                        }

                    }
                }
                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

      



        public void RunTextHighLight(bool clear = true)
        {
            bg.WorkerReportsProgress = true;
            bg.WorkerSupportsCancellation = true;

            bg.DoWork += RichBox_HighLight;
            if (!bg.IsBusy)
            {
                //bindicator.IsBusy = true;
                bg.RunWorkerAsync(clear);
            }

            bg.ProgressChanged += (s1, e1) =>
            {
                //bindicator.BusyContent = String.Format("Слов найдено {0}", e1.ProgressPercentage);
            };

            bg.RunWorkerCompleted += (s1, e1) =>
            {

                Parallel.For(0, m_tags.Count, f =>
                {
                   
                        try
                        {
                            //bindicator.BusyContent = String.Format("Раскрашиваю.... {0}", f);
                            TextRange range = new TextRange(m_tags[f].StartPosition, m_tags[f].EndPosition);
                            range.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.GreenYellow));
                            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                        }
                        catch { }
                });
                //bindicator.IsBusy = false;
                //keyslistbox.ItemsSource = words();
                bg.DoWork -= RichBox_HighLight;
            };
        }
    }
}
