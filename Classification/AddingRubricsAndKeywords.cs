using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.IO;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Windows;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Classification
{
    
    class AddingRubricsAndKeywords
    {
        SQLMethods sq = new SQLMethods();
        public AddingRubricsAndKeywords()
        {
            Otraslyst = OtraslystEtalon();            
            KeyList = KeyListEtalon();
            RichTextKeyList = KeyListEtalon();          
        }
        public List<TextBlockSelection> KeyListEtalon()
        {
            return sq.GetEtalonKeywords();
        }
        //public async Task<List<TextBlockSelection>> OtraslystEtalon()
        //{
        //    return await Task<List<TextBlockSelection>>.Factory.StartNew(() => {
        //        return sq.GetEtalonRubrics();            
        //    });
           
        //}

        public List<TextBlockSelection> OtraslystEtalon()
        {
                return sq.GetEtalonRubrics();
        }

        public List<TextBlockSelection> Otraslyst { get; set; }
        public List<TextBlockSelection> KeyList { get; set; }
        public List<TextBlockSelection> RichTextKeyList { get; set; }


        public void increment(string searchtext, List<TextBlockSelection> list, List<TextBlockSelection> etalon)
        {
            if (searchtext.Length >= 0)
            {
                list.Clear();
                for (int i = 0; i < etalon.Count; i++)
                {
                    list.Add(new TextBlockSelection(etalon[i].TextBefore, string.Empty));
                }
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var LBItem = list[i] as TextBlockSelection;
                    if (!LBItem.TextBefore.ToLower().Contains(searchtext.ToLower()))
                    {
                        list.RemoveAt(i);
                    }
                    else
                    {
                        try
                        {
                            if ((LBItem.TextBefore.ToLower().IndexOf(searchtext)) != -1)
                            {
                                int t = (LBItem.TextBefore.ToLower().IndexOf(searchtext));
                                LBItem.TextBeforeSelect = LBItem.TextBefore.Substring(0, t);
                                LBItem.TextSelect = LBItem.TextBefore.Substring(LBItem.TextBefore.ToLower().IndexOf(searchtext), searchtext.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            Support.LoggingError("Classification", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                        }
                    }
                }
            }
            else
            {
                list = etalon;
            }
        }

        



    }


}
