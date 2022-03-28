using Classification.Core.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Classification.Modules.DocumentTextViewerModule.Synonyms
{
    public class IncrementSearch
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public void Search(string searchtext, ref IEnumerable<TextInlineSelection> collection)
        {
            try
            {
                if (searchtext != null && searchtext.Length >= 0)
                {

                    for (int i = 0; i < collection.Count(); i++)
                    {
                        collection.ElementAt(i).Visible = true;
                        collection.ElementAt(i).SelectedText = null;
                        collection.ElementAt(i).TextBeforeSelect = null;
                    }
                    for (int i = collection.Count() - 1; i >= 0; i--)
                    {
                        var item = collection.ElementAt(i) as TextInlineSelection;
                        if (item.SourceText != null)
                        {
                            if (!item.SourceText.ToLower().Contains(searchtext.ToLower()))
                            {
                                collection.ElementAt(i).Visible = false;
                            }
                            else
                            {
                                if ((item.SourceText.ToLower().IndexOf(searchtext.ToLower())) != -1)
                                {
                                    int t = (item.SourceText.ToLower().IndexOf(searchtext.ToLower()));
                                    item.TextBeforeSelect = item.SourceText.Substring(0, t);
                                    item.SelectedText = item.SourceText.Substring(item.SourceText.ToLower().IndexOf(searchtext.ToLower()), searchtext.Length);
                                }

                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < collection.Count(); i++)
                    {
                        collection.ElementAt(i).Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        public void Search(string searchtext, ObservableCollection<TextInlineSelection> collection, List<TextInlineSelection> etalon)
        {
            if (searchtext != null && searchtext.Length >= 0)
            {
                collection.Clear();
                for (int i = 0; i < etalon.Count(); i++)
                {
                    collection.Add(new TextInlineSelection(etalon[i].Id, etalon[i].SourceText, string.Empty));
                }

                for (int i = collection.Count - 1; i >= 0; i--)
                {
                    var Item = collection[i] as TextInlineSelection;
                    if (Item.SourceText != null)
                    {
                        if (!Item.SourceText.ToLower().Contains(searchtext.ToLower()))
                        {
                            collection.RemoveAt(i);
                        }
                        else
                        {

                            if ((Item.SourceText.ToLower().IndexOf(searchtext)) != -1)
                            {
                                int t = (Item.SourceText.ToLower().IndexOf(searchtext));
                                Item.TextBeforeSelect = Item.SourceText.Substring(0, t);
                                Item.SelectedText = Item.SourceText.Substring(Item.SourceText.ToLower().IndexOf(searchtext), searchtext.Length);
                            }
                        }
                    }
                }

            }
            else
            {
                collection.Clear();
                for (int i = 0; i < etalon.Count(); i++)
                {
                    collection.Add(new TextInlineSelection(etalon[i].Id, etalon[i].SourceText, string.Empty));
                }
            }
        }
    }
}
